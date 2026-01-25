// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
#nullable disable

using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using Models.Data;
using Models.Entities;
using Models.Extensions.Mappings;
using System.ComponentModel.DataAnnotations;

namespace Web.Areas.Identity.Pages.Account;

public class RegisterModel : PageModel {
	private readonly SignInManager<CoffeeUser> _signInManager;
	private readonly UserManager<CoffeeUser> _userManager;
	private readonly IUserStore<CoffeeUser> _userStore;
	private readonly IUserEmailStore<CoffeeUser> _emailStore;
	private readonly ILogger<RegisterModel> _logger;
	private readonly LocalDbContext _contextLocal;
	private readonly GlobalDbContext _contextGlobal;

	public RegisterModel(
		UserManager<CoffeeUser> userManager,
		IUserStore<CoffeeUser> userStore,
		SignInManager<CoffeeUser> signInManager,
		ILogger<RegisterModel> logger,
		LocalDbContext contextLocal,
		GlobalDbContext contextGlobal) {
		_userManager = userManager;
		_userStore = userStore;
		_emailStore = GetEmailStore();
		_signInManager = signInManager;
		_logger = logger;
		_contextLocal = contextLocal;
		_contextGlobal = contextGlobal;
	}

	[BindProperty]
	public InputModel Input {
		get; set;
	}
	public string ReturnUrl {
		get; set;
	}

	public class InputModel {
		[StringLength(35, MinimumLength = 2)]
		[Display(Name = "FirstName")]
		public required string FirstName {
			get; set;
		}

		[StringLength(35, MinimumLength = 2)]
		[Display(Name = "LastName")]
		public required string LastName {
			get; set;
		}

		[EmailAddress]
		[Display(Name = "Email")]
		public required string Email {
			get; set;
		}

		[StringLength(100, ErrorMessage = "The {0} must be at least {2} and at max {1} characters long.", MinimumLength = 6)]
		[DataType(DataType.Password)]
		[Display(Name = "Password")]
		public required string Password {
			get; set;
		}

		[DataType(DataType.Password)]
		[Display(Name = "Confirm password")]
		[Compare("Password", ErrorMessage = "The password and confirmation password do not match.")]
		public string ConfirmPassword {
			get; set;
		}
	}

	public void OnGet(string returnUrl = null) {
		ReturnUrl = returnUrl;
	}

	public async Task<IActionResult> OnPostAsync(string returnUrl = null) {
		/* 
		 * PROCESS: 
		 * 1. Create a local CoffeeUser account.
		 * 2. Synchronize to backup database to obtain a GlobalID.
		 * 3. Finalize local account with Global link and establish session.
		 * 
		 * ISSUES:
		 * 1. Breaks if the backup isn't available.
		 * 2. Breaks if the local database isn't available.
		 */
		returnUrl ??= Url.Content("~/");

		if (!ModelState.IsValid) {
			return Page();
		}

		var localUser = CreateUserFromInput();
		var result = await _userManager.CreateAsync(localUser, Input.Password);

		if (result.Succeeded) {
			_logger.LogInformation("User created a new account with password.");

			// 1. Create and save global entity
			var globalUser = localUser.ShallowCopy();
			_contextGlobal.Users.Add(globalUser);
			await _contextGlobal.SaveChangesAsync();

			// 2. Link GlobalId back to Local User
			localUser.GlobalId = globalUser.Id;
			var updateResult = await _userManager.UpdateAsync(localUser);

			if (updateResult.Succeeded) {
				// 3. Only sign in once BOTH databases are consistent
				await _signInManager.SignInAsync(localUser, isPersistent: false);
				return LocalRedirect(returnUrl);
			}

			foreach (var error in updateResult.Errors) {
				ModelState.AddModelError(string.Empty, error.Description);
			}
		}

		foreach (var error in result.Errors) {
			ModelState.AddModelError(string.Empty, error.Description);
		}

		// If we got this far, something failed, redisplay form
		return Page();
	}

	private CoffeeUser CreateUserFromInput() {
		try {
			return new() {
				UserName = $"{Input.FirstName.ToLower()}.{Input.LastName.ToLower()}",
				Email = Input.Email,
				FirstName = Input.FirstName,
				LastName = Input.LastName,
				EmailConfirmed = true
			};
		} catch {
			throw new InvalidOperationException($"Can't create an instance of '{nameof(CoffeeUser)}'. " +
				$"Ensure that '{nameof(CoffeeUser)}' is not an abstract class and has a parameterless constructor, or alternatively " +
				$"override the register page in /Areas/Identity/Pages/Account/Register.cshtml");
		}
	}

	private IUserEmailStore<CoffeeUser> GetEmailStore() {
		if (!_userManager.SupportsUserEmail) {
			throw new NotSupportedException("The default UI requires a user store with email support.");
		}
		return (IUserEmailStore<CoffeeUser>) _userStore;
	}
}
