using Microsoft.EntityFrameworkCore;
using Models.Entities;

namespace Models.Data;

public class GlobalDbContext : BaseDbContext<CoffeeUser> {

	public DbSet<Address> Addresses {
		get; set;
	}
	public DbSet<Order> Orders {
		get; set;
	}
	public DbSet<OrderItem> OrderItems {
		get; set;
	}
	public DbSet<PaymentDetail> PaymentDetails {
		get; set;
	}
	public DbSet<Coffee> Coffees {
		get; set;
	}

	public GlobalDbContext(DbContextOptions<GlobalDbContext> options) : base(options) { }

	protected override void OnModelCreating(ModelBuilder modelBuilder) {
		base.OnModelCreating(modelBuilder);

		// Restore standard configuration for base entities
		modelBuilder.Entity<CoffeeUser>(entity => {
			entity.Property(e => e.Id).HasColumnName("CoffeeUserId");
			entity.HasIndex(e => e.Email).IsUnique();
		});

		modelBuilder.Entity<Address>(entity => {
			entity.Property(e => e.Id).HasColumnName("AddressId");
			entity.Property(e => e.Type).HasConversion<string>();
		});

		modelBuilder.Entity<Order>(entity => {
			entity.Property(e => e.Id).HasColumnName("OrderId");
			entity.Property(e => e.Status).HasConversion<string>();
		});

		modelBuilder.Entity<OrderItem>(entity =>
			 entity.Property(e => e.Id).HasColumnName("OrderItemId")
		);

		modelBuilder.Entity<PaymentDetail>(entity =>
			 entity.Property(e => e.Id).HasColumnName("PaymentDetailId")
		);

		modelBuilder.Entity<Coffee>(entity => {
			entity.Property(e => e.Id).HasColumnName("CoffeeId");
			entity.Property(e => e.Name).HasConversion<string>();
			entity.Property(e => e.Type).HasConversion<string>();
		});
	}
}