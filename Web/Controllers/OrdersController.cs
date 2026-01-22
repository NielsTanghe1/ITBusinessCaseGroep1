using MassTransit;
using MassTransit.Transports;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Models.Data;
using Models.Entities;
using Models.Entities.DTO;
using Models.Entities.Enums;
using Web.Models;
using Web.Services;

namespace Web.Controllers;

public class OrdersController : Controller {
	private readonly LocalDbContext _context;
	private readonly Utilities _utilities;
	private readonly ISendEndpointProvider _sendEndpointProvider;
	private readonly IConfiguration _configuration;

	public OrdersController(LocalDbContext context, Utilities utilities, ISendEndpointProvider sendEndpointProvider, IConfiguration configuration) {
		_context = context;
		_utilities = utilities;
		_sendEndpointProvider = sendEndpointProvider;
		_configuration = configuration;
	}

	// GET: Orders
	public async Task<IActionResult> Index() {
		var localDbContext = _context.Orders.Include(o => o.CoffeeUser);
		return View(await localDbContext.ToListAsync());
	}

	// GET: Orders/Details/5
	public async Task<IActionResult> Details(long? id) {
		if (id == null) {
			return NotFound();
		}

		var order = await _context.Orders
			 .Include(o => o.CoffeeUser)
			 .FirstOrDefaultAsync(m => m.Id == id);
		if (order == null) {
			return NotFound();
		}

		return View(order);
	}

	// GET: Orders/Create
	public IActionResult Create() {
		ViewData["CoffeeUserId"] = new SelectList(_context.Users, "Id", "FirstName");
		ViewData["CoffeeTypes"] = _utilities.GetEnumSelectList<CoffeeType>(ignored: ["Unknown"]);
		ViewData["StatusTypes"] = _utilities.GetEnumSelectList<StatusType>(ignored: ["Unknown"]);
		//var queryable = _context.Orders.Where(order => order.CoffeeUserId == 11);
		//Order? order = queryable.ElementAt(2);
		Order order = new() {
			CoffeeUserId = 11
		};

		OrderViewModel viewModel = new() {
			CoffeeUserId = order.CoffeeUserId,
			OrderId = order.Id,
			Items = []
			//Items = _context.OrderItems.Where(item => item.OrderId == order.Id).ToList()
		};
		return View(viewModel);

		//bool parseResult = long.TryParse(User.FindFirstValue(ClaimTypes.NameIdentifier), out long id);
		//if (parseResult) {
		//	return View(new OrderViewModel {
		//		CoffeeUser = _context.Users.Find(id),
		//		Address = _context.Addresses.Find(id)
		//	});
		//}
		//return View();
	}

	// POST: Orders/Create
	// To protect from overposting attacks, enable the specific properties you want to bind to.
	// For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
	[HttpPost]
	[ValidateAntiForgeryToken]
	public async Task<IActionResult> Create(OrderViewModel viewModel) {
		//if (ModelState.IsValid) {
		//	_context.Add(order);
		//	await _context.SaveChangesAsync();
		//	return RedirectToAction(nameof(Index));
		//}
		//ViewData["CoffeeUserId"] = new SelectList(_context.Users, "Id", "FirstName", order.CoffeeUserId);
		//return View(order);

		//bool parseResult = long.TryParse(User.FindFirstValue(ClaimTypes.NameIdentifier), out long userId);
		//Order newOrder = new() {
		//	CoffeeUserId = userId
		//};

		//order.Items ??= new List<OrderItem>();

		//// Minstens 1 product gekozen (qty > 0)
		//if (!order.Items.Any(i => i.Quantity > 0))
		//	ModelState.AddModelError("", "Kies minstens één product en zet het aantal groter dan 0.");

		//if (!ModelState.IsValid) {
		//	// Zorg dat alle items er blijven instaan (voor de view)
		//	if (order.Items.Count == 0) {
		//		order.Items = _context.Coffees
		//			.Select(coffee => new OrderItem {
		//				OrderId = newOrder.Id,
		//				CoffeeId = coffee.Id,
		//				Quantity = 0,
		//				UnitPrice = coffee.Price
		//			})
		//			.ToList();
		//	}
		//	return View(order);
		//}

		//var orderItems = _context.OrderItems.Where(orderItem => orderItem.OrderId == order.Id);
		decimal total = 0;
		var size = viewModel.Items.Count;

		if (!ModelState.IsValid) {
			return View(nameof(Index));
		}

		var myvar = viewModel.Items;

		foreach (OrderItem item in viewModel.Items) {
			//total += (item.UnitPrice * item.Quantity);
			var message = new OrderDTO(
				viewModel.CoffeeUserId,
				viewModel.OrderId,
				item.CoffeeId,
				item.Coffee.Type,
				item.Quantity,
				item.UnitPrice
			);
			var rabbit = _configuration.GetSection("RabbitMQConfig");

			var scheme = rabbit["Scheme"];
			var host = rabbit["Host"];
			var vhost = rabbit["VirtualHost"] ?? "/";
			var port = rabbit.GetValue<int?>("Port:Cluster") ?? 5672;

			// Build URI: vhost "/" -> no path, otherwise "/<vhost>"
			var vhostPath = (string.IsNullOrWhiteSpace(vhost) || vhost == "/")
				? string.Empty
				: "/" + vhost.TrimStart('/');

			var uri = new Uri($"{scheme}://{host}:{port}{vhostPath}/OrderSubmitted");

			var endpoint = await _sendEndpointProvider.GetSendEndpoint(uri);
			await endpoint.Send(message);
		}

		//TempData["OrderPlaced"] = $"Order sent to RabbitMQ! Totaal: €{total:0.00}";
		return RedirectToAction(nameof(Create));
	}

