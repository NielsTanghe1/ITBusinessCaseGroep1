// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
#nullable disable

using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Models.Data;
using Models.Entities;
using Models.Entities.Enums;
using System.ComponentModel.DataAnnotations;
using Web.Services;

namespace Web.Areas.Identity.Pages.Account.Manage;
public class IndexModel : PageModel {
	private readonly UserManager<CoffeeUser> _userManager;
	private readonly SignInManager<CoffeeUser> _signInManager;
	private readonly LocalDbContext _localContext;
	private readonly GlobalDbContext _globalContext;
	private readonly Utilities _utilities;

	public IndexModel(
		UserManager<CoffeeUser> userManager,
		SignInManager<CoffeeUser> signInManager,
		LocalDbContext localContext,
		GlobalDbContext globalContext,
		Utilities utilities) {
		_userManager = userManager;
		_signInManager = signInManager;
		_localContext = localContext;
		_globalContext = globalContext;
		_utilities = utilities;
	}

	public string Username {
		get; set;
	}

	[TempData]
	public string StatusMessage {
		get; set;
	}

	[BindProperty]
	public InputModel Input {
		get; set;
	}

	public class InputModel {
		[Display(Name = "FirstName")]
		public string FirstName {
			get; set;
		} = "";

		[Display(Name = "LastName")]
		public string LastName {
			get; set;
		} = "";

		[EmailAddress]
		[Display(Name = "Email")]
		public string Email {
			get; set;
		} = "";

		[Phone]
		[Display(Name = "PhoneNumber")]
		public string PhoneNumber {
			get; set;
		} = "";

		[Display(Name = "Type")]
		public AddressType? AddressType {
			get; set;
		}

		[StringLength(50)]
		[Display(Name = "Street")]
		public string Street {
			get; set;
		} = "";

		[Range(1, 1000000)]
		[Display(Name = "HouseNumber")]
		public int? HouseNumber {
			get; set;
		}

		[StringLength(20)]
		[Display(Name = "City")]
		public string City {
			get; set;
		} = "";

		[StringLength(10, MinimumLength = 2)]
		[Display(Name = "PostalCode")]
		public string PostalCode {
			get; set;
		} = "";

		[StringLength(3, MinimumLength = 2)]
		[Display(Name = "CountryISO")]
		public string CountryISO {
			get; set;
		} = "";

		[StringLength(10)]
		[Display(Name = "UnitNumber")]
		public string UnitNumber {
			get; set;
		} = "";
	}

	private async Task LoadAsync(CoffeeUser user) {
		var userName = await _userManager.GetUserNameAsync(user);
		var phoneNumber = await _userManager.GetPhoneNumberAsync(user);

		Username = userName ?? string.Empty;

		Input = new InputModel {
			PhoneNumber = phoneNumber
		};

		ViewData["AddressTypes"] = _utilities.GetEnumSelectList<StatusType>(ignored: ["Unknown"]);
	}

	public async Task<IActionResult> OnGetAsync() {
		var user = await _userManager.GetUserAsync(User);
		if (user == null) {
			return NotFound($"Unable to load user with ID '{_userManager.GetUserId(User)}'.");
		}

		await LoadAsync(user);
		return Page();
	}

	public async Task<IActionResult> OnPostAsync() {
		var user = await _userManager.GetUserAsync(User);
		var address = _localContext.Addresses.First(e => e.CoffeeUserId == user.Id);
		if (user == null) {
			return NotFound($"Unable to load user with ID '{_userManager.GetUserId(User)}'.");
		}

		if (!ModelState.IsValid) {
			await LoadAsync(user);
			return Page();
		}

		if (!string.IsNullOrEmpty(Input.FirstName) && Input.FirstName != user.FirstName) {
			user.FirstName = Input.FirstName;
			var setResult = await _userManager.UpdateAsync(user);
			if (!setResult.Succeeded) {
				StatusMessage = "Unexpected error when trying to set the first name.";
				return RedirectToPage();
			}
		}

		if (!string.IsNullOrEmpty(Input.LastName) && Input.LastName != user.LastName) {
			user.LastName = Input.LastName;
			var setResult = await _userManager.UpdateAsync(user);
			if (!setResult.Succeeded) {
				StatusMessage = "Unexpected error when trying to set the last name.";
				return RedirectToPage();
			}
		}

		if (!string.IsNullOrEmpty(Input.Email) && Input.Email != user.Email) {
			user.Email = Input.Email;
			var setResult = await _userManager.UpdateAsync(user);
			if (!setResult.Succeeded) {
				StatusMessage = "Unexpected error when trying to set the email.";
				return RedirectToPage();
			}
		}

		var phoneNumber = await _userManager.GetPhoneNumberAsync(user);
		if (Input.PhoneNumber != phoneNumber) {
			var setPhoneResult = await _userManager.SetPhoneNumberAsync(user, Input.PhoneNumber);
			if (!setPhoneResult.Succeeded) {
				StatusMessage = "Unexpected error when trying to set the phone number.";
				return RedirectToPage();
			}
		}

		try {
			if (address != null) {
				if (Input.AddressType != null && Input.AddressType != address.Type) {
					address.Type = (AddressType) Input.AddressType;
				}

				if (!string.IsNullOrEmpty(Input.Street) && Input.Street != address.Street) {
					address.Street =  Input.Street;
				}

				if (Input.HouseNumber != null && Input.HouseNumber != address.HouseNumber) {
					address.HouseNumber = (int) Input.HouseNumber;
				}

				if (!string.IsNullOrEmpty(Input.City) && Input.City != address.City) {
					address.City = Input.City;
				}

				if (!string.IsNullOrEmpty(Input.PostalCode) && Input.PostalCode != address.PostalCode) {
					address.PostalCode = Input.PostalCode;
				}

				if (!string.IsNullOrEmpty(Input.CountryISO) && Input.CountryISO != address.CountryISO) {
					address.CountryISO = Input.CountryISO;
				}

				if (!string.IsNullOrEmpty(Input.UnitNumber) && Input.UnitNumber != address.UnitNumber) {
					address.UnitNumber = Input.UnitNumber;
				}
				address = _localContext.Addresses.Update(address).Entity;
				await _localContext.SaveChangesAsync();
			}
		} catch {
			StatusMessage = "Unexpected error when trying to update the address information.";
			return RedirectToPage();
		}

		await _signInManager.RefreshSignInAsync(user);
		StatusMessage = "Your profile has been updated";
		return RedirectToPage();
	}
}
