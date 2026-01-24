using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Models.Entities;
using Models.Extensions.Mappings;

namespace Models.Data;

/// <summary>
/// The database context for the local ITBusinessCoffee SQL environment, responsible for
/// offline data storage.
/// Manages access to addresses, user, orders, pending RabbitMQ messages/payloads, etc...
/// </summary>
public class LocalDbContext : BaseIdentityDbContext {
	/// <summary>
	/// Initializes a new instance of the <see cref="LocalDbContext"/> class
	/// with default options.
	/// </summary>
	public LocalDbContext() : base() { }

	/// <summary>
	/// Initializes a new instance of the <see cref="LocalDbContext"/> class
	/// with the specified configuration.
	/// </summary>
	/// <param name="options">
	/// The configuration options for this database context.
	/// </param>
	public LocalDbContext(DbContextOptions<LocalDbContext> options) : base(options) { }

	/// <summary>
	/// Adds initial data (seed data) to the database if it is empty.
	/// </summary>
	/// <param name="serviceProvider">
	/// The service provider for retrieving required services such as the database context.
	/// </param>
	/// <returns>
	/// A task representing the asynchronous operation.
	/// </returns>
	public static async Task Seeder(IServiceProvider serviceProvider) {
		var contextGlobal = serviceProvider.GetRequiredService<GlobalDbContext>();
		var contextLocal = serviceProvider.GetRequiredService<LocalDbContext>();
		var userManager = serviceProvider.GetRequiredService<UserManager<CoffeeUser>>();
		var roleManager = serviceProvider.GetRequiredService<RoleManager<IdentityRole<long>>>();

		// When seeding the local context with global context items, we do the following:
		// 1. Copy global entities to DTOs (preserves data, meant to create new local IDs) avoids foreign key conflicts
		// 2. Use AsNoTracking() to avoid "Entity is already tracked" errors.
		// 3. Randomize the order of global entities for varied local dataset
		// 4. Create fresh model entities with new local IDs, but copy global data
		// 5. Use the null forgiving operator to store the GlobalId for every new model entity

		if (!contextLocal.Roles.Any()) {
			var globalRoles = await contextGlobal.Roles.AsNoTracking().ToArrayAsync();

			foreach (var role in globalRoles) {
				await roleManager.CreateAsync(new IdentityRole<long>() { Name = role.Name });
			}
		}

		// We need these Maps to translate Global IDs to new Local IDs
		// Key = GlobalId, Value = LocalId
		Dictionary<long, long> userMap = [];
		Dictionary<long, long> coffeeMap = [];
		Dictionary<long, long> orderMap = [];

		if (!contextLocal.Users.Any()) {
			var globalAsDTOs = await contextGlobal.Users
				.AsNoTracking()
				.Select(e => e.ToDTO())
				.ToArrayAsync();

			Random.Shared.Shuffle(globalAsDTOs);

			var localEntities = globalAsDTOs.Select(dto => {
				var localItem = Activator.CreateInstance<CoffeeUser>();
				dto.ToExisting(localItem);

				localItem.GlobalId = dto.GlobalId;
				return localItem;
			}).ToArray();

			foreach (var user in localEntities) {
				await userManager.CreateAsync(user, "P@ssword1");
			}

			var admin1 = localEntities.FirstOrDefault(u => u.Email == "alice@example.com");
			if (admin1 != null) {
				await userManager.AddToRoleAsync(admin1, "Admin");
			}

			var admin2 = localEntities.FirstOrDefault(u => u.Email == "bob@example.com");
			if (admin2 != null) {
				await userManager.AddToRoleAsync(admin2, "Admin");
			}

			foreach (var u in localEntities) {
				if (!await userManager.IsInRoleAsync(u, "Admin")) {
					await userManager.AddToRoleAsync(u, "User");
				}
			}
		}

		// Populate User Map (Query the Local DB to see what IDs were generated)
		userMap = await contextLocal.Users.ToDictionaryAsync(u => (long) u.GlobalId!, u => u.Id);

		if (!contextLocal.Coffees.Any()) {
			var globalAsDTOs = await contextGlobal.Coffees
				.AsNoTracking()
				.Select(e => e.ToDTO())
				.ToArrayAsync();

			Random.Shared.Shuffle(globalAsDTOs);

			var localEntities = globalAsDTOs.Select(dto => {
				var localItem = Activator.CreateInstance<Coffee>();
				dto.ToExisting(localItem);

				localItem.GlobalId = dto.GlobalId;
				return localItem;
			}).ToList();

			contextLocal.Coffees.AddRange(localEntities);
			await contextLocal.SaveChangesAsync();
		}

		// Populate Coffee Map
		coffeeMap = await contextLocal.Coffees.ToDictionaryAsync(c => (long) c.GlobalId!, c => c.Id);

		if (!contextLocal.Addresses.Any()) {
			var globalAsDTOs = await contextGlobal.Addresses
				.AsNoTracking()
				.Select(e => e.ToDTO())
				.ToArrayAsync();

			Random.Shared.Shuffle(globalAsDTOs);

			var localEntities = globalAsDTOs.Select(dto => {
				var localItem = Activator.CreateInstance<Address>();
				dto.ToExisting(localItem);

				localItem.GlobalId = dto.GlobalId;

				// Translate Foreign Keys
				if (userMap.TryGetValue((long) localItem.GlobalId!, out long localUserId)) {
					localItem.CoffeeUserId = localUserId;
				}
				return localItem;
			}).ToArray();

			contextLocal.Addresses.AddRange(localEntities);
			await contextLocal.SaveChangesAsync();
		}

		if (!contextLocal.PaymentDetails.Any()) {
			var globalAsDTOs = await contextGlobal.PaymentDetails
				.AsNoTracking()
				.Select(e => e.ToDTO())
				.ToArrayAsync();

			Random.Shared.Shuffle(globalAsDTOs);

			var localEntities = globalAsDTOs.Select(dto => {
				var localItem = Activator.CreateInstance<PaymentDetail>();
				dto.ToExisting(localItem);

				localItem.GlobalId = dto.GlobalId;

				// Translate Foreign Keys
				if (userMap.TryGetValue((long) localItem.GlobalId!, out long localUserId)) {
					localItem.CoffeeUserId = localUserId;
				}
				return localItem;
			}).ToArray();

			contextLocal.PaymentDetails.AddRange(localEntities);
			await contextLocal.SaveChangesAsync();
		}

		if (!contextLocal.Orders.Any()) {
			var globalAsDTOs = await contextGlobal.Orders
				.AsNoTracking()
				.Select(e => e.ToDTO())
				.ToArrayAsync();

			Random.Shared.Shuffle(globalAsDTOs);

			var localEntities = globalAsDTOs.Select(dto => {
				var localItem = Activator.CreateInstance<Order>();
				dto.ToExisting(localItem);

				localItem.GlobalId = dto.GlobalId;

				// Translate Foreign Keys
				if (userMap.TryGetValue((long) localItem.GlobalId!, out long localUserId)) {
					localItem.CoffeeUserId = localUserId;
				}
				return localItem;
			}).ToArray();

			contextLocal.Orders.AddRange(localEntities);
			await contextLocal.SaveChangesAsync();
		}

		// Populate Order Map
		orderMap = await contextLocal.Orders.ToDictionaryAsync(o => (long) o.GlobalId!, o => o.Id);

		if (!contextLocal.OrderItems.Any()) {
			var globalAsDTOs = await contextGlobal.OrderItems
				.AsNoTracking()
				.Select(e => e.ToDTO())
				.ToArrayAsync();

			Random.Shared.Shuffle(globalAsDTOs);

			var localEntities = globalAsDTOs.Select(dto => {
				var localItem = Activator.CreateInstance<OrderItem>();
				dto.ToExisting(localItem);

				localItem.GlobalId = dto.GlobalId;

				// Translate Foreign Keys
				bool orderExists = orderMap.TryGetValue(dto.OrderId, out var localOrderId);
				bool coffeeExists = coffeeMap.TryGetValue(dto.CoffeeId, out var localCoffeeId);

				if (orderExists && coffeeExists) {
					localItem.OrderId = localOrderId;
					localItem.CoffeeId = localCoffeeId;
				}
				return localItem;
			}).ToArray();

			contextLocal.OrderItems.AddRange(localEntities);
			await contextLocal.SaveChangesAsync();
		}
	}
}
