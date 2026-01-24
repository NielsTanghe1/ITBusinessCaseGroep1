using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;

namespace Models.Entities.DTO;

/// <summary>
/// Data Transfer Object representing a simplified version of a Order, 
/// used for transporting data across application layers.
/// </summary>
public class OrderDTO : Order {
	/// <summary>
	/// Gets or sets the coffee user associated with this Order.
	/// This hides the base <see cref="Order.CoffeeUser"/> to provide a DTO-specific version.
	/// </summary>
	/// <remarks>
	/// The 'new' keyword is used here to specialize the return type for DTO consumers.
	/// </remarks>
	[DeleteBehavior(DeleteBehavior.Restrict)]
	[Display(Name = "CoffeeUser")]
	public new CoffeeUserDTO? CoffeeUser {
		get; set;
	}
}
