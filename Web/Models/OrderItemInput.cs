using System.ComponentModel.DataAnnotations;

namespace Web.Models;

public class OrderItemInput {
	public long ProductId {
		get; set;
	}

	// 0 = niet gekozen, 1..10 = gekozen
	[Range(0, 10, ErrorMessage = "Aantal moet tussen 0 en 10 liggen.")]
	public int Quantity { get; set; } = 0;
}
