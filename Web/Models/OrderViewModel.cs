using Models.Entities;

namespace Web.Models;

public class OrderViewModel {
	// Customer details
	public required CoffeeUser CoffeeUser {
		get; set;
	}

	// Shipping address
	public required Address Address {
		get; set;
	}

	// Product
	public List<OrderItem>? Items {
		get; set;
	}

	public decimal TotalPrice {
		get {
			decimal total = 0;
			Items.ForEach(item => total += item.UnitPrice);
			return total;
		}
	}
}
