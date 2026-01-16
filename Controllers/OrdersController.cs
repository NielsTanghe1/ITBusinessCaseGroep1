using MassTransit;
using Microsoft.AspNetCore.Mvc;
using ITBusinessCase.Contracts;
using ITBusinessCase.Models;
using ITBusinessCase.Data;
using System.Security.Claims;

namespace ITBusinessCase.Controllers;

public class OrdersController : Controller {
	private readonly IPublishEndpoint _publish;

	public OrdersController(IPublishEndpoint publish) {
		_publish = publish;
	}

	[HttpGet]
	public IActionResult Create() {
		var vm = new PlaceOrderViewModel {
			Items = BeanCatalog.Beans
				  .Select(b => new BeanOrderItemInput {
					  BeanId = b.Id,
					  ProductType = "Roasted",
					  Kg = 0
				  })
				  .ToList()
		};

		return View(vm);
	}

	[HttpPost]
	[ValidateAntiForgeryToken]
	public async Task<IActionResult> Create(PlaceOrderViewModel model) {
		model.Items ??= new List<BeanOrderItemInput>();

		model.FirstName = (model.FirstName ?? "").Trim();
		model.LastName = (model.LastName ?? "").Trim();
		model.Email = (model.Email ?? "").Trim();
		model.Street = (model.Street ?? "").Trim();
		model.Postbus = (model.Postbus ?? "").Trim();
		model.City = (model.City ?? "").Trim();
		model.Postcode = (model.Postcode ?? "").Trim();
		model.Country = (model.Country ?? "").Trim();

		if (!model.Items.Any(i => i.Kg > 0))
			ModelState.AddModelError("", "Kies minstens één product en zet KG groter dan 0.");

		if (!ModelState.IsValid) {
			if (model.Items.Count == 0) {
				model.Items = BeanCatalog.Beans
					 .Select(b => new BeanOrderItemInput { BeanId = b.Id, ProductType = "Roasted", Kg = 0 })
					 .ToList();
			}
			return View(model);
		}

		var beansById = BeanCatalog.Beans.ToDictionary(b => b.Id, b => b);

		var lines = model.Items
			 .Where(i => i.Kg > 0)
			 .Select(i => {
				 if (!beansById.TryGetValue(i.BeanId, out var bean))
					 throw new Exception($"Onbekende beanId: {i.BeanId}");

				 var type = (i.ProductType ?? "Roasted").Trim();
				 var unit = BeanCatalog.GetPricePerKg(type, bean.Id);
				 var total = unit * i.Kg;

				 return new OrderLine(bean.Id, bean.Name, type, i.Kg, unit, total);
			 })
			 .ToList();

		var totalSum = lines.Sum(l => l.LineTotal);

		// ✅ UserId en UserName uit claims halen
		var userId = User.FindFirstValue(ClaimTypes.NameIdentifier) ?? "anonymous";
		var userName = User.Identity?.Name ?? "anonymous";

		var message = new OrderSubmitted(
			 Guid.NewGuid(),

			 userId,
			 userName,

			 model.FirstName,
			 model.LastName,
			 model.Email,
			 model.Street,
			 model.Postbus,
			 model.City,
			 model.Postcode,
			 model.Country,
			 totalSum,
			 lines
		);

		await _publish.Publish(message);

		TempData["OrderPlaced"] = $"Order sent to RabbitMQ! Totaal: €{totalSum:0.00}";
		return RedirectToAction(nameof(Create));
	}
}
