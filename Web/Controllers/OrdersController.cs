using MassTransit;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Models.Data;
using Models.Entities;
using Models.Entities.DTO; // Required
using Models.Entities.Enums;
using Web.Services;

namespace Web.Controllers;

public class OrdersController : Controller {
	private readonly LocalDbContext _localContext;
	private readonly GlobalDbContext _globalContext; // Global Context for sync
	private readonly Utilities _utilities;
	private readonly IPublishEndpoint _publish;

	public OrdersController(LocalDbContext localContext, GlobalDbContext globalContext, Utilities utilities, IPublishEndpoint publish) {
		_localContext = localContext;
		_globalContext = globalContext;
		_utilities = utilities;
		_publish = publish;
	}

	// GET: Orders
	public async Task<IActionResult> Index() {
		// Now works because OrderDTO shadows the CoffeeUser property
		var orders = await _localContext.Orders.Include(o => o.CoffeeUser).ToListAsync();
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
	public IActionResult Create() {
		ViewData["CoffeeUserId"] = new SelectList(_localContext.Users, "Id", "FirstName");
		ViewData["StatusTypes"] = _utilities.GetEnumSelectList<StatusType>(ignored: ["Unknown"]);
		return View();
	}

	// POST: Orders/Create
	[HttpPost]
	[ValidateAntiForgeryToken]
	// 1. REMOVED CreatedAt and DeletedAt from the Bind list
	public async Task<IActionResult> Create([Bind("Id,CoffeeUserId,Status")] OrderDTO orderDto) {
		if (ModelState.IsValid) {
			// 2. Ensure CreatedAt is set (though the class default handles this, it's safe to be explicit)
			orderDto.CreatedAt = DateTime.UtcNow;

			// 3. Prepare Local Data
			if (orderDto.GlobalId == 0)
				orderDto.GlobalId = Random.Shared.NextInt64();

			_localContext.Add(orderDto);
			await _localContext.SaveChangesAsync();

			// 4. Sync to Global Database
			try {
				var localUser = await _localContext.Users.AsNoTracking()
					  .FirstOrDefaultAsync(u => u.Id == orderDto.CoffeeUserId);

				if (localUser != null) {
					var globalUser = await _globalContext.Users
						  .FirstOrDefaultAsync(u => u.GlobalId == localUser.GlobalId || u.Email == localUser.Email);

					if (globalUser == null) {
						globalUser = new CoffeeUser {
							GlobalId = localUser.GlobalId,
							UserName = localUser.UserName,
							Email = localUser.Email,
							FirstName = localUser.FirstName,
							LastName = localUser.LastName,
							EmailConfirmed = localUser.EmailConfirmed,
							PhoneNumber = localUser.PhoneNumber,
							CreatedAt = localUser.CreatedAt
						};
						_globalContext.Users.Add(globalUser);
						await _globalContext.SaveChangesAsync();
					}

					var globalOrder = new Order {
						GlobalId = orderDto.GlobalId,
						CoffeeUserId = globalUser.Id, // Use the GLOBAL ID
						Status = orderDto.Status,
						CreatedAt = orderDto.CreatedAt,
						DeletedAt = orderDto.DeletedAt
					};

					_globalContext.Orders.Add(globalOrder);
					await _globalContext.SaveChangesAsync();
				}
			} catch (Exception ex) {
				Console.WriteLine($"SYNC ERROR: {ex.Message}");
			}

			return RedirectToAction(nameof(Index));
		}

		// If we get here, something is invalid.
		// This will help you see WHY it failed if it happens again:
		var errors = ModelState.Values.SelectMany(v => v.Errors);
		foreach (var error in errors) {
			Console.WriteLine($"Validation Error: {error.ErrorMessage}");
		}

		ViewData["CoffeeUserId"] = new SelectList(_localContext.Users, "Id", "FirstName", orderDto.CoffeeUserId);
		return View(orderDto);
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
	public async Task<IActionResult> Edit(long id, [Bind("Id,CoffeeUserId,Status,CreatedAt,DeletedAt")] OrderDTO order) {
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
		if (id == null)
			return NotFound();

		var order = await _localContext.Orders
			  .Include(o => o.CoffeeUser)
			  .FirstOrDefaultAsync(m => m.Id == id);
		if (order == null)
			return NotFound();

		return View(order);
	}

	// POST: Orders/Delete/5
	[HttpPost, ActionName("Delete")]
	[ValidateAntiForgeryToken]
	public async Task<IActionResult> DeleteConfirmed(long id) {
		var order = await _localContext.Orders.FindAsync(id);
		if (order != null) {
			_localContext.Orders.Remove(order);
		}

		await _localContext.SaveChangesAsync();
		return RedirectToAction(nameof(Index));
	}

	private bool OrderExists(long id) {
		return _localContext.Orders.Any(e => e.Id == id);
	}
}