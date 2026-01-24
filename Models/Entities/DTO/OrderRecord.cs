using Models.Entities.Enums;

namespace Models.Entities.DTO;

public record OrderRecord(
	long CoffeeUserId,
	long OrderId,
	long CoffeeId,
	CoffeeType CoffeeType,
	int Quantity,
	decimal UnitPrice
);
