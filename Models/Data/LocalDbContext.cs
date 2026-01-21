using Microsoft.EntityFrameworkCore;

namespace Models.Data;

/// <summary>
/// The database context for the local ITBusinessCoffee SQL environment, responsible for
/// offline data storage.
/// Manages access to addresses, user, orders, pending RabbitMQ messages/payloads, etc...
/// </summary>
public class LocalDbContext : BaseDbContext {
	/// <summary>
	/// Initializes a new instance of the <see cref="LocalDbContext"/> class
	/// with default options.
	/// </summary>
	public LocalDbContext() : base() { }

	/// <summary>
	/// Initializes a new instance of the <see cref="LocalDbContext"/> class
	/// with the specified configuration.
	/// </summary>
	/// <param name="options">
	/// The configuration options for this database context.
	/// </param>
	public LocalDbContext(DbContextOptions<LocalDbContext> options) : base(options) { }
}
