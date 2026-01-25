namespace Web.Models;

public class AdminOrderListItemViewModel {
	public int Id {
		get; set;
	}
	public string OrderId { get; set; } = "";
	public string Status { get; set; } = "";
	public DateTime CreatedAtUtc {
		get; set;
	}
	public string FullName { get; set; } = "";
	public string Email { get; set; } = "";
	public decimal? Total {
		get; set;
	}
}
