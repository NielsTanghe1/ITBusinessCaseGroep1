using Web.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Models.Data;
using Models.Entities.DTO; // Added namespace

namespace Web.Controllers;

public class AccountController : Controller {
	// Updated types to CoffeeUserDTO
	private readonly UserManager<CoffeeUserDTO> _userManager;
	private readonly SignInManager<CoffeeUserDTO> _signInManager;
	private readonly LocalDbContext _context;

	public AccountController(
		  UserManager<CoffeeUserDTO> userManager,
		  SignInManager<CoffeeUserDTO> signInManager,
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
		// Since the register logic is commented out in your file, 
		// I am leaving it as is. If you uncomment it later, ensure you:
		// 1. Use 'new CoffeeUserDTO' instead of 'new IdentityUser'
		// 2. Set 'GlobalId = Random.Shared.NextInt64()'
		return Ok();
	}

	[Authorize]
	[HttpPost]
	public async Task<IActionResult> Logout() {
		await _signInManager.SignOutAsync();
		return RedirectToAction(nameof(LoginRegister));
	}
}