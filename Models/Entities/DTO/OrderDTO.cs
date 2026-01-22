using Models.Entities.Enums;

namespace Models.Entities.DTO;

public record OrderDTO(
	long CoffeeUserId,
	long OrderId,
	long CoffeeId,
	CoffeeType CoffeeType,
	int Quantity,
	decimal UnitPrice
);
