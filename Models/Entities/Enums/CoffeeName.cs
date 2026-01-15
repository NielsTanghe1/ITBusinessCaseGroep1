namespace Models.Entities.Enums;

/// <summary>
/// Specifies the name (species) of coffee.
/// </summary>
/// <remarks>
/// Use this enumeration to indicate the name of a coffee, such as Arabica, Robusta, Liberica,
/// Excelsa, or unknown.<br/>
/// The <see cref="Unknown"/> value can be used when the coffee name is not specified or
/// does not match any of the defined types.
/// </remarks>
public enum CoffeeName {
	Unknown = 0,
	Arabica = 1,
	Robusta = 2,
	Liberica = 3,
	Excelsa = 4
}
