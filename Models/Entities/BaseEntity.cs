using System.ComponentModel.DataAnnotations;

namespace Models.Entities;

/// <summary>
/// Provides a base type for entities that require tracking of creation and deletion timestamps.
/// </summary>
/// <remarks>
/// Inherit from <see cref="BaseEntity"/> to add standardized auditing properties to your domain
/// entities. The <see cref="CreatedAt"/> property is automatically initialized to the current UTC date and time when
/// the entity is instantiated. The <see cref="DeletedAt"/> property can be set to indicate when the entity was deleted,
/// or left <see langword="null"/> if the entity is active.
/// </remarks>
public abstract class BaseEntity {
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
