using MassTransit;
using ITBusinessCase.Contracts;

namespace ITBusinessCase.Consumers;

public class OrderCreatedConsumer : IConsumer<OrderCreated> {
	public Task Consume(ConsumeContext<OrderCreated> context) {
		// Use the new FirstName and LastName properties from the updated contract
		Console.WriteLine($"Received OrderCreated: OrderId={context.Message.OrderId}, Customer={context.Message.FirstName} {context.Message.LastName}");

		return Task.CompletedTask;
	}
}