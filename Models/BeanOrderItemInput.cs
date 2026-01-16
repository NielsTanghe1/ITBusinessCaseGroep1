namespace ITBusinessCase.Models;

public class BeanOrderItemInput {
	public string BeanId { get; set; } = "";
	public string ProductType { get; set; } = "Roasted";
	public int Kg { get; set; } = 0; // aantal KG
}
