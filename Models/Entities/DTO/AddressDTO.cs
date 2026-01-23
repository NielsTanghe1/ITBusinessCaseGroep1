using System.ComponentModel.DataAnnotations;
using Models.Entities;

namespace Models.Entities.DTO;

public class AddressDTO : Address {
	public long GlobalId {
		get; set;
	}

	public new CoffeeUserDTO? CoffeeUser {
		get; set;
	}
}