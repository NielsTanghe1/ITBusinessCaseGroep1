using Microsoft.AspNetCore.Mvc.RazorPages;
using Web.Consumers;

namespace Web.Pages.Orders;

/// <summary>
/// Page model for displaying confirmed orders from the OrderConfirmed queue.
/// </summary>
public class ConfirmedOrdersModel : PageModel {
	private readonly ILogger<ConfirmedOrdersModel> _logger;

	public ConfirmedOrdersModel(ILogger<ConfirmedOrdersModel> logger) {
		_logger = logger;
	}

	public List<OrderConfirmedMessage> Orders { get; set; } = new();

	public void OnGet() {
		// Fetch all orders from the queue
		Orders = OrderConfirmedConsumer.ReceivedOrders.ToList();
		_logger.LogInformation("Displaying {Count} confirmed orders", Orders.Count);
	}
}