using Models.Entities.Enums;
using System.ComponentModel.DataAnnotations;

namespace Models.Entities;

/// <summary>
/// Represents a customer order, including its unique identifier, associated
/// customer, and current status.
/// </summary>
/// <remarks>
/// The <see cref="Order"/> class encapsulates information about a single order
/// placed by a customer. It includes references to the customer, the order's status,
/// and unique identifiers for both the order and the customer.
/// </remarks>
public class Order : BaseEntity {
	/// <summary>
	/// Gets or sets the primary key of this entity.
	/// </summary>
	[Key]
	[Display(Name = "OrderId")]
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
	/// Gets or sets the status of this entity.
	/// </summary>
	[Display(Name = "Status")]
	public required StatusType Status {
		get; set;
	}
}

