namespace ITBusinessCase.Data;

public record CoffeeItem(string Id, string Name, decimal Price, string ImageUrl);

public static class CoffeeCatalog {
	public static readonly IReadOnlyList<CoffeeItem> Items = new List<CoffeeItem>
	{
		  new("espresso", "Espresso", 2.00m, "/images/products/espresso.jpg"),
		  new("americano", "Americano", 2.50m, "/images/products/americano.jpg"),
		  new("cappuccino", "Cappuccino", 3.20m, "/images/products/cappuccino.jpg"),
		  new("latte", "Caffè Latte", 3.50m, "/images/products/latte.jpg"),
		  new("mocha", "Mocha", 3.80m, "/images/products/mocha.jpg"),
		  new("flatwhite", "Flat White", 3.40m, "/images/products/flatwhite.jpg"),
		  new("icedlatte", "Iced Latte", 3.70m, "/images/products/icedlatte.jpg"),
	 };
}
