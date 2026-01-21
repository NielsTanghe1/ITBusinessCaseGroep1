using MassTransit;
using Microsoft.AspNetCore.Mvc;
using Models.Data;
using Models.Entities.DTO;

[ApiController]
[Route("api/messages")]
public class MessagesController : ControllerBase {
	private readonly LocalDbContext _context;
	private readonly IPublishEndpoint _publishEndpoint;

	public MessagesController(LocalDbContext context, IPublishEndpoint publishEndpoint) {
		_context = context;
		_publishEndpoint = publishEndpoint;
	}

	// Voor snelle tests via PowerShell/Postman (1 product)
	// vb:
	// POST http://localhost:5264/api/messages/order?productId=espresso&qty=2&firstName=Robin&lastName=Test&email=a@b.be&street=Test&postbus=1&city=Brussel&postcode=1000&country=BE
	[HttpPost("order")]
	public async Task<IActionResult> PublishOrder(
		[FromQuery] long productId,
		[FromQuery] int qty,
		[FromQuery] string firstName,
		[FromQuery] string lastName,
		[FromQuery] string email,
		[FromQuery] string type,
		[FromQuery] string street,
		[FromQuery] int houseNumber,
		[FromQuery] string city,
		[FromQuery] string postalCode,
		[FromQuery] string countryISO,
		[FromQuery] string unitNumber
	) {
		//var product = _context.Coffees.FirstOrDefault(x => x.Id == productId);
		//if (product is null) {
		//	return BadRequest($"Onbekend productId: {productId}");
		//}

		//if (qty <= 0) {
		//	qty = 1;
		//}

		//var line = new OrderDTO(
		//	Guid.NewGuid().ToString(),
		//	product.Id,
		//	product.Name.ToString(),
		//	product.Price,
		//	qty,
		//	product.Price * qty
		//);

		//var total = line.LineTotal;

		//var message = new OrderSubmitted(
		//	Guid.NewGuid().ToString(),
		//	firstName,
		//	lastName,
		//	email,

		//	type,
		//	street,
		//	houseNumber,
		//	city,
		//	postalCode,
		//	countryISO,
		//	unitNumber,

		//	total,
		//	new List<OrderDTO> { line }
		//);

		//await _publishEndpoint.Publish(message);

		return Ok(new {
			published = true,
			//total
		});
	}
}