	[HttpPost]
	[ValidateAntiForgeryToken]
	public async Task<IActionResult> AddOrderItem([FromBody] OrderItem posted, int index) {
		if (!ModelState.IsValid) {
			// This will return the exact field names and reasons they failed
			return BadRequest(ModelState);
		}

		if (posted == null)
			return BadRequest();

		// Pass the index to the partial via ViewData
		ViewData["Index"] = index;

		// In a real app, you might save this to a Session or database
		// For dynamic UI, often we return a Partial View of the new row
		//return View(nameof(Create), viewModel);
		return PartialView("_OrderItemRow", posted);
	}

	// GET: Orders/Edit/5
	public async Task<IActionResult> Edit(long? id) {
		if (id == null) {
			return NotFound();
		}

		var order = await _context.Orders.FindAsync(id);
		if (order == null) {
			return NotFound();
		}
		ViewData["CoffeeUserId"] = new SelectList(_context.Users, "Id", "FirstName", order.CoffeeUserId);
		ViewData["StatusTypes"] = _utilities.GetEnumSelectList<StatusType>(ignored: ["Unknown"]);
		return View(order);
	}

	// POST: Orders/Edit/5
	// To protect from overposting attacks, enable the specific properties you want to bind to.
	// For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
	[HttpPost]
	[ValidateAntiForgeryToken]
	public async Task<IActionResult> Edit(long id, [Bind("Id,CoffeeUserId,Status,CreatedAt,DeletedAt")] Order order) {
		if (id != order.Id) {
			return NotFound();
		}

		if (ModelState.IsValid) {
			try {
				_context.Update(order);
				await _context.SaveChangesAsync();
			} catch (DbUpdateConcurrencyException) {
				if (!OrderExists(order.Id)) {
					return NotFound();
				} else {
					throw;
				}
			}
			return RedirectToAction(nameof(Index));
		}
		ViewData["CoffeeUserId"] = new SelectList(_context.Users, "Id", "FirstName", order.CoffeeUserId);
		return View(order);
	}

	// GET: Orders/Delete/5
	public async Task<IActionResult> Delete(long? id) {
		if (id == null) {
			return NotFound();
		}

		var order = await _context.Orders
			 .Include(o => o.CoffeeUser)
			 .FirstOrDefaultAsync(m => m.Id == id);
		if (order == null) {
			return NotFound();
		}

		return View(order);
	}

	// POST: Orders/Delete/5
	[HttpPost, ActionName("Delete")]
	[ValidateAntiForgeryToken]
	public async Task<IActionResult> DeleteConfirmed(long id) {
		var order = await _context.Orders.FindAsync(id);
		if (order != null) {
			_context.Orders.Remove(order);
		}

		await _context.SaveChangesAsync();
		return RedirectToAction(nameof(Index));
	}

	private bool OrderExists(long id) {
		return _context.Orders.Any(e => e.Id == id);
	}
}
