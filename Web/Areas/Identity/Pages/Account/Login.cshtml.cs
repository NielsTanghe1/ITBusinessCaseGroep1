// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
#nullable disable

using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Models.Entities;
using System.ComponentModel.DataAnnotations;

namespace Web.Areas.Identity.Pages.Account;

public class LoginModel : PageModel {
	private readonly SignInManager<CoffeeUser> _signInManager;
	private readonly ILogger<LoginModel> _logger;
	private readonly UserManager<CoffeeUser> _userManager;

	public LoginModel(
		SignInManager<CoffeeUser> signInManager,
		ILogger<LoginModel> logger,
		UserManager<CoffeeUser> userManager) {
		_signInManager = signInManager;
		_logger = logger;
		_userManager = userManager;
	}

	[BindProperty]
	public InputModel Input {
		get; set;
	}

	public string ReturnUrl {
		get; set;
	}

	[TempData]
	public string ErrorMessage {
		get; set;
	}

	public class InputModel {
		[EmailAddress]
		public required string Email {
			get; set;
		}

		[DataType(DataType.Password)]
		public required string Password {
			get; set;
		}

		[Display(Name = "Remember me?")]
		public bool RememberMe {
			get; set;
		}
	}

	public async Task OnGetAsync(string returnUrl = null) {
		if (!string.IsNullOrEmpty(ErrorMessage)) {
			ModelState.AddModelError(string.Empty, ErrorMessage);
		}

		returnUrl ??= Url.Content("~/");

		// Clear the existing external cookie to ensure a clean login process
		await HttpContext.SignOutAsync(IdentityConstants.ExternalScheme);

		ReturnUrl = returnUrl;
	}

	public async Task<IActionResult> OnPostAsync(string returnUrl = null) {
		returnUrl ??= Url.Content("~/");

		if (ModelState.IsValid) {
			// This doesn't count login failures towards account lockout
			// To enable password failures to trigger account lockout, set lockoutOnFailure: true
			CoffeeUser contextUser = await _userManager.FindByEmailAsync(Input.Email);
			var result = await _signInManager.PasswordSignInAsync(contextUser, Input.Password, Input.RememberMe, lockoutOnFailure: false);

			if (result.Succeeded) {
				_logger.LogInformation("User logged in.");
				return LocalRedirect(returnUrl);
			}

			if (result.RequiresTwoFactor) {
				return RedirectToPage("./LoginWith2fa", new {
					ReturnUrl = returnUrl,
					Input.RememberMe
				});
			}

			if (result.IsLockedOut) {
				_logger.LogWarning("User account locked out.");
				return RedirectToPage("./Lockout");
			} else {
				ModelState.AddModelError(string.Empty, "Invalid login attempt. Check your input email and password.");
				return Page();
			}
		}

		// If we got this far, something failed, redisplay form
		return Page();
	}
}
