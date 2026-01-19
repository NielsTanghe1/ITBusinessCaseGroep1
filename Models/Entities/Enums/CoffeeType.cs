namespace Models.Entities.Enums;

/// <summary>
/// Specifies the type of coffee.
/// </summary>
/// <remarks>
/// Use this enumeration to indicate the type of a coffee, such as whether it is roasted,
/// grounded, raw, or unknown.<br/>
/// The <see cref="Unknown"/> value can be used when the coffee type is not specified or
/// does not match any of the defined types.
/// </remarks>
public enum CoffeeType {
	Unknown = 0,
	Roasted = 1,
	Grounded = 2,
	Raw = 3
}
