using MassTransit;
using Microsoft.AspNetCore.Mvc;
using Models.Entities.DTO;
using Web.Models;

namespace Web.Controllers.API;

[ApiController]
[Route("api/[controller]")]
public class OrdersController : ControllerBase {
	private readonly IPublishEndpoint _publishEndpoint;

	public OrdersController(IPublishEndpoint publishEndpoint) {
		_publishEndpoint = publishEndpoint;
	}

	[HttpPost]
	public async Task<IActionResult> CreateOrder([FromBody] OrderViewModel request) {
		//// Map the request to the full OrderCreated contract
		//var message = new OrderDTO(
		//);
		//await _publishEndpoint.Publish(message);

		return Accepted();
	}
}
