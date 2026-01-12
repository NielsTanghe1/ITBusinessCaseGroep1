using System.ComponentModel.DataAnnotations;

namespace ITBusinessCase.Models;

public class LoginRegisterViewModel {
	// LOGIN
	[Required]
	[EmailAddress]
	public string? LoginEmail { get; set; }

	[Required]
	[DataType(DataType.Password)]
	public string? LoginPassword { get; set; }

	// REGISTER
	[Required]
	[EmailAddress]
	public string? RegisterEmail { get; set; }

	[Required]
	[DataType(DataType.Password)]
	public string? RegisterPassword { get; set; }

	[Required]
	[DataType(DataType.Password)]
	[Compare(nameof(RegisterPassword), ErrorMessage = "Wachtwoorden komen niet overeen.")]
	public string? ConfirmPassword { get; set; }
}
