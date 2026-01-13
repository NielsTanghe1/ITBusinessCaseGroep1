using System.ComponentModel.DataAnnotations;

namespace Models.Entities;

/// <summary>
/// Represents an individual item within an order, including coffee details, quantity,
/// and purchase price.
/// </summary>
/// <remarks>
/// The <see cref="OrderItem"/> class associates a specific coffee with an order and
/// records the quantity and price at the time of purchase. It is typically used as
/// part of an order management or e-commerce system to track the items included in
/// each order.
/// </remarks>
public class OrderItem : BaseEntity {
	/// <summary>
	/// Gets or sets the primary key of this entity.
	/// </summary>
	[Key]
	[Display(Name = "OrderItemId")]
	public long Id {
		get; private set;
	}

	/// <summary>
	/// Gets or sets the foreign key of the order associated with this entity.
	/// </summary>
	[Display(Name = "OrderId")]
	public required long OrderId {
		get; set;
	}

	/// <summary>
	/// Gets or sets the order associated with this entity.
	/// </summary>
	[Display(Name = "Order")]
	public Order? Order {
		get; set;
	}

	/// <summary>
	/// Gets or sets the foreign key of the coffee associated with this entity.
	/// </summary>
	[Display(Name = "CoffeeId")]
	public required long CoffeeId {
		get; set;
	}

	/// <summary>
	/// Gets or sets the coffee associated with this entity.
	/// </summary>
	[Display(Name = "Coffee")]
	public Coffee? Coffee {
		get; set;
	}

	/// <summary>
	/// Gets or sets the quantity associated with this entity.
	/// </summary>
	[Range(0, 1000)]
	[Display(Name = "Quantity")]
	public required int Quantity {
		get; set;
	}

	/// <summary>
	/// Gets or sets the price of the total amount of <see cref="Coffee"/> at the time of purchase.
	/// </summary>
	[Range(0, 150000)]
	[Display(Name = "PriceAtPurchase")]
	public required decimal PriceAtPurchase {
		get; set;
	}

	/// <summary>
	/// Generates a fixed set of <see cref="OrderItem"/> entities for seeding purposes.
	/// </summary>
	/// <param name="orderIds">
	/// An array of order identifiers. Each index is mapped directly to a
	/// generated <see cref="OrderItem"/> instance.
	/// </param>
	/// <param name="coffees">
	/// A key-value-pair of coffee identifiers and their corresponding price. Each index is mapped directly to a
	/// generated <see cref="OrderItem"/> instance.
	/// </param>
	/// <returns>
	/// An array of <see cref="OrderItem"/> objects containing predefined order item data
	/// associated with the provided order and coffee identifiers.
	/// </returns>
	public static OrderItem[] SeedingData(long[] orderIds, KeyValuePair<long, decimal>[] coffees) {
		Random rnd = new();
		var items = new OrderItem[orderIds.Length];

		for (int i = 0; i < orderIds.Length; i++) {
			var coffee = coffees[rnd.Next(coffees.Length)];
			int quantity = rnd.Next(1, 6); // 1–5 units

			items[i] = new OrderItem {
				OrderId = orderIds[i],
				CoffeeId = coffee.Key,
				Quantity = quantity,
				PriceAtPurchase = Math.Round(
					(decimal) (coffee.Value * quantity),
					2
				 )
			};
		}

		return items;
	}
}
