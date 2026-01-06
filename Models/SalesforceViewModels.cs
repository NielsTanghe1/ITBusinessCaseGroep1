namespace ITBusinessCase.Models {
	public class OrderViewModel {
		public string LastName { get; set; } = string.Empty;
		public string FirstName { get; set; } = string.Empty;
		public string Email { get; set; } = string.Empty;
		public string ProductType { get; set; } = string.Empty; // De lijst van 3 keuzes
		public int Quantity { get; set; }
	}

	public class UserManagementViewModel {
		public string Id { get; set; } = string.Empty;
		public string Name { get; set; } = string.Empty;
		public string Email { get; set; } = string.Empty;
		public string Role { get; set; } = "User";
	}
}
