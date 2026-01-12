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
	[StringLength(4)]
	[Display(Name = "LastFour")]
	public required string LastFour {
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
}

