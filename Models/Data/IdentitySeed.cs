using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.DependencyInjection;

namespace Web.Data;

public static class IdentitySeed {
	public static async Task EnsureAdminAsync(IServiceProvider services) {
		using var scope = services.CreateScope();

		var roleManager = scope.ServiceProvider.GetRequiredService<RoleManager<IdentityRole>>();
		var userManager = scope.ServiceProvider.GetRequiredService<UserManager<IdentityUser>>();

		const string adminRole = "Admin";
		const string adminEmail = "admin@itbusinesscase.local";
		const string adminPassword = "Admin123!";

		// Role
		if (!await roleManager.RoleExistsAsync(adminRole))
			await roleManager.CreateAsync(new IdentityRole(adminRole));

		// User
		var user = await userManager.FindByEmailAsync(adminEmail);
		if (user == null) {
			user = new IdentityUser { UserName = adminEmail, Email = adminEmail, EmailConfirmed = true };
			var created = await userManager.CreateAsync(user, adminPassword);
			if (!created.Succeeded)
				throw new Exception("Admin user kon niet aangemaakt worden: " +
					 string.Join(", ", created.Errors.Select(e => e.Description)));
		}

		// Add to role
		if (!await userManager.IsInRoleAsync(user, adminRole))
			await userManager.AddToRoleAsync(user, adminRole);
	}
}
