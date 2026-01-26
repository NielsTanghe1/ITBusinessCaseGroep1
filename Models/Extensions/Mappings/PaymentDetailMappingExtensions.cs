using Models.Entities;
using Models.Entities.DTO;

namespace Models.Extensions.Mappings;

/// <summary>
/// Contains extension methods for transforming <see cref="PaymentDetail"/>
/// and <see cref="PaymentDetailDTO"/> entities, including:
/// <list type = "bullet" >
/// <item><description>Creating new model instances.</description></item>
/// <item><description>Converting model instances to DTOs.</description></item>
/// <item><description>Updating existing model instances based on DTO data.</description></item>
/// <item><description>Creating a shallow, untracked copy of a model instance into a new one.</description></item>
/// </list>
/// </summary>
public static class PaymentDetailMappingExtensions {
	/// <summary>
	/// Converts a <see cref="PaymentDetail"/> entity model to a new <see cref="PaymentDetailDTO"/>.
	/// </summary>
	/// <param name="model">
	/// The source <see cref="PaymentDetail"/> entity to convert.
	/// </param>
	/// <returns>
	/// A populated <see cref="PaymentDetailDTO"/> instance.
	/// </returns>
	/// <exception cref="ArgumentNullException">
	/// Thrown when <paramref name="model"/> is null.
	/// </exception>
	public static PaymentDetailDTO ToDTO(this PaymentDetail model) {
		// Throwing here guarantees the return is never null to the caller
		ArgumentNullException.ThrowIfNull(model);

		return new() {
			GlobalId = model.Id,
			CoffeeUserId = model.CoffeeUserId,
			LastFour = model.LastFour,
			ExpiryDate = model.ExpiryDate,
			GatewayToken = model.GatewayToken,
			CreatedAt = model.CreatedAt,
			DeletedAt = model.DeletedAt
		};
	}

	/// <summary>
	/// Converts a <see cref="PaymentDetailDTO"/> to a new <see cref="PaymentDetail"/> entity model.
	/// </summary>
	/// <param name="model">
	/// The source <see cref="PaymentDetailDTO"/> to convert.
	/// </param>
	/// <returns>
	/// A populated <see cref="PaymentDetail"/> instance.
	/// </returns>
	/// <exception cref="ArgumentNullException">
	/// Thrown when <paramref name="model"/> is null.
	/// </exception>
	public static PaymentDetail ToModel(this PaymentDetailDTO model) {
		// Throwing here guarantees the return is never null to the caller
		ArgumentNullException.ThrowIfNull(model);

		return new() {
			Id = model.GlobalId ?? 0L,
			CoffeeUserId = model.CoffeeUserId,
			LastFour = model.LastFour,
			ExpiryDate = model.ExpiryDate,
			GatewayToken = model.GatewayToken,
			CreatedAt = model.CreatedAt,
			DeletedAt = model.DeletedAt
		};
	}

	/// <summary>
	/// Copies the data from a <see cref="PaymentDetailDTO"/> to an
	/// existing instance of <see cref="PaymentDetail"/>.
	/// </summary>
	/// <param name="model">
	/// The source model from which the data is taken.
	/// </param>
	/// <param name="target">
	/// The existing target model that is updated with the data from <paramref name="model"/>.
	/// </param>
	public static void ToExisting(this PaymentDetailDTO model, PaymentDetail target) {
		target.CoffeeUserId = model.CoffeeUserId;
		target.LastFour = model.LastFour;
		target.ExpiryDate = model.ExpiryDate;
		target.GatewayToken = model.GatewayToken;
		target.CreatedAt = model.CreatedAt;
		target.DeletedAt = model.DeletedAt;
	}

	/// <summary>
	/// Creates a shallow, untracked copy of a <see cref="PaymentDetail"/> into a new <see cref="PaymentDetail"/> instance.
	/// </summary>
	/// <param name="model">
	/// The source <see cref="PaymentDetail"/> entity to copy.
	/// </param>
	/// <returns>
	/// A new <see cref="PaymentDetail"/> instance with identical property values, 
	/// but detached from any <c>DbContext</c> tracking.
	/// </returns>
	/// <remarks>
	/// This is a shallow copy; reference navigation properties are shared between instances. 
	/// Useful for transferring data between a local and global database context.
	/// </remarks>
	/// <exception cref="ArgumentNullException">
	/// Thrown when <paramref name="model"/> is <see langword="null"/>.
	/// </exception>
	public static PaymentDetail ShallowCopy(this PaymentDetail model) {
		// Throwing here guarantees the return is never null to the caller
		ArgumentNullException.ThrowIfNull(model);

		return new() {
			CoffeeUserId = model.CoffeeUserId,
			LastFour = model.LastFour,
			ExpiryDate = model.ExpiryDate,
			GatewayToken = model.GatewayToken,
			CreatedAt = model.CreatedAt,
			DeletedAt = model.DeletedAt
		};
	}
}
