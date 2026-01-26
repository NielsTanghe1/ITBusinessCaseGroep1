using MassTransit;
using MassTransit.Transports;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Models.Data;
using Models.Entities;
using Models.Entities.DTO;
using Models.Entities.Enums;
using Web.Models;
using Web.Services;

namespace Web.Controllers;

public class OrdersController : Controller {
	private readonly LocalDbContext _localContext;
	private readonly GlobalDbContext _globalContext;
	private readonly Utilities _utilities;
	private readonly ISendEndpointProvider _sendEndpointProvider;
	private readonly IConfiguration _configuration;
	private readonly UserManager<CoffeeUser> _userManager;

	public OrdersController(
		LocalDbContext localContext,
		GlobalDbContext globalContext,
		Utilities utilities,
		ISendEndpointProvider sendEndpointProvider,
		IConfiguration configuration,
		UserManager<CoffeeUser> userManager) {
		_localContext = localContext;
		_globalContext = globalContext;
		_utilities = utilities;
		_sendEndpointProvider = sendEndpointProvider;
		_configuration = configuration;
		_userManager = userManager;
	}

	// GET: Orders
	public async Task<IActionResult> Index() {
		var orders = await _localContext.Orders.Include(o => o.CoffeeUser).Where(o => o.DeletedAt == null).ToListAsync();
		return View(orders);
	}

	// GET: Orders/Details/5
	public async Task<IActionResult> Details(long? id) {
		if (id == null)
			return NotFound();

		var order = await _localContext.Orders
			  .Include(o => o.CoffeeUser)
			  .FirstOrDefaultAsync(m => m.Id == id);

		if (order == null)
			return NotFound();

		return View(order);
	}

	// GET: Orders/Create
	public async Task<IActionResult> Create() {
		ViewData["CoffeeUserId"] = new SelectList(_localContext.Users, "Id", "FirstName");
		ViewData["CoffeeTypes"] = _utilities.GetEnumSelectList<CoffeeType>(ignored: ["Unknown"]);
		ViewData["StatusTypes"] = _utilities.GetEnumSelectList<StatusType>(ignored: ["Unknown"]);

		bool result = long.TryParse(_userManager.GetUserId(User), out long userId);

		if (!result || userId == 0) {
			return RedirectToAction("Index", "Home");
		}

		return View(new OrderViewModel() {
			CoffeeUserId = userId,
			OrderId = 0
		});
	}

	// POST: Orders/Create
	[HttpPost]
	[ValidateAntiForgeryToken]
	public async Task<IActionResult> Create([Bind("CoffeeUserId,OrderId,Address,Items")] OrderViewModel viewModel) {

		if (!ModelState.IsValid) {
			var errors = ModelState.Values.SelectMany(v => v.Errors);
			foreach (var error in errors) {
				Console.WriteLine($"Validation Error: {error.ErrorMessage}");
			}
			// Re-populate ViewData and return to the view
			ViewData["CoffeeUserId"] = new SelectList(_localContext.Users, "Id", "FirstName");
			ViewData["CoffeeTypes"] = _utilities.GetEnumSelectList<CoffeeType>(ignored: ["Unknown"]);
			ViewData["StatusTypes"] = _utilities.GetEnumSelectList<StatusType>(ignored: ["Unknown"]);
			return View(viewModel);
		}

		if (viewModel.Items == null || !viewModel.Items.Any()) {
			return BadRequest("Order must have items");
		}

		var localUser = await _localContext.Users.FindAsync(viewModel.CoffeeUserId);
		if (localUser == null) {
			return BadRequest("User not found");
		}

		var localOrder = new Order {
			CoffeeUserId = localUser.Id,
			Status = StatusType.Pending,
			CreatedAt = DateTime.UtcNow
		};

		_localContext.Orders.Add(localOrder);
		await _localContext.SaveChangesAsync(); 

		decimal total = 0;

		foreach (var item in viewModel.Items) {
			var newItem = new OrderItem {
				OrderId = localOrder.Id,
				CoffeeId = item.CoffeeId,
				Quantity = item.Quantity,
				UnitPrice = item.UnitPrice
			};

			_localContext.OrderItems.Add(newItem);
			var coffee = await _localContext.Coffees.FindAsync(item.CoffeeId);
			if (coffee == null) {
				ModelState.AddModelError("", $"Coffee with ID {item.CoffeeId} not found.");
				continue;
			}

			await _utilities.SendMessageTo("OrderSubmitted", new OrderSubmitted(
				viewModel.CoffeeUserId,
				item.Id,
				item.CoffeeId,
				coffee.Type,
				item.Quantity,
				item.UnitPrice
			));
			total += item.Quantity * item.UnitPrice;
		}

		await _localContext.SaveChangesAsync();

		TempData["OrderPlaced"] = $"Order sent to RabbitMQ! Totaal: €{total:0.00}";

		try {
			if (await _globalContext.Database.CanConnectAsync()) {
				await CreateGlobal(localUser, localOrder);
			}
		} catch (Exception ex) {
			Console.WriteLine($"Global Sync Warning: {ex.Message}");
		}

		// Redirect to Index so we don't accidentally repost if user refreshes
		return RedirectToAction(nameof(Index));
	}

