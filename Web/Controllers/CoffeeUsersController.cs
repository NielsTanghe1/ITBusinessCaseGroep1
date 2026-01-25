using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Models.Data;
using Models.Entities;

namespace Web.Controllers;

public class CoffeeUsersController : Controller {
	private readonly LocalDbContext _context;

	public CoffeeUsersController(LocalDbContext context) {
		_context = context;
	}

	// GET: CoffeeUsers
	public async Task<IActionResult> Index() {
		return View(await _context.Users.ToListAsync());
	}

	// GET: CoffeeUsers/Details/5
	public async Task<IActionResult> Details(long? id) {
		if (id == null) {
			return NotFound();
		}

		var coffeeUser = await _context.Users
			.FirstOrDefaultAsync(m => m.Id == id);
		if (coffeeUser == null) {
			return NotFound();
		}

		return View(coffeeUser);
	}

	// GET: CoffeeUsers/Create
	public IActionResult Create() {
		return View();
	}

	// POST: CoffeeUsers/Create
	// To protect from overposting attacks, enable the specific properties you want to bind to.
	// For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
	[HttpPost]
	[ValidateAntiForgeryToken]
	// Changed parameter to CoffeeUserDTO
	public async Task<IActionResult> Create([Bind("FirstName,LastName,GlobalId,CreatedAt,DeletedAt,Id,UserName,NormalizedUserName,Email,NormalizedEmail,EmailConfirmed,PasswordHash,SecurityStamp,ConcurrencyStamp,PhoneNumber,PhoneNumberConfirmed,TwoFactorEnabled,LockoutEnd,LockoutEnabled,AccessFailedCount")] CoffeeUser coffeeUser) {
		if (ModelState.IsValid) {
			// Ensure GlobalId is set
			coffeeUser.GlobalId = Random.Shared.NextInt64();

			_context.Add(coffeeUser);
			await _context.SaveChangesAsync();
			return RedirectToAction(nameof(Index));
		}
		return View(coffeeUser);
	}

	// GET: CoffeeUsers/Edit/5
	public async Task<IActionResult> Edit(long? id) {
		if (id == null) {
			return NotFound();
		}

		var coffeeUser = await _context.Users.FindAsync(id);
		if (coffeeUser == null) {
			return NotFound();
		}
		return View(coffeeUser);
	}

	// POST: CoffeeUsers/Edit/5
	// To protect from overposting attacks, enable the specific properties you want to bind to.
	// For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
	[HttpPost]
	[ValidateAntiForgeryToken]
	// Changed parameter to CoffeeUserDTO
	public async Task<IActionResult> Edit(long id, [Bind("FirstName,LastName,GlobalId,CreatedAt,DeletedAt,Id,UserName,NormalizedUserName,Email,NormalizedEmail,EmailConfirmed,PasswordHash,SecurityStamp,ConcurrencyStamp,PhoneNumber,PhoneNumberConfirmed,TwoFactorEnabled,LockoutEnd,LockoutEnabled,AccessFailedCount")] CoffeeUser coffeeUser) {
		if (id != coffeeUser.Id) {
			return NotFound();
		}

		if (ModelState.IsValid) {
			try {
				_context.Update(coffeeUser);
				await _context.SaveChangesAsync();
			} catch (DbUpdateConcurrencyException) {
				if (!CoffeeUserExists(coffeeUser.Id)) {
					return NotFound();
				} else {
					throw;
				}
			}
			return RedirectToAction(nameof(Index));
		}
		return View(coffeeUser);
	}

	// GET: CoffeeUsers/Delete/5
	public async Task<IActionResult> Delete(long? id) {
		if (id == null) {
			return NotFound();
		}

		var coffeeUser = await _context.Users
			  .FirstOrDefaultAsync(m => m.Id == id);
		if (coffeeUser == null) {
			return NotFound();
		}

		return View(coffeeUser);
	}

	// POST: CoffeeUsers/Delete/5
	[HttpPost, ActionName("Delete")]
	[ValidateAntiForgeryToken]
	public async Task<IActionResult> DeleteConfirmed(long id) {
		var coffeeUser = await _context.Users.FindAsync(id);
		if (coffeeUser != null) {
			_context.Users.Remove(coffeeUser);
		}

		await _context.SaveChangesAsync();
		return RedirectToAction(nameof(Index));
	}

	private bool CoffeeUserExists(long id) {
		return _context.Users.Any(e => e.Id == id);
	}
}
