using Microsoft.AspNetCore.Mvc.Rendering;

namespace Web.Services;

public class Utilities {
	public Utilities() {
	}

	public List<SelectListItem> GetEnumSelectList<T>(Enum? selectedValue = null) where T : notnull {
		List<SelectListItem> list = [];

		// Placeholder only if no selected value
		if (selectedValue == null) {
			list.Add(new() {
				Value = "",
				Text = "Select a value",
				Selected = true
			});
		}

		foreach (Enum item in Enum.GetValues(typeof(T))) {
			list.Add(new() {
				Value = item.ToString(),
				Text = item.ToString(),
				Selected = (selectedValue?.ToString() == item.ToString())
			});
		}
		return list;
	}
}
