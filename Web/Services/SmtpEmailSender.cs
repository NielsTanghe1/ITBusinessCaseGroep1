using MailKit.Net.Smtp;
using MailKit.Security;
using Microsoft.Extensions.Options;
using MimeKit;

namespace Web.Services;

public class SmtpEmailSender : IEmailSender {
	private readonly EmailSettings _settings;

	public SmtpEmailSender(IOptions<EmailSettings> options) {
		_settings = options.Value;
	}

	public async Task SendAsync(string toEmail, string subject, string htmlBody) {
		var message = new MimeMessage();
		message.From.Add(new MailboxAddress(_settings.FromName, _settings.FromEmail));
		message.To.Add(MailboxAddress.Parse(toEmail));
		message.Subject = subject;

		message.Body = new BodyBuilder { HtmlBody = htmlBody }.ToMessageBody();

		using var client = new SmtpClient();
		var secure = _settings.UseStartTls ? SecureSocketOptions.StartTls : SecureSocketOptions.Auto;

		await client.ConnectAsync(_settings.Host, _settings.Port, secure);
		await client.AuthenticateAsync(_settings.Username, _settings.Password);
		await client.SendAsync(message);
		await client.DisconnectAsync(true);
	}
}
