using MassTransit;
using ITBusinessCase.Models;
using ITBusinessCase.Contracts;
using Microsoft.AspNetCore.Mvc;

[ApiController]
[Route("api/[controller]")]
public class OrderController : ControllerBase {
	private readonly IPublishEndpoint _publishEndpoint;

	public OrderController(IPublishEndpoint publishEndpoint) {
		_publishEndpoint = publishEndpoint;
	}

	[HttpPost]
	public async Task<IActionResult> CreateOrder([FromBody] CreateOrderRequest request) {
		var message = new OrderCreated(
			 Guid.NewGuid(),
			 request.CustomerName,
			 request.TotalAmount
		);

		await _publishEndpoint.Publish(message);

		return Accepted();
	}
}

public record CreateOrderRequest(string CustomerName, decimal TotalAmount);