using System.ComponentModel.DataAnnotations;

namespace Web.Models;

public class PlaceOrderViewModel {
	// Customer
	[Required] public string? FirstName { get; set; }
	[Required] public string? LastName { get; set; }
	[Required, EmailAddress] public string? Email { get; set; }

	// Address
	[Required] public string? Street { get; set; }
	[Required] public string? Postbus { get; set; }
	[Required] public string? City { get; set; }
	[Required] public string? Postcode { get; set; }
	[Required] public string? Country { get; set; }

	// Products
	public List<BeanOrderItemInput> Items { get; set; } = new();
}
