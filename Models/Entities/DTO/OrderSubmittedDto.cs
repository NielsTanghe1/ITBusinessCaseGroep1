namespace Models.Entities.Records;

public class OrderSubmittedDto {
	public string? OrderId {
		get; set;
	}
	public string? FirstName {
		get; set;
	}
	public string? LastName {
		get; set;
	}
	public string? Email {
		get; set;
	}

	public string? Street {
		get; set;
	}
	public string? Postbus {
		get; set;
	}
	public string? City {
		get; set;
	}
	public string? Postcode {
		get; set;
	}
	public string? Country {
		get; set;
	}

	public decimal? Total {
		get; set;
	}
	public List<OrderLineJsonDto>? Lines {
		get; set;
	}
}
