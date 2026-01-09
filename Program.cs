using MassTransit;
using ITBusinessCase.Consumers;

var builder = WebApplication.CreateBuilder(args);

// MVC
builder.Services.AddControllersWithViews();

// =======================
// MASS TRANSIT + RABBITMQ
// =======================
builder.Services.AddMassTransit(x => {
	x.AddConsumer<OrderSubmittedConsumer>();

	x.UsingRabbitMq((context, cfg) => {
		var rabbit = builder.Configuration.GetSection("RabbitMq");

		var host = rabbit["Host"] ?? "localhost";
		var vhost = rabbit["VirtualHost"] ?? "/";
		var username = rabbit["Username"] ?? "guest";
		var password = rabbit["Password"] ?? "guest";
		var port = rabbit.GetValue<int?>("Port") ?? 5672;

		// ✅ Overal compatibel: Uri gebruiken
		// Let op: vhost "/" -> leeg in uri, anders "/<vhost>"
		var vhostPath = (vhost == "/" || string.IsNullOrWhiteSpace(vhost))
			 ? ""
			 : "/" + vhost.TrimStart('/');

		var uri = new Uri($"rabbitmq://{host}:{port}{vhostPath}");

		cfg.Host(uri, h => {
			h.Username(username);
			h.Password(password);
		});

		cfg.ConfigureEndpoints(context);
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

// ✅ Nodig voor wwwroot/images/css/js (jouw foto's!)
app.UseStaticFiles();

// Als je enkel HTTP gebruikt, laat deze uit
// app.UseHttpsRedirection();

app.UseRouting();
app.UseAuthorization();

app.MapControllerRoute(
	 name: "default",
	 pattern: "{controller=Home}/{action=Index}/{id?}");

app.Run();
