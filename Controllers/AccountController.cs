using ITBusinessCase.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace ITBusinessCase.Controllers;

public class AccountController : Controller {
	private readonly UserManager<IdentityUser> _userManager;
	private readonly SignInManager<IdentityUser> _signInManager;

	public AccountController(
		 UserManager<IdentityUser> userManager,
		 SignInManager<IdentityUser> signInManager) {
		_userManager = userManager;
		_signInManager = signInManager;
	}

	// --------
	// LOGIN UI
	// --------
	[AllowAnonymous]
	[HttpGet]
	public IActionResult Login() {
		return View(new LoginRegisterViewModel());
	}

	[AllowAnonymous]
	[HttpPost]
	public async Task<IActionResult> Login(LoginRegisterViewModel model) {
		// ✅ Alleen login velden valideren
		ModelState.Remove(nameof(LoginRegisterViewModel.RegisterEmail));
		ModelState.Remove(nameof(LoginRegisterViewModel.RegisterPassword));
		ModelState.Remove(nameof(LoginRegisterViewModel.ConfirmPassword));

		if (!ModelState.IsValid)
			return View(model);

		var result = await _signInManager.PasswordSignInAsync(
			 model.LoginEmail!,
			 model.LoginPassword!,
			 isPersistent: false,
			 lockoutOnFailure: false);

		if (result.Succeeded)
			return RedirectToAction("Index", "Home");

		ModelState.AddModelError("", "Login mislukt. Controleer je email en wachtwoord.");
		return View(model);
	}

	// -----------
	// REGISTER UI
	// -----------
	[AllowAnonymous]
	[HttpGet]
	public IActionResult Register() {
		return View(new LoginRegisterViewModel());
	}

	[AllowAnonymous]
	[HttpPost]
	public async Task<IActionResult> Register(LoginRegisterViewModel model) {
		// ✅ Alleen register velden valideren
		ModelState.Remove(nameof(LoginRegisterViewModel.LoginEmail));
		ModelState.Remove(nameof(LoginRegisterViewModel.LoginPassword));

		if (!ModelState.IsValid)
			return View(model);

		var user = new IdentityUser {
			UserName = model.RegisterEmail,
			Email = model.RegisterEmail
		};

		var result = await _userManager.CreateAsync(user, model.RegisterPassword!);

		if (result.Succeeded) {
			await _signInManager.SignInAsync(user, isPersistent: false);
			return RedirectToAction("Index", "Home");
		}

		foreach (var error in result.Errors)
			ModelState.AddModelError("", error.Description);

		return View(model);
	}

	// ------
	// LOGOUT
	// ------
	[Authorize]
	[HttpPost]
	public async Task<IActionResult> Logout() {
		await _signInManager.SignOutAsync();
		return RedirectToAction("Login");
	}
}
