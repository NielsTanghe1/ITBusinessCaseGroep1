namespace Models.Entities.DTO;

public record CoffeeUserSubmitted(
	long CoffeeUserId,
	string Email,
	string FirstName,
	string LastName,
	string? Street,
	string? City,
	string? PostalCode,
	string? Country
);
