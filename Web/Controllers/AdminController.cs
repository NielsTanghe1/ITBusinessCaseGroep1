using System.Text.Json;
using Models.Data;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Models.Entities;

namespace Web.Controllers;

[Authorize(Roles = "Admin")]
public class AdminController : Controller {
	private readonly UserManager<CoffeeUser> _userManager;
	private readonly RoleManager<IdentityRole<long>> _roleManager;
	private readonly LocalDbContext _db;

	public AdminController(
		UserManager<CoffeeUser> userManager,
		RoleManager<IdentityRole<long>> roleManager,
		LocalDbContext db) {
		_userManager = userManager;
		_roleManager = roleManager;
		_db = db;
	}

	public IActionResult Index() => View();

	public async Task<IActionResult> Users() {
		var users = await _userManager.Users
			  .OrderBy(u => u.Email)
			  .ToListAsync();

		// 3. Update Tuple type to use CoffeeUserDTO
		var vm = new List<(CoffeeUser user, IList<string> roles)>();
		foreach (var u in users)
			vm.Add((u, await _userManager.GetRolesAsync(u)));

		return View(vm);
	}

	[HttpGet]
	public IActionResult CreateUser() => View(new CreateUserViewModel());

	[HttpPost]
	public async Task<IActionResult> CreateUser(CreateUserViewModel model) {
		if (!ModelState.IsValid)
			return View(model);

		// 4. Instantiate CoffeeUserDTO instead of CoffeeUser
		var user = new CoffeeUser {
			FirstName = model.FirstName,
			LastName = model.LastName,
			UserName = model.Email,
			Email = model.Email,
			EmailConfirmed = true,
			GlobalId = Random.Shared.NextInt64() // Generate a GlobalId
		};

		var result = await _userManager.CreateAsync(user, model.Password);
		if (!result.Succeeded) {
			foreach (var e in result.Errors)
				ModelState.AddModelError("", e.Description);
			return View(model);
		}

		if (!string.IsNullOrWhiteSpace(model.Role)) {
			if (!await _roleManager.RoleExistsAsync(model.Role))
				await _roleManager.CreateAsync(new IdentityRole<long>(model.Role));

			await _userManager.AddToRoleAsync(user, model.Role);
		}

		return RedirectToAction(nameof(Users));
	}

	[HttpPost]
	public async Task<IActionResult> DeleteUser(string id) {
		var user = await _userManager.FindByIdAsync(id);
		if (user != null)
			await _userManager.DeleteAsync(user);

		return RedirectToAction(nameof(Users));
	}

	// -------------------
	// ROLES
	// -------------------
	public async Task<IActionResult> Roles() {
		var roles = await _roleManager.Roles.OrderBy(r => r.Name).ToListAsync();
		return View(roles);
	}

	[HttpPost]
	public async Task<IActionResult> CreateRole(string roleName) {
		if (!string.IsNullOrWhiteSpace(roleName) && !await _roleManager.RoleExistsAsync(roleName))
			await _roleManager.CreateAsync(new IdentityRole<long>(roleName));

		return RedirectToAction(nameof(Roles));
	}

	[HttpPost]
	public async Task<IActionResult> AddRoleToUser(string userId, string roleName) {
		var user = await _userManager.FindByIdAsync(userId);
		if (user != null) {
			if (!await _roleManager.RoleExistsAsync(roleName))
				await _roleManager.CreateAsync(new IdentityRole<long>(roleName));

			await _userManager.AddToRoleAsync(user, roleName);
		}
		return RedirectToAction(nameof(Users));
	}

	[HttpPost]
	public async Task<IActionResult> RemoveRoleFromUser(string userId, string roleName) {
		var user = await _userManager.FindByIdAsync(userId);
		if (user != null)
			await _userManager.RemoveFromRoleAsync(user, roleName);

		return RedirectToAction(nameof(Users));
	}

	// -------------------
	// ORDERS (LIST)
	// -------------------
	public async Task<IActionResult> Orders() {
		return Ok();
	}

	// -------------------
	// ORDERS (DETAILS)
	// -------------------
	public async Task<IActionResult> OrderDetails(int id) {
		return Ok();
	}

	private static readonly JsonSerializerOptions _jsonOptions = new() {
		PropertyNameCaseInsensitive = true
	};

	private static OrderSubmittedDto? TryParseOrder(string json) {
		try {
			return JsonSerializer.Deserialize<OrderSubmittedDto>(json, _jsonOptions);
		} catch {
			return null;
		}
	}
}

public class CreateUserViewModel {
	public string FirstName { get; set; } = "";
	public string LastName { get; set; } = "";
	public string Email { get; set; } = "";
	public string Password { get; set; } = "";
	public string? Role {
		get; set;
	}
}

public class AdminOrderListItemViewModel {
	public int Id {
		get; set;
	}
	public string OrderId { get; set; } = "";
	public string Status { get; set; } = "";
	public DateTime CreatedAtUtc {
		get; set;
	}
	public string FullName { get; set; } = "";
	public string Email { get; set; } = "";
	public decimal? Total {
		get; set;
	}
}

public class AdminOrderDetailsViewModel {
	public int Id {
		get; set;
	}
	public string OrderId { get; set; } = "";
	public string Status { get; set; } = "";
	public DateTime CreatedAtUtc {
		get; set;
	}
	public string RawJson { get; set; } = "";
	public OrderSubmittedDto? Payload {
		get; set;
	}
}

// Fixed DTO to use OrderLineJsonDto instead of OrderDTO (which is now an Entity)
public class OrderSubmittedDto {
	public string? OrderId {
		get; set;
	}
	public string? FirstName {
		get; set;
	}
	public string? LastName {
		get; set;
	}
	public string? Email {
		get; set;
	}

	public string? Street {
		get; set;
	}
	public string? Postbus {
		get; set;
	}
	public string? City {
		get; set;
	}
	public string? Postcode {
		get; set;
	}
	public string? Country {
		get; set;
	}

	public decimal? Total {
		get; set;
	}
	public List<OrderLineJsonDto>? Lines {
		get; set;
	}
}

// New class to represent the line items in the JSON payload
public class OrderLineJsonDto {
	public long CoffeeUserId {
		get; set;
	}
	public long OrderId {
		get; set;
	}
	public long CoffeeId {
		get; set;
	}
	public string CoffeeType { get; set; } = "";
	public int Quantity {
		get; set;
	}
	public decimal UnitPrice {
		get; set;
	}
}