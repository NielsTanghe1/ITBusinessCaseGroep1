using System.ComponentModel.DataAnnotations;
using Models.Entities;

namespace Models.Entities.DTO;

public class OrderItemDTO : OrderItem {
	public long GlobalId {
		get; set;
	}

	// Fix: Point to OrderDTO and CoffeeDTO
	public new OrderDTO? Order {
		get; set;
	}
	public new CoffeeDTO? Coffee {
		get; set;
	}
}