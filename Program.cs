using ITBusinessCase.Consumers;
using MassTransit;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddMassTransit(x => {

	x.AddConsumer<SalesforceConsumer>();
	x.AddConsumer<SapIdocConsumer>();
	x.AddConsumer<OrderCreatedConsumer>(); // Kies voor RabbitMQ als transport
	x.UsingRabbitMq((context, cfg) =>
	{
		var rabbitMqHost = builder.Configuration.GetConnectionString("RabbitMQ");

		if (builder.Environment.IsDevelopment()) {
			// Local dev: connect to localhost
			cfg.Host("localhost", h =>
			{
				h.Username("guest");
				h.Password("guest");
			});
		} else {
			// Docker/prod: use container name
			cfg.Host(rabbitMqHost, "/", h =>
			{
				h.Username("guest");
				h.Password("guest");
			});
		}

		cfg.ReceiveEndpoint("order-queue", e =>
		{
			e.ConfigureConsumer<SalesforceConsumer>(context);
			e.ConfigureConsumer<SapIdocConsumer>(context);
			e.ConfigureConsumer<OrderCreatedConsumer>(context);
			e.ConcurrentMessageLimit = 4;
		});
	});

});

// Add services to the container.
builder.Services.AddControllersWithViews();

var app = builder.Build();

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

app.MapControllerRoute(
	 name: "default",
	 pattern: "{controller=Home}/{action=Index}/{id?}")
	 .WithStaticAssets();


app.Run();
