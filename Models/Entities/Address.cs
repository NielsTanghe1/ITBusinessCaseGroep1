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
	/// Gets or sets the country name of this entity.
	/// </summary>
	[StringLength(60)]
	[Display(Name = "Country")]
	public required string Country {
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
}
