namespace Models.Entities.Records;

public class OrderLineJsonDto {
	public long CoffeeUserId {
		get; set;
	}
	public long OrderId {
		get; set;
	}
	public long CoffeeId {
		get; set;
	}
	public string CoffeeType { get; set; } = "";
	public int Quantity {
		get; set;
	}
	public decimal UnitPrice {
		get; set;
	}
}
