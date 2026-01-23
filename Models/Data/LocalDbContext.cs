using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Models.Entities;
using Models.Entities.DTO;

namespace Models.Data;

public class LocalDbContext : BaseDbContext<CoffeeUserDTO> {

	public DbSet<AddressDTO> Addresses {
		get; set;
	}
	public DbSet<OrderDTO> Orders {
		get; set;
	}
	public DbSet<OrderItemDTO> OrderItems {
		get; set;
	}
	public DbSet<PaymentDetailDTO> PaymentDetails {
		get; set;
	}
	public DbSet<CoffeeDTO> Coffees {
		get; set;
	}

	public LocalDbContext() : base() { }

	public LocalDbContext(DbContextOptions<LocalDbContext> options) : base(options) { }

	protected override void OnModelCreating(ModelBuilder modelBuilder) {
		// 1. IMPORTANT: Ignore the base entities so EF doesn't create a hierarchy.
		// This must be done BEFORE other configurations involving these types.
		modelBuilder.Ignore<CoffeeUser>();
		modelBuilder.Ignore<Coffee>();
		modelBuilder.Ignore<Order>();
		modelBuilder.Ignore<OrderItem>();
		modelBuilder.Ignore<PaymentDetail>();
		modelBuilder.Ignore<Address>();

		// 2. Call Identity configuration (sets up AspNetUsers, etc.)
		base.OnModelCreating(modelBuilder);

		// 3. Configure DTOs as Root Entities
		modelBuilder.Entity<CoffeeUserDTO>(entity => {
			entity.ToTable("AspNetUsers");
			entity.Property(e => e.Id).HasColumnName("CoffeeUserId");
		});

		modelBuilder.Entity<AddressDTO>(entity => {
			entity.ToTable("Addresses");
			entity.Property(e => e.Id).HasColumnName("AddressId");
			entity.Property(e => e.Type).HasConversion<string>();
		});

		modelBuilder.Entity<OrderDTO>(entity => {
			entity.ToTable("Orders");
			entity.Property(e => e.Id).HasColumnName("OrderId");
			entity.Property(e => e.Status).HasConversion<string>();
		});

		modelBuilder.Entity<OrderItemDTO>(entity => {
			entity.ToTable("OrderItems");
			entity.Property(e => e.Id).HasColumnName("OrderItemId");
		});

		modelBuilder.Entity<PaymentDetailDTO>(entity => {
			entity.ToTable("PaymentDetails");
			entity.Property(e => e.Id).HasColumnName("PaymentDetailId");
		});

		modelBuilder.Entity<CoffeeDTO>(entity => {
			entity.ToTable("Coffees");
			entity.Property(e => e.Id).HasColumnName("CoffeeId");
			entity.Property(e => e.Name).HasConversion<string>();
			entity.Property(e => e.Type).HasConversion<string>();
		});
	}

	public static async Task Seeder(IServiceProvider serviceProvider) {
		var context = serviceProvider.GetRequiredService<LocalDbContext>();
		var userManager = serviceProvider.GetRequiredService<UserManager<CoffeeUserDTO>>();
		var roleManager = serviceProvider.GetRequiredService<RoleManager<IdentityRole<long>>>();

		if (!context.Roles.Any()) {
			await roleManager.CreateAsync(new IdentityRole<long> { Name = "Admin", NormalizedName = "ADMIN" });
			await roleManager.CreateAsync(new IdentityRole<long> { Name = "User", NormalizedName = "USER" });
			await roleManager.CreateAsync(new IdentityRole<long> { Name = "Customer", NormalizedName = "Customer" });
		}

		if (!context.Users.Any()) {
			var originalUsers = CoffeeUser.SeedingData();
			foreach (var original in originalUsers) {
				var userDto = new CoffeeUserDTO {
					UserName = original.UserName,
					Email = original.Email,
					FirstName = original.FirstName,
					LastName = original.LastName,
					EmailConfirmed = original.EmailConfirmed,
					PhoneNumber = original.PhoneNumber,
					GlobalId = Random.Shared.NextInt64()
				};
				await userManager.CreateAsync(userDto, "P@ssword1");
			}

			var users = context.Users.ToList();
			var admin1 = users.FirstOrDefault(u => u.Email == "alice@example.com");
			if (admin1 != null)
				await userManager.AddToRoleAsync(admin1, "Admin");
			var admin2 = users.FirstOrDefault(u => u.Email == "bob@example.com");
			if (admin2 != null)
				await userManager.AddToRoleAsync(admin2, "Admin");

			foreach (var u in users) {
				if (!await userManager.IsInRoleAsync(u, "Admin")) {
					await userManager.AddToRoleAsync(u, "Customer");
				}
			}
		}
		await context.SaveChangesAsync();

		long[] customerUserIds = context.Users.Select(u => u.Id).ToArray();

		if (!context.Coffees.Any()) {
			var coffees = Coffee.SeedingData().Select(c => new CoffeeDTO {
				Name = c.Name,
				Type = c.Type,
				Price = c.Price,
				GlobalId = Random.Shared.NextInt64()
			});
			context.Coffees.AddRange(coffees);
			await context.SaveChangesAsync();
		}

		if (!context.Addresses.Any()) {
			var addresses = Address.SeedingData(customerUserIds).Select(a => new AddressDTO {
				CoffeeUserId = a.CoffeeUserId,
				Type = a.Type,
				Street = a.Street,
				HouseNumber = a.HouseNumber,
				City = a.City,
				PostalCode = a.PostalCode,
				CountryISO = a.CountryISO,
				UnitNumber = a.UnitNumber,
				GlobalId = Random.Shared.NextInt64()
			});
			context.Addresses.AddRange(addresses);
			await context.SaveChangesAsync();
		}

		if (!context.PaymentDetails.Any()) {
			var payments = PaymentDetail.SeedingData(customerUserIds).Select(p => new PaymentDetailDTO {
				CoffeeUserId = p.CoffeeUserId,
				LastFour = p.LastFour,
				ExpiryDate = p.ExpiryDate,
				GatewayToken = p.GatewayToken,
				GlobalId = Random.Shared.NextInt64()
			});
			context.PaymentDetails.AddRange(payments);
			await context.SaveChangesAsync();
		}

		if (!context.Orders.Any()) {
			var orders = Order.SeedingData(customerUserIds).Select(o => new OrderDTO {
				CoffeeUserId = o.CoffeeUserId,
				Status = o.Status,
				GlobalId = Random.Shared.NextInt64()
			});
			context.Orders.AddRange(orders);
			await context.SaveChangesAsync();
		}

		if (!context.OrderItems.Any()) {
			var orderIds = context.Orders.Select(o => o.Id).ToArray();
			var coffeeInfo = context.Coffees.Select(c => new KeyValuePair<long, decimal>(c.Id, c.Price)).ToArray();
			var items = OrderItem.SeedingData(orderIds, coffeeInfo).Select(i => new OrderItemDTO {
				OrderId = i.OrderId,
				CoffeeId = i.CoffeeId,
				Quantity = i.Quantity,
				UnitPrice = i.UnitPrice,
				GlobalId = Random.Shared.NextInt64()
			});
			context.OrderItems.AddRange(items);
			await context.SaveChangesAsync();
		}
	}
}