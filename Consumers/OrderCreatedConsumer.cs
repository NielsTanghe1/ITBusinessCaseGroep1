using MassTransit;
using ITBusinessCase.Contracts;

namespace ITBusinessCase.Consumers;

public class OrderCreatedConsumer : IConsumer<OrderCreated> {
	public Task Consume(ConsumeContext<OrderCreated> context) {
		Console.WriteLine($"Received OrderCreated: OrderId={context.Message.OrderId}, Customer={context.Message.CustomerName}");

		// Your business logic here
		return Task.CompletedTask;
	}
}