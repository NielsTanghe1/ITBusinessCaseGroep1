namespace Models.Entities.DTO;

public record OrderDTO(
	long CoffeeUserId,
	long OrderId,
	long CoffeeId,
	string CoffeeType,
	int Quantity,
	decimal UnitPrice
);
