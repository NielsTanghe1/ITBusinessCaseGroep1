using Web.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Models.Data;
using Models.Entities; // Needed for UserProfile if it's an entity
using Models.Entities.DTO; // Added namespace

namespace Web.Controllers;

[Authorize]
public class ProfileController : Controller {
	private readonly LocalDbContext _db;
	private readonly UserManager<CoffeeUserDTO> _userManager; // Updated

	public ProfileController(LocalDbContext db, UserManager<CoffeeUserDTO> userManager) {
		_db = db;
		_userManager = userManager;
	}

	// -------------------
	// PROFIEL EDIT
	// -------------------
	[HttpGet]
	public async Task<IActionResult> Edit() {
		return Ok();
	}

	[HttpPost]
	public async Task<IActionResult> Edit(UserProfile model) {
		return Ok();
	}

	// -------------------
	// MIJN BESTELLINGEN
	// -------------------
	public async Task<IActionResult> MyOrders() {
		var user = await _userManager.GetUserAsync(User);
		if (user == null)
			return Challenge();

		var email = user.Email ?? user.UserName ?? "";

		// _db.Orders returns DbSet<OrderDTO> now
		var orders = await _db.Orders
			  .Where(o => o.CoffeeUser.Email == email)
			  .OrderByDescending(o => o.CreatedAt)
			  .ToListAsync();

		return View(orders);
	}

	public async Task<IActionResult> MyOrderDetails(int id) {
		var user = await _userManager.GetUserAsync(User);
		if (user == null)
			return Challenge();

		var email = user.Email ?? user.UserName ?? "";

		var order = await _db.Orders.FirstOrDefaultAsync(o => o.Id == id && o.CoffeeUser.Email == email);
		if (order == null)
			return NotFound();

		return View(order);
	}
}