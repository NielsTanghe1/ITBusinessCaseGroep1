using Microsoft.EntityFrameworkCore;

namespace Models.Data;

/// <summary>
/// The database context for the local ITBusinessCoffee SQL environment, responsible for
/// online data storage.
/// Manages access to addresses, user, orders, pending RabbitMQ messages/payloads, etc...
/// </summary>
public class GlobalDbContext : BaseDbContext {
	/// <summary>
	/// Initializes a new instance of the <see cref="GlobalDbContext"/> class
	/// with default options.
	/// </summary>
	public GlobalDbContext() : base() { }

	/// <summary>
	/// Initializes a new instance of the <see cref="GlobalDbContext"/> class
	/// with the specified configuration.
	/// </summary>
	/// <param name="options">
	/// The configuration options for this database context.
	/// </param>
	public GlobalDbContext(DbContextOptions<GlobalDbContext> options) : base(options) { }
}
