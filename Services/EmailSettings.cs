namespace ITBusinessCase.Services;

public class EmailSettings {
	public string Host { get; set; } = "";
	public int Port { get; set; } = 587;
	public bool UseStartTls { get; set; } = true;

	public string FromName { get; set; } = "ITBusinessCase";
	public string FromEmail { get; set; } = "";

	public string Username { get; set; } = "";
	public string Password { get; set; } = "";
}
