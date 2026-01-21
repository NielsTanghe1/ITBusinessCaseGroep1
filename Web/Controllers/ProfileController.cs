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
	private readonly LocalDbContext _db;
	private readonly UserManager<CoffeeUser> _userManager;

	public ProfileController(LocalDbContext db, UserManager<CoffeeUser> userManager) {
		_db = db;
		_userManager = userManager;
	}

	// -------------------
	// PROFIEL EDIT
	// -------------------
	[HttpGet]
	public async Task<IActionResult> Edit() {
		//var user = await _userManager.GetUserAsync(User);
		//if (user == null)
		//	return Challenge();

		//var profile = await _db.UserProfiles.FirstOrDefaultAsync(p => p.UserId == user.Id);

		//if (profile == null) {
		//	profile = new UserProfile {
		//		UserId = user.Id,
		//		Email = user.Email ?? user.UserName ?? ""
		//	};
		//}

		//return View(profile);
		return Ok();
	}

	[HttpPost]
	public async Task<IActionResult> Edit(UserProfile model) {
		//var user = await _userManager.GetUserAsync(User);
		//if (user == null)
		//	return Challenge();

		//// Zorg dat niemand een andere userId post
		//model.UserId = user.Id;

		//if (!ModelState.IsValid)
		//	return View(model);

		//var existing = await _db.UserProfiles.FirstOrDefaultAsync(p => p.UserId == user.Id);

		//if (existing == null) {
		//	_db.UserProfiles.Add(model);
		//} else {
		//	existing.FirstName = model.FirstName;
		//	existing.LastName = model.LastName;
		//	existing.Email = model.Email;
		//	existing.Street = model.Street;
		//	existing.Postbus = model.Postbus;
		//	existing.City = model.City;
		//	existing.Postcode = model.Postcode;
		//	existing.Country = model.Country;
		//}

		//// (optioneel) sync Identity email
		//if (!string.IsNullOrWhiteSpace(model.Email) && user.Email != model.Email) {
		//	user.Email = model.Email;
		//	user.UserName = model.Email;
		//	await _userManager.UpdateAsync(user);
		//}

		//await _db.SaveChangesAsync();

		//TempData["ProfileSaved"] = "Profiel opgeslagen!";
		//return RedirectToAction(nameof(Edit));
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
