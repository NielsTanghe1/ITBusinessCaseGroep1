namespace Models.Entities.DTO;

/// <summary>
/// Data Transfer Object representing a simplified version of a CoffeeUser,
/// used for transporting data across application layers.
/// </summary>
public class CoffeeUserDTO : CoffeeUser {
	// // Optional: Update reverse navigations too
	// public new ICollection<OrderDTO>? Orders {
	// 	get; set;
	// }
	// public new ICollection<AddressDTO>? Addresses {
	// 	get; set;
	// }
	// public new ICollection<PaymentDetailDTO>? PaymentDetails {
	// 	get; set;
	// }
}
