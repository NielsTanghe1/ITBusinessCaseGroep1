using MassTransit;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Models.Data;
using Models.Entities;
using Models.Entities.DTO;
using System.Security.Claims;
using Web.Models;

namespace Web.Controllers;

public class OrdersController : Controller {
	private readonly LocalDbContext _context;
	private readonly IPublishEndpoint _publish;

	public OrdersController(LocalDbContext context, IPublishEndpoint publish) {
		_context = context;
		_publish = publish;
	}

	// GET: /Orders/Create
	[HttpGet]
	public IActionResult Create() {
		//ViewData["CoffeeUserId"] = new SelectList(_context.Users, "Id", "FirstName");
		//return View();

		bool parseResult = long.TryParse(User.FindFirstValue(ClaimTypes.NameIdentifier), out long id);
		if (parseResult) {
			return View(new OrderViewModel {
				CoffeeUser = _context.Users.Find(id),
				Address = _context.Addresses.Find(id)
			});
		}
		return View();
	}

	// POST: /Orders/Create
	// To protect from overposting attacks, enable the specific properties you want to bind to.
	// For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
	[HttpPost]
	[ValidateAntiForgeryToken]
	public async Task<IActionResult> Create(OrderViewModel order) {
		//if (ModelState.IsValid) {
		//	_context.Add(order);
		//	await _context.SaveChangesAsync();
		//	return RedirectToAction(nameof(Index));
		//}
		//ViewData["CoffeeUserId"] = new SelectList(_context.Users, "Id", "FirstName", order.CoffeeUserId);
		//return View(order);

		bool parseResult = long.TryParse(User.FindFirstValue(ClaimTypes.NameIdentifier), out long userId);
		Order newOrder = new() { CoffeeUserId = userId };
		order.Items ??= new List<OrderItem>();

		// Minstens 1 product gekozen (qty > 0)
		if (!order.Items.Any(i => i.Quantity > 0))
			ModelState.AddModelError("", "Kies minstens één product en zet het aantal groter dan 0.");

		if (!ModelState.IsValid) {
			// Zorg dat alle items er blijven instaan (voor de view)
			if (order.Items.Count == 0) {
				order.Items = _context.Coffees
					.Select(coffee => new OrderItem {
						OrderId = newOrder.Id,
						CoffeeId = coffee.Id,
						Quantity = 0,
						PriceAtPurchase = coffee.Price
					})
					.ToList();
			}
			return View(order);
		}

		var catalogById = _context.Coffees.ToDictionary(x => x.Id, x => x);

		var lines = order.Items
			 .Where(i => i.Quantity > 0)
			 .Select(i => {
				 if (!catalogById.TryGetValue(i.CoffeeId, out var product))
					 throw new Exception($"Onbekend product: {i.CoffeeId}");

				 var qty = i.Quantity;
				 var lineTotal = product.Price * qty;

				 return new OrderLine(Guid.NewGuid().ToString(), product.Id, product.Name.ToString(), product.Price, qty, lineTotal);
			 })
			 .ToList();

		var total = lines.Sum(l => l.LineTotal);
		var address = _context.Addresses.First(addr => addr.CoffeeUserId == order.CoffeeUser.Id);

		var message = new OrderSubmitted(
			 Guid.NewGuid().ToString(),
			 order.CoffeeUser.FirstName,
			 order.CoffeeUser.LastName,
			 order.CoffeeUser.Email,

			 address.Type.ToString(),
			 address.Street,
			 address.HouseNumber,
			 address.City,
			 address.PostalCode,
			 address.CountryISO,
			 address.UnitNumber,

			 total,
			 lines
		);

		await _publish.Publish(message);

		TempData["OrderPlaced"] = $"Order sent to RabbitMQ! Totaal: €{total:0.00}";
		return RedirectToAction(nameof(Create));
	}

	private bool OrderExists(long id) {
		return _context.Orders.Any(e => e.Id == id);
	}
}
