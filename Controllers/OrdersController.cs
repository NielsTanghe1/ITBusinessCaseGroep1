using MassTransit;
using Microsoft.AspNetCore.Mvc;
using ITBusinessCase.Contracts;
using ITBusinessCase.Models;
using ITBusinessCase.Data;

namespace ITBusinessCase.Controllers;

public class OrdersController : Controller {
	private readonly IPublishEndpoint _publish;

	public OrdersController(IPublishEndpoint publish) {
		_publish = publish;
	}

	// GET: /Orders/Create
	[HttpGet]
	public IActionResult Create() {
		var vm = new PlaceOrderViewModel {
			Items = CoffeeCatalog.Items
				  .Select(x => new OrderItemInput { ProductId = x.Id, Quantity = 0 })
				  .ToList()
		};

		return View(vm);
	}

	// POST: /Orders/Create
	[HttpPost]
	[ValidateAntiForgeryToken]
	public async Task<IActionResult> Create(PlaceOrderViewModel model) {
		model.Items ??= new List<OrderItemInput>();

		// Minstens 1 product gekozen (qty > 0)
		if (!model.Items.Any(i => i.Quantity > 0))
			ModelState.AddModelError("", "Kies minstens één product en zet het aantal groter dan 0.");

		if (!ModelState.IsValid) {
			// Zorg dat alle items er blijven instaan (voor de view)
			if (model.Items.Count == 0) {
				model.Items = CoffeeCatalog.Items
					 .Select(x => new OrderItemInput { ProductId = x.Id, Quantity = 0 })
					 .ToList();
			}
			return View(model);
		}

		var catalogById = CoffeeCatalog.Items.ToDictionary(x => x.Id, x => x);

		var lines = model.Items
			 .Where(i => i.Quantity > 0)
			 .Select(i => {
				 if (!catalogById.TryGetValue(i.ProductId, out var product))
					 throw new Exception($"Onbekend product: {i.ProductId}");

				 var qty = i.Quantity;
				 var lineTotal = product.Price * qty;

				 return new OrderLine(product.Id, product.Name, product.Price, qty, lineTotal);
			 })
			 .ToList();

		var total = lines.Sum(l => l.LineTotal);

		var message = new OrderSubmitted(
			 Guid.NewGuid(),
			 model.FirstName,
			 model.LastName,
			 model.Email,
			 model.Street,
			 model.Postbus,
			 model.City,
			 model.Postcode,
			 model.Country,
			 total,
			 lines
		);

		await _publish.Publish(message);

		TempData["OrderPlaced"] = $"Order sent to RabbitMQ! Totaal: €{total:0.00}";
		return RedirectToAction(nameof(Create));
	}
}
