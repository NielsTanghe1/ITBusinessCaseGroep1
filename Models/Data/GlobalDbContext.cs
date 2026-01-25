using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Models.Entities;

namespace Models.Data;

/// <summary>
/// The database context for the global ITBusinessCoffee SQL environment, responsible for
/// online data storage.
/// Manages access to addresses, user, orders, pending RabbitMQ messages/payloads, etc...
/// </summary>
public class GlobalDbContext : BaseBackupDbContext {
	/// <summary>
	/// Initializes a new instance of the <see cref="GlobalDbContext"/> class
	/// with default options.
	/// </summary>
	public GlobalDbContext() : base() { }

	/// <summary>
	/// Initializes a new instance of the <see cref="GlobalDbContext"/> class
	/// with the specified configuration.
	/// </summary>
	/// <param name="options">
	/// The configuration options for this database context.
	/// </param>
	public GlobalDbContext(DbContextOptions<GlobalDbContext> options) : base(options) { }

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

		if (!contextGlobal.Roles.Any()) {
			contextGlobal.Roles.AddRange(new List<IdentityRole<long>> {
				new() { Name = "Admin" },
				new() { Name = "User" }
			});
			await contextGlobal.SaveChangesAsync();
		}

		if (!contextGlobal.Users.Any()) {
			contextGlobal.Users.AddRange(CoffeeUser.SeedingData());
			await contextGlobal.SaveChangesAsync();
		}

		string[] sourceArray = ["alice@example.com", "bob@example.com"];
		long[] userIds = contextGlobal.Users
			.Where(user => !sourceArray.Contains(user.Email))
			.Select(user => user.Id)
			.ToArray();

		if (!contextGlobal.Coffees.Any()) {
			contextGlobal.Coffees.AddRange(Coffee.SeedingData());
			await contextGlobal.SaveChangesAsync();
		}

		if (!contextGlobal.Addresses.Any()) {
			contextGlobal.Addresses.AddRange(Address.SeedingData(userIds));
			await contextGlobal.SaveChangesAsync();
		}

		if (!contextGlobal.PaymentDetails.Any()) {
			contextGlobal.PaymentDetails.AddRange(PaymentDetail.SeedingData(userIds));
			await contextGlobal.SaveChangesAsync();
		}

		if (!contextGlobal.Orders.Any()) {
			contextGlobal.Orders.AddRange(Order.SeedingData(userIds));
			await contextGlobal.SaveChangesAsync();
		}

		if (!contextGlobal.OrderItems.Any()) {
			var orderIds = contextGlobal.Orders.Select(o => o.Id).ToArray();
			var coffeeInfo = contextGlobal.Coffees.Select(c => new KeyValuePair<long, decimal>(c.Id, c.Price)).ToArray();

			contextGlobal.OrderItems.AddRange(OrderItem.SeedingData(orderIds, coffeeInfo));
			await contextGlobal.SaveChangesAsync();
		}
	}
}
