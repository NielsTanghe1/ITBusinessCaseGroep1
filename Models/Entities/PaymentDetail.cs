using System.ComponentModel.DataAnnotations;

namespace Models.Entities;

/// <summary>
/// Represents payment information associated with a customer, including account
/// details, expiration, and payment
/// gateway data.
/// </summary>
/// <remarks>
/// The <see cref="PaymentDetail"/> class is used to store and manage payment-related
/// data for a customer, such as the last four digits of a bank account, expiration
/// date, and a token from a payment gateway service (for example, Stripe, PayPal, or
/// Payconiq). This class is typically used in scenarios where payment details need to
/// be referenced or validated without storing sensitive account information.
/// </remarks>
public class PaymentDetail : BaseEntity {
	/// <summary>
	/// Gets or sets the primary key of this entity.
	/// </summary>
	[Key]
	[Display(Name = "PaymentInfoId")]
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
	/// Gets or sets the last four digits of the bank account.
	/// </summary>
	[Range(4, 4)]
	[Display(Name = "LastFour")]
	public required int LastFour {
		get; set;
	}

	/// <summary>
	/// Gets or sets the date and time when the entity expires.
	/// </summary>
	[DataType(DataType.DateTime)]
	[DisplayFormat(DataFormatString = "{0:dd-MM-yyyy HH:mm}", ApplyFormatInEditMode = true)]
	[Display(Name = "ExpiryDate")]
	public required DateTime ExpiryDate {
		get; set;
	}

	/// <summary>
	/// Gets or sets the payment gateway token associated with the transaction.
	/// </summary>
	/// <remarks>
	/// Something which would be implemented using a service like Stripe, PayPal, Payconiq, ...
	/// </remarks>
	[Range(40, 255)]
	[Display(Name = "GatewayToken")]
	public required string GatewayToken {
		get; set;
	}

	/// <summary>
	/// Generates a fixed set of <see cref="PaymentDetail"/> entities for seeding purposes.
	/// </summary>
	/// <param name="customerIds">
	/// An array of customer identifiers. Identifiers are pseudo-randomly mapped to one
	/// generated <see cref="PaymentDetail"/> instance.
	/// </param>
	/// <returns>
	/// An array of <see cref="PaymentDetail"/> objects containing predefined payment detail data
	/// associated with the provided customer identifiers.
	/// </returns>
	public static PaymentDetail[] SeedingData(long[] customerIds) {
		Random rnd = new();
		return [
			new() {
				CustomerId = customerIds[rnd.Next(customerIds.Length)],
				LastFour = 8530,
				ExpiryDate = DateTime.UtcNow.AddYears(2),
				GatewayToken = new Guid().ToString()
			},
			new() {
				CustomerId = customerIds[rnd.Next(customerIds.Length)],
				LastFour = 6394,
				ExpiryDate = DateTime.UtcNow.AddYears(4),
				GatewayToken = new Guid().ToString()
			},
			new() {
				CustomerId = customerIds[rnd.Next(customerIds.Length)],
				LastFour = 2303,
				ExpiryDate = DateTime.UtcNow.AddYears(4),
				GatewayToken = new Guid().ToString()
			},
			new() {
				CustomerId = customerIds[rnd.Next(customerIds.Length)],
				LastFour = 7863,
				ExpiryDate = DateTime.UtcNow.AddYears(5),
				GatewayToken = new Guid().ToString()
			},
			new() {
				CustomerId = customerIds[rnd.Next(customerIds.Length)],
				LastFour = 0975,
				ExpiryDate = DateTime.UtcNow.AddYears(2),
				GatewayToken = new Guid().ToString()
			},
			new() {
				CustomerId = customerIds[rnd.Next(customerIds.Length)],
				LastFour = 1212,
				ExpiryDate = DateTime.UtcNow.AddYears(2),
				GatewayToken = new Guid().ToString()
			},
			new() {
				CustomerId = customerIds[rnd.Next(customerIds.Length)],
				LastFour = 3465,
				ExpiryDate = DateTime.UtcNow.AddYears(1),
				GatewayToken = new Guid().ToString()
			},
			new() {
				CustomerId = customerIds[rnd.Next(customerIds.Length)],
				LastFour = 7893,
				ExpiryDate = DateTime.UtcNow.AddYears(1),
				GatewayToken = new Guid().ToString()
			},
			new() {
				CustomerId = customerIds[rnd.Next(customerIds.Length)],
				LastFour = 4234,
				ExpiryDate = DateTime.UtcNow.AddYears(2),
				GatewayToken = new Guid().ToString()
			},
			new() {
				CustomerId = customerIds[rnd.Next(customerIds.Length)],
				LastFour = 4895,
				ExpiryDate = DateTime.UtcNow.AddYears(5),
				GatewayToken = new Guid().ToString()
			},
			new() {
				CustomerId = customerIds[rnd.Next(customerIds.Length)],
				LastFour = 0976,
				ExpiryDate = DateTime.UtcNow.AddYears(2),
				GatewayToken = new Guid().ToString()
			},
			new() {
				CustomerId = customerIds[rnd.Next(customerIds.Length)],
				LastFour = 0303,
				ExpiryDate = DateTime.UtcNow.AddYears(3),
				GatewayToken = new Guid().ToString()
			},
			new() {
				CustomerId = customerIds[rnd.Next(customerIds.Length)],
				LastFour = 1214,
				ExpiryDate = DateTime.UtcNow.AddYears(3),
				GatewayToken = new Guid().ToString()
			},
			new() {
				CustomerId = customerIds[rnd.Next(customerIds.Length)],
				LastFour = 5665,
				ExpiryDate = DateTime.UtcNow.AddYears(4),
				GatewayToken = new Guid().ToString()
			}
		];
	}
}
