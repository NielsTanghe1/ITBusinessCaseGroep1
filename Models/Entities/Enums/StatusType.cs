namespace Models.Entities.Enums;

/// <summary>
/// Specifies the status of an order or operation.
/// </summary>
/// <remarks>
/// Use this enumeration to represent the current state of an order or process, such as whether it is
/// pending, shipped, or in an unknown state.<br/>
/// The <see cref="Unknown"/> value can be used when the status is not specified or does not match
/// any of the defined types.
/// </remarks>
public enum StatusType {
	Unknown = 0,
	Pending = 1,
	Shipped = 2
}

