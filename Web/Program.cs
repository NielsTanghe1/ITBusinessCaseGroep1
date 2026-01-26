using MassTransit;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc.Authorization;
using Microsoft.EntityFrameworkCore;
using Models.Data;
using Models.Entities;
using Web.Services;
using Web.Consumers;
using System.Globalization;
using Microsoft.AspNetCore.Localization;

var builder = WebApplication.CreateBuilder(args);
bool isDebugging = bool.TryParse(builder.Configuration["GlobalAppSettings:IsDebugging"], out bool isDebuggingResult);

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
	 .AddEntityFrameworkStores<LocalDbContext>()
	 .AddDefaultUI();

// MVC
builder.Services.AddControllersWithViews();

// Utilities
builder.Services.AddTransient<Utilities>();

builder.Services.AddRazorPages();

// =======================
// MASS TRANSIT + RABBITMQ
// =======================
builder.Services.AddMassTransit(x => {
	// Register the OrderConfirmed consumer
	x.AddConsumer<OrderConfirmedConsumer>();

	x.UsingRabbitMq((context, cfg) => {
		var rabbit = builder.Configuration.GetSection("RabbitMQConfig");

		var scheme = isDebugging ? "rabbitmq" : rabbit["Scheme"];
		var host = rabbit["Host"] ?? "localhost";
		var vhost = rabbit["VirtualHost"] ?? "/";
		var port = rabbit.GetValue<int?>("Port:Cluster") ?? 5672;
		var username = rabbit["Username"] ?? "guest";
		var password = rabbit["Password"] ?? "guest";

		// Build URI: vhost "/" -> no path, otherwise "/<vhost>"
		var vhostPath = (string.IsNullOrWhiteSpace(vhost) || vhost == "/")
			? string.Empty
			: "/" + vhost.TrimStart('/');

		var uri = new Uri($"{scheme}://{host}:{port}{vhostPath}");

		cfg.Host(uri, h => {
			if (isDebugging && isDebuggingResult) {
				h.Username("guest");
				h.Password("guest");
			} else {
				h.Username(username);
				h.Password(password);
			}
		});

		// Configure the OrderConfirmed queue endpoint
		cfg.ReceiveEndpoint("OrderConfirmed", e => {
			e.UseRawJsonDeserializer(RawSerializerOptions.AnyMessageType);
			e.ConfigureConsumer<OrderConfirmedConsumer>(context);
		});
	});
});

// ... inside builder section
var supportedCultures = new[] { "nl-BE", "fr-BE", "en-US" };
var localizationOptions = new RequestLocalizationOptions()
	 .SetDefaultCulture("nl-BE") // Forces Euro and comma by default
	 .AddSupportedCultures(supportedCultures)
	 .AddSupportedUICultures(supportedCultures);

var app = builder.Build();

// Change this for regional UnitPrice customisation
//app.UseRequestLocalization(localizationOptions);

// Populate the local database with data
using (var scope = app.Services.CreateScope()) {
	var services = scope.ServiceProvider;
	var localContext = services.GetRequiredService<LocalDbContext>();
	var globalContext = services.GetRequiredService<GlobalDbContext>();

	try {
		if (!localContext.Coffees.Any()) {
			await LocalDbContext.Seeder(services);
		}
		if (!globalContext.Coffees.Any()) {
			await GlobalDbContext.Seeder(services);
		}
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

// Als je enkel HTTP gebruikt, laat deze uit
// app.UseHttpsRedirection();
app.UseStaticFiles();
app.UseRouting();

app.UseRouting();
app.UseAuthorization();

app.MapStaticAssets();
app.MapRazorPages();

app.MapControllerRoute(
	name: "default",
	pattern: "{controller=Home}/{action=Index}/{id?}")
	.WithStaticAssets();

app.Run();
