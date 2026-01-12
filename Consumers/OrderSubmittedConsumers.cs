using System.Text.Json;
using ITBusinessCase.Contracts;
using ITBusinessCase.Data;
using ITBusinessCase.Models;
using MassTransit;

namespace ITBusinessCase.Consumers;

public class OrderSubmittedConsumer : IConsumer<OrderSubmitted> {
	private readonly ApplicationDbContext _db;

	public OrderSubmittedConsumer(ApplicationDbContext db) {
		_db = db;
	}

	public async Task Consume(ConsumeContext<OrderSubmitted> context) {
		Console.WriteLine("📦 BESTELLING ONTVANGEN VIA RABBITMQ");
		Console.WriteLine($"OrderId: {context.Message.OrderId}");

		var payloadJson = JsonSerializer.Serialize(context.Message);

		var fullName = $"{context.Message.FirstName} {context.Message.LastName}".Trim();

		var order = new Order {
			OrderId = context.Message.OrderId.ToString(),
			CreatedAtUtc = DateTime.UtcNow,
			Status = "Submitted",
			CustomerEmail = context.Message.Email,
			CustomerName = fullName,
			PayloadJson = payloadJson
			// UserId vullen we via publish (zie verder) OF laten we null
		};

		_db.Orders.Add(order);
		await _db.SaveChangesAsync();
	}
}
