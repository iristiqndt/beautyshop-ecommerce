using ECommerce.Application.DTOs.Cart;
using ECommerce.Domain.Entities;
using ECommerce.Domain.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace ECommerce.API.Controllers;

[Authorize]
[ApiController]
[Route("api/[controller]")]
public class CartController : ControllerBase
{
    private readonly IUnitOfWork _unitOfWork;

    public CartController(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }

    [HttpGet]
    public async Task<IActionResult> GetCart()
    {
        try
        {
            var userId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value ?? "0");
            var cart = await _unitOfWork.Carts.GetCartWithItemsAsync(userId);

            if (cart == null)
            {
                // Create cart if doesn't exist
                cart = new Cart { UserId = userId };
                await _unitOfWork.Carts.AddAsync(cart);
                await _unitOfWork.SaveChangesAsync();
                
                // Fetch the newly created cart with items
                cart = await _unitOfWork.Carts.GetCartWithItemsAsync(userId);
            }

            return Ok(MapToDto(cart!));
        }
        catch (Exception ex)
        {
            return BadRequest(new { message = ex.Message });
        }
    }

    [HttpPost("add")]
    [HttpPost("items")]
    public async Task<IActionResult> AddToCart([FromBody] AddToCartRequest request)
    {
        try
        {
            var userId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value ?? "0");
            var cart = await _unitOfWork.Carts.GetCartWithItemsAsync(userId);

            if (cart == null)
            {
                cart = new Cart { UserId = userId };
                await _unitOfWork.Carts.AddAsync(cart);
                await _unitOfWork.SaveChangesAsync();
            }

            // Check if product exists
            var product = await _unitOfWork.Products.GetByIdAsync(request.ProductId);
            if (product == null)
                return NotFound(new { message = "Product not found" });

            // Check stock
            if (product.StockQuantity < request.Quantity)
                return BadRequest(new { message = "Insufficient stock" });

            // Check if item already in cart
            var existingItem = cart.CartItems.FirstOrDefault(ci => ci.ProductId == request.ProductId);
            if (existingItem != null)
            {
                existingItem.Quantity += request.Quantity;
                await _unitOfWork.CartItems.UpdateAsync(existingItem);
            }
            else
            {
                var cartItem = new CartItem
                {
                    CartId = cart.Id,
                    ProductId = request.ProductId,
                    Quantity = request.Quantity
                };
                await _unitOfWork.CartItems.AddAsync(cartItem);
            }

            await _unitOfWork.SaveChangesAsync();

            // Refresh cart
            cart = await _unitOfWork.Carts.GetCartWithItemsAsync(userId);
            return Ok(MapToDto(cart!));
        }
        catch (Exception ex)
        {
            return BadRequest(new { message = ex.Message });
        }
    }

    [HttpPut("update")]
    [HttpPut("items/{cartItemId}")]
    public async Task<IActionResult> UpdateCartItem(int cartItemId, [FromBody] UpdateCartItemRequest request)
    {
        try
        {
            var userId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value ?? "0");
            var cart = await _unitOfWork.Carts.GetCartWithItemsAsync(userId);

            if (cart == null)
                return NotFound(new { message = "Cart not found" });

            var cartItem = cart.CartItems.FirstOrDefault(ci => ci.Id == cartItemId);
            if (cartItem == null)
                return NotFound(new { message = "Cart item not found" });

            if (request.Quantity <= 0)
            {
                await _unitOfWork.CartItems.DeleteAsync(cartItem.Id);
            }
            else
            {
                var product = await _unitOfWork.Products.GetByIdAsync(cartItem.ProductId);
                if (product != null && product.StockQuantity < request.Quantity)
                    return BadRequest(new { message = "Insufficient stock" });

                cartItem.Quantity = request.Quantity;
                await _unitOfWork.CartItems.UpdateAsync(cartItem);
            }

            await _unitOfWork.SaveChangesAsync();

            // Refresh cart
            cart = await _unitOfWork.Carts.GetCartWithItemsAsync(userId);
            return Ok(MapToDto(cart!));
        }
        catch (Exception ex)
        {
            return BadRequest(new { message = ex.Message });
        }
    }

    [HttpDelete("item/{cartItemId}")]
    [HttpDelete("items/{cartItemId}")]
    public async Task<IActionResult> RemoveItem(int cartItemId)
    {
        try
        {
            var userId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value ?? "0");
            var cart = await _unitOfWork.Carts.GetCartWithItemsAsync(userId);

            if (cart == null)
                return NotFound(new { message = "Cart not found" });

            var cartItem = cart.CartItems.FirstOrDefault(ci => ci.Id == cartItemId);
            if (cartItem == null)
                return NotFound(new { message = "Cart item not found" });

            await _unitOfWork.CartItems.DeleteAsync(cartItemId);
            await _unitOfWork.SaveChangesAsync();

            // Refresh cart
            cart = await _unitOfWork.Carts.GetCartWithItemsAsync(userId);
            return Ok(MapToDto(cart!));
        }
        catch (Exception ex)
        {
            return BadRequest(new { message = ex.Message });
        }
    }

    [HttpDelete]
    [HttpDelete("clear")]
    public async Task<IActionResult> ClearCart()
    {
        try
        {
            var userId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value ?? "0");
            var cart = await _unitOfWork.Carts.GetCartWithItemsAsync(userId);

            if (cart == null)
                return NotFound(new { message = "Cart not found" });

            foreach (var item in cart.CartItems.ToList())
            {
                await _unitOfWork.CartItems.DeleteAsync(item.Id);
            }

            await _unitOfWork.SaveChangesAsync();
            return NoContent();
        }
        catch (Exception ex)
        {
            return BadRequest(new { message = ex.Message });
        }
    }

    private CartDto MapToDto(Cart cart)
    {
        return new CartDto
        {
            Id = cart.Id,
            UserId = cart.UserId,
            Items = cart.CartItems.Select(ci => new CartItemDto
            {
                Id = ci.Id,
                ProductId = ci.ProductId,
                ProductName = ci.Product.Name,
                Price = ci.Product.Price,
                Quantity = ci.Quantity,
                ImageUrl = ci.Product.ImageUrl,
                Subtotal = ci.Quantity * ci.Product.Price
            }).ToList(),
            TotalAmount = cart.CartItems.Sum(ci => ci.Quantity * ci.Product.Price)
        };
    }
}
