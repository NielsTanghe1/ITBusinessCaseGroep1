using System.ComponentModel.DataAnnotations;

namespace Web.Models;

public class UserProfile {
	[Key]
	public string UserId { get; set; } = "";

	[Required]
	public string FirstName { get; set; } = "";

	[Required]
	public string LastName { get; set; } = "";

	[Required, EmailAddress]
	public string Email { get; set; } = "";

	public string Street { get; set; } = "";
	public string Postbus { get; set; } = "";
	public string City { get; set; } = "";
	public string Postcode { get; set; } = "";
	public string Country { get; set; } = "Belgium";
}
