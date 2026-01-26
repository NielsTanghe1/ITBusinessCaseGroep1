using System.Text.Json;
using Models.Data;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Models.Entities;
using Web.Models;
using Models.Entities.Records;
using Web.Consumers;
using Models.Entities.DTO;

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

	// -------------------
	// CONFIRMED ORDERS (from Queue)
	// -------------------
	public IActionResult ConfirmedOrders() {
		var confirmedOrders = OrderConfirmedConsumer.ReceivedOrders.ToList();
		return View(confirmedOrders);
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
