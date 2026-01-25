using Models.Entities;
using Models.Entities.DTO;

namespace Models.Extensions.Mappings;

/// <summary>
/// Contains extension methods for transforming <see cref="OrderItem"/>
/// and <see cref="OrderItemDTO"/> entities, including creating new
/// model instances, converting them to DTOs, and updating existing
/// models based on DTO data.
/// </summary>
public static class OrderItemMappingExtensions {
	/// <summary>
	/// Converts a <see cref="OrderItem"/> entity model to a new <see cref="OrderItemDTO"/>.
	/// </summary>
	/// <param name="model">
	/// The source <see cref="OrderItem"/> entity to convert.
	/// </param>
	/// <returns>
	/// A populated <see cref="OrderItemDTO"/> instance.
	/// </returns>
	/// <exception cref="ArgumentNullException">
	/// Thrown when <paramref name="model"/> is null.
	/// </exception>
	public static OrderItemDTO ToDTO(this OrderItem model) {
		// Throwing here guarantees the return is never null to the caller
		ArgumentNullException.ThrowIfNull(model);

		return new() {
			GlobalId = model.Id,
			OrderId = model.OrderId,
			CoffeeId = model.CoffeeId,
			Quantity = model.Quantity,
			UnitPrice = model.UnitPrice,
			CreatedAt = model.CreatedAt,
			DeletedAt = model.DeletedAt
		};
	}

	/// <summary>
	/// Converts a <see cref="OrderItemDTO"/> to a new <see cref="OrderItem"/> entity model.
	/// </summary>
	/// <param name="model">
	/// The source <see cref="OrderItemDTO"/> to convert.
	/// </param>
	/// <returns>
	/// A populated <see cref="OrderItem"/> instance.
	/// </returns>
	/// <exception cref="ArgumentNullException">
	/// Thrown when <paramref name="model"/> is null.
	/// </exception>
	public static OrderItem ToModel(this OrderItemDTO model) {
		// Throwing here guarantees the return is never null to the caller
		ArgumentNullException.ThrowIfNull(model);

		return new() {
			Id = model.GlobalId ?? 0L,
			OrderId = model.OrderId,
			CoffeeId = model.CoffeeId,
			Quantity = model.Quantity,
			UnitPrice = model.UnitPrice,
			CreatedAt = model.CreatedAt,
			DeletedAt = model.DeletedAt
		};
	}

	/// <summary>
	/// Copies the data from a <see cref="OrderItemDTO"/> to an
	/// existing instance of <see cref="OrderItem"/>.
	/// </summary>
	/// <param name="model">
	/// The source model from which the data is taken.
	/// </param>
	/// <param name="target">
	/// The existing target model that is updated with the data from <paramref name="model"/>.
	/// </param>
	public static void ToExisting(this OrderItemDTO model, OrderItem target) {
		target.OrderId = model.OrderId;
		target.CoffeeId = model.CoffeeId;
		target.Quantity = model.Quantity;
		target.UnitPrice = model.UnitPrice;
		target.CreatedAt = model.CreatedAt;
		target.DeletedAt = model.DeletedAt;
	}
}
