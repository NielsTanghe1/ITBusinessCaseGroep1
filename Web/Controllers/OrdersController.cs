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
		decimal total = 0;
		var size = viewModel.Items.Count;

		if (!ModelState.IsValid) {
			// Return the form with validation errors
			ViewData["CoffeeUserId"] = new SelectList(_context.Users, "Id", "FirstName");
			ViewData["CoffeeTypes"] = _utilities.GetEnumSelectList<CoffeeType>(ignored: ["Unknown"]);
			ViewData["StatusTypes"] = _utilities.GetEnumSelectList<StatusType>(ignored: ["Unknown"]);
			return View(viewModel);
		}

		// Create the order in the database first
		var order = new Order {
			CoffeeUserId = viewModel.CoffeeUserId,
			Status = StatusType.Pending,
		};
		_context.Orders.Add(order);
		await _context.SaveChangesAsync(); // This generates the Order ID

		foreach (OrderItem item in viewModel.Items) {
			// Load Coffee from database using CoffeeId
			var coffee = await _context.Coffees.FindAsync(item.CoffeeId);

			if (coffee == null) {
				ModelState.AddModelError("", $"Coffee with ID {item.CoffeeId} not found.");
				continue;
			}

			// Create OrderItem in database to get unique ID
			item.OrderId = order.Id;
			_context.OrderItems.Add(item);
			await _context.SaveChangesAsync(); // This generates the OrderItem ID

			var message = new OrderRecord(
				viewModel.CoffeeUserId,
				item.Id, // Use the unique OrderItem ID instead of Order ID
				item.CoffeeId,
				coffee.Type,
				item.Quantity,
				item.UnitPrice
			);

			var rabbit = _configuration.GetSection("RabbitMQConfig");

			var host = rabbit["Host"];
			var vhost = rabbit["VirtualHost"] ?? "/";
			var port = rabbit.GetValue<int?>("Port:Cluster") ?? 5672;

			// Build URI: vhost "/" -> no path, otherwise "/<vhost>"
			var vhostPath = (string.IsNullOrWhiteSpace(vhost) || vhost == "/")
				? string.Empty
				: "/" + vhost.TrimStart('/');

			var uri = new Uri($"rabbitmq://{host}:{port}{vhostPath}/OrderSubmitted");

			var endpoint = await _sendEndpointProvider.GetSendEndpoint(uri);
			await endpoint.Send(message);

			total += item.Quantity * item.UnitPrice;
		}

		TempData["OrderPlaced"] = $"Order sent to RabbitMQ! Totaal: €{total:0.00}";

		// ✅ CHANGED: stay on /Orders/Create instead of going to /Orders (Index)
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
