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
using Models.Entities.Enums;
using Web.Services;

namespace Web.Controllers;

[Authorize(Roles = "Admin")]
public class AdminController : Controller {
	private readonly UserManager<CoffeeUser> _userManager;
	private readonly RoleManager<IdentityRole<long>> _roleManager;
	private readonly LocalDbContext _db;
	private readonly GlobalDbContext _globalDb;
	private readonly Utilities _utilities;

	public AdminController(
		UserManager<CoffeeUser> userManager,
		RoleManager<IdentityRole<long>> roleManager,
		LocalDbContext db,
		GlobalDbContext globalDb,
		Utilities utilities) {
		_userManager = userManager;
		_roleManager = roleManager;
		_db = db;
		_globalDb = globalDb;
		_utilities = utilities;
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

	[HttpGet]
	public IActionResult StressTest() => View();

	[HttpPost]
	public async Task<IActionResult> GenerateTestUser() {
		try {
			var firstName = GenerateRandomName(true);
			var lastName = GenerateRandomName(false);
			var email = $"{firstName.ToLower()}.{lastName.ToLower()}.{Random.Shared.Next(1000, 9999)}@test.com";

			// Create local user
			var localUser = new CoffeeUser {
				FirstName = firstName,
				LastName = lastName,
				UserName = email,
				Email = email,
				EmailConfirmed = true
			};

			var result = await _userManager.CreateAsync(localUser, "Test@123");
			if (!result.Succeeded) {
				return Json(new { success = false, error = string.Join(", ", result.Errors.Select(e => e.Description)) });
			}

			// Create global user
			var globalUser = new CoffeeUser {
				FirstName = firstName,
				LastName = lastName,
				UserName = email,
				Email = email,
				EmailConfirmed = true
			};

			_globalDb.Users.Add(globalUser);
			await _globalDb.SaveChangesAsync();

			// Link GlobalId back to local user
			localUser.GlobalId = globalUser.Id;
			await _userManager.UpdateAsync(localUser);

			// Send message to RabbitMQ
			var queueSent = false;
			try {
				await _utilities.SendMessageTo("AccountSubmitted", new CoffeeUserSubmitted(
					globalUser.Id,
					globalUser.Email,
					globalUser.FirstName,
					globalUser.LastName,
					null,
					null,
					null,
					null
				));
				queueSent = true;
			} catch (Exception ex) {
				Console.WriteLine($"RabbitMQ publish failed: {ex.Message}");
			}

			return Json(new { 
				success = true, 
				userId = localUser.Id,
				globalUserId = globalUser.Id,
				email = localUser.Email,
				name = $"{localUser.FirstName} {localUser.LastName}",
				queueSent = queueSent
			});
		} catch (Exception ex) {
			return Json(new { success = false, error = ex.Message });
		}
	}

	[HttpPost]
	public async Task<IActionResult> GenerateTestOrder(long? userId = null) {
		try {
			// Get a random user with GlobalId
			var targetUserId = userId ?? await _db.Users
				.Where(u => u.GlobalId != null)
				.OrderBy(u => Guid.NewGuid())
				.Select(u => u.GlobalId)
				.FirstOrDefaultAsync();

			if (targetUserId == null || targetUserId == 0) {
				return Json(new { success = false, error = "No users with GlobalId available" });
			}

			// Create global order
			var globalOrder = new Order {
				CoffeeUserId = targetUserId.Value,
				Status = (StatusType)Random.Shared.Next(0, 4)
			};
			_globalDb.Orders.Add(globalOrder);
			await _globalDb.SaveChangesAsync();

			// Create local order with GlobalId reference
			var localOrder = new Order {
				CoffeeUserId = targetUserId.Value,
				Status = globalOrder.Status,
				GlobalId = globalOrder.Id
			};
			_db.Orders.Add(localOrder);
			await _db.SaveChangesAsync();

			// Get available coffees from global DB
			var globalCoffees = await _globalDb.Coffees.ToListAsync();
			if (!globalCoffees.Any()) {
				return Json(new { success = false, error = "No coffees available in global database" });
			}

			// Add 1-5 random order items
			var itemCount = Random.Shared.Next(1, 6);
			var messagesPublished = 0;

			for (int i = 0; i < itemCount; i++) {
				var globalCoffee = globalCoffees[Random.Shared.Next(globalCoffees.Count)];
				var quantity = Random.Shared.Next(1, 26);

				// Create global order item
				var globalOrderItem = new OrderItem {
					OrderId = globalOrder.Id,
					CoffeeId = globalCoffee.Id,
					Quantity = quantity,
					UnitPrice = globalCoffee.Price
				};
				_globalDb.OrderItems.Add(globalOrderItem);

				// Send message to RabbitMQ
				try {
					await _utilities.SendMessageTo("OrderSubmitted", new OrderSubmitted(
						targetUserId.Value,
						globalOrder.Id,
						globalCoffee.Id,
						globalCoffee.Type,
						quantity,
						globalCoffee.Price
					));
					messagesPublished++;
				} catch (Exception ex) {
					Console.WriteLine($"RabbitMQ publish failed for order item: {ex.Message}");
				}
			}

			await _globalDb.SaveChangesAsync();

			return Json(new { 
				success = true, 
				orderId = globalOrder.Id,
				localOrderId = localOrder.Id,
				userId = targetUserId,
				itemCount = itemCount,
				messagesPublished = messagesPublished
			});
		} catch (Exception ex) {
			return Json(new { success = false, error = ex.Message });
		}
	}

	[HttpPost]
	public async Task<IActionResult> CleanupTestData() {
		try {
			// Get test users from both databases
			var localTestUsers = await _db.Users
				.Where(u => u.Email.EndsWith("@test.com"))
				.ToListAsync();

			var globalTestUsers = await _globalDb.Users
				.Where(u => u.Email.EndsWith("@test.com"))
				.ToListAsync();

			var count = localTestUsers.Count;

			// Clean up local database
			foreach (var user in localTestUsers) {
				var orders = await _db.Orders
					.Where(o => o.CoffeeUserId == user.Id || o.CoffeeUserId == user.GlobalId)
					.ToListAsync();
				_db.Orders.RemoveRange(orders);

				var addresses = await _db.Addresses.Where(a => a.CoffeeUserId == user.Id).ToListAsync();
				_db.Addresses.RemoveRange(addresses);

				var payments = await _db.PaymentDetails.Where(p => p.CoffeeUserId == user.Id).ToListAsync();
				_db.PaymentDetails.RemoveRange(payments);

				await _userManager.DeleteAsync(user);
			}

			// Clean up global database
			foreach (var user in globalTestUsers) {
				var orders = await _globalDb.Orders.Where(o => o.CoffeeUserId == user.Id).ToListAsync();
				foreach (var order in orders) {
					var orderItems = await _globalDb.OrderItems.Where(oi => oi.OrderId == order.Id).ToListAsync();
					_globalDb.OrderItems.RemoveRange(orderItems);
				}
				_globalDb.Orders.RemoveRange(orders);

				var addresses = await _globalDb.Addresses.Where(a => a.CoffeeUserId == user.Id).ToListAsync();
				_globalDb.Addresses.RemoveRange(addresses);

				var payments = await _globalDb.PaymentDetails.Where(p => p.CoffeeUserId == user.Id).ToListAsync();
				_globalDb.PaymentDetails.RemoveRange(payments);

				_globalDb.Users.Remove(user);
			}

			await _db.SaveChangesAsync();
			await _globalDb.SaveChangesAsync();

			return Json(new { success = true, deletedCount = count, deletedFromGlobal = globalTestUsers.Count });
		} catch (Exception ex) {
			return Json(new { success = false, error = ex.Message });
		}
	}

	private static string GenerateRandomName(bool isFirstName) {
		string[] firstNames = ["Alice", "Bob", "Carol", "David", "Emma", "Frank", "Grace", "Henry", 
			"Irene", "Jack", "Kate", "Liam", "Mia", "Noah", "Olivia", "Peter", "Quinn", "Rachel"];
		string[] lastNames = ["Smith", "Johnson", "Williams", "Brown", "Jones", "Garcia", "Miller", 
			"Davis", "Rodriguez", "Martinez", "Hernandez", "Lopez", "Gonzalez", "Wilson", "Anderson"];
		
		return isFirstName 
			? firstNames[Random.Shared.Next(firstNames.Length)]
			: lastNames[Random.Shared.Next(lastNames.Length)];
	}
}
