using MassTransit;
using Microsoft.AspNetCore.Mvc;
using Models.Entities.DTO;

namespace Web.Controllers.API;

[ApiController]
[Route("api/[controller]")]
public class OrdersController : ControllerBase {
	private readonly IPublishEndpoint _publishEndpoint;

	public OrdersController(IPublishEndpoint publishEndpoint) {
		_publishEndpoint = publishEndpoint;
	}

	[HttpPost]
	public async Task<IActionResult> CreateOrder([FromBody] OrderCreated request) {
		// Map the request to the full OrderCreated contract
		var message = new OrderCreated(
			 Guid.NewGuid().ToString(),
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
