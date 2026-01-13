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

	/// <summary>
	/// Generates a fixed set of <see cref="Customer"/> entities for seeding purposes.
	/// </summary>
	/// <returns>
	/// An array of <see cref="Customer"/> objects containing predefined customer data.
	/// </returns>
	public static Customer[] SeedingData() {
		return [
			new() {
				UserName = "alice.smith",
				Email = "alice@example.com",
				FirstName = "Alice",
				LastName = "Smith"
			},
			new() {
				UserName = "bob.johnson",
				Email = "bob@example.com",
				FirstName = "Bob",
				LastName = "Johnson"
			},
			new() {
				UserName = "carol.wilson",
				Email = "carol@example.com",
				FirstName = "Carol",
				LastName = "Wilson"
			},
			new() {
				UserName = "david.brown",
				Email = "david@example.com",
				FirstName = "David",
				LastName = "Brown"
			},
			new() {
				UserName = "emma.davis",
				Email = "emma@example.com",
				FirstName = "Emma",
				LastName = "Davis"
			},
			new() {
				UserName = "frank.miller",
				Email = "frank@example.com",
				FirstName = "Frank",
				LastName = "Miller"
			},
			new() {
				UserName = "grace.moore",
				Email = "grace@example.com",
				FirstName = "Grace",
				LastName = "Moore"
			},
			new() {
				UserName = "henry.taylor",
				Email = "henry@example.com",
				FirstName = "Henry",
				LastName = "Taylor"
			},
			new() {
				UserName = "irene.anderson",
				Email = "irene@example.com",
				FirstName = "Irene",
				LastName = "Anderson"
			},
			new() {
				UserName = "jack.thomas",
				Email = "jack@example.com",
				FirstName = "Jack",
				LastName = "Thomas"
			},
		];
	}
}
