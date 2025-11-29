using ECommerce.Application.Interfaces;
using ECommerce.Domain.Entities;
using ECommerce.Domain.Interfaces;
using Microsoft.Extensions.Configuration;
using Stripe;
using Stripe.Checkout;

namespace ECommerce.Infrastructure.Services;

public class StripePaymentService : IPaymentService
{
    private readonly IConfiguration _configuration;
    private readonly IUnitOfWork _unitOfWork;

    public StripePaymentService(IConfiguration configuration, IUnitOfWork unitOfWork)
    {
        _configuration = configuration;
        _unitOfWork = unitOfWork;
        StripeConfiguration.ApiKey = _configuration["Stripe:SecretKey"];
    }

    public async Task<string> CreateCheckoutSessionAsync(int orderId, string successUrl, string cancelUrl)
    {
        var order = await _unitOfWork.Orders.GetByIdAsync(orderId);
        if (order == null)
        {
            throw new Exception("Order not found");
        }

        var options = new SessionCreateOptions
        {
            PaymentMethodTypes = new List<string> { "card" },
            LineItems = new List<SessionLineItemOptions>(),
            Mode = "payment",
            SuccessUrl = successUrl,
            CancelUrl = cancelUrl,
            ClientReferenceId = orderId.ToString(),
            Metadata = new Dictionary<string, string>
            {
                { "order_id", orderId.ToString() },
                { "order_number", order.OrderNumber }
            }
        };

        foreach (var item in order.OrderItems)
        {
            options.LineItems.Add(new SessionLineItemOptions
            {
                PriceData = new SessionLineItemPriceDataOptions
                {
                    Currency = "usd",
                    ProductData = new SessionLineItemPriceDataProductDataOptions
                    {
                        Name = item.Product.Name,
                        Images = !string.IsNullOrEmpty(item.Product.ImageUrl) 
                            ? new List<string> { item.Product.ImageUrl } 
                            : null
                    },
                    UnitAmount = (long)(item.UnitPrice * 100) // Convert to cents
                },
                Quantity = item.Quantity
            });
        }

        // Add shipping fee if exists
        if (order.ShippingFee > 0)
        {
            options.LineItems.Add(new SessionLineItemOptions
            {
                PriceData = new SessionLineItemPriceDataOptions
                {
                    Currency = "usd",
                    ProductData = new SessionLineItemPriceDataProductDataOptions
                    {
                        Name = "Shipping Fee"
                    },
                    UnitAmount = (long)(order.ShippingFee * 100)
                },
                Quantity = 1
            });
        }

        var service = new SessionService();
        var session = await service.CreateAsync(options);

        // Save session ID to order
        order.StripeSessionId = session.Id;
        await _unitOfWork.Orders.UpdateAsync(order);
        await _unitOfWork.SaveChangesAsync();

        return session.Url;
    }

    public async Task<bool> VerifyPaymentAsync(string sessionId)
    {
        var service = new SessionService();
        var session = await service.GetAsync(sessionId);

        return session.PaymentStatus == "paid";
    }

    public async Task HandleWebhookAsync(string payload, string signature)
    {
        var webhookSecret = _configuration["Stripe:WebhookSecret"];
        
        try
        {
            var stripeEvent = EventUtility.ConstructEvent(
                payload, signature, webhookSecret);

            if (stripeEvent.Type == Events.CheckoutSessionCompleted)
            {
                var session = stripeEvent.Data.Object as Session;
                if (session != null)
                {
                    await HandleSuccessfulPayment(session);
                }
            }
        }
        catch (Exception ex)
        {
            throw new Exception($"Webhook error: {ex.Message}");
        }
    }

    private async Task HandleSuccessfulPayment(Session session)
    {
        var order = await _unitOfWork.Orders.GetByStripeSessionIdAsync(session.Id);
        if (order != null)
        {
            order.Status = OrderStatus.Paid;
            order.PaidAt = DateTime.UtcNow;
            order.StripePaymentIntentId = session.PaymentIntentId;
            order.PaymentMethod = "Stripe";

            await _unitOfWork.Orders.UpdateAsync(order);
            await _unitOfWork.SaveChangesAsync();

            // TODO: Send order confirmation email
        }
    }
}
