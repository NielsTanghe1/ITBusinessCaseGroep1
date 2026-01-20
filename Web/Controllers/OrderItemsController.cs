using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Models.Data;
using Models.Entities;

namespace Web.Controllers;

public class OrderItemsController : Controller {
	private readonly LocalDbContext _context;

	public OrderItemsController(LocalDbContext context) {
		_context = context;
	}

	// GET: OrderItems
	public async Task<IActionResult> Index() {
		var localDbContext = _context.OrderItems.Include(o => o.Coffee).Include(o => o.Order);
		return View(await localDbContext.ToListAsync());
	}

	// GET: OrderItems/Details/5
	public async Task<IActionResult> Details(long? id) {
		if (id == null) {
			return NotFound();
		}

		var orderItem = await _context.OrderItems
			 .Include(o => o.Coffee)
			 .Include(o => o.Order)
			 .FirstOrDefaultAsync(m => m.Id == id);
		if (orderItem == null) {
			return NotFound();
		}

		return View(orderItem);
	}

	// GET: OrderItems/Create
	public IActionResult Create() {
		ViewData["CoffeeId"] = new SelectList(_context.Coffees, "Id", "Id");
		ViewData["OrderId"] = new SelectList(_context.Orders, "Id", "Id");
		return View();
	}

	// POST: OrderItems/Create
	// To protect from overposting attacks, enable the specific properties you want to bind to.
	// For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
	[HttpPost]
	[ValidateAntiForgeryToken]
	public async Task<IActionResult> Create([Bind("Id,OrderId,CoffeeId,Quantity,UnitPrice,CreatedAt,DeletedAt")] OrderItem orderItem) {
		if (ModelState.IsValid) {
			_context.Add(orderItem);
			await _context.SaveChangesAsync();
			return RedirectToAction(nameof(Index));
		}
		ViewData["CoffeeId"] = new SelectList(_context.Coffees, "Id", "Id", orderItem.CoffeeId);
		ViewData["OrderId"] = new SelectList(_context.Orders, "Id", "Id", orderItem.OrderId);
		return View(orderItem);
	}

	// GET: OrderItems/Edit/5
	public async Task<IActionResult> Edit(long? id) {
		if (id == null) {
			return NotFound();
		}

		var orderItem = await _context.OrderItems.FindAsync(id);
		if (orderItem == null) {
			return NotFound();
		}
		ViewData["CoffeeId"] = new SelectList(_context.Coffees, "Id", "Id", orderItem.CoffeeId);
		ViewData["OrderId"] = new SelectList(_context.Orders, "Id", "Id", orderItem.OrderId);
		return View(orderItem);
	}

	// POST: OrderItems/Edit/5
	// To protect from overposting attacks, enable the specific properties you want to bind to.
	// For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
	[HttpPost]
	[ValidateAntiForgeryToken]
	public async Task<IActionResult> Edit(long id, [Bind("Id,OrderId,CoffeeId,Quantity,UnitPrice,CreatedAt,DeletedAt")] OrderItem orderItem) {
		if (id != orderItem.Id) {
			return NotFound();
		}

		if (ModelState.IsValid) {
			try {
				_context.Update(orderItem);
				await _context.SaveChangesAsync();
			} catch (DbUpdateConcurrencyException) {
				if (!OrderItemExists(orderItem.Id)) {
					return NotFound();
				} else {
					throw;
				}
			}
			return RedirectToAction(nameof(Index));
		}
		ViewData["CoffeeId"] = new SelectList(_context.Coffees, "Id", "Id", orderItem.CoffeeId);
		ViewData["OrderId"] = new SelectList(_context.Orders, "Id", "Id", orderItem.OrderId);
		return View(orderItem);
	}

	// GET: OrderItems/Delete/5
	public async Task<IActionResult> Delete(long? id) {
		if (id == null) {
			return NotFound();
		}

		var orderItem = await _context.OrderItems
			 .Include(o => o.Coffee)
			 .Include(o => o.Order)
			 .FirstOrDefaultAsync(m => m.Id == id);
		if (orderItem == null) {
			return NotFound();
		}

		return View(orderItem);
	}

	// POST: OrderItems/Delete/5
	[HttpPost, ActionName("Delete")]
	[ValidateAntiForgeryToken]
	public async Task<IActionResult> DeleteConfirmed(long id) {
		var orderItem = await _context.OrderItems.FindAsync(id);
		if (orderItem != null) {
			_context.OrderItems.Remove(orderItem);
		}

		await _context.SaveChangesAsync();
		return RedirectToAction(nameof(Index));
	}

	private bool OrderItemExists(long id) {
		return _context.OrderItems.Any(e => e.Id == id);
	}
}