	// TO IMPLEMENT
	//private async Task<IActionResult> CreateGlobal() {
	//	var globalUser = _globalContext.Users.Find(localUser.GlobalId);

	//	if (globalUser == null) {
	//		// Assuming the local user is correctly validated, we back them up in the global context.
	//		globalUser = new CoffeeUser {
	//			UserName = localUser.UserName,
	//			Email = localUser.Email,
	//			FirstName = localUser.FirstName,
	//			LastName = localUser.LastName,
	//			EmailConfirmed = localUser.EmailConfirmed,
	//			PhoneNumber = localUser.PhoneNumber,
	//			CreatedAt = localUser.CreatedAt
	//		};
	//		globalUser = _globalContext.Users.Add(globalUser).Entity;
	//		await _globalContext.SaveChangesAsync();

	//		localUser.GlobalId = globalUser.Id;
	//		await _localContext.SaveChangesAsync();
	//	}

	//	var globalOrder = _globalContext.Orders.Add(new() {
	//		CoffeeUserId = globalUser.Id
	//	}).Entity;
	//	await _globalContext.SaveChangesAsync();

	//	// Run a foreach as the Order Id has to be generated by the database first
	//	foreach (var newItem in _localContext.OrderItems.Where(oi => oi.OrderId == localOrder.Id).ToArray()) {
	//		var globalCoffeeId = _globalContext.Coffees.Find(newItem.Coffee?.GlobalId)?.Id;

	//		if (globalCoffeeId == null) {
	//			continue;
	//		}

	//		_globalContext.OrderItems.Add(new() {
	//			OrderId = globalOrder.Id,
	//			CoffeeId = (long) globalCoffeeId,
	//			Quantity = newItem.Quantity,
	//			UnitPrice = newItem.UnitPrice
	//		});
	//	}
	//	await _localContext.SaveChangesAsync();
	//}

	// GET: Orders/Edit/5
	public async Task<IActionResult> Edit(long? id) {
		if (id == null)
			return NotFound();

		var order = await _localContext.Orders.FindAsync(id);
		if (order == null)
			return NotFound();

		ViewData["CoffeeUserId"] = new SelectList(_localContext.Users, "Id", "FirstName", order.CoffeeUserId);
		ViewData["StatusTypes"] = _utilities.GetEnumSelectList<StatusType>(ignored: ["Unknown"]);
		return View(order);
	}

	// POST: Orders/Edit/5
	[HttpPost]
	[ValidateAntiForgeryToken]
	public async Task<IActionResult> Edit(long id, [Bind("Id,CoffeeUserId,Status,CreatedAt,DeletedAt")] Order order) {
		if (id != order.Id)
			return NotFound();

		if (ModelState.IsValid) {
			try {
				_localContext.Update(order);
				await _localContext.SaveChangesAsync();
			} catch (DbUpdateConcurrencyException) {
				if (!OrderExists(order.Id))
					return NotFound();
				else
					throw;
			}
			return RedirectToAction(nameof(Index));
		}
		ViewData["CoffeeUserId"] = new SelectList(_localContext.Users, "Id", "FirstName", order.CoffeeUserId);
		return View(order);
	}

