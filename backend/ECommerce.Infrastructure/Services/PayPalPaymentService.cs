using ECommerce.Application.Interfaces;
using ECommerce.Domain.Interfaces;
using Microsoft.Extensions.Configuration;
using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;

namespace ECommerce.Infrastructure.Services;

/// <summary>
/// PayPal payment service implementation
/// </summary>
public class PayPalPaymentService
{
    private readonly IConfiguration _configuration;
    private readonly IUnitOfWork _unitOfWork;
    private readonly HttpClient _httpClient;
    private readonly string _baseUrl;
    private readonly string _clientId;
    private readonly string _clientSecret;

    public PayPalPaymentService(IConfiguration configuration, IUnitOfWork unitOfWork, HttpClient httpClient)
    {
        _configuration = configuration;
        _unitOfWork = unitOfWork;
        _httpClient = httpClient;
        
        _clientId = _configuration["PayPal:ClientId"] ?? throw new ArgumentNullException("PayPal:ClientId");
        _clientSecret = _configuration["PayPal:ClientSecret"] ?? throw new ArgumentNullException("PayPal:ClientSecret");
        _baseUrl = _configuration["PayPal:BaseUrl"] ?? "https://api-m.sandbox.paypal.com"; // sandbox by default
    }

    /// <summary>
    /// Get PayPal access token
    /// </summary>
    private async Task<string> GetAccessTokenAsync()
    {
        var auth = Convert.ToBase64String(Encoding.UTF8.GetBytes($"{_clientId}:{_clientSecret}"));
        
        var request = new HttpRequestMessage(HttpMethod.Post, $"{_baseUrl}/v1/oauth2/token");
        request.Headers.Authorization = new AuthenticationHeaderValue("Basic", auth);
        request.Content = new StringContent("grant_type=client_credentials", Encoding.UTF8, "application/x-www-form-urlencoded");

        var response = await _httpClient.SendAsync(request);
        response.EnsureSuccessStatusCode();

        var content = await response.Content.ReadAsStringAsync();
        var tokenResponse = JsonSerializer.Deserialize<JsonElement>(content);
        
        return tokenResponse.GetProperty("access_token").GetString() ?? throw new Exception("Failed to get access token");
    }

    /// <summary>
    /// Create PayPal order for checkout
    /// </summary>
    public async Task<string> CreatePayPalOrderAsync(int orderId, string returnUrl, string cancelUrl)
    {
        var order = await _unitOfWork.Orders.GetByIdAsync(orderId);
        if (order == null)
        {
            throw new Exception("Order not found");
        }

        var accessToken = await GetAccessTokenAsync();

        // Convert VND to USD (1 USD ≈ 25,000 VND), ensure minimum $0.01
        var amountInUsd = Math.Max(0.01m, order.TotalAmount / 25000m);
        var amountString = amountInUsd.ToString("F2", System.Globalization.CultureInfo.InvariantCulture);

        var orderRequest = new
        {
            intent = "CAPTURE",
            purchase_units = new[]
            {
                new
                {
                    reference_id = orderId.ToString(),
                    description = $"Order #{order.OrderNumber}",
                    amount = new
                    {
                        currency_code = "USD",
                        value = amountString
                    }
                }
            },
            application_context = new
            {
                return_url = returnUrl,
                cancel_url = cancelUrl,
                brand_name = "BeautyShop",
                user_action = "PAY_NOW"
            }
        };

        var request = new HttpRequestMessage(HttpMethod.Post, $"{_baseUrl}/v2/checkout/orders");
        request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", accessToken);
        request.Content = new StringContent(
            JsonSerializer.Serialize(orderRequest),
            Encoding.UTF8,
            "application/json"
        );

        var response = await _httpClient.SendAsync(request);
        response.EnsureSuccessStatusCode();

        var content = await response.Content.ReadAsStringAsync();
        var paypalOrder = JsonSerializer.Deserialize<JsonElement>(content);

        // Get approval URL
        var links = paypalOrder.GetProperty("links");
        foreach (var link in links.EnumerateArray())
        {
            if (link.GetProperty("rel").GetString() == "approve")
            {
                return link.GetProperty("href").GetString() ?? throw new Exception("Approval URL not found");
            }
        }

        throw new Exception("Failed to create PayPal order");
    }

    /// <summary>
    /// Capture PayPal payment after approval
    /// </summary>
    public async Task<bool> CapturePayPalPaymentAsync(string paypalOrderId)
    {
        var accessToken = await GetAccessTokenAsync();

        var request = new HttpRequestMessage(HttpMethod.Post, $"{_baseUrl}/v2/checkout/orders/{paypalOrderId}/capture");
        request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", accessToken);
        request.Content = new StringContent("{}", Encoding.UTF8, "application/json");

        var response = await _httpClient.SendAsync(request);
        
        if (!response.IsSuccessStatusCode)
        {
            return false;
        }

        var content = await response.Content.ReadAsStringAsync();
        var captureResponse = JsonSerializer.Deserialize<JsonElement>(content);

        var status = captureResponse.GetProperty("status").GetString();
        return status == "COMPLETED";
    }

    /// <summary>
    /// Verify PayPal webhook signature
    /// </summary>
    public async Task<bool> VerifyWebhookSignatureAsync(string transmissionId, string transmissionTime, 
        string certUrl, string authAlgo, string transmissionSig, string webhookId, string webhookEvent)
    {
        var accessToken = await GetAccessTokenAsync();

        var verifyRequest = new
        {
            transmission_id = transmissionId,
            transmission_time = transmissionTime,
            cert_url = certUrl,
            auth_algo = authAlgo,
            transmission_sig = transmissionSig,
            webhook_id = webhookId,
            webhook_event = JsonSerializer.Deserialize<JsonElement>(webhookEvent)
        };

        var request = new HttpRequestMessage(HttpMethod.Post, $"{_baseUrl}/v1/notifications/verify-webhook-signature");
        request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", accessToken);
        request.Content = new StringContent(
            JsonSerializer.Serialize(verifyRequest),
            Encoding.UTF8,
            "application/json"
        );

        var response = await _httpClient.SendAsync(request);
        
        if (!response.IsSuccessStatusCode)
        {
            return false;
        }

        var content = await response.Content.ReadAsStringAsync();
        var verifyResponse = JsonSerializer.Deserialize<JsonElement>(content);

        var status = verifyResponse.GetProperty("verification_status").GetString();
        return status == "SUCCESS";
    }
}
