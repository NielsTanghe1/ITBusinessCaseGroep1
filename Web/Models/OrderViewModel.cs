using Models.Entities;

namespace Web.Models;

public class OrderViewModel {
	public long CoffeeUserId {
		get; set;
	}

	public long OrderId {
		get; set;
	}

	public List<OrderItem> Items {
		get; set;
	} = [];
}