	// GET: Orders/Delete/5
	public async Task<IActionResult> Delete(long? id) {
		if (id == null) {
			return NotFound();
		}

		var order = await _localContext.Orders
			 .Include(o => o.CoffeeUser)
			 .FirstOrDefaultAsync(m => m.Id == id);
		if (order == null) {
			return NotFound();
		}

		return View(order);
	}

	// POST: Orders/Delete/5
	[HttpPost, ActionName("Delete")]
	[ValidateAntiForgeryToken]
	public async Task<IActionResult> DeleteConfirmed(long id) {
		var order = await _localContext.Orders.FindAsync(id);
		if (order != null) {
			order.DeletedAt = DateTime.UtcNow;
			_localContext.Orders.Update(order);
		}

		await _localContext.SaveChangesAsync();
		return RedirectToAction(nameof(Index));
	}

	private bool OrderExists(long id) {
		return _localContext.Orders.Any(e => e.Id == id);
	}

	private async Task CreateGlobal(CoffeeUser localUser, Order localOrder) {
		// 1. ENSURE USER EXISTS GLOBALLY
		// We check if the user already has a GlobalId.
		CoffeeUser? globalUser = null;

		if (localUser.GlobalId != null) {
			globalUser = await _globalContext.Users.FindAsync(localUser.GlobalId);
		}

		// If user doesn't exist globally (or didn't have a GlobalId), create them
		if (globalUser == null) {
			globalUser = new CoffeeUser {
				// Copy relevant fields from localUser
				UserName = localUser.UserName,
				Email = localUser.Email,
				FirstName = localUser.FirstName,
				LastName = localUser.LastName,
				EmailConfirmed = localUser.EmailConfirmed,
				PhoneNumber = localUser.PhoneNumber,
				CreatedAt = localUser.CreatedAt,
				// SecurityStamp is important for some Identity checks, copy it if needed or let it regenerate
				SecurityStamp = localUser.SecurityStamp,
				NormalizedEmail = localUser.NormalizedEmail,
				NormalizedUserName = localUser.NormalizedUserName
			};

			_globalContext.Users.Add(globalUser);
			await _globalContext.SaveChangesAsync(); // Save to generate the Global ID

			// Link the local user to the new global user
			localUser.GlobalId = globalUser.Id;
			_localContext.Users.Update(localUser);
			await _localContext.SaveChangesAsync();
		}

		// 2. CREATE GLOBAL ORDER
		var globalOrder = new Order {
			CoffeeUserId = globalUser.Id, // Use the Global User ID
			Status = localOrder.Status,
			CreatedAt = localOrder.CreatedAt
			// Add other properties if your Order entity has them (e.g., Address)
		};

		_globalContext.Orders.Add(globalOrder);
		await _globalContext.SaveChangesAsync(); // Save to generate the Global Order ID

		// Update the local order with the new GlobalId
		localOrder.GlobalId = globalOrder.Id;
		_localContext.Orders.Update(localOrder);
		await _localContext.SaveChangesAsync();

		// 3. SYNC ORDER ITEMS
		// Fetch the items we just saved locally
		var localItems = await _localContext.OrderItems
			 .Where(oi => oi.OrderId == localOrder.Id)
			 .Include(oi => oi.Coffee) // Include Coffee to get its GlobalId
			 .ToListAsync();

		foreach (var localItem in localItems) {
			// We need the Global ID of the Coffee product
			if (localItem.Coffee != null && localItem.Coffee.GlobalId != null) {
				_globalContext.OrderItems.Add(new OrderItem {
					OrderId = globalOrder.Id, // Link to the new Global Order
					CoffeeId = (long) localItem.Coffee.GlobalId, // Use the Global Coffee ID
					Quantity = localItem.Quantity,
					UnitPrice = localItem.UnitPrice
				});
			}
		}

		await _globalContext.SaveChangesAsync();
	}
}
