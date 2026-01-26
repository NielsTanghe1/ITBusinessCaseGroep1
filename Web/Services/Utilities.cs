using MassTransit;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Infrastructure;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Models.Data;
using Models.Entities;
using Models.Extensions.Mappings;

namespace Web.Services;

/// <summary>
/// Provides shared helper methods for the ITBusinessCoffee application,
/// including UI utilities and messaging helpers.
/// </summary>
/// <remarks>
/// This class centralizes cross-cutting helpers that do not belong to a specific
/// domain service, such as:<br/>
/// - Building dropdown lists for enums<br/>
/// - Sending messages to the configured message broker<br/>
/// 
/// It is registered with dependency injection and should be consumed via
/// constructor injection where needed.
/// </remarks>
public class Utilities {
	private readonly LocalDbContext _localContext;
	private readonly GlobalDbContext _globalContext;
	private readonly ISendEndpointProvider _sendEndpointProvider;
	private readonly IConfiguration _configuration;

	/// <summary>
	/// Initializes a new instance of the <see cref="Utilities"/> class with
	/// the required dependencies.
	/// </summary>
	/// <param name="sendEndpointProvider">
	/// The send endpoint provider used to obtain endpoints for sending messages
	/// (for example, to specific RabbitMQ queues via MassTransit).
	/// </param>
	/// <param name="configuration">
	/// The application configuration instance used to read settings that affect
	/// utility behavior, such as queue names or enum display options.
	/// </param>
	public Utilities(
		LocalDbContext localContext,
		GlobalDbContext globalContext,
		ISendEndpointProvider sendEndpointProvider,
		IConfiguration configuration) {
		_localContext = localContext;
		_globalContext = globalContext;
		_sendEndpointProvider = sendEndpointProvider;
		_configuration = configuration;
	}

	/// <summary>
	/// Creates a <see cref="List{SelectListItem}"/> suitable for ASP.NET Core dropdowns (e.g. <c>&lt;select asp-items="@Model.CoffeeTypes"&gt;</c>)
	/// from the values of the specified enum type.
	/// </summary>
	/// <typeparam name="T">
	/// The enum type to convert to select options.
	/// </typeparam>
	/// <param name="selectedValue">
	/// Optional. The enum value that should be pre-selected in the dropdown. Defaults to <see langword="null"/> (no selection).
	/// </param>
	/// <param name="ignored">
	/// Optional array of enum names (case-sensitive) to exclude from the select list.
	/// Useful for hiding "None", "Unknown", or internal enum values.
	/// </param>
	/// <returns>
	/// A <see cref="List{SelectListItem}"/> containing all valid enum values as options,
	/// with the specified <paramref name="selectedValue"/> pre-selected if provided.
	/// </returns>
	/// <remarks>
	/// Each <see cref="SelectListItem"/> uses:<br />
	/// - <c>Value</c> = enum's underlying integer value (as string)<br/>
	/// - <c>Text</c> = enum's underlying integer value (as string)<br/>
	/// - <c>Selected</c> = matches <paramref name="selectedValue"/><br/>
	/// </remarks>
	/// <example>
	/// <code>
	/// var coffeeTypes = GetEnumSelectList&lt;CoffeeType&gt;(CoffeeType.Espresso, ["Unknown"]);
	/// Results in: ["Espresso" (selected), "Latte", "Cappuccino"]
	/// </code>
	/// </example>
	public List<SelectListItem> GetEnumSelectList<T>(Enum? selectedValue = null, string[]? ignored = null) where T : notnull, Enum {
		List<SelectListItem> list = [];

		// Placeholder only if no selected value
		if (selectedValue == null) {
			list.Add(new() {
				Value = "",
				Text = "Select a value",
				Selected = true
			});
		}

		foreach (Enum item in Enum.GetValues(typeof(T))) {
			if (ignored != null && ignored.Contains(item.ToString())) {
				continue;
			}

			list.Add(new() {
				Value = item.ToString(),
				Text = item.ToString(),
				Selected = (selectedValue?.ToString() == item.ToString())
			});
		}
		return list;
	}

