using System.ComponentModel.DataAnnotations;

namespace Models.Entities;

/// <summary>
/// Represents a customer entity with identity and personal information.
/// </summary>
/// <remarks>
/// The <see cref="Customer"/> class encapsulates details about an individual customer,
/// including their unique identifier and name information. This type is typically used
/// in scenarios where customer records are managed, such as in customer relationship
/// management (CRM) systems or order processing.
/// </remarks>
public class Customer : BaseIdentityEntity {
	/// <summary>
	/// Gets or sets the first name of this entity.
	/// </summary>
	[StringLength(35, MinimumLength = 2)]
	[Display(Name = "FirstName")]
	public required string FirstName {
		get; set;
	}

	/// <summary>
	/// Gets or sets the last name of this entity.
	/// </summary>
	[StringLength(35, MinimumLength = 2)]
	[Display(Name = "LastName")]
	public required string LastName {
		get; set;
	}
}
