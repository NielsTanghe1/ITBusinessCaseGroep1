using Models.Entities;
using Models.Entities.DTO;

namespace Models.Extensions.Mappings;

/// <summary>
/// Contains extension methods for transforming <see cref="Address"/>
/// and <see cref="AddressDTO"/> entities, including creating new
/// model instances, converting them to DTOs, and updating existing
/// models based on DTO data.
/// </summary>
public static class AddressMappingExtensions {
	/// <summary>
	/// Converts a <see cref="Address"/> entity model to a new <see cref="AddressDTO"/>.
	/// </summary>
	/// <param name="model">
	/// The source <see cref="Address"/> entity to convert.
	/// </param>
	/// <returns>
	/// A populated <see cref="AddressDTO"/> instance.
	/// </returns>
	/// <exception cref="ArgumentNullException">
	/// Thrown when <paramref name="model"/> is null.
	/// </exception>
	public static AddressDTO ToDTO(this Address model) {
		// Throwing here guarantees the return is never null to the caller
		ArgumentNullException.ThrowIfNull(model);

		return new() {
			CoffeeUserId = model.CoffeeUserId,
			Type = model.Type,
			Street = model.Street,
			HouseNumber = model.HouseNumber,
			City = model.City,
			PostalCode = model.PostalCode,
			CountryISO = model.CountryISO,
			CreatedAt = model.CreatedAt,
			DeletedAt = model.DeletedAt
		};
	}

	/// <summary>
	/// Converts a <see cref="AddressDTO"/> to a new <see cref="Address"/> entity model.
	/// </summary>
	/// <param name="model">
	/// The source <see cref="AddressDTO"/> to convert.
	/// </param>
	/// <returns>
	/// A populated <see cref="Address"/> instance.
	/// </returns>
	/// <exception cref="ArgumentNullException">
	/// Thrown when <paramref name="model"/> is null.
	/// </exception>
	public static Address ToModel(this AddressDTO model) {
		// Throwing here guarantees the return is never null to the caller
		ArgumentNullException.ThrowIfNull(model);

		return new() {
			CoffeeUserId = model.CoffeeUserId,
			Type = model.Type,
			Street = model.Street,
			HouseNumber = model.HouseNumber,
			City = model.City,
			PostalCode = model.PostalCode,
			CountryISO = model.CountryISO,
			CreatedAt = model.CreatedAt,
			DeletedAt = model.DeletedAt
		};
	}

	/// <summary>
	/// Copies the data from a <see cref="AddressDTO"/> to an
	/// existing instance of <see cref="Address"/>.
	/// </summary>
	/// <param name="model">
	/// The source model from which the data is taken.
	/// </param>
	/// <param name="target">
	/// The existing target model that is updated with the data from <paramref name="model"/>.
	/// </param>
	public static void ToExisting(this AddressDTO model, Address target) {
		target.CoffeeUserId = model.CoffeeUserId;
		target.Type = model.Type;
		target.Street = model.Street;
		target.HouseNumber = model.HouseNumber;
		target.City = model.City;
		target.PostalCode = model.PostalCode;
		target.CountryISO = model.CountryISO;
		target.CreatedAt = model.CreatedAt;
		target.DeletedAt = model.DeletedAt;
	}
}
