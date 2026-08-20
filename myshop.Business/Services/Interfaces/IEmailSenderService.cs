namespace myshop.Business.Services.Interfaces
{
    public interface IEmailSenderService
    {
        Task SendEmailAsync(string toEmail, string subject, string htmlMessage);
        Task SendWelcomeEmailAsync(string toEmail, string userName);
        Task SendOrderConfirmationEmailAsync(string toEmail, string userName, int orderId, decimal totalAmount);
    }
}