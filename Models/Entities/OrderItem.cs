using System.ComponentModel.DataAnnotations;

namespace Models.Entities;

/// <summary>
/// Represents an individual item within an order, including product details, quantity,
/// and purchase price.
/// </summary>
/// <remarks>
/// The <see cref="OrderItem"/> class associates a specific product with an order and
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
	/// Gets or sets the foreign key of the product associated with this entity.
	/// </summary>
	[Display(Name = "ProductId")]
	public required long ProductId {
		get; set;
	}

	/// <summary>
	/// Gets or sets the product associated with this entity.
	/// </summary>
	[Display(Name = "Product")]
	public Product? Product {
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
	/// Gets or sets the price of the total amount of <see cref="Product"/> at the time of purchase.
	/// </summary>
	[Range(0, 150000)]
	[Display(Name = "PriceAtPurchase")]
	public required decimal PriceAtPurchase {
		get; set;
	}
}

