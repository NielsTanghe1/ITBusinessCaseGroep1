using System.Text.Json;
using Models.Data;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Models.Entities.DTO;

namespace Web.Controllers;

[Authorize(Roles = "Admin")]
public class AdminController : Controller {
	private readonly UserManager<IdentityUser> _userManager;
	private readonly RoleManager<IdentityRole> _roleManager;
	private readonly LocalDbContext _db;

	public AdminController(
		 UserManager<IdentityUser> userManager,
		 RoleManager<IdentityRole> roleManager,
		 LocalDbContext db) {
		_userManager = userManager;
		_roleManager = roleManager;
		_db = db;
	}

	// Dashboard
	public IActionResult Index() => View();

	// -------------------
	// USERS
	// -------------------
	public async Task<IActionResult> Users() {
		var users = await _userManager.Users
			 .OrderBy(u => u.Email)
			 .ToListAsync();

		var vm = new List<(IdentityUser user, IList<string> roles)>();
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

		var user = new IdentityUser {
			UserName = model.Email,
			Email = model.Email,
			EmailConfirmed = true
		};

		var result = await _userManager.CreateAsync(user, model.Password);
		if (!result.Succeeded) {
			foreach (var e in result.Errors)
				ModelState.AddModelError("", e.Description);
			return View(model);
		}

		if (!string.IsNullOrWhiteSpace(model.Role)) {
			if (!await _roleManager.RoleExistsAsync(model.Role))
				await _roleManager.CreateAsync(new IdentityRole(model.Role));

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
			await _roleManager.CreateAsync(new IdentityRole(roleName));

		return RedirectToAction(nameof(Roles));
	}

	[HttpPost]
	public async Task<IActionResult> AddRoleToUser(string userId, string roleName) {
		var user = await _userManager.FindByIdAsync(userId);
		if (user != null) {
			if (!await _roleManager.RoleExistsAsync(roleName))
				await _roleManager.CreateAsync(new IdentityRole(roleName));

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
		//var orders = await _db.Orders
		//	 .OrderByDescending(o => o.CreatedAt)
		//	 .ToListAsync();

		//var list = orders.Select(o =>
		//{
		//	var parsed = new OrderDTO() {
		//		CoffeeUserId = o.CoffeeUserId,
		//		OrderId = o.Id,
		//		CoffeeId = o.
		//	};

		//	return new AdminOrderListItemVm {
		//		Id = o.Id,
		//		OrderId = o.OrderId,
		//		Status = o.Status,
		//		CreatedAtUtc = o.CreatedAtUtc,
		//		FullName = parsed != null ? $"{parsed.FirstName} {parsed.LastName}".Trim() : "(onbekend)",
		//		Email = parsed?.Email ?? "(onbekend)",
		//		Total = parsed?.Total
		//	};
		//}).ToList();

		//return View(list);
		return Ok();
	}

	// -------------------
	// ORDERS (DETAILS)
	// -------------------
	public async Task<IActionResult> OrderDetails(int id) {
		//var order = await _db.Orders.FirstOrDefaultAsync(o => o.Id == id);
		//if (order == null)
		//	return NotFound();

		//var parsed = TryParseOrder(order.PayloadJson);

		//var vm = new AdminOrderDetailsVm {
		//	Id = order.Id,
		//	OrderId = order.OrderId,
		//	Status = order.Status,
		//	CreatedAtUtc = order.CreatedAtUtc,
		//	RawJson = order.PayloadJson,
		//	Payload = parsed
		//};

		//return View(vm);
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
	public string Email { get; set; } = "";
	public string Password { get; set; } = "";
	public string? Role {
		get; set;
	}
}

// ---------- ViewModels ----------
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

// ---------- DTOs (matcht jouw JSON uit screenshot) ----------
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
	public List<OrderDTO>? Lines {
		get; set;
	}
}