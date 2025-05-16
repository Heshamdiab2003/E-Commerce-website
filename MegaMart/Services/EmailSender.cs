using MailKit.Net.Smtp;
using MailKit.Security;
using MimeKit;
using System;
using System.Threading.Tasks;
using Microsoft.Extensions.Options;
using Microsoft.Extensions.Logging;
using MegaMart.Models;

namespace MegaMart.Services
{
    public interface IEmailSender
    {
        Task SendEmailAsync(string toEmail, string subject, string body);
        Task SendConfirmationEmailAsync(string email, string confirmationLink);
    }

    public class EmailSender : IEmailSender
    {
        private readonly EmailSettings _emailSettings;
        private readonly ILogger<EmailSender> _logger;

        public EmailSender(IOptions<EmailSettings> emailSettings, ILogger<EmailSender> logger)
        {
            _emailSettings = emailSettings.Value;
            _logger = logger;
        }

        public async Task SendConfirmationEmailAsync(string email, string confirmationLink)
        {
            try
            {
                _logger.LogInformation($"Attempting to send confirmation email to {email}");
                
                var message = new MimeMessage();
                message.From.Add(new MailboxAddress("MegaMart", _emailSettings.FromEmail));
                message.To.Add(new MailboxAddress("", email));
                message.Subject = "تأكيد البريد الإلكتروني - MegaMart";

                var bodyBuilder = new BodyBuilder
                {
                    HtmlBody = $@"
                        <div dir='rtl' style='font-family: Arial, sans-serif; max-width: 600px; margin: 0 auto; padding: 20px;'>
                            <h2 style='color: #333;'>مرحباً بك في MegaMart</h2>
                            <p>شكراً لتسجيلك في موقعنا. يرجى تأكيد بريدك الإلكتروني بالنقر على الرابط أدناه:</p>
                            <p style='margin: 20px 0;'>
                                <a href='{confirmationLink}' 
                                   style='background-color: #4CAF50; color: white; padding: 10px 20px; 
                                          text-decoration: none; border-radius: 5px; display: inline-block;'>
                                    تأكيد البريد الإلكتروني
                                </a>
                            </p>
                            <p>ينتهي هذا الرابط خلال 24 ساعة.</p>
                            <p>إذا لم تقم بإنشاء هذا الحساب، يمكنك تجاهل هذا البريد الإلكتروني.</p>
                            <hr style='margin: 20px 0; border: none; border-top: 1px solid #eee;'>
                            <p style='color: #666; font-size: 12px;'>
                                هذا بريد إلكتروني تلقائي، يرجى عدم الرد عليه.
                            </p>
                        </div>"
                };

                message.Body = bodyBuilder.ToMessageBody();

                using var client = new SmtpClient();
                await client.ConnectAsync(_emailSettings.SmtpHost, _emailSettings.SmtpPort, SecureSocketOptions.StartTls);
                await client.AuthenticateAsync(_emailSettings.Username, _emailSettings.Password);
                await client.SendAsync(message);
                await client.DisconnectAsync(true);

                _logger.LogInformation($"Confirmation email sent successfully to {email}");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error sending confirmation email to {email}");
                throw;
            }
        }

        public async Task SendEmailAsync(string toEmail, string subject, string body)
        {
            try
            {
                _logger.LogInformation($"Attempting to send email to {toEmail} with subject: {subject}");

                var emailMessage = new MimeMessage();
                emailMessage.From.Add(new MailboxAddress("MegaMart", _emailSettings.FromEmail));
                emailMessage.To.Add(new MailboxAddress("", toEmail));
                emailMessage.Subject = subject;

                var bodyBuilder = new BodyBuilder
                {
                    HtmlBody = $@"
                        <div dir='rtl' style='font-family: Arial, sans-serif; max-width: 600px; margin: 0 auto; padding: 20px;'>
                            {body}
                        </div>"
                };

                emailMessage.Body = bodyBuilder.ToMessageBody();

                using var client = new SmtpClient();
                await client.ConnectAsync(_emailSettings.SmtpHost, _emailSettings.SmtpPort, SecureSocketOptions.StartTls);
                await client.AuthenticateAsync(_emailSettings.Username, _emailSettings.Password);
                await client.SendAsync(emailMessage);
                await client.DisconnectAsync(true);

                _logger.LogInformation($"Email sent successfully to {toEmail}");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error sending email to {toEmail}");
                throw;
            }
        }
    }
}
