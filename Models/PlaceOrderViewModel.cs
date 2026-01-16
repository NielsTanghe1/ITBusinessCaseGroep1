using System.ComponentModel.DataAnnotations;

namespace ITBusinessCase.Models;

public class PlaceOrderViewModel {
	public List<BeanOrderItemInput> Items { get; set; } = new();

	[Required(ErrorMessage = "Voornaam is verplicht")]
	public string FirstName { get; set; } = "";

	[Required(ErrorMessage = "Achternaam is verplicht")]
	public string LastName { get; set; } = "";

	[Required(ErrorMessage = "Email is verplicht")]
	[EmailAddress(ErrorMessage = "Email is niet geldig")]
	public string Email { get; set; } = "";

	[Required(ErrorMessage = "Straat is verplicht")]
	public string Street { get; set; } = "";

	[Required(ErrorMessage = "Postbus is verplicht")]
	public string Postbus { get; set; } = "";

	[Required(ErrorMessage = "Stad is verplicht")]
	public string City { get; set; } = "";

	[Required(ErrorMessage = "Postcode is verplicht")]
	public string Postcode { get; set; } = "";

	[Required(ErrorMessage = "Land is verplicht")]
	public string Country { get; set; } = "";
}
