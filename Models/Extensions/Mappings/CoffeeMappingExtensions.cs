using Models.Entities;
using Models.Entities.DTO;

namespace Models.Extensions.Mappings;

/// <summary>
/// Contains extension methods for transforming <see cref="Coffee"/>
/// and <see cref="CoffeeDTO"/> entities, including creating new
/// model instances, converting them to DTOs, and updating existing
/// models based on DTO data.
/// </summary>
public static class CoffeeMappingExtensions {
	/// <summary>
	/// Converts a <see cref="Coffee"/> entity model to a new <see cref="CoffeeDTO"/>.
	/// </summary>
	/// <param name="model">
	/// The source <see cref="Coffee"/> entity to convert.
	/// </param>
	/// <returns>
	/// A populated <see cref="CoffeeDTO"/> instance.
	/// </returns>
	/// <exception cref="ArgumentNullException">
	/// Thrown when <paramref name="model"/> is null.
	/// </exception>
	public static CoffeeDTO ToDTO(this Coffee model) {
		// Throwing here guarantees the return is never null to the caller
		ArgumentNullException.ThrowIfNull(model);

		return new() {
			Name = model.Name,
			Type = model.Type,
			Price = model.Price,
			CreatedAt = model.CreatedAt,
			DeletedAt = model.DeletedAt
		};
	}

	/// <summary>
	/// Converts a <see cref="CoffeeDTO"/> to a new <see cref="Coffee"/> entity model.
	/// </summary>
	/// <param name="model">
	/// The source <see cref="CoffeeDTO"/> to convert.
	/// </param>
	/// <returns>
	/// A populated <see cref="Coffee"/> instance.
	/// </returns>
	/// <exception cref="ArgumentNullException">
	/// Thrown when <paramref name="model"/> is null.
	/// </exception>
	public static Coffee ToModel(this CoffeeDTO model) {
		// Throwing here guarantees the return is never null to the caller
		ArgumentNullException.ThrowIfNull(model);

		return new() {
			Name = model.Name,
			Type = model.Type,
			Price = model.Price,
			CreatedAt = model.CreatedAt,
			DeletedAt = model.DeletedAt
		};
	}

	/// <summary>
	/// Copies the data from a <see cref="CoffeeDTO"/> to an
	/// existing instance of <see cref="Coffee"/>.
	/// </summary>
	/// <param name="model">
	/// The source model from which the data is taken.
	/// </param>
	/// <param name="target">
	/// The existing target model that is updated with the data from <paramref name="model"/>.
	/// </param>
	public static void ToExisting(this CoffeeDTO model, Coffee target) {
		target.Name = model.Name;
		target.Type = model.Type;
		target.Price = model.Price;
		target.CreatedAt = model.CreatedAt;
		target.DeletedAt = model.DeletedAt;
	}
}
