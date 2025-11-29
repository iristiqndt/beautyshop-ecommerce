namespace ECommerce.Application.Interfaces;

public interface IEmailService
{
    Task SendEmailAsync(string to, string subject, string body);
    Task SendRegistrationConfirmationAsync(string email, string fullName);
    Task SendPasswordResetEmailAsync(string email, string resetToken);
    Task SendOrderConfirmationAsync(string email, string orderNumber, decimal totalAmount);
    Task SendPasswordChangedAsync(string email, string fullName);
}
