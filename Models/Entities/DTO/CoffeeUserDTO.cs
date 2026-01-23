using System.ComponentModel.DataAnnotations;
using Models.Entities;

namespace Models.Entities.DTO;

public class CoffeeUserDTO : CoffeeUser {
	public long GlobalId {
		get; set;
	}

	// Optional: Update reverse navigations too
	public new ICollection<OrderDTO>? Orders {
		get; set;
	}
	public new ICollection<AddressDTO>? Addresses {
		get; set;
	}
	public new ICollection<PaymentDetailDTO>? PaymentDetails {
		get; set;
	}
}