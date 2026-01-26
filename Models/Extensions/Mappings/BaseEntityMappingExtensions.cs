using Models.Entities;

namespace Models.Extensions.Mappings;

public static class BaseEntityMappingExtensions {
	/// <summary>
	/// Creates a shallow, untracked copy of a <see cref="IBaseEntity"/> into a new <see cref="IBaseEntity"/> instance.
	/// </summary>
	/// <param name="model">
	/// The source <see cref="IBaseEntity"/> entity to copy.
	/// </param>
	/// <returns>
	/// A new <see cref="IBaseEntity"/> instance with identical property values, 
	/// but detached from any <c>DbContext</c> tracking.
	/// </returns>
	/// <remarks>
	/// This is a shallow copy; reference navigation properties are shared between instances. 
	/// Useful for transferring data between a local and global database context.
	/// </remarks>
	/// <exception cref="ArgumentNullException">
	/// Thrown when <paramref name="model"/> is <see langword="null"/>.
	/// </exception>
	public static T ShallowCopy<T>(this IBaseEntity model) where T : class, IBaseEntity {
		return model switch {
			Address e => (T) (object) e.ShallowCopy(),
			Coffee e => (T) (object) e.ShallowCopy(),
			CoffeeUser e => (T) (object) e.ShallowCopy(),
			OrderItem e => (T) (object) e.ShallowCopy(),
			Order e => (T) (object) e.ShallowCopy(),
			PaymentDetail e => (T) (object) e.ShallowCopy(),
			_ => throw new InvalidOperationException($"No ShallowCopy mapping for {typeof(T).Name}")
		};
	}
}
