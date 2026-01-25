using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Models.Data;
using Models.Entities;

namespace Web.Controllers;

public class HomeController : Controller {
	private readonly ILogger<HomeController> _logger;
	private readonly IConfiguration _configuration;
	private readonly UserManager<CoffeeUser> _userManager;
	private readonly LocalDbContext _localContext;

	public HomeController(
		ILogger<HomeController> logger,
		IConfiguration configuration,
		UserManager<CoffeeUser> userManager,
		LocalDbContext localContext) {
		_logger = logger;
		_configuration = configuration;
		_userManager = userManager;
		_localContext = localContext;
	}

	public IActionResult Index() {
		return View();
	}

	public IActionResult Dashboard() {
		var rabbit = _configuration.GetSection("RabbitMQConfig");
		var port = rabbit.GetValue<int?>("Port:Dashboard") ?? 15672;

		var uri = new Uri($"{rabbit["Scheme"]}://{rabbit["Host"]}:{port}");
		string script = $"window.open('{uri}', '_blank').focus();";

		// Pass the JavaScript to the view using ViewBag or ViewData
		ViewBag.CustomScript = script;
		return View(nameof(Index));
	}

	public IActionResult Privacy() {
		return View();
	}

	//[ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
	//public IActionResult Error() {
	//	return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
	//}

	public IActionResult ManageCustomers() {
		return View();
	}

	public IActionResult PlaceOrder() {
		return View();
	}

	//[HttpPost]
	//public IActionResult SubmitOrder(OrderViewModel order) {
	//	TempData["Message"] = "Order sent to RabbitMQ!";
	//	return RedirectToAction("Index");
	//}

	public async Task<IActionResult> MyOrders() {
		var user = await _userManager.GetUserAsync(User);
		if (user == null) {
			return Challenge();
		}

		var email = user.Email ?? user.UserName ?? "";

		var orders = await _localContext.Orders
			.Where(o => o.CoffeeUser.Email == email)
			.OrderByDescending(o => o.CreatedAt)
			.ToListAsync();

		return View(orders);
	}

	public async Task<IActionResult> MyOrderDetails(int id) {
		var user = await _userManager.GetUserAsync(User);
		if (user == null)
			return Challenge();

		var email = user.Email ?? user.UserName ?? "";

		var order = await _localContext.Orders.FirstOrDefaultAsync(o => o.Id == id && o.CoffeeUser.Email == email);
		if (order == null) {
			return NotFound();
		}

		return View(order);
	}
}
