using ITBusinessCase.Models;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace ITBusinessCase.Data;

public class ApplicationDbContext : IdentityDbContext {
	public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
		 : base(options) { }

	public DbSet<Order> Orders => Set<Order>();
	public DbSet<UserProfile> UserProfiles => Set<UserProfile>();
}
