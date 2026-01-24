using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;

namespace Models.Entities.DTO;

/// <summary>
/// Data Transfer Object representing a simplified version of a OrderItem, 
/// used for transporting data across application layers.
/// </summary>
public class OrderItemDTO : OrderItem {
	/// <summary>
	/// Gets or sets the order associated with this OrderItem.
	/// This hides the base <see cref="OrderItem.Order"/> to provide a DTO-specific version.
	/// </summary>
	/// <remarks>
	/// The 'new' keyword is used here to specialize the return type for DTO consumers.
	/// </remarks>
	[DeleteBehavior(DeleteBehavior.Cascade)]
	[Display(Name = "Order")]
	public new OrderDTO? Order {
		get; set;
	}

	/// <summary>
	/// Gets or sets the coffee associated with this OrderItem.
	/// This hides the base <see cref="OrderItem.Coffee"/> to provide a DTO-specific version.
	/// </summary>
	/// <remarks>
	/// The 'new' keyword is used here to specialize the return type for DTO consumers.
	/// </remarks>
	[DeleteBehavior(DeleteBehavior.Restrict)]
	[Display(Name = "Coffee")]
	public new CoffeeDTO? Coffee {
		get; set;
	}
}
