using Models.Entities.Enums;
using System.ComponentModel.DataAnnotations;

namespace Models.Entities;

/// <summary>
/// Represents a physical or mailing address associated with a customer.
/// </summary>
/// <remarks>
/// The <see cref="Address"/> class encapsulates address details such as street, city,
/// postal code, and country, and associates them with a specific customer. Each address
/// is uniquely identified by its <see cref="AddressID"/> and can be categorized by its
/// <see cref="Type"/> (for example, billing or shipping). The class supports both
/// physical and mailbox addresses.
/// </remarks>
public class Address : BaseEntity {
	/// <summary>
	/// Gets or sets the primary key of this entity.
	/// </summary>
	[Key]
	[Display(Name = "AddressId")]
	public long Id {
		get; private set;
	}

	/// <summary>
	/// Gets or sets the foreign key of the customer associated with this entity.
	/// </summary>
	[Display(Name = "CustomerId")]
	public required long CustomerId {
		get; set;
	}

	/// <summary>
	/// Gets or sets the customer associated with this entity.
	/// </summary>
	[Display(Name = "Customer")]
	public Customer? Customer {
		get; set;
	}

	/// <summary>
	/// Gets or sets the type of this entity.
	/// </summary>
	[Display(Name = "Type")]
	public required AddressType Type {
		get; set;
	}

	/// <summary>
	/// Gets or sets the street name of this entity.
	/// </summary>
	[StringLength(50)]
	[Display(Name = "Street")]
	public required string Street {
		get; set;
	}

	/// <summary>
	/// Gets or sets the street name of this entity.
	/// </summary>
	[Range(1, 1000000)]
	[Display(Name = "HouseNumber")]
	public required int HouseNumber {
		get; set;
	}

	/// <summary>
	/// Gets or sets the city name of this entity.
	/// </summary>
	[StringLength(20)]
	[Display(Name = "City")]
	public required string City {
		get; set;
	}

	/// <summary>
	/// Gets or sets the postal code of this entity.
	/// </summary>
	[StringLength(10, MinimumLength = 2)]
	[Display(Name = "PostalCode")]
	public required string PostalCode {
		get; set;
	}

	/// <summary>
	/// Gets or sets the country ISO of this entity.
	/// </summary>
	[StringLength(3, MinimumLength = 2)]
	[Display(Name = "CountryISO")]
	public required string CountryISO {
		get; set;
	}

	/// <summary>
	/// Gets or sets the unit number associated with this entity.
	/// </summary>
	[StringLength(10)]
	[Display(Name = "UnitNumber")]
	public string? UnitNumber {
		get; set;
	}

	/// <summary>
	/// Generates a fixed set of <see cref="Address"/> entities for seeding purposes.
	/// </summary>
	/// <param name="customerIds">
	/// An array of customer identifiers. Each index is mapped directly to one
	/// generated <see cref="Address"/> instance, ensuring at minimum a one-to-one relationship.
	/// </param>
	/// <returns>
	/// An array of <see cref="Address"/> objects containing predefined address data
	/// associated with the provided customer identifiers.
	/// </returns>
	public static Address[] SeedingData(long[] customerIds) {
		return [
			new() {
				CustomerId = customerIds[0],
				Type = AddressType.Home,
				Street = "Korenveldstraat",
				HouseNumber = 12,
				City = "Zavelgem",
				PostalCode = "8421",
				CountryISO = "BE",
				UnitNumber = null
			},
			new() {
				CustomerId = customerIds[1],
				Type = AddressType.Shipping,
				Street = "Lindenhoflaan",
				HouseNumber = 47,
				City = "Bergdorp",
				PostalCode = "1753",
				CountryISO = "BE",
				UnitNumber = "B2"
			},
			new() {
				CustomerId = customerIds[2],
				Type = AddressType.Billing,
				Street = "Vijverparkweg",
				HouseNumber = 5,
				City = "Meerbeek",
				PostalCode = "3098",
				CountryISO = "BE",
				UnitNumber = null
			},
			new () {
				CustomerId = customerIds[3],
				Type = AddressType.Business,
				Street = "Industriehaven",
				HouseNumber = 102,
				City = "Havenstad",
				PostalCode = "2045",
				CountryISO = "BE",
				UnitNumber = null
			},
			new () {
				CustomerId = customerIds[4],
				Type = AddressType.Home,
				Street = "Beukenlaan",
				HouseNumber = 33,
				City = "Ravenbeek",
				PostalCode = "4120",
				CountryISO = "BE",
				UnitNumber = "1A"
			},
			new () {
				CustomerId = customerIds[5],
				Type = AddressType.Shipping,
				Street = "Kasteelstraat",
				HouseNumber = 89,
				City = "Oosterzele",
				PostalCode = "9862",
				CountryISO = "BE",
				UnitNumber = null
			},
			new () {
				CustomerId = customerIds[6],
				Type = AddressType.Billing,
				Street = "Molenakker",
				HouseNumber = 14,
				City = "Wittem",
				PostalCode = "6234",
				CountryISO = "BE",
				UnitNumber = null
			},
			new () {
				CustomerId = customerIds[7],
				Type = AddressType.Business,
				Street = "Handelsplein",
				HouseNumber = 7,
				City = "Marktveld",
				PostalCode = "2581",
				CountryISO = "BE",
				UnitNumber = "3C"
			},
			new() {
				CustomerId = customerIds[8],
				Type = AddressType.Home,
				Street = "Bosrandweg",
				HouseNumber = 56,
				City = "Groenwoud",
				PostalCode = "7814",
				CountryISO = "BE",
				UnitNumber = null
			},
			new() {
				CustomerId = customerIds[9],
				Type = AddressType.Shipping,
				Street = "Zonnewijzerstraat",
				HouseNumber = 21,
				City = "Sterrebeek",
				PostalCode = "1934",
				CountryISO = "BE",
				UnitNumber = "2F"
			}
		];
	}
}
