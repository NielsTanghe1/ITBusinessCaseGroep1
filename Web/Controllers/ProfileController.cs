using Web.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Models.Data;
using Models.Entities;

namespace Web.Controllers;

[Authorize]
public class ProfileController : Controller {
	private readonly LocalDbContext _context;
	private readonly UserManager<CoffeeUser> _userManager;

	public ProfileController(LocalDbContext context, UserManager<CoffeeUser> userManager) {
		_context = context;
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
		var orders = await _context.Orders
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

		var order = await _context.Orders.FirstOrDefaultAsync(o => o.Id == id && o.CoffeeUser.Email == email);
		if (order == null)
			return NotFound();

		return View(order);
	}
}