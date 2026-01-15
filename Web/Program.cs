using MassTransit;
using Web.Consumers;

var builder = WebApplication.CreateBuilder(args);
bool isDebugging = bool.TryParse(builder.Configuration["GlobalAppSettings:IsDebugging"], out bool isDebuggingResult);

// MVC
builder.Services.AddControllersWithViews();

// =======================
// MASS TRANSIT + RABBITMQ
// =======================
builder.Services.AddMassTransit(x => {
	x.AddConsumer<SalesforceConsumer>();
	x.AddConsumer<SapIdocConsumer>();
	x.AddConsumer<OrderCreatedConsumer>();
	x.AddConsumer<OrderSubmittedConsumer>();

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

		// Prep queues in RabbitMQ:
		cfg.ReceiveEndpoint("order-queue", e => {
			e.ConfigureConsumer<OrderCreatedConsumer>(context);
			e.ConcurrentMessageLimit = 4;
		});

		cfg.ReceiveEndpoint("salesforce-queue", e => {
			e.ConfigureConsumer<SalesforceConsumer>(context);
			e.ConcurrentMessageLimit = 4;
		});

		cfg.ReceiveEndpoint("sapidoc-queue", e => {
			e.ConfigureConsumer<SapIdocConsumer>(context);
			e.ConcurrentMessageLimit = 4;
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

// ✅ Nodig voor wwwroot/images/css/js
app.UseStaticFiles();

// Als je enkel HTTP gebruikt, laat deze uit
// app.UseHttpsRedirection();

app.UseRouting();
app.UseAuthorization();

app.MapControllerRoute(
	name: "default",
	pattern: "{controller=Home}/{action=Index}/{id?}")
	.WithStaticAssets();

app.Run();
