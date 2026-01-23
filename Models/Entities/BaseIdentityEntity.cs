using Microsoft.AspNetCore.Identity;
using System.ComponentModel.DataAnnotations;

namespace Models.Entities;

public abstract class BaseIdentityEntity : IdentityUser<long> {

	// 1. ADD THIS PROPERTY
	/// <summary>
	/// Unique ID for synchronization between Local and Global databases.
	/// </summary>
	public long GlobalId {
		get; set;
	}

	/// <summary>
	/// Gets or sets the date and time when the entity was created.
	/// </summary>
	[DataType(DataType.DateTime)]
	[DisplayFormat(DataFormatString = "{0:dd-MM-yyyy HH:mm}", ApplyFormatInEditMode = true)]
	[Display(Name = "CreatedAt")]
	public DateTime CreatedAt {
		get; set;
	} = DateTime.UtcNow; // 2. CHANGED: removed 'private' before set

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