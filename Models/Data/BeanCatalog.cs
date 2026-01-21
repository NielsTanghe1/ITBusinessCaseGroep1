namespace Web.Data;

public record BeanItem(string Id, string Name, string ImageUrl);

public static class BeanCatalog {
	public static readonly string[] ProductTypes = { "Roasted", "Ground", "Raw" };

	public static readonly IReadOnlyList<BeanItem> Beans = new List<BeanItem>
	{
        // Je kan later eigen foto's zetten in wwwroot/images/beans/
        new("arabica",  "Arabica",  "/images/beans/arabica.jpg"),
		  new("robusta",  "Robusta",  "/images/beans/robusta.jpg"),
		  new("liberica", "Liberica", "/images/beans/liberica.jpg"),
		  new("excelsa",  "Excelsa",  "/images/beans/excelsa.jpg"),
	 };

	// prijzen per KG per combinatie (pas aan zoals je wil)
	private static readonly Dictionary<(string productType, string beanId), decimal> PricesPerKg = new()
	{
		  { ("Roasted","arabica"),  18m },
		  { ("Roasted","robusta"),  14m },
		  { ("Roasted","liberica"), 22m },
		  { ("Roasted","excelsa"),  20m },

		  { ("Ground","arabica"),   19m },
		  { ("Ground","robusta"),   15m },
		  { ("Ground","liberica"),  23m },
		  { ("Ground","excelsa"),   21m },

		  { ("Raw","arabica"),      16m },
		  { ("Raw","robusta"),      12m },
		  { ("Raw","liberica"),     20m },
		  { ("Raw","excelsa"),      18m },
	 };

	public static decimal GetPricePerKg(string productType, string beanId) {
		productType = (productType ?? "").Trim();
		beanId = (beanId ?? "").Trim();

		return PricesPerKg.TryGetValue((productType, beanId), out var p) ? p : 0m;
	}

	// handig voor JS rendering (optioneel)
	public static IReadOnlyDictionary<string, decimal> GetFlatPriceMap() {
		// key: "Roasted|arabica"
		return PricesPerKg.ToDictionary(k => $"{k.Key.productType}|{k.Key.beanId}", v => v.Value);
	}
}
