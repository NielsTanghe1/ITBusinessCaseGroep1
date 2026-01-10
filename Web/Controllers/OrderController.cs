using MassTransit;
using Web.Contracts;
using Microsoft.AspNetCore.Mvc;

namespace Web.Controllers;

[ApiController]

[Route("api/[controller]")]
public class OrderController : ControllerBase {
	private readonly IPublishEndpoint _publishEndpoint;

	public OrderController(IPublishEndpoint publishEndpoint) {
		_publishEndpoint = publishEndpoint;
	}

	[HttpPost]
	public async Task<IActionResult> CreateOrder([FromBody] CreateOrderRequest request) {
		// Map the request to the full OrderCreated contract
		var message = new OrderCreated(
			 Guid.NewGuid(),
			 request.ProductName,
			 request.ProductType,
			 request.Price,
			 request.Quantity,
			 request.FirstName,
			 request.LastName,
			 request.Email,
			 request.Postcode,
			 request.City,
			 request.Street,
			 request.Postbus,
			 request.Country,
			 request.Cardnumber,
			 request.CVV
		);

		await _publishEndpoint.Publish(message);

		return Accepted();
	}
}

// Request DTO matching the architecture tables
public record CreateOrderRequest(
	 string ProductName, string ProductType, decimal Price, int Quantity,
	 string FirstName, string LastName, string Email,
	 int Postcode, string City, string Street, string Postbus, string Country,
	 string Cardnumber, int CVV
);