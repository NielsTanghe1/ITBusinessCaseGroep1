using System.ComponentModel.DataAnnotations;

namespace Web.Models;

public class BeanOrderItemInput {
	[Required] public string BeanId { get; set; } = "";

	// Meerdere types tegelijk mogelijk:
	[Range(0, 999)] public int RoastedKg { get; set; }
	[Range(0, 999)] public int GroundKg { get; set; }
	[Range(0, 999)] public int RawKg { get; set; }
}
