using MassTransit;
using Microsoft.AspNetCore.Mvc;
using Web.Contracts;
using Web.Data;

[ApiController]
[Route("api/messages")]
public class MessagesController : ControllerBase {
	private readonly IPublishEndpoint _publishEndpoint;

	public MessagesController(IPublishEndpoint publishEndpoint) {
		_publishEndpoint = publishEndpoint;
	}

	// Voor snelle tests via PowerShell/Postman (1 product)
	// vb:
	// POST http://localhost:5264/api/messages/order?productId=espresso&qty=2&firstName=Robin&lastName=Test&email=a@b.be&street=Test&postbus=1&city=Brussel&postcode=1000&country=BE
	[HttpPost("order")]
	public async Task<IActionResult> PublishOrder(
		 [FromQuery] string productId,
		 [FromQuery] int qty,
		 [FromQuery] string firstName,
		 [FromQuery] string lastName,
		 [FromQuery] string email,
		 [FromQuery] string street,
		 [FromQuery] string postbus,
		 [FromQuery] string city,
		 [FromQuery] string postcode,
		 [FromQuery] string country
	) {
		var product = CoffeeCatalog.Items.FirstOrDefault(x => x.Id == productId);
		if (product is null)
			return BadRequest($"Onbekend productId: {productId}");

		if (qty <= 0)
			qty = 1;

		var line = new OrderLine(
			 product.Id,
			 product.Name,
			 product.Price,
			 qty,
			 product.Price * qty
		);

		var total = line.LineTotal;

		var message = new OrderSubmitted(
			 Guid.NewGuid(),
			 firstName,
			 lastName,
			 email,
			 street,
			 postbus,
			 city,
			 postcode,
			 country,
			 total,
			 new List<OrderLine> { line }
		);

		await _publishEndpoint.Publish(message);

		return Ok(new {
			published = true,
			total
		});
	}
}
