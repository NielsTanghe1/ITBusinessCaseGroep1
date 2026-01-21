using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Models.Data;
using Models.Entities;
using Models.Entities.Enums;
using Web.Services;

namespace Web.Controllers;

public class CoffeesController : Controller {
	private readonly LocalDbContext _context;
	private readonly Utilities _utilities;

	public CoffeesController(LocalDbContext context, Utilities utilities) {
		_context = context;
		_utilities = utilities;
	}

	// GET: Coffees
	public async Task<IActionResult> Index() {
		return View(await _context.Coffees.ToListAsync());
	}

	// GET: Coffees/Details/5
	public async Task<IActionResult> Details(long? id) {
		if (id == null) {
			return NotFound();
		}

		var coffee = await _context.Coffees
			 .FirstOrDefaultAsync(m => m.Id == id);
		if (coffee == null) {
			return NotFound();
		}

		return View(coffee);
	}

	// GET: Coffees/Create
	public IActionResult Create() {
		ViewData["CoffeeNames"] = _utilities.GetEnumSelectList<CoffeeName>(ignored: ["Unknown"]);
		ViewData["CoffeeTypes"] = _utilities.GetEnumSelectList<CoffeeType>(ignored: ["Unknown"]);
		return View();
	}

	// POST: Coffees/Create
	// To protect from overposting attacks, enable the specific properties you want to bind to.
	// For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
	[HttpPost]
	[ValidateAntiForgeryToken]
	public async Task<IActionResult> Create([Bind("Id,Name,Type,Price,CreatedAt,DeletedAt")] Coffee coffee) {
		if (ModelState.IsValid) {
			_context.Add(coffee);
			await _context.SaveChangesAsync();
			return RedirectToAction(nameof(Index));
		}
		return View(coffee);
	}

	// GET: Coffees/Edit/5
	public async Task<IActionResult> Edit(long? id) {
		if (id == null) {
			return NotFound();
		}

		var coffee = await _context.Coffees.FindAsync(id);
		if (coffee == null) {
			return NotFound();
		}
		ViewData["CoffeeNames"] = _utilities.GetEnumSelectList<CoffeeName>(ignored: ["Unknown"]);
		ViewData["CoffeeTypes"] = _utilities.GetEnumSelectList<CoffeeType>(ignored: ["Unknown"]);
		return View(coffee);
	}

	// POST: Coffees/Edit/5
	// To protect from overposting attacks, enable the specific properties you want to bind to.
	// For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
	[HttpPost]
	[ValidateAntiForgeryToken]
	public async Task<IActionResult> Edit(long id, [Bind("Id,Name,Type,Price,CreatedAt,DeletedAt")] Coffee coffee) {
		if (id != coffee.Id) {
			return NotFound();
		}

		if (ModelState.IsValid) {
			try {
				_context.Update(coffee);
				await _context.SaveChangesAsync();
			} catch (DbUpdateConcurrencyException) {
				if (!CoffeeExists(coffee.Id)) {
					return NotFound();
				} else {
					throw;
				}
			}
			return RedirectToAction(nameof(Index));
		}
		return View(coffee);
	}

	// GET: Coffees/Delete/5
	public async Task<IActionResult> Delete(long? id) {
		if (id == null) {
			return NotFound();
		}

		var coffee = await _context.Coffees
			 .FirstOrDefaultAsync(m => m.Id == id);
		if (coffee == null) {
			return NotFound();
		}

		return View(coffee);
	}

	// POST: Coffees/Delete/5
	[HttpPost, ActionName("Delete")]
	[ValidateAntiForgeryToken]
	public async Task<IActionResult> DeleteConfirmed(long id) {
		var coffee = await _context.Coffees.FindAsync(id);
		if (coffee != null) {
			_context.Coffees.Remove(coffee);
		}

		await _context.SaveChangesAsync();
		return RedirectToAction(nameof(Index));
	}

	private bool CoffeeExists(long id) {
		return _context.Coffees.Any(e => e.Id == id);
	}
}
