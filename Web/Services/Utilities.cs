using MassTransit;
using Microsoft.AspNetCore.Mvc.Rendering;
using Models.Entities;

namespace Web.Services;

/// <summary>
/// Provides shared helper methods for the ITBusinessCoffee application,
/// including UI utilities and messaging helpers.
/// </summary>
/// <remarks>
/// This class centralizes cross-cutting helpers that do not belong to a specific
/// domain service, such as:<br/>
/// - Building dropdown lists for enums<br/>
/// - Sending messages to the configured message broker<br/>
/// 
/// It is registered with dependency injection and should be consumed via
/// constructor injection where needed.
/// </remarks>
public class Utilities {
	private readonly ISendEndpointProvider _sendEndpointProvider;
	private readonly IConfiguration _configuration;

	/// <summary>
	/// Initializes a new instance of the <see cref="Utilities"/> class with
	/// the required dependencies.
	/// </summary>
	/// <param name="sendEndpointProvider">
	/// The send endpoint provider used to obtain endpoints for sending messages
	/// (for example, to specific RabbitMQ queues via MassTransit).
	/// </param>
	/// <param name="configuration">
	/// The application configuration instance used to read settings that affect
	/// utility behavior, such as queue names or enum display options.
	/// </param>
	public Utilities(
		ISendEndpointProvider sendEndpointProvider,
		IConfiguration configuration) {
		_sendEndpointProvider = sendEndpointProvider;
		_configuration = configuration;
	}

	/// <summary>
	/// Creates a <see cref="List{SelectListItem}"/> suitable for ASP.NET Core dropdowns (e.g. <c>&lt;select asp-items="@Model.CoffeeTypes"&gt;</c>)
	/// from the values of the specified enum type.
	/// </summary>
	/// <typeparam name="T">
	/// The enum type to convert to select options.
	/// </typeparam>
	/// <param name="selectedValue">
	/// Optional. The enum value that should be pre-selected in the dropdown. Defaults to <see langword="null"/> (no selection).
	/// </param>
	/// <param name="ignored">
	/// Optional array of enum names (case-sensitive) to exclude from the select list.
	/// Useful for hiding "None", "Unknown", or internal enum values.
	/// </param>
	/// <returns>
	/// A <see cref="List{SelectListItem}"/> containing all valid enum values as options,
	/// with the specified <paramref name="selectedValue"/> pre-selected if provided.
	/// </returns>
	/// <remarks>
	/// Each <see cref="SelectListItem"/> uses:<br />
	/// - <c>Value</c> = enum's underlying integer value (as string)<br/>
	/// - <c>Text</c> = enum's underlying integer value (as string)<br/>
	/// - <c>Selected</c> = matches <paramref name="selectedValue"/><br/>
	/// </remarks>
	/// <example>
	/// <code>
	/// var coffeeTypes = GetEnumSelectList&lt;CoffeeType&gt;(CoffeeType.Espresso, ["Unknown"]);
	/// Results in: ["Espresso" (selected), "Latte", "Cappuccino"]
	/// </code>
	/// </example>
	public List<SelectListItem> GetEnumSelectList<T>(Enum? selectedValue = null, string[]? ignored = null) where T : notnull, Enum {
		List<SelectListItem> list = [];

		// Placeholder only if no selected value
		if (selectedValue == null) {
			list.Add(new() {
				Value = "",
				Text = "Select a value",
				Selected = true
			});
		}

		foreach (Enum item in Enum.GetValues(typeof(T))) {
			if (ignored != null && ignored.Contains(item.ToString())) {
				continue;
			}

			list.Add(new() {
				Value = item.ToString(),
				Text = item.ToString(),
				Selected = (selectedValue?.ToString() == item.ToString())
			});
		}
		return list;
	}

	/// <summary>
	/// Sends a message of type <typeparamref name="T"/> to the specified RabbitMQ queue/endpoint
	/// using MassTransit.
	/// </summary>
	/// <typeparam name="T">
	/// The message type to send. Must be a record or a simple object.
	/// </typeparam>
	/// <param name="targetQueue">
	/// The RabbitMQ queue name or endpoint address where the message should be routed.
	/// </param>
	/// <param name="message">
	/// The message payload to publish/send.
	/// </param>
	/// <returns>
	/// A <see cref="Task"/> representing the asynchronous send operation.
	/// </returns>
	/// <exception cref="ArgumentNullException">
	/// Thrown when <paramref name="targetQueue"/> or <paramref name="message"/> is <see langword="null"/>.
	/// </exception>
	/// <exception cref="MassTransitException">
	/// Thrown when the send operation fails (e.g. broker unreachable, queue doesn't exist).
	/// </exception>
	/// <remarks>
	/// This method resolves the MassTransit <see cref="ISendEndpoint"/> for the target queue
	/// and sends the message asynchronously. The message will be routed to consumers
	/// configured for type <typeparamref name="T"/> on the target queue.
	/// </remarks>
	/// <example>
	/// <code>
	/// await SendMessageTo("order-queue", new OrderCreatedEvent {
	///     OrderId = 123,
	///     CustomerEmail = "user@example.com"
	/// });
	/// </code>
	/// </example>
	public async Task SendMessageTo<T>(string targetQueue, T message) where T : notnull {
		var rabbit = _configuration.GetSection("RabbitMQConfig");
		var host = rabbit["Host"];
		var vhost = rabbit["VirtualHost"] ?? "/";
		var port = rabbit.GetValue<int?>("Port:Cluster") ?? 5672;

		// Build URI: vhost "/" -> no path, otherwise "/<vhost>"
		var vhostPath = (string.IsNullOrWhiteSpace(vhost) || vhost == "/")
			? string.Empty
			: "/" + vhost.TrimStart('/');

		var uri = new Uri($"rabbitmq://{host}:{port}{vhostPath}/{targetQueue}");
		var endpoint = await _sendEndpointProvider.GetSendEndpoint(uri);

		await endpoint.Send(message);
	}

	/// <summary>
	/// Calculates the total price for a single order item by multiplying quantity by unit price.
	/// </summary>
	/// <param name="orderItem">
	/// The order item containing <see cref="OrderItem.Quantity"/> and <see cref="OrderItem.UnitPrice"/>.
	/// </param>
	/// <returns>
	/// The total price as a <see cref="decimal"/>, calculated as <c>orderItem.Quantity * orderItem.UnitPrice</c>.
	/// </returns>
	public static decimal GetTotalPrice(OrderItem orderItem) {
		return (orderItem.Quantity * orderItem.UnitPrice);
	}
}
