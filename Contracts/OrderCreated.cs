namespace ITBusinessCase.Contracts;

// Dit is het bericht dat op de queue komt te staan
public record OrderCreated(
	 Guid OrderId,
	 string CustomerName,
	 decimal TotalAmount);