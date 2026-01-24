using System.ComponentModel.DataAnnotations;

namespace Models.Entities;

/// <summary>
/// Represents a user entity with identity and personal information.
/// </summary>
/// <remarks>
/// The <see cref="CoffeeUser"/> class encapsulates details about an individual user,
/// including their unique identifier and name information. This type is typically used
/// in scenarios where user records are managed, such as in customer relationship
/// management (CRM) systems or order processing.
/// </remarks>
public class CoffeeUser : BaseIdentityEntity {
	/// <summary>
	/// Gets or sets the primary key for this entity.
	/// </summary>
	[Key]
	[Display(Name = "CoffeeUserId")]
	public new long Id {
		get; init;
	}

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
	/// Generates a fixed set of <see cref="CoffeeUser"/> entities for seeding purposes.
	/// </summary>
	/// <returns>
	/// An array of <see cref="CoffeeUser"/> objects containing predefined user data.
	/// </returns>
	public static CoffeeUser[] SeedingData() {
		return [
			new() {
				UserName = "alice.smith",
				Email = "alice@example.com",
				FirstName = "Alice",
				LastName = "Smith",
				EmailConfirmed = true
			},
			new() {
				UserName = "bob.johnson",
				Email = "bob@example.com",
				FirstName = "Bob",
				LastName = "Johnson",
				EmailConfirmed = true
			},
			new() {
				UserName = "carol.wilson",
				Email = "carol@example.com",
				FirstName = "Carol",
				LastName = "Wilson",
				EmailConfirmed = true
			},
			new() {
				UserName = "david.brown",
				Email = "david@example.com",
				FirstName = "David",
				LastName = "Brown",
				EmailConfirmed = true
			},
			new() {
				UserName = "emma.davis",
				Email = "emma@example.com",
				FirstName = "Emma",
				LastName = "Davis",
				EmailConfirmed = true
			},
			new() {
				UserName = "frank.miller",
				Email = "frank@example.com",
				FirstName = "Frank",
				LastName = "Miller",
				EmailConfirmed = true
			},
			new() {
				UserName = "grace.moore",
				Email = "grace@example.com",
				FirstName = "Grace",
				LastName = "Moore",
				EmailConfirmed = true
			},
			new() {
				UserName = "henry.taylor",
				Email = "henry@example.com",
				FirstName = "Henry",
				LastName = "Taylor",
				EmailConfirmed = true
			},
			new() {
				UserName = "irene.anderson",
				Email = "irene@example.com",
				FirstName = "Irene",
				LastName = "Anderson",
				EmailConfirmed = true
			},
			new() {
				UserName = "jack.thomas",
				Email = "jack@example.com",
				FirstName = "Jack",
				LastName = "Thomas",
				EmailConfirmed = true
			},
			new() {
				UserName = "katherine.white",
				Email = "katherine@example.com",
				FirstName = "Katherine",
				LastName = "White",
				EmailConfirmed = true
			},
			new() {
				UserName = "liam.harris",
				Email = "liam@example.com",
				FirstName = "Liam",
				LastName = "Harris",
				EmailConfirmed = true
			},
			new() {
				UserName = "mia.clark",
				Email = "mia@example.com",
				FirstName = "Mia",
				LastName = "Clark",
				EmailConfirmed = true
			},
			new() {
				UserName = "nathan.lewis",
				Email = "nathan@example.com",
				FirstName = "Nathan",
				LastName = "Lewis",
				EmailConfirmed = true
			}
		];
	}
}
