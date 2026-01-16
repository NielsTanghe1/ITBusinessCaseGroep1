using MassTransit;
using Microsoft.AspNetCore.Mvc;
using ITBusinessCase.Contracts;
using ITBusinessCase.Models;
using ITBusinessCase.Data;
using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;

namespace ITBusinessCase.Controllers;

[Authorize]
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
					  RoastedKg = 0,
					  GroundKg = 0,
					  RawKg = 0
				  })
				  .ToList()
		};

		return View(vm);
	}

	[HttpPost]
	[ValidateAntiForgeryToken]
	public async Task<IActionResult> Create(PlaceOrderViewModel model) {
		model.Items ??= new List<BeanOrderItemInput>();

		// Minstens één type > 0
		if (!model.Items.Any(i => i.RoastedKg > 0 || i.GroundKg > 0 || i.RawKg > 0))
			ModelState.AddModelError("", "Kies minstens één producttype en zet KG groter dan 0.");

		if (!ModelState.IsValid) {
			// zorg dat items er blijven instaan
			if (model.Items.Count == 0) {
				model.Items = BeanCatalog.Beans
					 .Select(b => new BeanOrderItemInput { BeanId = b.Id, RoastedKg = 0, GroundKg = 0, RawKg = 0 })
					 .ToList();
			}
			return View(model);
		}

		var beansById = BeanCatalog.Beans.ToDictionary(b => b.Id, b => b);

		var lines = new List<OrderLine>();

		foreach (var item in model.Items) {
			if (!beansById.TryGetValue(item.BeanId, out var bean))
				throw new Exception($"Onbekende beanId: {item.BeanId}");

			// helper om lijn toe te voegen
			void AddLine(string type, int kg) {
				if (kg <= 0)
					return;

				var unit = BeanCatalog.GetPricePerKg(type, bean.Id);
				var lineTotal = unit * kg;

				lines.Add(new OrderLine(
					 BeanId: bean.Id,
					 BeanName: bean.Name,
					 ProductType: type,
					 Kg: kg,
					 UnitPricePerKg: unit,
					 LineTotal: lineTotal
				));
			}

			AddLine("Roasted", item.RoastedKg);
			AddLine("Ground", item.GroundKg);
			AddLine("Raw", item.RawKg);
		}

		var total = lines.Sum(l => l.LineTotal);

		var userId = User.FindFirstValue(ClaimTypes.NameIdentifier) ?? "";
		var userName = User.Identity?.Name ?? "";

		var message = new OrderSubmitted(
			 OrderId: Guid.NewGuid(),
			 UserId: userId,
			 UserName: userName,
			 FirstName: model.FirstName ?? "",
			 LastName: model.LastName ?? "",
			 Email: model.Email ?? "",
			 Street: model.Street ?? "",
			 Postbus: model.Postbus ?? "",
			 City: model.City ?? "",
			 Postcode: model.Postcode ?? "",
			 Country: model.Country ?? "",
			 Total: total,
			 Lines: lines
		);

		await _publish.Publish(message);

		TempData["OrderPlaced"] = $"Order sent to RabbitMQ! Totaal: €{total:0.00}";
		return RedirectToAction(nameof(Create));
	}
}
