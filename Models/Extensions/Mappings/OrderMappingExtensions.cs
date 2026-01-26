using Models.Entities;
using Models.Entities.DTO;

namespace Models.Extensions.Mappings;

/// <summary>
/// Contains extension methods for transforming <see cref="Order"/>
/// and <see cref="OrderDTO"/> entities, including:
/// <list type = "bullet" >
/// <item><description>Creating new model instances.</description></item>
/// <item><description>Converting model instances to DTOs.</description></item>
/// <item><description>Updating existing model instances based on DTO data.</description></item>
/// <item><description>Creating a shallow, untracked copy of a model instance into a new one.</description></item>
/// </list>
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
			GlobalId = model.Id,
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
			Id = model.GlobalId ?? 0L,
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

	/// <summary>
	/// Creates a shallow, untracked copy of a <see cref="Order"/> into a new <see cref="Order"/> instance.
	/// </summary>
	/// <param name="model">
	/// The source <see cref="Order"/> entity to copy.
	/// </param>
	/// <returns>
	/// A new <see cref="Order"/> instance with identical property values, 
	/// but detached from any <c>DbContext</c> tracking.
	/// </returns>
	/// <remarks>
	/// This is a shallow copy; reference navigation properties are shared between instances. 
	/// Useful for transferring data between a local and global database context.
	/// </remarks>
	/// <exception cref="ArgumentNullException">
	/// Thrown when <paramref name="model"/> is <see langword="null"/>.
	/// </exception>
	public static Order ShallowCopy(this Order model) {
		// Throwing here guarantees the return is never null to the caller
		ArgumentNullException.ThrowIfNull(model);

		return new() {
			CoffeeUserId = model.CoffeeUserId,
			CreatedAt = model.CreatedAt,
			DeletedAt = model.DeletedAt
		};
	}
}
