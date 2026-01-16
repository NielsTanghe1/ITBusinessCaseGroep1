using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace ITBusinessCase.Controllers;

[AllowAnonymous]
public class AccountController : Controller {
	[HttpGet]
	public IActionResult Login(string? returnUrl = null) {
		ViewBag.ReturnUrl = returnUrl;
		return View();
	}

	[HttpPost]
	[ValidateAntiForgeryToken]
	public async Task<IActionResult> Login(string username, string password, string? returnUrl = null) {
		ViewBag.ReturnUrl = returnUrl;

		username = (username ?? "").Trim();

		// ✅ Demo login check (pas dit aan als je meerdere accounts wil)
		// Voorbeeld: je laat iedereen inloggen met password "1234"
		if (!string.IsNullOrWhiteSpace(username) && password == "1234") {
			// ✅ Unieke en stabiele UserId per username
			var userId = CreateStableUserId(username); // string GUID

			var claims = new List<Claim>
			{
					 new Claim(ClaimTypes.NameIdentifier, userId), // ✅ unieke UserId
                new Claim(ClaimTypes.Name, username),         // ✅ username
                new Claim(ClaimTypes.Role, "User")
				};

			var identity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);
			var principal = new ClaimsPrincipal(identity);

			await HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, principal);

			if (!string.IsNullOrWhiteSpace(returnUrl) && Url.IsLocalUrl(returnUrl))
				return Redirect(returnUrl);

			return RedirectToAction("Index", "Home");
		}

		ViewBag.Error = "Foute gebruikersnaam of wachtwoord.";
		return View();
	}

	[HttpPost]
	[ValidateAntiForgeryToken]
	public async Task<IActionResult> Logout() {
		await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
		return RedirectToAction("Login", "Account");
	}

	// ✅ Maakt een GUID-achtige UserId die altijd dezelfde blijft voor dezelfde username
	private static string CreateStableUserId(string username) {
		// Normalize zodat "Robin" en "robin" dezelfde id geven
		var normalized = username.Trim().ToLowerInvariant();

		// Hash -> 16 bytes -> Guid
		using var sha = SHA256.Create();
		var hash = sha.ComputeHash(Encoding.UTF8.GetBytes(normalized));

		var guidBytes = new byte[16];
		Array.Copy(hash, guidBytes, 16);

		var guid = new Guid(guidBytes);
		return guid.ToString();
	}
}
