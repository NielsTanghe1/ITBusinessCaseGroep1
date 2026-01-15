namespace Web.Contracts;

public record OrderLine(
	 string ProductId,
	 string ProductName,
	 decimal UnitPrice,
	 int Quantity,
	 decimal LineTotal
);

public record OrderSubmitted(
	 Guid OrderId,
	 string FirstName,
	 string LastName,
	 string Email,
	 string Street,
	 string Postbus,
	 string City,
	 string Postcode,
	 string Country,
	 decimal Total,
	 IReadOnlyList<OrderLine> Lines
);
