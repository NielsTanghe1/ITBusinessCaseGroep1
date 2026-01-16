using ITBusinessCase.Contracts;
using MassTransit;

namespace ITBusinessCase.Consumers;

public class OrderSubmittedConsumer : IConsumer<OrderSubmitted> {
	public Task Consume(ConsumeContext<OrderSubmitted> context) {
		var m = context.Message;

		Console.WriteLine("==================================================");
		Console.WriteLine($"[ORDER] {m.OrderId}");
		Console.WriteLine($"[USER]  {m.UserId} ({m.UserName})");
		Console.WriteLine($"{m.FirstName} {m.LastName} - {m.Email}");
		Console.WriteLine($"Adres: {m.Street}, {m.Postcode} {m.City}, {m.Country} (Postbus: {m.Postbus})");
		Console.WriteLine($"Totaal: €{m.Total:0.00}");

		if (m.Lines != null && m.Lines.Count > 0) {
			Console.WriteLine("Items:");
			foreach (var l in m.Lines) {
				Console.WriteLine(
					 $" - {l.ProductType} {l.BeanName} ({l.BeanId}) " +
					 $"{l.Kg}KG @ €{l.UnitPricePerKg:0.00}/kg = €{l.LineTotal:0.00}"
				);
			}
		} else {
			Console.WriteLine("Items: (geen)");
		}

		Console.WriteLine("==================================================");
		return Task.CompletedTask;
	}
}
