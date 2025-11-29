namespace ECommerce.Application.Interfaces;

public interface IPaymentService
{
    Task<string> CreateCheckoutSessionAsync(int orderId, string successUrl, string cancelUrl);
    Task<bool> VerifyPaymentAsync(string sessionId);
    Task HandleWebhookAsync(string payload, string signature);
}
