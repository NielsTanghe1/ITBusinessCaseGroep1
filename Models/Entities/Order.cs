using Microsoft.EntityFrameworkCore;
using Models.Entities.Enums;
using System.ComponentModel.DataAnnotations;

namespace Models.Entities;

/// <summary>
/// Represents a order, including its unique identifier, associated
/// user, and current status.
/// </summary>
/// <remarks>
/// The <see cref="Order"/> class encapsulates information about a single order
/// placed by a user. It includes references to the user, the order's status,
/// and unique identifiers for both the order and the user.
/// </remarks>
public class Order : BaseEntity {
	/// <summary>
	/// Gets or sets the primary key of this entity.
	/// </summary>
	[Key]
	[Display(Name = "OrderId")]
	public override long Id {
		get; init;
	}

	/// <summary>
	/// Gets or sets the foreign key of the user associated with this entity.
	/// </summary>
	[Display(Name = "CoffeeUserId")]
	public required long CoffeeUserId {
		get; set;
	}

	/// <summary>
	/// Gets or sets the user associated with this entity.
	/// </summary>
	[DeleteBehavior(DeleteBehavior.Restrict)]
	[Display(Name = "CoffeeUser")]
	public CoffeeUser? CoffeeUser {
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
	/// <param name="userIds">
	/// An array of user identifiers. Identifiers are pseudo-randomly mapped to one or more
	/// generated <see cref="PaymentDetail"/> instances.
	/// </param>
	/// <returns>
	/// An array of <see cref="Order"/> objects containing predefined order data
	/// associated with the provided user identifiers.
	/// </returns>
	public static Order[] SeedingData(long[] userIds) {
		var orders = new Order[20];

		for (int i = 0; i < orders.Length; i++) {
			orders[i] = new() {
				CoffeeUserId = userIds[Random.Shared.Next(userIds.Length)]
			};
		}

		return orders;
	}
}
