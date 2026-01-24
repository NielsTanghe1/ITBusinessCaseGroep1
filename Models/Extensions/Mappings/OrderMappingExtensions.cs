using Models.Entities;
using Models.Entities.DTO;

namespace Models.Extensions.Mappings;

/// <summary>
/// Contains extension methods for transforming <see cref="Order"/>
/// and <see cref="OrderDTO"/> entities, including creating new
/// model instances, converting them to DTOs, and updating existing
/// models based on DTO data.
/// </summary>
public static class OrderMappingExtensions {
	/// <summary>
	/// Converts a <see cref="Order"/> entity model to a new <see cref="OrderDTO"/>.
	/// </summary>
	/// <param name="model">
	/// The source <see cref="Order"/> entity to convert.
	/// </param>
	/// <returns>
	/// A populated <see cref="OrderDTO"/> instance.
	/// </returns>
	/// <exception cref="ArgumentNullException">
	/// Thrown when <paramref name="model"/> is null.
	/// </exception>
	public static OrderDTO ToDTO(this Order model) {
		// Throwing here guarantees the return is never null to the caller
		ArgumentNullException.ThrowIfNull(model);

		return new() {
			CoffeeUserId = model.CoffeeUserId,
			CreatedAt = model.CreatedAt,
			DeletedAt = model.DeletedAt
		};
	}

	/// <summary>
	/// Converts a <see cref="OrderDTO"/> to a new <see cref="Order"/> entity model.
	/// </summary>
	/// <param name="model">
	/// The source <see cref="OrderDTO"/> to convert.
	/// </param>
	/// <returns>
	/// A populated <see cref="Order"/> instance.
	/// </returns>
	/// <exception cref="ArgumentNullException">
	/// Thrown when <paramref name="model"/> is null.
	/// </exception>
	public static Order ToModel(this OrderDTO model) {
		// Throwing here guarantees the return is never null to the caller
		ArgumentNullException.ThrowIfNull(model);

		return new() {
			CoffeeUserId = model.CoffeeUserId,
			CreatedAt = model.CreatedAt,
			DeletedAt = model.DeletedAt
		};
	}

	/// <summary>
	/// Copies the data from a <see cref="OrderDTO"/> to an
	/// existing instance of <see cref="Order"/>.
	/// </summary>
	/// <param name="model">
	/// The source model from which the data is taken.
	/// </param>
	/// <param name="target">
	/// The existing target model that is updated with the data from <paramref name="model"/>.
	/// </param>
	public static void ToExisting(this OrderDTO model, Order target) {
		target.CoffeeUserId = model.CoffeeUserId;
		target.CreatedAt = model.CreatedAt;
		target.DeletedAt = model.DeletedAt;
	}
}
