using Web.Consumers;
using Web.Data;
using MassTransit;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc.Authorization;
using Microsoft.EntityFrameworkCore;
using Models.Data;
using Models.Entities;

var builder = WebApplication.CreateBuilder(args);

// =======================
// DATABASE + IDENTITY (SQLITE)
// =======================
builder.Services.AddDbContext<LocalDbContext>(options =>
	options.UseSqlServer(
		builder.Configuration.GetConnectionString("LocalDbContextConnection")
		?? throw new InvalidOperationException("Connection string 'LocalDbContextConnection' not found."),
		options => options.EnableRetryOnFailure(
			maxRetryCount: 3,
			maxRetryDelay: TimeSpan.FromSeconds(5),
			errorNumbersToAdd: [4060]
		)
	));

builder.Services.AddDbContext<GlobalDbContext>(options =>
	options.UseSqlServer(
		builder.Configuration.GetConnectionString("GlobalDbContextConnection")
		?? throw new InvalidOperationException("Connection string 'GlobalDbContextConnection' not found."),
		options => options.EnableRetryOnFailure(
			maxRetryCount: 3,
			maxRetryDelay: TimeSpan.FromSeconds(5),
			errorNumbersToAdd: [4060]
		)
	));

// Identity system
builder.Services
	.AddIdentity<CoffeeUser, IdentityRole<long>>(options => {
		options.Password.RequireDigit = false;
		options.Password.RequireLowercase = false;
		options.Password.RequireNonAlphanumeric = false;
		options.Password.RequireUppercase = false;
		options.Password.RequiredLength = 3;
		options.Password.RequiredUniqueChars = 1;
		options.User.RequireUniqueEmail = true;
		options.SignIn.RequireConfirmedAccount = false;
	})
	.AddEntityFrameworkStores<LocalDbContext>();

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

builder.Services.AddRazorPages();

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

// Populate the local database with data
using (var scope = app.Services.CreateScope()) {
	var services = scope.ServiceProvider;
	try {
		await LocalDbContext.Seeder(services);
	} catch (Exception ex) {
		var logger = services.GetRequiredService<ILogger<Program>>();
		logger.LogError(ex, "An error occurred while seeding the database.");
	}
}

// =======================
// MIDDLEWARE
// =======================
// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment()) {
	app.UseExceptionHandler("/Home/Error");
	// The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
	app.UseHsts();
}

app.UseHttpsRedirection();
app.UseRouting();

app.UseAuthorization();

app.MapStaticAssets();
app.MapRazorPages();

app.MapControllerRoute(
	name: "default",
	pattern: "{controller=Home}/{action=Index}/{id?}")
	.WithStaticAssets();

await IdentitySeed.EnsureAdminAsync(app.Services);

app.Run();
