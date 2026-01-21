using MassTransit;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Models.Data;
using Models.Entities;
using Web.Models;

namespace Web.Controllers;

public class AccountController : Controller {
	private readonly UserManager<CoffeeUser> _userManager;
	private readonly SignInManager<CoffeeUser> _signInManager;
	private readonly ISendEndpointProvider _sendEndpointProvider;

	public AccountController(
		 UserManager<CoffeeUser> userManager,
		 SignInManager<CoffeeUser> signInManager,
		 ISendEndpointProvider sendEndpointProvider) {
		_userManager = userManager;
		_signInManager = signInManager;
		_sendEndpointProvider = sendEndpointProvider;
	}

	// =========================
	// LOGIN + REGISTER PAGE
	// =========================
	[AllowAnonymous]
	[HttpGet]
	public IActionResult LoginRegister(string? returnUrl = null) {
		ViewData["ReturnUrl"] = returnUrl;
		return View(new LoginRegisterViewModel());
	}

	// =========================
	// LOGIN
	// =========================
	[AllowAnonymous]
	[HttpPost]
	[ValidateAntiForgeryToken]
	public async Task<IActionResult> Login(LoginRegisterViewModel model, string? returnUrl = null) {
		ViewData["ReturnUrl"] = returnUrl;

		// Alleen login velden valideren
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

		var result = await _signInManager.PasswordSignInAsync(
			 model.LoginEmail!,
			 model.LoginPassword!,
			 isPersistent: false,
			 lockoutOnFailure: false);

		if (result.Succeeded) {
			if (!string.IsNullOrWhiteSpace(returnUrl) && Url.IsLocalUrl(returnUrl))
				return Redirect(returnUrl);

			return RedirectToAction("Index", "Home");
		}

		ModelState.AddModelError("", "Login mislukt. Controleer je email/wachtwoord.");
		return View("LoginRegister", model);
	}

	// =========================
	// REGISTER
	// =========================
	[AllowAnonymous]
	[HttpPost]
	[ValidateAntiForgeryToken]
	public async Task<IActionResult> Register(LoginRegisterViewModel model) {
		// Alleen register velden valideren
		ModelState.Remove(nameof(LoginRegisterViewModel.LoginEmail));
		ModelState.Remove(nameof(LoginRegisterViewModel.LoginPassword));

		if (!ModelState.IsValid)
			return View("LoginRegister", model);

		// ✅ CoffeeUser (long identity) + required FirstName/LastName invullen
		var user = new CoffeeUser {
			UserName = model.RegisterEmail,
			Email = model.RegisterEmail,

			FirstName = model.FirstName ?? "",
			LastName = model.LastName ?? "",


		};

		var createResult = await _userManager.CreateAsync(user, model.RegisterPassword!);

		if (!createResult.Succeeded) {
			foreach (var err in createResult.Errors)
				ModelState.AddModelError("", err.Description);

			return View("LoginRegister", model);
		}

		// ✅ (OPTIONEEL) Stuur naar RabbitMQ queue AccountSubmitted
		// Zet dit blok UIT als je (nog) geen AccountSubmitted contract hebt.
		try {
			var endpoint = await _sendEndpointProvider.GetSendEndpoint(new Uri("queue:AccountSubmitted"));

			// Als je dit contract al hebt, pas de namespace hieronder aan:
			// using Web.Contracts;
			await endpoint.Send(new {
				CoffeeUserId = user.Id,
				Email = user.Email ?? "",
				FirstName = user.FirstName,
				LastName = user.LastName,
				Street = model.Street ?? "",
				Postbus = model.Postbus ?? "",
				City = model.City ?? "",
				Postcode = model.Postcode ?? "",
				Country = model.Country ?? ""
			});
		} catch {
			// We willen register niet laten falen als RabbitMQ even down is.
			// Als je wél hard wil falen, verwijder deze try/catch.
		}

		await _signInManager.SignInAsync(user, isPersistent: false);
		return RedirectToAction("Index", "Home");
	}

	// =========================
	// LOGOUT
	// =========================
	[Authorize]
	[HttpPost]
	[ValidateAntiForgeryToken]
	public async Task<IActionResult> Logout() {
		await _signInManager.SignOutAsync();
		return RedirectToAction(nameof(LoginRegister));
	}
}
