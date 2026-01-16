using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;
using Web.Models;

namespace Web.Controllers;

public class HomeController : Controller {
	private readonly ILogger<HomeController> _logger;
	private readonly IConfiguration _configuration;

	public HomeController(ILogger<HomeController> logger, IConfiguration configuration) {
		_logger = logger;
		_configuration = configuration;
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

	[ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
	public IActionResult Error() {
		return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
	}

	public IActionResult ManageCustomers() {
		return View();
	}

	public IActionResult PlaceOrder() {
		return View();
	}

	[HttpPost]
	public IActionResult SubmitOrder(OrderViewModel order) {
		TempData["Message"] = "Order sent to RabbitMQ!";
		return RedirectToAction("Index");
	}
}
