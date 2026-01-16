using MassTransit;
using ITBusinessCase.Consumers;
using ITBusinessCase.Data;
using ITBusinessCase.Services;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

// MVC
builder.Services.AddControllersWithViews();

// =======================
// DATABASE + IDENTITY (SQLite)
// =======================
builder.Services.AddDbContext<ApplicationDbContext>(options => {
	options.UseSqlite(builder.Configuration.GetConnectionString("DefaultConnection"));
});

builder.Services.AddIdentity<IdentityUser, IdentityRole>(options => {
	options.Password.RequireDigit = false;
	options.Password.RequireLowercase = false;
	options.Password.RequireUppercase = false;
	options.Password.RequireNonAlphanumeric = false;
	options.Password.RequiredLength = 6;
})
.AddEntityFrameworkStores<ApplicationDbContext>()
.AddDefaultTokenProviders();

builder.Services.ConfigureApplicationCookie(options => {
	options.LoginPath = "/Account/Login";
	options.AccessDeniedPath = "/Account/Login";
});

// =======================
// EMAIL (DI REGISTRATIE) ✅
// =======================
builder.Services.Configure<EmailSettings>(builder.Configuration.GetSection("Email"));
builder.Services.AddScoped<IEmailSender, SmtpEmailSender>();

// =======================
// MASS TRANSIT + RABBITMQ
// =======================
builder.Services.AddMassTransit(x => {
	x.AddConsumer<OrderSubmittedConsumer>();

	x.UsingRabbitMq((context, cfg) =>
	{
		var rabbit = builder.Configuration.GetSection("RabbitMq");

		var host = rabbit["Host"] ?? "10.2.160.222";
		var vhost = rabbit["VirtualHost"] ?? "/";
		var username = rabbit["Username"] ?? "admin";
		var password = rabbit["Password"] ?? "curler-squishy-monogamy";
		var port = rabbit.GetValue<int?>("Port") ?? 5672;

		var vhostPath = (vhost == "/" || string.IsNullOrWhiteSpace(vhost))
			 ? ""
			 : "/" + vhost.TrimStart('/');

		var uri = new Uri($"rabbitmq://{host}:{port}{vhostPath}");

		cfg.Host(uri, h =>
		{
			h.Username(username);
			h.Password(password);
		});

		cfg.ConfigureEndpoints(context);
	});
});

var app = builder.Build();

// ✅ Admin seeding (als je dit al had, laten staan)
await IdentitySeed.EnsureAdminAsync(app.Services);

// =======================
// MIDDLEWARE
// =======================
if (!app.Environment.IsDevelopment()) {
	app.UseExceptionHandler("/Home/Error");
	app.UseHsts();
}

app.UseStaticFiles();
// app.UseHttpsRedirection();

app.UseRouting();

app.UseAuthentication();
app.UseAuthorization();

app.MapControllerRoute(
	 name: "default",
	 pattern: "{controller=Home}/{action=Index}/{id?}");

app.Run();
