using System.ComponentModel.DataAnnotations;

namespace Web.Models;

public class PlaceOrderViewModel {
	// Customer details
	[Required(ErrorMessage = "Voornaam is verplicht.")]
	public string FirstName { get; set; } = "";

	[Required(ErrorMessage = "Achternaam is verplicht.")]
	public string LastName { get; set; } = "";

	[Required(ErrorMessage = "Email is verplicht.")]
	[EmailAddress(ErrorMessage = "Geef een geldig emailadres op.")]
	public string Email { get; set; } = "";

	// Shipping address
	[Required(ErrorMessage = "Straat is verplicht.")]
	public string Street { get; set; } = "";

	[Required(ErrorMessage = "Postbus is verplicht.")]
	public string Postbus { get; set; } = "";

	[Required(ErrorMessage = "Stad is verplicht.")]
	public string City { get; set; } = "";

	[Required(ErrorMessage = "Postcode is verplicht.")]
	public string Postcode { get; set; } = "";

	[Required(ErrorMessage = "Land is verplicht.")]
	public string Country { get; set; } = "";

	// Products (meerdere)
	public List<OrderItemInput> Items { get; set; } = new();
}

public class OrderItemInput {
	public string ProductId { get; set; } = "";

	// 0 = niet gekozen, 1..10 = gekozen
	[Range(0, 10, ErrorMessage = "Aantal moet tussen 0 en 10 liggen.")]
	public int Quantity { get; set; } = 0;
}
