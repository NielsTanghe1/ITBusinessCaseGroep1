namespace Models.Entities.DTO;

// Main message contract including all table data
// Request DTO matching the architecture tables
public record OrderCreated(
	// Order Table
	string OrderId,
	string ProductName,
	string ProductType,
	decimal Price,
	int Quantity,

	// Customer Table
	string FirstName,
	string LastName,
	string Email,

	// Address Table
	int Postcode,
	string City,
	string Street,
	string Postbus,
	string Country,

	// PaymentInfo Table
	string Cardnumber,
	int CVV
);
