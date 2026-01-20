using Models.Entities.Enums;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Models.Entities;

/// <summary>
/// Represents a coffee with a unique identifier, name, type, and price.
/// </summary>
/// <remarks>
/// The <see cref="Coffee"/> class encapsulates the core information for a coffee
/// entity, including its unique identifier, descriptive name, type, and price.
/// This class is typically used in scenarios where coffee details need to be stored,
/// retrieved, or manipulated within an application, such as in inventory management or
/// e-commerce systems.
/// </remarks>
public class Coffee : BaseEntity {
	/// <summary>
	/// Gets or sets the primary key of this entity.
	/// </summary>
	[Key]
	[Display(Name = "CoffeeId")]
	public long Id {
		get; private set;
	}

	/// <summary>
	/// Gets or sets the name of this entity.
	/// </summary>
	[Display(Name = "Name")]
	public required CoffeeName Name {
		get; set;
	}

	/// <summary>
	/// Gets or sets the type of this entity.
	/// </summary>
	[Display(Name = "Type")]
	public required CoffeeType Type {
		get; set;
	}

	/// <summary>
	/// Gets or sets the price of this entity.
	/// </summary>
	/// <remarks>
	/// Has a range limit between 0 and 50.00 but is allowed to contain a total of 18 digits.
	/// </remarks>
	[DataType(DataType.Currency)]
	[Column(TypeName = "decimal(18, 2)")]
	[Range(0.00, 50.00)]
	[Display(Name = "Price")]
	public required decimal Price {
		get; set;
	}

	/// <summary>
	/// Generates a fixed set of <see cref="Coffee"/> entities for seeding purposes.
	/// </summary>
	/// <returns>
	/// An array of <see cref="Coffee"/> objects containing predefined coffee data.
	/// </returns>
	public static Coffee[] SeedingData() {
		return [
			new Coffee {
				Name = CoffeeName.Arabica,
				Type = CoffeeType.Roasted,
				Price = 14.50m
			},
			new Coffee {
				Name = CoffeeName.Robusta,
				Type = CoffeeType.Roasted,
				Price = 11.20m
			},
			new Coffee {
				Name = CoffeeName.Arabica,
				Type = CoffeeType.Grounded,
				Price = 13.75m
			},
			new Coffee {
				Name = CoffeeName.Robusta,
				Type = CoffeeType.Grounded,
				Price = 10.90m
			},
			new Coffee {
				Name = CoffeeName.Arabica,
				Type = CoffeeType.Raw,
				Price = 8.40m
			},
			new Coffee {
				Name = CoffeeName.Robusta,
				Type = CoffeeType.Raw,
				Price = 6.95m
			},
			new Coffee {
				Name = CoffeeName.Liberica,
				Type = CoffeeType.Roasted,
				Price = 16.80m
			},
			new Coffee {
				Name = CoffeeName.Liberica,
				Type = CoffeeType.Grounded,
				Price = 15.30m
			},
			new Coffee {
				Name = CoffeeName.Excelsa,
				Type = CoffeeType.Roasted,
				Price = 17.25m
			},
			new Coffee {
				Name = CoffeeName.Excelsa,
				Type = CoffeeType.Raw,
				Price = 9.10m
			}
		];
	}
}
