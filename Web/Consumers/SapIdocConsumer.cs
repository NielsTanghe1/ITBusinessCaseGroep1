using MassTransit;
using Web.Contracts;

namespace Web.Consumers;

public class SapIdocConsumer : IConsumer<OrderCreated> {
	public async Task Consume(ConsumeContext<OrderCreated> context) {
		// Logic to generate and send SAP IDOC
		Console.WriteLine($"Sending Order {context.Message.OrderId} to SAP-IDOC...");
		await Task.CompletedTask;
	}
}