using Helix.Service.Interfaces;
using Helix.Service.Settings;
using MailKit.Net.Smtp;
using Microsoft.Extensions.Options;
using MimeKit;

namespace Helix.Service.Services
{
    public class EmailServices : IEmailService
    {
        private readonly EmailSettings emailSettings;
        public EmailServices(IOptions<EmailSettings> _emailSettings)
        {
            emailSettings = _emailSettings.Value;
        }
        public async Task<string> SendEmail(string email, string _message, string? reason)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(email))
                {
                    throw new ArgumentException("Email address cannot be empty.", nameof(email));
                }

                if (string.IsNullOrWhiteSpace(_message))
                {
                    throw new ArgumentException("Email message cannot be empty.", nameof(_message));
                }

                // Validate email settings
                if (string.IsNullOrWhiteSpace(emailSettings.Host) || 
                    string.IsNullOrWhiteSpace(emailSettings.FromEmail) || 
                    string.IsNullOrWhiteSpace(emailSettings.Password))
                {
                    throw new InvalidOperationException("Email settings are not properly configured.");
                }

                // Sending the message
                using (var client = new SmtpClient())
                {
                    await client.ConnectAsync(emailSettings.Host, emailSettings.Port, true);
                    client.Authenticate(emailSettings.FromEmail, emailSettings.Password);
                    
                    var bodybuilder = new BodyBuilder
                    {
                        HtmlBody = _message,
                        TextBody = System.Text.RegularExpressions.Regex.Replace(_message, "<[^>]*>", "") // Strip HTML for plain text
                    };
                    
                    var message = new MimeMessage
                    {
                        Body = bodybuilder.ToMessageBody()
                    };
                    
                    message.From.Add(new MailboxAddress("Helix Team", emailSettings.FromEmail));
                    message.To.Add(new MailboxAddress(email, email));
                    message.Subject = reason ?? "Helix Notification";
                    
                    await client.SendAsync(message);
                    await client.DisconnectAsync(true);
                }
                
                return "Success";
            }
            catch (Exception ex)
            {
                // Log the exception (in a real application, use ILogger)
                // For now, throw to allow calling code to handle it
                throw new InvalidOperationException($"Failed to send email: {ex.Message}", ex);
            }
        }
    }
}
