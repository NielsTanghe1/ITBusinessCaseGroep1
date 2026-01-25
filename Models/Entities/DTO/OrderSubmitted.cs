using Models.Entities.Enums;

namespace Models.Entities.Records;

public record OrderSubmitted(
	long CoffeeUserId,
	long OrderId,
	long CoffeeId,
	CoffeeType CoffeeType,
	int Quantity,
	decimal UnitPrice
);
