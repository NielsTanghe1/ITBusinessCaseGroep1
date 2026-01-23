using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Models.Entities;

namespace Models.Data;

public abstract class BaseDbContext<TUser> : IdentityDbContext<TUser, IdentityRole<long>, long>
	 where TUser : CoffeeUser {
	// Constructor required for derived contexts
	protected BaseDbContext() {
	}

	protected BaseDbContext(DbContextOptions options) : base(options) { }

	protected override void OnModelCreating(ModelBuilder modelBuilder) {
		base.OnModelCreating(modelBuilder);
		// We moved specific entity configuration to Local/Global DbContexts
		// to strictly separate DTOs (Local) from Entities (Global).
	}
}