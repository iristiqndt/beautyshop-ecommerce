using ECommerce.Application.DTOs.Order;
using ECommerce.Application.Interfaces;
using ECommerce.Domain.Entities;
using ECommerce.Domain.Interfaces;
using ECommerce.Infrastructure.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace ECommerce.API.Controllers;

[Authorize]
[ApiController]
[Route("api/[controller]")]
public class OrdersController : ControllerBase
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IPaymentService _paymentService;
    private readonly PayPalPaymentService _paypalService;
    private readonly IEmailService _emailService;
    private readonly ILogger<OrdersController> _logger;

    public OrdersController(
        IUnitOfWork unitOfWork,
        IPaymentService paymentService,
        PayPalPaymentService paypalService,
        IEmailService emailService,
        ILogger<OrdersController> logger)
    {
        _unitOfWork = unitOfWork;
        _paymentService = paymentService;
        _paypalService = paypalService;
        _emailService = emailService;
        _logger = logger;
    }

    [HttpGet]
    public async Task<IActionResult> GetMyOrders()
    {
        try
        {
            var userId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value ?? "0");
            var orders = await _unitOfWork.Orders.GetByUserIdAsync(userId);
            var orderDtos = orders.Select(o => MapToDto(o));
            return Ok(orderDtos);
        }
        catch (Exception ex)
        {
            return BadRequest(new { message = ex.Message });
        }
    }

    [HttpGet("{id}")]
    public async Task<IActionResult> GetById(int id)
    {
        try
        {
            var userId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value ?? "0");
            var isAdmin = User.IsInRole("Admin");

            var order = await _unitOfWork.Orders.GetByIdAsync(id);
            if (order == null)
                return NotFound();

            // Check authorization
            if (!isAdmin && order.UserId != userId)
                return Forbid();

            return Ok(MapToDto(order));
        }
        catch (Exception ex)
        {
            return BadRequest(new { message = ex.Message });
        }
    }

    [HttpPost("create")]
    public async Task<IActionResult> CreateOrder([FromBody] CreateOrderRequest request)
    {
        try
        {
            var userId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value ?? "0");
            
            // Get cart
            var cart = await _unitOfWork.Carts.GetCartWithItemsAsync(userId);
            if (cart == null || !cart.CartItems.Any())
                return BadRequest(new { message = "Cart is empty" });

            // Check stock for all items
            foreach (var cartItem in cart.CartItems)
            {
                var product = await _unitOfWork.Products.GetByIdAsync(cartItem.ProductId);
                if (product == null || product.StockQuantity < cartItem.Quantity)
                    return BadRequest(new { message = $"Insufficient stock for {product?.Name}" });
            }

            await _unitOfWork.BeginTransactionAsync();

            // Create order
            var orderNumber = GenerateOrderNumber();
            var order = new Order
            {
                UserId = userId,
                OrderNumber = orderNumber,
                Status = OrderStatus.Pending,
                ShippingAddress = request.ShippingAddress,
                ShippingCity = request.ShippingCity,
                ShippingPhone = request.ShippingPhone,
                RecipientName = request.RecipientName,
                PaymentMethod = request.PaymentMethod,
                ShippingFee = 5.00m, // Fixed shipping fee
                TaxAmount = 0,
                TotalAmount = 0
            };

            await _unitOfWork.Orders.AddAsync(order);
            await _unitOfWork.SaveChangesAsync();

            // Create order items and update stock
            decimal subtotal = 0;
            foreach (var cartItem in cart.CartItems)
            {
                var product = await _unitOfWork.Products.GetByIdAsync(cartItem.ProductId);
                if (product != null)
                {
                    var orderItem = new OrderItem
                    {
                        OrderId = order.Id,
                        ProductId = cartItem.ProductId,
                        Quantity = cartItem.Quantity,
                        UnitPrice = product.Price,
                        TotalPrice = cartItem.Quantity * product.Price
                    };

                    await _unitOfWork.OrderItems.AddAsync(orderItem);
                    subtotal += orderItem.TotalPrice;

                    // Update stock
                    product.StockQuantity -= cartItem.Quantity;
                    await _unitOfWork.Products.UpdateAsync(product);
                }
            }

            // Update order total
            order.TotalAmount = subtotal + order.ShippingFee + order.TaxAmount;
            await _unitOfWork.Orders.UpdateAsync(order);

            // Clear cart - delete all items
            _logger.LogInformation($"Clearing cart for user {userId}. Cart has {cart.CartItems.Count} items");
            var cartItemIds = cart.CartItems.Select(ci => ci.Id).ToList();
            foreach (var itemId in cartItemIds)
            {
                await _unitOfWork.CartItems.DeleteAsync(itemId);
            }
            _logger.LogInformation($"Deleted {cartItemIds.Count} cart items");

            await _unitOfWork.SaveChangesAsync();
            await _unitOfWork.CommitTransactionAsync();

            // Get updated order with items
            var createdOrder = await _unitOfWork.Orders.GetByIdAsync(order.Id);
            
            // Send order confirmation email
            var user = await _unitOfWork.Users.GetByIdAsync(userId);
            if (user != null)
            {
                try
                {
                    _logger.LogInformation($"Sending order confirmation email to {user.Email} for order {order.OrderNumber}");
                    await _emailService.SendOrderConfirmationAsync(user.Email, order.OrderNumber, order.TotalAmount);
                    _logger.LogInformation($"Order confirmation email sent successfully to {user.Email}");
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, $"Failed to send order confirmation email to {user.Email}: {ex.Message}");
                    // Log error but don't fail the order
                }
            }
            
            return CreatedAtAction(nameof(GetById), new { id = order.Id }, MapToDto(createdOrder!));
        }
        catch (Exception ex)
        {
            await _unitOfWork.RollbackTransactionAsync();
            return BadRequest(new { message = ex.Message });
        }
    }

    [HttpPost("{orderId}/checkout")]
    public async Task<IActionResult> CreateCheckoutSession(int orderId)
    {
        try
        {
            var userId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value ?? "0");
            var order = await _unitOfWork.Orders.GetByIdAsync(orderId);

            if (order == null)
                return NotFound();

            if (order.UserId != userId)
                return Forbid();

            if (order.Status != OrderStatus.Pending)
                return BadRequest(new { message = "Order already processed" });

            var successUrl = $"{Request.Scheme}://{Request.Host}/payment/success?session_id={{CHECKOUT_SESSION_ID}}";
            var cancelUrl = $"{Request.Scheme}://{Request.Host}/payment/cancel";

            var checkoutUrl = await _paymentService.CreateCheckoutSessionAsync(orderId, successUrl, cancelUrl);

            return Ok(new { url = checkoutUrl });
        }
        catch (Exception ex)
        {
            return BadRequest(new { message = ex.Message });
        }
    }

    [HttpPost("{orderId}/paypal")]
    public async Task<IActionResult> CreatePayPalOrder(int orderId)
    {
        try
        {
            var userId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value ?? "0");
            var order = await _unitOfWork.Orders.GetByIdAsync(orderId);

            if (order == null)
                return NotFound();

            if (order.UserId != userId)
                return Forbid();

            if (order.Status != OrderStatus.Pending)
                return BadRequest(new { message = "Order already processed" });

            var returnUrl = $"{Request.Scheme}://{Request.Host}/payment/paypal/success";
            var cancelUrl = $"{Request.Scheme}://{Request.Host}/payment/paypal/cancel";

            var approvalUrl = await _paypalService.CreatePayPalOrderAsync(orderId, returnUrl, cancelUrl);

            return Ok(new { approvalUrl });
        }
        catch (Exception ex)
        {
            return BadRequest(new { message = ex.Message });
        }
    }

    [HttpPost("paypal/capture")]
    public async Task<IActionResult> CapturePayPalPayment([FromBody] PayPalCaptureRequest request)
    {
        try
        {
            var success = await _paypalService.CapturePayPalPaymentAsync(request.PayPalOrderId);

            if (success && request.OrderId > 0)
            {
                var order = await _unitOfWork.Orders.GetByIdAsync(request.OrderId);
                if (order != null)
                {
                    order.Status = OrderStatus.Processing;
                    order.PaymentStatus = PaymentStatus.Paid;
                    await _unitOfWork.Orders.UpdateAsync(order);
                    await _unitOfWork.SaveChangesAsync();

                    // Send confirmation email
                    try
                    {
                        await _emailService.SendOrderConfirmationAsync(
                            order.User.Email,
                            order.User.FullName,
                            order.OrderNumber,
                            order.TotalAmount,
                            order.OrderDate
                        );
                    }
                    catch (Exception emailEx)
                    {
                        _logger.LogError(emailEx, "Failed to send order confirmation email");
                    }

                    return Ok(new { success = true, orderNumber = order.OrderNumber });
                }
            }

            return BadRequest(new { message = "Payment capture failed" });
        }
        catch (Exception ex)
        {
            return BadRequest(new { message = ex.Message });
        }
    }

    [Authorize(Roles = "Admin")]
    [HttpGet("all")]
    public async Task<IActionResult> GetAllOrders()
    {
        try
        {
            var orders = await _unitOfWork.Orders.GetAllAsync();
            var orderDtos = orders.Select(o => MapToDto(o));
            return Ok(orderDtos);
        }
        catch (Exception ex)
        {
            return BadRequest(new { message = ex.Message });
        }
    }

    [Authorize(Roles = "Admin")]
    [HttpPut("{id}/status")]
    public async Task<IActionResult> UpdateStatus(int id, [FromBody] string status)
    {
        try
        {
            var order = await _unitOfWork.Orders.GetByIdAsync(id);
            if (order == null)
                return NotFound();

            if (Enum.TryParse<OrderStatus>(status, out var orderStatus))
            {
                order.Status = orderStatus;
                await _unitOfWork.Orders.UpdateAsync(order);
                await _unitOfWork.SaveChangesAsync();

                return Ok(MapToDto(order));
            }

            return BadRequest(new { message = "Invalid status" });
        }
        catch (Exception ex)
        {
            return BadRequest(new { message = ex.Message });
        }
    }

    [HttpPut("{id}/cancel")]
    public async Task<IActionResult> CancelOrder(int id)
    {
        try
        {
            var userId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value ?? "0");
            var isAdmin = User.IsInRole("Admin");

            var order = await _unitOfWork.Orders.GetByIdWithItemsAsync(id);
            if (order == null)
                return NotFound(new { message = "Order not found" });

            // Check authorization
            if (!isAdmin && order.UserId != userId)
                return Forbid();

            // Only allow cancelling pending or processing orders
            if (order.Status != OrderStatus.Pending && order.Status != OrderStatus.Processing)
                return BadRequest(new { message = "Không thể hủy đơn hàng ở trạng thái này" });

            // Restore product stock
            foreach (var item in order.OrderItems)
            {
                var product = await _unitOfWork.Products.GetByIdAsync(item.ProductId);
                if (product != null)
                {
                    product.StockQuantity += item.Quantity;
                    await _unitOfWork.Products.UpdateAsync(product);
                }
            }

            // Update order status
            order.Status = OrderStatus.Cancelled;
            await _unitOfWork.Orders.UpdateAsync(order);
            await _unitOfWork.SaveChangesAsync();

            return Ok(MapToDto(order));
        }
        catch (Exception ex)
        {
            return BadRequest(new { message = ex.Message });
        }
    }

    private OrderDto MapToDto(Order order)
    {
        return new OrderDto
        {
            Id = order.Id,
            OrderNumber = order.OrderNumber,
            Status = order.Status.ToString(),
            TotalAmount = order.TotalAmount,
            ShippingFee = order.ShippingFee,
            TaxAmount = order.TaxAmount,
            ShippingAddress = order.ShippingAddress,
            ShippingCity = order.ShippingCity,
            ShippingPhone = order.ShippingPhone,
            RecipientName = order.RecipientName,
            PaymentMethod = order.PaymentMethod,
            CreatedAt = order.CreatedAt,
            PaidAt = order.PaidAt,
            Items = order.OrderItems.Select(oi => new OrderItemDto
            {
                Id = oi.Id,
                ProductId = oi.ProductId,
                ProductName = oi.Product?.Name ?? "Unknown",
                ProductImageUrl = oi.Product?.ImageUrl,
                Quantity = oi.Quantity,
                UnitPrice = oi.UnitPrice,
                TotalPrice = oi.TotalPrice,
                Price = oi.UnitPrice,
                Product = oi.Product != null ? new ProductDto
                {
                    Id = oi.Product.Id,
                    Name = oi.Product.Name,
                    ImageUrl = oi.Product.ImageUrl,
                    Price = oi.Product.Price
                } : null
            }).ToList()
        };
    }

    private string GenerateOrderNumber()
    {
        var date = DateTime.UtcNow.ToString("yyyyMMdd");
        var random = new Random().Next(1000, 9999);
        return $"ORD-{date}-{random}";
    }
}
