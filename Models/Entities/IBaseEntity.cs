namespace Models.Entities;

/// <summary>
/// Defines the core contract for all entities within the system, providing standard 
/// auditing properties.
/// </summary>
public interface IBaseEntity {
	/// <summary>
	/// Gets the unique identifier for the entity.
	/// </summary>
	/// <value>
	/// The primary key stored in the database.
	/// </value>
	long Id {
		get; init;
	}

	/// <summary>
	/// Gets the unique global identifier used for synchronization across environments.
	/// </summary>
	/// <value>
	/// The ID corresponding to the record in the global context, or <see langword="null"/> if not yet synced.
	/// </value>
	long? GlobalId {
		get; set;
	}

	/// <summary>
	/// Gets the timestamp of when the entity was initially created.
	/// </summary>
	DateTime CreatedAt {
		get; set;
	}

	/// <summary>
	/// Gets the timestamp of when the entity was soft-deleted.
	/// </summary>
	/// <value>
	/// The deletion date, or <see langword="null"/> if the entity is active.
	/// </value>
	DateTime? DeletedAt {
		get; set;
	}
}
