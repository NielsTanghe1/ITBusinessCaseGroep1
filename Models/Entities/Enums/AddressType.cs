namespace Models.Entities.Enums;

/// <summary>
/// Specifies the type of an address.
/// </summary>
/// <remarks>
/// Use this enumeration to represent the type of an address, such as whether it is
/// personal, shipping, billing, or an unknown type.<br/>
/// The <see cref="Unknown"/> value can be used when the address type is not specified or does not match
/// any of the defined types.
/// </remarks>
public enum AddressType {
	Unknown = 0,
	Personal = 1,
	Shipping = 2,
	Billing = 3
}

