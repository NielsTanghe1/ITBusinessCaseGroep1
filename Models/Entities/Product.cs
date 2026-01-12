using Models.Entities.Enums;
using System.ComponentModel.DataAnnotations;

namespace Models.Entities;

/// <summary>
/// Represents a product with a unique identifier, name, type, and price.
/// </summary>
/// <remarks>
/// The <see cref="Product"/> class encapsulates the core information for a product
/// entity, including its unique identifier, descriptive name, product type, and price.
/// This class is typically used in scenarios where product details need to be stored,
/// retrieved, or manipulated within an application, such as in inventory management or
/// e-commerce systems.
/// </remarks>
public class Product : BaseEntity {
	/// <summary>
	/// Gets or sets the primary key of this entity.
	/// </summary>
	[Key]
	[Display(Name = "ProductId")]
	public long Id {
		get; private set;
	}

	/// <summary>
	/// Gets or sets the name of this entity.
	/// </summary>
	[StringLength(35, MinimumLength = 2)]
	[Display(Name = "Name")]
	public required string Name {
		get; set;
	}

	/// <summary>
	/// Gets or sets the type of this entity.
	/// </summary>
	[Display(Name = "Type")]
	public required ProductType Type {
		get; set;
	}

	/// <summary>
	/// Gets or sets the price of this entity.
	/// </summary>
	[Range(0, 50)]
	[Display(Name = "Price")]
	public required decimal Price {
		get; set;
	}

	/// <summary>
	/// Gets or sets the net weight of this entity (in grams).
	/// </summary>
	/// <remarks>
	/// The weight range lies between (inclusive) 100g and 1kg.
	/// </remarks>
	[Range(100, 1000)]
	[Display(Name = "NetWeight")]
	public required decimal NetWeight {
		get; set;
	}
}

