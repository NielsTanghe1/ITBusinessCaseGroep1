//using Microsoft.AspNetCore.Mvc;
using Models.Entities;

namespace Web.Models;

public class OrderViewModel {
	//private List<OrderItem> _items = [];

	//// Customer details
	//public required CoffeeUser CoffeeUser {
	//	get; set;
	//}

	//// Shipping address
	//public required Address Address {
	//	get; set;
	//}

	// Product
	public long CoffeeUserId {
		get; set;
	}

	public long OrderId {
		get; set;
	}

	//[BindProperty]
	public List<OrderItem> Items {
		get; set;
		//get {
		//	return this._items;
		//}
		//set {
		//	this._items = value;
		//	this.QuantityDictionary = _items.ToDictionary(item => item.CoffeeId, item => item.Quantity);
		//}
	} = [];




	//// Long = Coffee ID, int = quantity (default 0)
	//public Dictionary<long, int> QuantityDictionary {
	//	get; set;
	//} = [];
	



	//=> Items?.ToDictionary(item => item.CoffeeId, item => item.Quantity);





	//public decimal TotalPrice {
	//	get {
	//		decimal total = 0;
	//		if (Items != null) {
	//			foreach (var item in Items) {
	//				total += (item.Quantity * item.UnitPrice);
	//			}
	//		}
	//		return total;
	//	}
	//}
}
