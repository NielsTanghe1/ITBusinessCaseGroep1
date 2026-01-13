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
	public StatusType Status {
		get; set;
	} = StatusType.Pending;

	/// <summary>
	/// Generates a fixed set of <see cref="Order"/> entities for seeding purposes.
	/// </summary>
	/// <param name="customerIds">
	/// An array of customer identifiers. Identifiers are pseudo-randomly mapped to one or more
	/// generated <see cref="PaymentDetail"/> instance.
	/// </param>
	/// <returns>
	/// An array of <see cref="Order"/> objects containing predefined order data
	/// associated with the provided customer identifiers.
	/// </returns>
	public static Order[] SeedingData(long[] customerIds) {
		Random rnd = new();
		var orders = new Order[20];

		for (int i = 0; i < orders.Length; i++) {
			orders[i] = new() {
				CustomerId = customerIds[rnd.Next(customerIds.Length)],
			};
		}

		return orders;
	}
}
