using MassTransit;
using Web.Contracts;

namespace Web.Consumers;

public class OrderSubmittedConsumer : IConsumer<OrderSubmitted> {
	public Task Consume(ConsumeContext<OrderSubmitted> context) {
		var m = context.Message;

		Console.WriteLine($"[ORDER] {m.FirstName} {m.LastName} - {m.Email} - Total €{m.Total:0.00}");
		Console.WriteLine($"Adres: {m.Street}, {m.Postcode} {m.City}, {m.Country} (Postbus: {m.Postbus})");

		if (m.Lines is null || m.Lines.Count == 0) {
			Console.WriteLine("  (Geen order lines ontvangen)");
		} else {
			foreach (var l in m.Lines) {
				Console.WriteLine($"  - {l.Quantity}x {l.ProductName} (€{l.UnitPrice:0.00}) = €{l.LineTotal:0.00}");
			}
		}

		Console.WriteLine($"OrderId: {m.OrderId}\n");
		return Task.CompletedTask;
	}
}
