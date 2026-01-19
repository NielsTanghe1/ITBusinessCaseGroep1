using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Models.Entities;

namespace Models.Data;

/// <summary>
/// Provides the common Entity Framework Core database context for the ITBusinessCoffee
/// application, including ASP.NET Core Identity support.
/// </summary>
/// <remarks>
/// This abstract base context defines the shared configuration and entity sets used by
/// concrete database contexts (for example, local development or production contexts).
/// It extends <see cref="IdentityDbContext{TUser, TRole, TKey}"/> to manage
/// authentication and authorization data for <see cref="CoffeeUser"/>.
/// </remarks>
public abstract class BaseDbContext : IdentityDbContext<CoffeeUser, IdentityRole<long>, long> {
	/// <summary>
	/// Gets or sets the <see cref="DbSet{TEntity}"/> of Addresses.
	/// </summary>
	public DbSet<Address> Addresses {
		get; set;
	}

	/// <summary>
	/// Gets or sets the <see cref="DbSet{TEntity}"/> of Orders.
	/// </summary>
	public DbSet<Order> Orders {
		get; set;
	}

	/// <summary>
	/// Gets or sets the <see cref="DbSet{TEntity}"/> of OrderItems.
	/// </summary>
	public DbSet<OrderItem> OrderItems {
		get; set;
	}

	/// <summary>
	/// Gets or sets the <see cref="DbSet{TEntity}"/> of PaymentDetails.
	/// </summary>
	public DbSet<PaymentDetail> PaymentDetails {
		get; set;
	}

	/// <summary>
	/// Gets or sets the <see cref="DbSet{TEntity}"/> of Coffees.
	/// </summary>
	public DbSet<Coffee> Coffees {
		get; set;
	}

	/// <summary>
	/// Initializes a new instance of the <see cref="BaseDbContext"/> class
	/// with default options.
	/// </summary>
	protected BaseDbContext() : base() { }

	/// <summary>
	/// Initializes a new instance of the <see cref="BaseDbContext"/> class
	/// with the specified configuration.
	/// </summary>
	/// <param name="options">
	/// The configuration options for this database context.
	/// </param>
	protected BaseDbContext(DbContextOptions options) : base(options) { }

	/// <summary>
	/// Configures the database model and entity relationships via the Fluent API.
	/// This defines things like column names, property requirements, conversions for enums and such.
	/// </summary>
	/// <param name="modelBuilder">
	/// The builder used to construct the model.
	/// </param>
	protected override void OnModelCreating(ModelBuilder modelBuilder) {
		base.OnModelCreating(modelBuilder);

		// Set primary key column names to match ERD schema
		modelBuilder.Entity<CoffeeUser>(entity =>
			entity.Property(entity => entity.Id).HasColumnName("CoffeeUserId")
		);

		modelBuilder.Entity<Address>(entity =>
			entity.Property(entity => entity.Id).HasColumnName("AddressId")
		);

		modelBuilder.Entity<Order>(entity =>
			entity.Property(entity => entity.Id).HasColumnName("OrderId")
		);

		modelBuilder.Entity<OrderItem>(entity =>
			entity.Property(entity => entity.Id).HasColumnName("OrderItemId")
		);

		modelBuilder.Entity<PaymentDetail>(entity =>
			entity.Property(entity => entity.Id).HasColumnName("PaymentDetailId")
		);

		modelBuilder.Entity<Coffee>(entity =>
			entity.Property(entity => entity.Id).HasColumnName("CoffeeId")
		);

		// Set required properties of the Identity entities
		modelBuilder.Entity<CoffeeUser>()
			.HasIndex(entity => entity.Email)
			.IsUnique();

		modelBuilder.Entity<CoffeeUser>()
			.HasIndex(entity => entity.PhoneNumber)
			.IsUnique();

		// Convert enum properties to string
		modelBuilder
			.Entity<Order>()
			.Property(entity => entity.Status)
			.HasConversion<string>();

		modelBuilder
			.Entity<Coffee>()
			.Property(entity => entity.Name)
			.HasConversion<string>();

		modelBuilder
			.Entity<Coffee>()
			.Property(entity => entity.Type)
			.HasConversion<string>();

		modelBuilder
			.Entity<Address>()
			.Property(entity => entity.Type)
			.HasConversion<string>();
	}

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
		var context = serviceProvider.GetRequiredService<LocalDbContext>();
		var userManager = serviceProvider.GetRequiredService<UserManager<CoffeeUser>>();

