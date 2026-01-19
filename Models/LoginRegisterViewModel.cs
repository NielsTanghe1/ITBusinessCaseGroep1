using System.ComponentModel.DataAnnotations;

namespace ITBusinessCase.Models;

public class LoginRegisterViewModel
{
    // Login
    [Required(ErrorMessage = "Email is verplicht.")]
    [EmailAddress]
    public string? LoginEmail { get; set; }

    [Required(ErrorMessage = "Wachtwoord is verplicht.")]
    [DataType(DataType.Password)]
    public string? LoginPassword { get; set; }

    // Register
    [Required(ErrorMessage = "Email is verplicht.")]
    [EmailAddress]
    public string? RegisterEmail { get; set; }

    [Required(ErrorMessage = "Wachtwoord is verplicht.")]
    [DataType(DataType.Password)]
    [MinLength(6, ErrorMessage = "Minstens 6 karakters.")]
    public string? RegisterPassword { get; set; }

    [Required(ErrorMessage = "Bevestigen is verplicht.")]
    [DataType(DataType.Password)]
    [Compare(nameof(RegisterPassword), ErrorMessage = "Passwords komen niet overeen.")]
    public string? ConfirmPassword { get; set; }

    // Profielgegevens (prefill bij orders)
    [Required(ErrorMessage = "Voornaam is verplicht.")]
    public string? FirstName { get; set; }

    [Required(ErrorMessage = "Achternaam is verplicht.")]
    public string? LastName { get; set; }

    [Required(ErrorMessage = "Straat is verplicht.")]
    public string? Street { get; set; }

    [Required(ErrorMessage = "Postbus/huisnr is verplicht.")]
    public string? Postbus { get; set; }

    [Required(ErrorMessage = "Stad is verplicht.")]
    public string? City { get; set; }

    [Required(ErrorMessage = "Postcode is verplicht.")]
    public string? Postcode { get; set; }

    [Required(ErrorMessage = "Land is verplicht.")]
    public string? Country { get; set; }
}
