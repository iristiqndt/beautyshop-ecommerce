namespace ECommerce.Application.DTOs.Order;

public class PayPalCaptureRequest
{
    public string PayPalOrderId { get; set; } = string.Empty;
    public int OrderId { get; set; }
}
