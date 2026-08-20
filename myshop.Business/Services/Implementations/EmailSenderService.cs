using MailKit.Net.Smtp;
using MailKit.Security;
using Microsoft.Extensions.Options;
using MimeKit;
using myshop.Business.Services.Interfaces;
using myshop.Business.Setting;

namespace myshop.Business.Services.Implementations
{
    public class EmailSenderService : IEmailSenderService
    {
        private readonly EmailSettings _emailSettings;

        public EmailSenderService(IOptions<EmailSettings> emailSettings)
        {
            _emailSettings = emailSettings.Value;
        }

        public async Task SendEmailAsync(string toEmail, string subject, string htmlMessage)
        {
            var message = new MimeMessage();
            message.From.Add(new MailboxAddress(_emailSettings.SenderName, _emailSettings.SenderEmail));
            message.To.Add(MailboxAddress.Parse(toEmail));
            message.Subject = subject;

            var bodyBuilder = new BodyBuilder
            {
                HtmlBody = htmlMessage
            };

            message.Body = bodyBuilder.ToMessageBody();

            using var client = new SmtpClient();

            // Connect to SMTP Server
            var secureOption = _emailSettings.EnableSsl ? SecureSocketOptions.StartTls : SecureSocketOptions.Auto;
            await client.ConnectAsync(_emailSettings.SmtpServer, _emailSettings.Port, secureOption);

            // Authenticate if credentials are provided
            if (!string.IsNullOrEmpty(_emailSettings.Username))
            {
                await client.AuthenticateAsync(_emailSettings.Username, _emailSettings.Password);
            }

            await client.SendAsync(message);
            await client.DisconnectAsync(true);
        }

        public async Task SendWelcomeEmailAsync(string toEmail, string userName)
        {
            var subject = "Welcome to MyShop! 🎉";
            var body = GetBaseEmailTemplate(
                title: $"Welcome, {userName}!",
                contentHtml: $@"
                    <p>We are thrilled to have you join <strong>MyShop</strong>.</p>
                    <p>Start exploring our catalog and discover amazing deals today!</p>
                    <div style='text-align: center; margin: 30px 0;'>
                        <a href='https://localhost:7198' style='background-color: #0d6efd; color: white; padding: 12px 24px; text-decoration: none; border-radius: 5px; font-weight: bold;'>Start Shopping Now</a>
                    </div>"
            );

            await SendEmailAsync(toEmail, subject, body);
        }

        public async Task SendOrderConfirmationEmailAsync(string toEmail, string userName, int orderId, decimal totalAmount)
        {
            var subject = $"Order Confirmation #{orderId} - MyShop";
            var body = GetBaseEmailTemplate(
                title: $"Order Confirmation #{orderId}",
                contentHtml: $@"
                    <p>Dear {userName},</p>
                    <p>Thank you for your order! We have received it and are preparing it for shipment.</p>
                    <div style='background-color: #f8f9fa; padding: 15px; border-radius: 5px; margin: 20px 0;'>
                        <p style='margin: 5px 0;'><strong>Order Number:</strong> #{orderId}</p>
                        <p style='margin: 5px 0;'><strong>Total Amount:</strong> ${totalAmount:F2}</p>
                        <p style='margin: 5px 0;'><strong>Status:</strong> Approved</p>
                    </div>
                    <p>You can track your order anytime from your profile.</p>"
            );

            await SendEmailAsync(toEmail, subject, body);
        }

        // قالب HTML رئيسي قابل للإنعاش والاستخدام في كل الإيميلات (Reusable Base Template)
        private static string GetBaseEmailTemplate(string title, string contentHtml)
        {
            return $@"
            <!DOCTYPE html>
            <html>
            <head>
                <meta charset='utf-8' />
                <style>
                    body {{ font-family: 'Segoe UI', Tahoma, Geneva, Verdana, sans-serif; background-color: #f4f6f9; margin: 0; padding: 0; }}
                    .container {{ max-width: 600px; margin: 30px auto; background-color: #ffffff; border-radius: 8px; overflow: hidden; box-shadow: 0 4px 6px rgba(0,0,0,0.1); }}
                    .header {{ background-color: #212529; color: #ffffff; padding: 20px; text-align: center; }}
                    .header h1 {{ margin: 0; font-size: 24px; }}
                    .content {{ padding: 30px; color: #333333; line-height: 1.6; }}
                    .footer {{ background-color: #f1f1f1; color: #666666; text-align: center; padding: 15px; font-size: 12px; }}
                </style>
            </head>
            <body>
                <div class='container'>
                    <div class='header'>
                        <h1>MyShop</h1>
                    </div>
                    <div class='content'>
                        <h2>{title}</h2>
                        {contentHtml}
                    </div>
                    <div class='footer'>
                        <p>&copy; {DateTime.Now.Year} MyShop. All rights reserved.</p>
                    </div>
                </div>
            </body>
            </html>";
        }
    }
}