	/// <summary>
	/// Sends a message of type <typeparamref name="T"/> to the specified RabbitMQ queue/endpoint
	/// using MassTransit.
	/// </summary>
	/// <typeparam name="T">
	/// The message type to send. Must be a record or a simple object.
	/// </typeparam>
	/// <param name="targetQueue">
	/// The RabbitMQ queue name or endpoint address where the message should be routed.
	/// </param>
	/// <param name="message">
	/// The message payload to publish/send.
	/// </param>
	/// <returns>
	/// A <see cref="Task"/> representing the asynchronous send operation.
	/// </returns>
	/// <exception cref="ArgumentNullException">
	/// Thrown when <paramref name="targetQueue"/> or <paramref name="message"/> is <see langword="null"/>.
	/// </exception>
	/// <exception cref="MassTransitException">
	/// Thrown when the send operation fails (e.g. broker unreachable, queue doesn't exist).
	/// </exception>
	/// <remarks>
	/// This method resolves the MassTransit <see cref="ISendEndpoint"/> for the target queue
	/// and sends the message asynchronously. The message will be routed to consumers
	/// configured for type <typeparamref name="T"/> on the target queue.
	/// </remarks>
	/// <example>
	/// <code>
	/// await SendMessageTo("order-queue", new OrderCreatedEvent {
	///     OrderId = 123,
	///     CustomerEmail = "user@example.com"
	/// });
	/// </code>
	/// </example>
	public async Task SendMessageTo<T>(string targetQueue, T message) where T : notnull {
		var rabbit = _configuration.GetSection("RabbitMQConfig");
		var host = rabbit["Host"];
		var vhost = rabbit["VirtualHost"] ?? "/";
		var port = rabbit.GetValue<int?>("Port:Cluster") ?? 5672;

		// Build URI: vhost "/" -> no path, otherwise "/<vhost>"
		var vhostPath = (string.IsNullOrWhiteSpace(vhost) || vhost == "/")
			? string.Empty
			: "/" + vhost.TrimStart('/');

		var uri = new Uri($"rabbitmq://{host}:{port}{vhostPath}/{targetQueue}");
		var endpoint = await _sendEndpointProvider.GetSendEndpoint(uri);

		await endpoint.Send(message);
	}

	/// <summary>
	/// Calculates the total price for a single order item by multiplying quantity by unit price.
	/// </summary>
	/// <param name="orderItem">
	/// The order item containing <see cref="OrderItem.Quantity"/> and <see cref="OrderItem.UnitPrice"/>.
	/// </param>
	/// <returns>
	/// The total price as a <see cref="decimal"/>, calculated as <c>orderItem.Quantity * orderItem.UnitPrice</c>.
	/// </returns>
	public static decimal GetTotalPrice(OrderItem orderItem) {
		return (orderItem.Quantity * orderItem.UnitPrice);
	}

	/// <summary>
	/// Backs up any entity implementing <see cref="IBaseEntity"/> from the local context to the global context.
	/// </summary>
	/// <typeparam name="T">
	/// The entity type which must implement <see cref="IBaseEntity"/>.
	/// </typeparam>
	/// <param name="localEntity">
	/// The local entity instance to back up.
	/// </param>
	/// <remarks>
	/// This method assumes no global copy exists yet. It performs a cross-context operation; 
	/// failure at the local update stage may result in orphaned global records.
	/// </remarks>
	/// <returns>
	/// A <see cref="Action{T1, T2}"/> with:<br/>
	///  - T1: A status code: 200 (Success), 400 (Error), or 503 (Database Unavailable).<br/>
	///  - T2: The global instance that was created/saved.
	/// </returns>
	private async Task<(int, T?)> BackupToGlobal<T>(T localEntity) where T : class, IBaseEntity {
		/* 
		 * PROCESS:
		 * 1. Ensure connectivity at the start
		 * 2. Get the global entity if a GlobalId is present.
		 * 
		 * No entity found?
		 * 1. Create a new global entity by shallow copying the local entity.
		 * 2. Save changes in the global context and obtain a new GlobalId.
		 * 3. Update the local entity GlobalID and save changes.
		 * 
		 * Entity found?
		 * 1. Update the global entity by shallow copying the local entity.
		 * 2. Save changes in the global context.
		 * 
		 * ISSUES:
		 * . Connectivity check happens only once at the start so failure between steps may result in orphaned global entities.
		 */
		try {
			// Connectivity Check
			if (!_globalContext.Database.CanConnect() || !_localContext.Database.CanConnect()) {
				return (StatusCodes.Status503ServiceUnavailable, null);
			}

			T? globalEntity = null;

			// Get from global context
			if (localEntity.GlobalId != null) {
				globalEntity = await _globalContext.FindAsync<T>(localEntity.GlobalId);
			}

			// Create new global instance, update local
			if (globalEntity == null) {
				globalEntity = localEntity.ShallowCopy<T>();
				_globalContext.Set<T>().Add(globalEntity);
				await _globalContext.SaveChangesAsync();

				localEntity.GlobalId = globalEntity.Id;
				_localContext.Set<T>().Update(localEntity);
				await _localContext.SaveChangesAsync();
				return (StatusCodes.Status200OK, globalEntity);
			}

			// Update the global instance
			globalEntity = localEntity.ShallowCopy<T>();
			_globalContext.Set<T>().Update(globalEntity);
			await _localContext.SaveChangesAsync();
			return (StatusCodes.Status200OK, globalEntity);
		} catch (Exception) {
			// Could do some logging here...
			return (StatusCodes.Status400BadRequest, null);
		}
	}

