using Web.Models;
using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;

namespace Web.Controllers {
	public class HomeController : Controller {
		private readonly ILogger<HomeController> _logger;

		public HomeController(ILogger<HomeController> logger) {
			_logger = logger;
		}

		public IActionResult Index() {
			return View();
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
}
