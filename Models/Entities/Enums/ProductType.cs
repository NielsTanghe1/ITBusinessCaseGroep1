namespace Models.Entities.Enums;

/// <summary>
/// Specifies the type or category of a product.
/// </summary>
/// <remarks>
/// Use this enumeration to indicate the general type of a product, such as whether it is a drink,
/// a food, retail, merchandise, or unknown.<br/>
/// The <see cref="Unknown"/> value can be used when the product category is not specified or
/// does not match any of the defined types.
/// </remarks>
public enum ProductType {
	Unknown = 0,
	Drink = 1,
	Food = 2,
	Retail = 3,
	Merchandise = 4
}
