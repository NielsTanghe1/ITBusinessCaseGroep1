using Models.Entities;
using Models.Entities.DTO;

namespace Models.Extensions.Mappings;

/// <summary>
/// Contains extension methods for transforming <see cref="Coffee"/>
/// and <see cref="CoffeeDTO"/> entities, including:
/// <list type = "bullet" >
/// <item><description>Creating new model instances.</description></item>
/// <item><description>Converting model instances to DTOs.</description></item>
/// <item><description>Updating existing model instances based on DTO data.</description></item>
/// <item><description>Creating a shallow, untracked copy of a model instance into a new one.</description></item>
/// </list>
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
			GlobalId = model.Id,
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
			Id = model.GlobalId ?? 0L,
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

	/// <summary>
	/// Creates a shallow, untracked copy of a <see cref="Coffee"/> into a new <see cref="Coffee"/> instance.
	/// </summary>
	/// <param name="model">
	/// The source <see cref="Coffee"/> entity to copy.
	/// </param>
	/// <returns>
	/// A new <see cref="Coffee"/> instance with identical property values, 
	/// but detached from any <c>DbContext</c> tracking.
	/// </returns>
	/// <remarks>
	/// This is a shallow copy; reference navigation properties are shared between instances. 
	/// Useful for transferring data between a local and global database context.
	/// </remarks>
	/// <exception cref="ArgumentNullException">
	/// Thrown when <paramref name="model"/> is <see langword="null"/>.
	/// </exception>
	public static Coffee ShallowCopy(this Coffee model) {
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
}
