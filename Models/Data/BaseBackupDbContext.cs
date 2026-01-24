using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Models.Entities;

namespace Models.Data;

/// <summary>
/// Provides the common Entity Framework Core database context for the ITBusinessCoffee
/// application, including ASP.NET Core Identity Users and Roles storage.
/// </summary>
/// <remarks>
/// This abstract base context defines the shared configuration and entity sets used by
/// concrete database contexts (for example, local development or production contexts).
/// It extends <see cref="DbContext"/> and does not manage
/// authentication and authorization data for <see cref="CoffeeUser"/> but only stores it.
/// </remarks>
public abstract class BaseBackupDbContext : DbContext {
	/// <summary>
	/// Gets or sets the <see cref="DbSet{TEntity}"/> of Users.
	/// </summary>
	public DbSet<CoffeeUser> Users {
		get; set;
	}

	/// <summary>
	/// Gets or sets the <see cref="DbSet{TEntity}"/> of Roles.
	/// </summary>
	public DbSet<IdentityRole<long>> Roles {
		get; set;
	}

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
	/// Initializes a new instance of the <see cref="BaseBackupDbContext"/> class
	/// with default options.
	/// </summary>
	protected BaseBackupDbContext() : base() { }

	/// <summary>
	/// Initializes a new instance of the <see cref="BaseBackupDbContext"/> class
	/// with the specified configuration.
	/// </summary>
	/// <param name="options">
	/// The configuration options for this database context.
	/// </param>
	protected BaseBackupDbContext(DbContextOptions options) : base(options) { }

	/// <summary>
	/// Configures the database model and entity relationships via the Fluent API.
	/// This defines things like column names, property requirements, conversions for enums and such.
	/// </summary>
	/// <param name="modelBuilder">
	/// The builder used to construct the model.
	/// </param>
	protected override void OnModelCreating(ModelBuilder modelBuilder) {
		base.OnModelCreating(modelBuilder);

		modelBuilder.Entity<CoffeeUser>(entity => {
			entity.Property(e => e.Id).HasColumnName("CoffeeUserId");
			entity.HasIndex(e => e.Email).IsUnique();
			entity.HasIndex(e => e.PhoneNumber).IsUnique();
			entity.Ignore("GlobalId");
		});

		modelBuilder.Entity<Address>(entity => {
			entity.Property(e => e.Id).HasColumnName("AddressId");
			entity.Property(e => e.Type).HasConversion<string>();
			entity.Ignore("GlobalId");
		});

		modelBuilder.Entity<Order>(entity => {
			entity.Property(e => e.Id).HasColumnName("OrderId");
			entity.Property(e => e.Status).HasConversion<string>();
			entity.Ignore("GlobalId");
		});

		modelBuilder.Entity<OrderItem>(entity => {
			entity.Property(e => e.Id).HasColumnName("OrderItemId");
			entity.Ignore("GlobalId");
		});

		modelBuilder.Entity<PaymentDetail>(entity => {
			entity.Property(e => e.Id).HasColumnName("PaymentDetailId");
			entity.Ignore("GlobalId");
		});

		modelBuilder.Entity<Coffee>(entity => {
			entity.Property(e => e.Id).HasColumnName("CoffeeId");
			entity.Property(e => e.Name).HasConversion<string>();
			entity.Property(e => e.Type).HasConversion<string>();
			entity.Ignore("GlobalId");
		});
	}
}
