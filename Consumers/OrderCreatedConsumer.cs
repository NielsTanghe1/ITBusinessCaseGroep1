using MassTransit;
using ITBusinessCase.Contracts;

namespace ITBusinessCase.Consumers;

public class OrderCreatedConsumer : IConsumer<OrderCreated> {
	public Task Consume(ConsumeContext<OrderCreated> context) {
		// Use the new FirstName and LastName properties from the updated contract
		//Console.WriteLine($"Received OrderCreated: OrderId={context.Message.OrderId}, Customer={context.Message.FirstName} {context.Message.LastName}");

		Console.WriteLine($"Received OrderCreated: OrderId={context.Message.OrderId}, ProductName={context.Message.ProductName},ProductType={context.Message.ProductType}, Price={context.Message.Price}, Quantity={context.Message.Quantity}, \n, Customer={context.Message.FirstName}, LastName={context.Message.LastName}, Email={context.Message.Email}, \n Postcode={context.Message.Postcode}, City={context.Message.City},  Street={context.Message.Street}, Postbus={context.Message.Postbus}, Country={context.Message.Country}, Cardnumber={context.Message.Cardnumber}, {context.Message.CVV}");

		return Task.CompletedTask;
	}
}