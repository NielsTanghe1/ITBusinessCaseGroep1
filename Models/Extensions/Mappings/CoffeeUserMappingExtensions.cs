using Models.Entities;
using Models.Entities.DTO;

namespace Models.Extensions.Mappings;

/// <summary>
/// Contains extension methods for transforming <see cref="CoffeeUser"/>
/// and <see cref="CoffeeUserDTO"/> entities, including creating new
/// model instances, converting them to DTOs, and updating existing
/// models based on DTO data.
/// </summary>
public static class CoffeeUserMappingExtensions {
	/// <summary>
	/// Converts a <see cref="CoffeeUser"/> entity model to a new <see cref="CoffeeUserDTO"/>.
	/// </summary>
	/// <param name="model">
	/// The source <see cref="CoffeeUser"/> entity to convert.
	/// </param>
	/// <returns>
	/// A populated <see cref="CoffeeUserDTO"/> instance.
	/// </returns>
	/// <exception cref="ArgumentNullException">
	/// Thrown when <paramref name="model"/> is null.
	/// </exception>
	public static CoffeeUserDTO ToDTO(this CoffeeUser model) {
		// Throwing here guarantees the return is never null to the caller
		ArgumentNullException.ThrowIfNull(model);

		return new() {
			GlobalId = model.Id,
			FirstName = model.FirstName,
			LastName = model.LastName,
			UserName = model.UserName,
			Email = model.Email,
			EmailConfirmed = model.EmailConfirmed,
			PhoneNumber = model.PhoneNumber,
			PhoneNumberConfirmed = model.PhoneNumberConfirmed,
			TwoFactorEnabled = model.TwoFactorEnabled,
			LockoutEnd = model.LockoutEnd,
			LockoutEnabled = model.LockoutEnabled,
			AccessFailedCount = model.AccessFailedCount,
			CreatedAt = model.CreatedAt,
			DeletedAt = model.DeletedAt
		};
	}

	/// <summary>
	/// Converts a <see cref="CoffeeUserDTO"/> to a new <see cref="CoffeeUser"/> entity model.
	/// </summary>
	/// <param name="model">
	/// The source <see cref="CoffeeUserDTO"/> to convert.
	/// </param>
	/// <returns>
	/// A populated <see cref="CoffeeUser"/> instance.
	/// </returns>
	/// <exception cref="ArgumentNullException">
	/// Thrown when <paramref name="model"/> is null.
	/// </exception>
	public static CoffeeUser ToModel(this CoffeeUserDTO model) {
		// Throwing here guarantees the return is never null to the caller
		ArgumentNullException.ThrowIfNull(model);

		return new() {
			Id = model.GlobalId ?? 0L,
			FirstName = model.FirstName,
			LastName = model.LastName,
			UserName = model.UserName,
			Email = model.Email,
			EmailConfirmed = model.EmailConfirmed,
			PhoneNumber = model.PhoneNumber,
			PhoneNumberConfirmed = model.PhoneNumberConfirmed,
			TwoFactorEnabled = model.TwoFactorEnabled,
			LockoutEnd = model.LockoutEnd,
			LockoutEnabled = model.LockoutEnabled,
			AccessFailedCount = model.AccessFailedCount,
			CreatedAt = model.CreatedAt,
			DeletedAt = model.DeletedAt
		};
	}

	/// <summary>
	/// Copies the data from a <see cref="CoffeeUserDTO"/> to an
	/// existing instance of <see cref="CoffeeUser"/>.
	/// </summary>
	/// <param name="model">
	/// The source model from which the data is taken.
	/// </param>
	/// <param name="target">
	/// The existing target model that is updated with the data from <paramref name="model"/>.
	/// </param>
	public static void ToExisting(this CoffeeUserDTO model, CoffeeUser target) {
		target.FirstName = model.FirstName;
		target.LastName = model.LastName;
		target.UserName = model.UserName;
		target.Email = model.Email;
		target.EmailConfirmed = model.EmailConfirmed;
		target.PhoneNumber = model.PhoneNumber;
		target.PhoneNumberConfirmed = model.PhoneNumberConfirmed;
		target.TwoFactorEnabled = model.TwoFactorEnabled;
		target.LockoutEnd = model.LockoutEnd;
		target.LockoutEnabled = model.LockoutEnabled;
		target.AccessFailedCount = model.AccessFailedCount;
		target.CreatedAt = model.CreatedAt;
		target.DeletedAt = model.DeletedAt;
	}

	/// <summary>
	/// Creates a shallow, untracked, copy of a <see cref="CoffeeUser"/> to a new <see cref="CoffeeUser"/> entity model.
	/// </summary>
	/// <param name="model">
	/// The source <see cref="CoffeeUser"/> to convert.
	/// </param>
	/// <returns>
	/// A populated <see cref="CoffeeUser"/> instance.
	/// </returns>
	/// <exception cref="ArgumentNullException">
	/// Thrown when <paramref name="model"/> is null.
	/// </exception>
	public static CoffeeUser ShallowCopy(this CoffeeUser model) {
		// Throwing here guarantees the return is never null to the caller
		ArgumentNullException.ThrowIfNull(model);

		return new() {
			Id = model.GlobalId ?? 0L,
			FirstName = model.FirstName,
			LastName = model.LastName,
			UserName = model.UserName,
			Email = model.Email,
			EmailConfirmed = model.EmailConfirmed,
			PhoneNumber = model.PhoneNumber,
			PhoneNumberConfirmed = model.PhoneNumberConfirmed,
			TwoFactorEnabled = model.TwoFactorEnabled,
			LockoutEnd = model.LockoutEnd,
			LockoutEnabled = model.LockoutEnabled,
			AccessFailedCount = model.AccessFailedCount,
			CreatedAt = model.CreatedAt,
			DeletedAt = model.DeletedAt
		};
	}
}
