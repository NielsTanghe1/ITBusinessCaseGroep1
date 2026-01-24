using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;

namespace Models.Entities.DTO;

/// <summary>
/// Data Transfer Object representing a simplified version of a PaymentDetail, 
/// used for transporting data across application layers.
/// </summary>
public class PaymentDetailDTO : PaymentDetail {
	/// <summary>
	/// Gets or sets the coffee user associated with this PaymentDetail.
	/// This hides the base <see cref="PaymentDetail.CoffeeUser"/> to provide a DTO-specific version.
	/// </summary>
	/// <remarks>
	/// The 'new' keyword is used here to specialize the return type for DTO consumers.
	/// </remarks>
	[DeleteBehavior(DeleteBehavior.Cascade)]
	[Display(Name = "CoffeeUser")]
	public new CoffeeUserDTO? CoffeeUser {
		get; set;
	}
}
