namespace Models.Entities.DTO;

public record OrderLine(
	string OrderLineId,
	long ProductId,
	string ProductName,
	decimal UnitPrice,
	int Quantity,
	decimal LineTotal
);
