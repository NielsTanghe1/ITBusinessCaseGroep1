using MassTransit;
using ITBusinessCase.Contracts;

namespace ITBusinessCase.Consumers;

public class SalesforceConsumer : IConsumer<OrderCreated> {
	public async Task Consume(ConsumeContext<OrderCreated> context) {
		// Logic to push to Salesforce API
		Console.WriteLine($"Pushing Order {context.Message.OrderId} to Salesforce...");
		await Task.CompletedTask;
	}
}