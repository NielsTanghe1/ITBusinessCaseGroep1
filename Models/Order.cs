using System.ComponentModel.DataAnnotations;

namespace ITBusinessCase.Models;

public class Order {
	[Key]
	public int Id { get; set; }

	[Required]
	public string OrderId { get; set; } = "";

	public DateTime CreatedAtUtc { get; set; } = DateTime.UtcNow;

	public string Status { get; set; } = "Submitted";

	// ✅ voor "mijn bestellingen"
	public string? UserId { get; set; }
	public string? CustomerEmail { get; set; }
	public string? CustomerName { get; set; }

	// ✅ volledige payload bewaren
	[Required]
	public string PayloadJson { get; set; } = "";
}
