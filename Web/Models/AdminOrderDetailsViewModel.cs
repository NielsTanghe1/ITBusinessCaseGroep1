using Models.Entities.DTO;

namespace Web.Models;

public class AdminOrderDetailsViewModel {
	public int Id {
		get; set;
	}
	public string OrderId { get; set; } = "";
	public string Status { get; set; } = "";
	public DateTime CreatedAtUtc {
		get; set;
	}
	public string RawJson { get; set; } = "";
	public OrderSubmittedDto? Payload {
		get; set;
	}
}
