using Web.Contracts;
using Web.Data;
using MassTransit;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace Web.Controllers;

[ApiController]
[Route("api/messages")]
public class MessagesController : ControllerBase {
	private readonly IPublishEndpoint _publish;

	public MessagesController(IPublishEndpoint publish) {
		_publish = publish;
	}

	// POST http://localhost:5264/api/messages/order?beanId=arabica&type=Roasted&kg=2&firstName=Robin&lastName=Test&email=a@b.be&street=Test&postbus=1&city=Brussel&postcode=1000&country=BE
	[HttpPost("order")]
	public async Task<IActionResult> PublishOrder(
		 [FromQuery] string beanId,
		 [FromQuery] string type,
		 [FromQuery] int kg,
		 [FromQuery] string firstName,
		 [FromQuery] string lastName,
		 [FromQuery] string email,
		 [FromQuery] string street,
		 [FromQuery] string postbus,
		 [FromQuery] string city,
		 [FromQuery] string postcode,
		 [FromQuery] string country
	) {
		beanId = (beanId ?? "").Trim().ToLower();
		type = string.IsNullOrWhiteSpace(type) ? "Roasted" : type.Trim();
		if (kg <= 0)
			kg = 1;

		var bean = BeanCatalog.Beans.FirstOrDefault(b => b.Id == beanId);
		if (bean is null)
			return BadRequest($"Onbekende beanId: {beanId}");

		var unit = BeanCatalog.GetPricePerKg(type, bean.Id);
		var lineTotal = unit * kg;

		var line = new OrderLine(bean.Id, bean.Name, type, kg, unit, lineTotal);

		var userId = User.FindFirstValue(ClaimTypes.NameIdentifier) ?? "anonymous";
		var userName = User.Identity?.Name ?? "anonymous";

		var message = new OrderSubmitted(
			 Guid.NewGuid(),

			 userId,
			 userName,

			 firstName,
			 lastName,
			 email,
			 street,
			 postbus,
			 city,
			 postcode,
			 country,
			 lineTotal,
			 new List<OrderLine> { line }
		);

		await _publish.Publish(message);

		return Ok(new { published = true, userId, userName, total = lineTotal });
	}
}
