namespace Models.Entities.DTO;

public record OrderSubmitted(
	string OrderId,
	string FirstName,
	string LastName,
	string Email,

	string Type,
	string Street,
	int HouseNumber,
	string City,
	string PostalCode,
	string CountryISO,
	string? UnitNumber,

	decimal Total,
	IReadOnlyList<OrderLine> Lines
);