		// Preemptively query for the user IDs as we'll need this quite often, but only query for Customer users
		long[] customerUserIds = context.Users
			.Where(user => context.UserRoles
				.Any(
					role => role.UserId == user.Id &&
					role.RoleId == context.Roles.First(otherrole => otherrole.Name == "Customer").Id))
			.Select(user => user.Id)
			.ToArray();

		if (!context.Roles.Any()) {
			context.Roles.AddRange(new List<IdentityRole<long>> {
				new() {
					Name = "Admin",
					NormalizedName = "ADMIN" },
				new() {
					Name = "User",
					NormalizedName = "USER" },
				new() {
					Name = "Customer",
					NormalizedName = "Customer" }
			});
			context.SaveChanges();
		}

		if (!context.Users.Any()) {
			CoffeeUser[] userList = CoffeeUser.SeedingData();
			foreach (var user in userList) {
				await userManager.CreateAsync(user, "P@ssword1");
			}
			await userManager.AddToRoleAsync(
				userList.First(usr => usr.Email == "alice@example.com"),
				"Admin");
			await userManager.AddToRoleAsync(
				userList.First(usr => usr.Email == "bob@example.com"),
				"Admin");
			await userManager.AddToRoleAsync(
				userList.First(usr => usr.Email == "carol@example.com"),
				"Customer");
			await userManager.AddToRoleAsync(
				userList.First(usr => usr.Email == "david@example.com"),
				"Customer");
			await userManager.AddToRoleAsync(
				userList.First(usr => usr.Email == "emma@example.com"),
				"Customer");
			await userManager.AddToRoleAsync(
				userList.First(usr => usr.Email == "frank@example.com"),
				"Customer");
			await userManager.AddToRoleAsync(
				userList.First(usr => usr.Email == "grace@example.com"),
				"Customer");
			await userManager.AddToRoleAsync(
				userList.First(usr => usr.Email == "henry@example.com"),
				"Customer");
			await userManager.AddToRoleAsync(
				userList.First(usr => usr.Email == "irene@example.com"),
				"Customer");
			await userManager.AddToRoleAsync(
				userList.First(usr => usr.Email == "jack@example.com"),
				"Customer");
			await userManager.AddToRoleAsync(
				userList.First(usr => usr.Email == "katherine@example.com"),
				"Customer");
			await userManager.AddToRoleAsync(
				userList.First(usr => usr.Email == "liam@example.com"),
				"Customer");
			await userManager.AddToRoleAsync(
				userList.First(usr => usr.Email == "mia@example.com"),
				"User");
			await userManager.AddToRoleAsync(
				userList.First(usr => usr.Email == "nathan@example.com"),
				"User");
			context.SaveChanges();

			// As we had to create the users here, we'll need to update our array of ids
			customerUserIds = userList
				.Where(user => context.UserRoles
					.Any(
						role => role.UserId == user.Id &&
						role.RoleId == context.Roles.First(otherrole => otherrole.Name == "Customer").Id))
				.Select(user => user.Id)
				.ToArray();
		}

		if (!context.Coffees.Any()) {
			context.Coffees.AddRange(Coffee.SeedingData());
			context.SaveChanges();
		}

		if (!context.Addresses.Any()) {
			context.Addresses.AddRange(Address.SeedingData(customerUserIds));
			context.SaveChanges();
		}

		if (!context.PaymentDetails.Any()) {
			context.PaymentDetails.AddRange(PaymentDetail.SeedingData(customerUserIds));
			context.SaveChanges();
		}

		if (!context.Orders.Any()) {
			context.Orders.AddRange(Order.SeedingData(customerUserIds));
			context.SaveChanges();
		}

		if (!context.OrderItems.Any()) {
			context.OrderItems.AddRange(OrderItem.SeedingData(
				customerUserIds,
				context.Coffees
					.Select(cff => new KeyValuePair<long, decimal>(cff.Id, cff.Price))
					.ToArray()));
			context.SaveChanges();
		}
	}
}
