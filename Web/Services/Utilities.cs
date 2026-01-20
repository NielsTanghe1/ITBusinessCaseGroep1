using Microsoft.AspNetCore.Mvc.Rendering;

namespace Web.Services;

public class Utilities {
	public Utilities() {
	}

	/// <summary>
	/// 
	/// </summary>
	/// <typeparam name="T"></typeparam>
	/// <param name="selectedValue"></param>
	/// <param name="ignored"></param>
	/// <returns></returns>
	public List<SelectListItem> GetEnumSelectList<T>(Enum? selectedValue = null, string[]? ignored = null) where T : notnull {
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
			if (ignored != null && ignored.Contains(item.ToString())) {
				continue;
			}

			list.Add(new() {
				Value = item.ToString(),
				Text = item.ToString(),
				Selected = (selectedValue?.ToString() == item.ToString())
			});
		}
		return list;
	}
}
