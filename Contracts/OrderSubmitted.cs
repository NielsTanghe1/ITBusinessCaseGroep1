namespace ITBusinessCase.Contracts;

public record OrderLine(
	 string BeanId,
	 string BeanName,
	 string ProductType,
	 int Kg,
	 decimal UnitPricePerKg,
	 decimal LineTotal
);

public record OrderSubmitted(
	 Guid OrderId,

	 // ✅ UNIEKE USER INFO
	 string UserId,
	 string UserName,

	 // ✅ KLANT INFO
	 string FirstName,
	 string LastName,
	 string Email,
	 string Street,
	 string Postbus,
	 string City,
	 string Postcode,
	 string Country,

	 // ✅ ORDER INFO
	 decimal Total,
	 IReadOnlyList<OrderLine> Lines
);
