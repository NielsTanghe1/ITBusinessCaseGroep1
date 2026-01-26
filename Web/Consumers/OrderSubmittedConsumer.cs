using MassTransit;
using System.Collections.Concurrent;

namespace Web.Consumers;

/// <summary>
/// Consumer that processes confirmed orders from the OrderConfirmed queue.
/// </summary>
public class OrderConfirmedConsumer : IConsumer<OrderConfirmedMessage> {
	// Thread-safe collection to store received orders
	public static readonly ConcurrentQueue<OrderConfirmedMessage> ReceivedOrders = new();
	private readonly ILogger<OrderConfirmedConsumer> _logger;

	public OrderConfirmedConsumer(ILogger<OrderConfirmedConsumer> logger) {
		_logger = logger;
	}

	public Task Consume(ConsumeContext<OrderConfirmedMessage> context) {
		var message = context.Message;
		_logger.LogInformation("Received confirmed order {OrderId} from user {UserId} with status {Status}", 
			message.OrderId, message.CoffeeUserId, message.Status);

		ReceivedOrders.Enqueue(message);
		return Task.CompletedTask;
	}
}

/// <summary>
/// Message contract for OrderConfirmed queue.
/// </summary>
public record OrderConfirmedMessage {
	public long OrderId { get; init; }
	public long CoffeeUserId { get; init; }
	public string Status { get; init; } = string.Empty;
	public DateTime ConfirmedAt { get; init; }
}