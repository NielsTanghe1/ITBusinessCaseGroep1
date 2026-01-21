using Microsoft.AspNetCore.Identity;
using System.ComponentModel.DataAnnotations;

namespace Models.Entities;

/// <summary>
/// Provides a base type for identity entities that require tracking of creation and
/// deletion timestamps.
/// </summary>
/// <remarks>
/// Inherits from <see cref="IdentityUser{TKey}"/> with <see langword="long"/> as the key
/// type. Inherit from <see cref="BaseIdentityEntity"/> to add standardized auditing properties
/// to your user domain entities. The <see cref="CreatedAt"/> property is automatically
/// initialized to the current UTC date and time when the entity is instantiated. The <see
/// cref="DeletedAt"/> property can be set to indicate when the entity was deleted, or left
/// <see langword="null"/> if the entity is active.
/// </remarks>
public abstract class BaseIdentityEntity : IdentityUser<long> {
	/// <summary>
	/// Gets or sets the date and time when the entity was created.
	/// </summary>
	[DataType(DataType.DateTime)]
	[DisplayFormat(DataFormatString = "{0:dd-MM-yyyy HH:mm}", ApplyFormatInEditMode = true)]
	[Display(Name = "CreatedAt")]
	public DateTime CreatedAt {
		get; private set;
	} = DateTime.UtcNow;

	/// <summary>
	/// Gets or sets the date and time when the entity was deleted.
	/// </summary>
	[DataType(DataType.DateTime)]
	[DisplayFormat(DataFormatString = "{0:dd-MM-yyyy HH:mm}", ApplyFormatInEditMode = true)]
	[Display(Name = "DeletedAt")]
	public DateTime? DeletedAt {
		get; set;
	}
}
