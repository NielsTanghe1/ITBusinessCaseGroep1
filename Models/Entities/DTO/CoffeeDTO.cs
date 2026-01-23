using System.ComponentModel.DataAnnotations;
using Models.Entities;

namespace Models.Entities.DTO;

public class CoffeeDTO : Coffee {
	public long GlobalId {
		get; set;
	}
}