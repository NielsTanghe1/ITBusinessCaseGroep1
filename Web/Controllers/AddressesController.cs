using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Models.Data;
using Models.Entities;
using Models.Entities.Enums;
using Web.Services;

namespace Web.Controllers;

public class AddressesController : Controller {
	private readonly LocalDbContext _context;
	private readonly Utilities _utilities;

	public AddressesController(LocalDbContext context, Utilities utilities) {
		_context = context;
		_utilities = utilities;
	}

	// GET: Addresses
	public async Task<IActionResult> Index() {
		var localDbContext = _context.Addresses.Include(a => a.CoffeeUser);
		return View(await localDbContext.ToListAsync());
	}

	// GET: Addresses/Details/5
	public async Task<IActionResult> Details(long? id) {
		if (id == null) {
			return NotFound();
		}

		var address = await _context.Addresses
			 .Include(a => a.CoffeeUser)
			 .FirstOrDefaultAsync(m => m.Id == id);
		if (address == null) {
			return NotFound();
		}

		return View(address);
	}

	// GET: Addresses/Create
	public IActionResult Create() {
		ViewData["CoffeeUserId"] = new SelectList(_context.Users, "Id", "FirstName");
		ViewData["AddressTypes"] = _utilities.GetEnumSelectList<AddressType>(ignored: ["Unknown"]);
		return View();
	}

	// POST: Addresses/Create
	// To protect from overposting attacks, enable the specific properties you want to bind to.
	// For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
	[HttpPost]
	[ValidateAntiForgeryToken]
	public async Task<IActionResult> Create([Bind("Id,CoffeeUserId,Type,Street,HouseNumber,City,PostalCode,CountryISO,UnitNumber,CreatedAt,DeletedAt")] Address address) {
		if (ModelState.IsValid) {
			_context.Add(address);
			await _context.SaveChangesAsync();
			return RedirectToAction(nameof(Index));
		}
		ViewData["CoffeeUserId"] = new SelectList(_context.Users, "Id", "FirstName", address.CoffeeUserId);
		return View(address);
	}

	// GET: Addresses/Edit/5
	public async Task<IActionResult> Edit(long? id) {
		if (id == null) {
			return NotFound();
		}

		var address = await _context.Addresses.FindAsync(id);
		if (address == null) {
			return NotFound();
		}
		ViewData["CoffeeUserId"] = new SelectList(_context.Users, "Id", "FirstName", address.CoffeeUserId);
		ViewData["AddressTypes"] = _utilities.GetEnumSelectList<AddressType>(ignored: ["Unknown"]);
		return View(address);
	}

	// POST: Addresses/Edit/5
	// To protect from overposting attacks, enable the specific properties you want to bind to.
	// For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
	[HttpPost]
	[ValidateAntiForgeryToken]
	public async Task<IActionResult> Edit(long id, [Bind("Id,CoffeeUserId,Type,Street,HouseNumber,City,PostalCode,CountryISO,UnitNumber,CreatedAt,DeletedAt")] Address address) {
		if (id != address.Id) {
			return NotFound();
		}

		if (ModelState.IsValid) {
			try {
				_context.Update(address);
				await _context.SaveChangesAsync();
			} catch (DbUpdateConcurrencyException) {
				if (!AddressExists(address.Id)) {
					return NotFound();
				} else {
					throw;
				}
			}
			return RedirectToAction(nameof(Index));
		}
		ViewData["CoffeeUserId"] = new SelectList(_context.Users, "Id", "FirstName", address.CoffeeUserId);
		return View(address);
	}

	// GET: Addresses/Delete/5
	public async Task<IActionResult> Delete(long? id) {
		if (id == null) {
			return NotFound();
		}

		var address = await _context.Addresses
			 .Include(a => a.CoffeeUser)
			 .FirstOrDefaultAsync(m => m.Id == id);
		if (address == null) {
			return NotFound();
		}

		return View(address);
	}

	// POST: Addresses/Delete/5
	[HttpPost, ActionName("Delete")]
	[ValidateAntiForgeryToken]
	public async Task<IActionResult> DeleteConfirmed(long id) {
		var address = await _context.Addresses.FindAsync(id);
		if (address != null) {
			_context.Addresses.Remove(address);
		}

		await _context.SaveChangesAsync();
		return RedirectToAction(nameof(Index));
	}

	private bool AddressExists(long id) {
		return _context.Addresses.Any(e => e.Id == id);
	}
}
