using Azure;
using Humanizer;
using MassTransit;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Infrastructure;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Models.Data;
using Models.Entities;
using Models.Entities.DTO;
using Models.Entities.Enums;
using Models.Extensions.Mappings;
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
		var orders = await _localContext.Orders
			 .Include(o => o.CoffeeUser)
			 .OrderByDescending(o => o.CreatedAt)
			 .ToListAsync();
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
	public async Task<IActionResult> Create([Bind("CoffeeUserId,OrderId,Items")] OrderViewModel viewModel) {
		if (!ModelState.IsValid) {
			var errors = ModelState.Values.SelectMany(v => v.Errors);
			foreach (var error in errors) {
				Console.WriteLine($"Validation Error: {error.ErrorMessage}");
			}

			PopulateViewData();
			return View(viewModel);
		}

		if (viewModel.Items == null || viewModel.Items.Count == 0) {
			ModelState.AddModelError("OrderViewModel", "There must be order items.");
			PopulateViewData();
			return View(viewModel);
		}

		var localUser = await _localContext.Users.FindAsync(viewModel.CoffeeUserId);
		if (localUser == null || localUser.GlobalId == null) {
			ModelState.AddModelError("LocalUser", "Provided user is not synchronized with the global system.");
			PopulateViewData();
			return View(viewModel);
		}

		var localOrder = new Order() {
			CoffeeUserId = (long) localUser.GlobalId
		};

		var globalOrder = await _utilities.ValidateAddAndBackup(ModelState, localOrder);
		if (!ModelState.IsValid || globalOrder == null) {
			PopulateViewData();
			return View(viewModel);
		}

		decimal total = 0;

		try {
			foreach (var viewModelItem in viewModel.Items) {
				var localCoffee = _localContext.Coffees.Find(viewModelItem.CoffeeId);
				if (localCoffee == null || localCoffee.GlobalId == null) {
					ModelState.AddModelError("OrderItem",
						localCoffee == null ? $"Skipped order item, no local coffee instance associated: {viewModelItem.Id}." :
						localCoffee.GlobalId == null ? $"Skipped order item, no global coffee instance associated with Coffee: {viewModelItem.Id}." :
						"This error is unknown to us at the time.");
					continue;
				}

				var globalCoffee = _globalContext.Coffees.Find(localCoffee.GlobalId);
				if (globalCoffee == null) {
					ModelState.AddModelError("OrderItem",
						globalCoffee == null ? $"Skipped order item, no local coffee instance associated: {viewModelItem.Id}." :
						globalCoffee.GlobalId == null ? $"Skipped order item, no global coffee instance associated with Coffee: {viewModelItem.Id}." :
						"This error is unknown to us at the time.");
					continue;
				}

				var localOrderItem = new OrderItem {
					OrderId = globalOrder.Id,
					CoffeeId = globalCoffee.Id,
					Quantity = viewModelItem.Quantity,
					UnitPrice = viewModelItem.UnitPrice
				};

				var globalOrderItem = await _utilities.ValidateAddAndBackup(ModelState, localOrderItem);
				if (!ModelState.IsValid) {
					PopulateViewData();
					return View(viewModel);
				}

				await _utilities.SendMessageTo("OrderSubmitted", new OrderSubmitted(
					(long) localUser.GlobalId,
					globalOrder.Id,
					globalCoffee.Id,
					globalCoffee.Type,
					viewModelItem.Quantity,
					viewModelItem.UnitPrice
				));

				total += viewModelItem.Quantity * viewModelItem.UnitPrice;
			}

			TempData["OrderStatus"] = $"Order sent to RabbitMQ! Totaal: €{total:0.00}";
		} catch (Exception ex) {
			Console.WriteLine($"Global Sync Warning: {ex.Message}");
		}

		// Redirect to Index so we don't accidentally repost if user refreshes
		return RedirectToAction(nameof(Index));
	}

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
			var localOrder = await _localContext.Orders.FindAsync(id);

			if (localOrder == null) {
				return NotFound();
			}

			localOrder.Status = order.Status;
			localOrder.CoffeeUserId = order.CoffeeUserId;

			_localContext.Orders.Update(localOrder);
			await _localContext.SaveChangesAsync();

			if (localOrder.GlobalId != null) {
				try {
					if (await _globalContext.Database.CanConnectAsync()) {
						var globalOrder = await _globalContext.Orders.FindAsync(localOrder.GlobalId);

						if (globalOrder != null) {
							globalOrder.Status = localOrder.Status;
							_globalContext.Orders.Update(globalOrder);
							await _globalContext.SaveChangesAsync();
						}
					}
				} catch (Exception ex) {
					Console.WriteLine($"Global Edit Sync Warning: {ex.Message}");
				}
			}

			return RedirectToAction(nameof(Index));
		}
		ViewData["CoffeeUserId"] = new SelectList(_localContext.Users, "Id", "FirstName", order.CoffeeUserId);
		ViewData["StatusTypes"] = _utilities.GetEnumSelectList<StatusType>(ignored: ["Unknown"]);
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

		// 2. Sync soft delete to Global
		if (order.GlobalId != null) {
			try {
				if (await _globalContext.Database.CanConnectAsync()) {
					var globalOrder = await _globalContext.Orders.FindAsync(order.GlobalId);

					if (globalOrder != null) {
						globalOrder.DeletedAt = order.DeletedAt;
						_globalContext.Orders.Update(globalOrder);
						await _globalContext.SaveChangesAsync();
					}
				}
			} catch (Exception ex) {
				Console.WriteLine($"Global Delete Sync Warning: {ex.Message}");
			}
		}
		await _localContext.SaveChangesAsync();
		return RedirectToAction(nameof(Index));
	}

	[HttpPost]
	[ValidateAntiForgeryToken]
	public IActionResult AddOrderItem([FromBody] OrderItem posted, int index) {
		if (!ModelState.IsValid) {
			return BadRequest(ModelState);
		}

		if (posted == null) {
			return BadRequest();
		}

		ViewData["Index"] = index;
		return PartialView("_OrderItemRow", posted);
	}

	private bool OrderExists(long id) {
		return _localContext.Orders.Any(e => e.Id == id);
	}

	private void PopulateViewData() {
		ViewData["CoffeeUserId"] = new SelectList(_localContext.Users, "Id", "FirstName");
		ViewData["CoffeeTypes"] = _utilities.GetEnumSelectList<CoffeeType>(ignored: ["Unknown"]);
		ViewData["StatusTypes"] = _utilities.GetEnumSelectList<StatusType>(ignored: ["Unknown"]);
	}
}