	/// <summary>
	/// Persists a new entity to the local database and automatically synchronizes it with the global backup server.
	/// </summary>
	/// <typeparam name="T">
	/// The entity type to persist. Must implement <see cref="IBaseEntity"/>.
	/// </typeparam>
	/// <param name="localEntity">
	/// The new entity to add to the local database. Must not exist in the database yet.
	/// </param>
	/// <returns>
	/// A <see cref="ActionResult{TEntity?}"/> containing the persisted entity on success,
	/// or an appropriate HTTP error result (e.g. <c>BadRequest</c>, <c>Conflict</c>) on failure.
	/// </returns>
	/// <exception cref="InvalidOperationException">
	/// Thrown when the entity type <typeparamref name="T"/> is not configured for backup operations,
	/// or when the backup synchronization fails after local persistence.
	/// </exception>
	/// <remarks>
	/// <para>
	/// This method performs a two-phase operation:
	/// <list type="number">
	/// <item>Adds the entity to the local database context and calls <c>SaveChangesAsync()</c>.</item>
	/// <item>Immediately synchronizes the newly created entity with the global backup server.</item>
	/// </list>
	/// </para>
	/// <para>
	/// On success, returns <c>Ok(entity)</c>. On local persistence failure, returns the appropriate MVC error.
	/// Backup failures after local success will throw <see cref="InvalidOperationException"/>.
	/// </para>
	/// </remarks>
	private async Task<ActionResult<T?>> AddAndBackup<T>(T localEntity) where T : class, IBaseEntity {
		/* 
		* PROCESS:
		* 1. Create and save localy
		* 2. Create global backup
		* 3. Return most eggregious error, if any, assuming BackupToGlobal returns (int StatusCode, T? globalEntity)
		* 
		* ISSUES:
		* 1. Should probably return all errors.
		*/
		_localContext.Set<T>().Add(localEntity);
		await _localContext.SaveChangesAsync();

		var syncResult = await BackupToGlobal(localEntity);
		return syncResult switch {
			(503, _) => new ObjectResult(new { message = "Connectivity failed: Unable to reach the local or backup database server." }) { StatusCode = StatusCodes.Status503ServiceUnavailable },
			(400, _) => new ObjectResult(new { message = $"Synchronization rejected: The backup server refused the {typeof(T).Name} entity." }) { StatusCode = StatusCodes.Status400BadRequest },
			(200, T globalEntity) when localEntity.GlobalId != null => new OkObjectResult(globalEntity) { StatusCode = StatusCodes.Status200OK },
			(200, null) => new ObjectResult(new { message = $"Sync partial success: {typeof(T).Name} GlobalId assigned, but global confirmation object was missing." }) { StatusCode = StatusCodes.Status409Conflict },
			(var code, _) when localEntity.GlobalId == null => new ObjectResult(new { message = $"Sync incomplete: local entity saved, but no global reference was established (Status: {code})." }) { StatusCode = StatusCodes.Status422UnprocessableEntity },
			_ => throw new InvalidOperationException($"Unexpected synchronization state. Status: {syncResult}, Entity: {typeof(T).Name}")
		};
	}

	/// <summary>
	/// Validates an entity, adds it to the local database, and synchronizes with the global backup server.
	/// Designed for use in controller actions with ModelState validation.
	/// </summary>
	/// <typeparam name="T">
	/// The entity type to validate and persist. Must implement <see cref="IBaseEntity"/>.
	/// </typeparam>
	/// <param name="targetModelState">
	/// The <see cref="ModelStateDictionary"/> to populate with validation errors.
	/// Typically passed from a controller's <c>ModelState</c> property.
	/// </param>
	/// <param name="localEntity">
	/// The entity to validate and persist.
	/// </param>
	/// <returns>
	/// The successfully persisted and backed-up entity on success, or <see langword="null"/> if validation/persistence fails.
	/// </returns>
	/// <exception cref="InvalidOperationException">
	/// Thrown when backup synchronization fails after successful local persistence.
	/// </exception>
	/// <remarks>
	/// <para>
	/// This method orchestrates the complete "validate → add → backup" workflow for controller actions:
	/// <list type="number">
	/// <item>Validates the entity and populates <paramref name="targetModelState"/> with errors.</item>
	/// <item>If valid, calls <see cref="AddAndBackup{T}(T)"/> to persist and backup.</item>
	/// <item>Returns the entity or <see langword="null"/> based on success/failure.</item>
	/// </list>
	/// </para>
	/// </remarks>
	public async Task<T?> ValidateAddAndBackup<T>(ModelStateDictionary targetModelState, T localEntity) where T : class, IBaseEntity {
		var response = await AddAndBackup(localEntity);
		T? globalEntity = response.Value;

		if (globalEntity == null && response.Result is OkObjectResult ok) {
			globalEntity = ok.Value as T;
		}

		if (globalEntity == null) {
			int statusCode = 0;
			string errorMsg = "Unknown synchronization error.";

			if (response.Result is ObjectResult obj) {
				statusCode = obj.StatusCode ?? 0;
				errorMsg = obj.Value?.ToString() ?? errorMsg;
			} else if (response.Result is IStatusCodeActionResult statusResult) {
				statusCode = statusResult.StatusCode ?? 0;
			}

			targetModelState.AddModelError("", $"Creation and backup failed (Status {statusCode}): {errorMsg}");
		}
		return globalEntity;
	}
}
