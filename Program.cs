using MassTransit;
using ITBusinessCase.Consumers;
using ITBusinessCase.Data;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc.Authorization;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

// =======================
// DATABASE + IDENTITY (SQLITE)
// =======================
builder.Services.AddDbContext<ApplicationDbContext>(options =>
	 options.UseSqlite(
		  builder.Configuration.GetConnectionString("DefaultConnection")));

builder.Services.AddIdentity<IdentityUser, IdentityRole>(options => {
	options.Password.RequireDigit = false;
	options.Password.RequireUppercase = false;
	options.Password.RequireNonAlphanumeric = false;
	options.Password.RequiredLength = 6;
})
.AddEntityFrameworkStores<ApplicationDbContext>()
.AddDefaultTokenProviders();

// Login redirect
builder.Services.ConfigureApplicationCookie(options => {
	options.LoginPath = "/Account/Register";
	options.AccessDeniedPath = "/Account/Register";
});



// =======================
// MVC (LOGIN VERPLICHT)
// =======================
builder.Services.AddControllersWithViews(options => {
	var policy = new AuthorizationPolicyBuilder()
		 .RequireAuthenticatedUser()
		 .Build();

	options.Filters.Add(new AuthorizeFilter(policy));
});

// =======================
// MASS TRANSIT + RABBITMQ
// =======================
builder.Services.AddMassTransit(x => {
	// ✅ Consumer registreren
	x.AddConsumer<OrderSubmittedConsumer>();

	// ✅ RabbitMQ configuratie
	x.UsingRabbitMq((context, cfg) => {
		cfg.Host("10.2.160.222", 5672, "/", h => {
			h.Username("admin");
			h.Password("curler-squishy-monogamy");
		});

		// ✅ Automatisch exchanges, queues & bindings
		cfg.ReceiveEndpoint("OrderSubmitted", e => {
			e.ConfigureConsumer<OrderSubmittedConsumer>(context);
		});
	});
});

var app = builder.Build();

// =======================
// MIDDLEWARE
// =======================
if (!app.Environment.IsDevelopment()) {
	app.UseExceptionHandler("/Home/Error");
	app.UseHsts();
}

app.UseStaticFiles();

// app.UseHttpsRedirection(); // optioneel

app.UseRouting();

// Identity middleware
app.UseAuthentication();
app.UseAuthorization();

app.MapControllerRoute(
	 name: "default",
	 pattern: "{controller=Home}/{action=Index}/{id?}");
await IdentitySeed.EnsureAdminAsync(app.Services);


app.Run();
