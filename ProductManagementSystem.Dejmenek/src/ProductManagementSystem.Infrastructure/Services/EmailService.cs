using MailKit.Net.Smtp;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using MimeKit;
using MimeKit.Text;
using ProductManagementSystem.Application.Abstractions;

namespace ProductManagementSystem.Infrastructure.Services;
public class EmailService : IEmailService
{
    private readonly EmailConfig _emailConfig;
    private readonly ILogger<EmailService> _logger;

    public EmailService(IOptions<EmailConfig> emailConfig, ILogger<EmailService> logger)
    {
        _emailConfig = emailConfig.Value;
        _logger = logger;
    }

    public async Task SendEmailAsync(string to, string subject, string htmlBody)
    {
        _logger.LogInformation("Sending email to {To} with subject {Subject}", to, subject);

        var message = new MimeMessage();
        message.From.Add(new MailboxAddress("PMS", _emailConfig.From));
        message.To.Add(MailboxAddress.Parse(to));
        message.Subject = subject;
        message.Body = new TextPart(TextFormat.Html) { Text = htmlBody };

        try
        {
            using var smtp = new SmtpClient();
            await smtp.ConnectAsync(_emailConfig.SmtpServer, _emailConfig.Port, true);
            await smtp.AuthenticateAsync(_emailConfig.Username, _emailConfig.Password);
            await smtp.SendAsync(message);
            await smtp.DisconnectAsync(true);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error sending email to {To}", to);
            throw;
        }
    }
}
