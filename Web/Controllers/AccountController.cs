using Web.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Models.Data;
using Models.Entities;

namespace Web.Controllers;

public class AccountController : Controller {
	private readonly UserManager<CoffeeUser> _userManager;
	private readonly SignInManager<CoffeeUser> _signInManager;
	private readonly LocalDbContext _context;

	public AccountController(
		 UserManager<CoffeeUser> userManager,
		 SignInManager<CoffeeUser> signInManager,
		 LocalDbContext context) {
		_userManager = userManager;
		_signInManager = signInManager;
		_context = context;
	}

	[AllowAnonymous]
	[HttpGet]
	public IActionResult LoginRegister() {
		return View(new LoginRegisterViewModel());
	}

	[AllowAnonymous]
	[HttpPost]
	public async Task<IActionResult> Login(LoginRegisterViewModel model) {
		// Alleen login velden relevant
		ModelState.Remove(nameof(LoginRegisterViewModel.RegisterEmail));
		ModelState.Remove(nameof(LoginRegisterViewModel.RegisterPassword));
		ModelState.Remove(nameof(LoginRegisterViewModel.ConfirmPassword));
		ModelState.Remove(nameof(LoginRegisterViewModel.FirstName));
		ModelState.Remove(nameof(LoginRegisterViewModel.LastName));
		ModelState.Remove(nameof(LoginRegisterViewModel.Street));
		ModelState.Remove(nameof(LoginRegisterViewModel.Postbus));
		ModelState.Remove(nameof(LoginRegisterViewModel.City));
		ModelState.Remove(nameof(LoginRegisterViewModel.Postcode));
		ModelState.Remove(nameof(LoginRegisterViewModel.Country));

		if (!ModelState.IsValid)
			return View("LoginRegister", model);
		var user = await _userManager.FindByEmailAsync(model.LoginEmail);
		if (user == null) {
			return View("LoginRegister", model);
		}
		var result = await _signInManager.PasswordSignInAsync(
			 user,
			 model.LoginPassword!,
			 isPersistent: false,
			 lockoutOnFailure: false);

		if (result.Succeeded)
			return RedirectToAction("Index", "Home");

		ModelState.AddModelError("", "Login mislukt. Controleer je email en wachtwoord.");
		return View("LoginRegister", model);
	}

	[AllowAnonymous]
	[HttpPost]
	public async Task<IActionResult> Register(LoginRegisterViewModel model) {
		//// Alleen register velden relevant
		//ModelState.Remove(nameof(LoginRegisterViewModel.LoginEmail));
		//ModelState.Remove(nameof(LoginRegisterViewModel.LoginPassword));

		//if (!ModelState.IsValid)
		//    return View("LoginRegister", model);

		//var user = new IdentityUser
		//{
		//    UserName = model.RegisterEmail,
		//    Email = model.RegisterEmail
		//};

		//var result = await _userManager.CreateAsync(user, model.RegisterPassword!);

		//if (!result.Succeeded)
		//{
		//    foreach (var error in result.Errors)
		//        ModelState.AddModelError("", error.Description);

		//    return View("LoginRegister", model);
		//}

		//// ✅ Profiel opslaan (prefill checkout)
		//var profile = await _context.UserProfiles.FirstOrDefaultAsync(p => p.UserId == user.Id);
		//if (profile == null)
		//{
		//    profile = new UserProfile { UserId = user.Id };
		//    _context.UserProfiles.Add(profile);
		//}

		//profile.Email = model.RegisterEmail ?? "";
		//profile.FirstName = model.FirstName ?? "";
		//profile.LastName = model.LastName ?? "";
		//profile.Street = model.Street ?? "";
		//profile.Postbus = model.Postbus ?? "";
		//profile.City = model.City ?? "";
		//profile.Postcode = model.Postcode ?? "";
		//profile.Country = model.Country ?? "";

		//await _context.SaveChangesAsync();

		//await _signInManager.SignInAsync(user, isPersistent: false);
		//return RedirectToAction("Index", "Home");
		return Ok();
	}

	[Authorize]
	[HttpPost]
	public async Task<IActionResult> Logout() {
		await _signInManager.SignOutAsync();
		return RedirectToAction(nameof(LoginRegister));
	}
}
