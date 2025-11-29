using ECommerce.Domain.Interfaces;
using ECommerce.Application.DTOs.User;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace ECommerce.API.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class UsersController : ControllerBase
{
    private readonly IUnitOfWork _unitOfWork;

    public UsersController(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }

    [HttpGet]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> GetAll()
    {
        try
        {
            var users = await _unitOfWork.Users.GetAllAsync();
            var userDtos = users.Select(u => new UserDto
            {
                Id = u.Id,
                Email = u.Email,
                FullName = u.FullName,
                PhoneNumber = u.PhoneNumber,
                Address = u.Address,
                AvatarUrl = u.AvatarUrl,
                CreatedAt = u.CreatedAt,
                UpdatedAt = u.UpdatedAt,
                Role = new RoleDto
                {
                    Id = u.Role.Id,
                    Name = u.Role.Name
                }
            });
            return Ok(userDtos);
        }
        catch (Exception ex)
        {
            return BadRequest(new { message = ex.Message });
        }
    }

    [HttpGet("{id}")]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> GetById(int id)
    {
        try
        {
            var user = await _unitOfWork.Users.GetByIdAsync(id);
            if (user == null)
                return NotFound();
            return Ok(user);
        }
        catch (Exception ex)
        {
            return BadRequest(new { message = ex.Message });
        }
    }

    [HttpPut("{id}/role")]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> UpdateRole(int id, [FromBody] UpdateRoleRequest request)
    {
        try
        {
            var user = await _unitOfWork.Users.GetByIdAsync(id);
            if (user == null)
                return NotFound();

            // Get role by name
            var role = (await _unitOfWork.Roles.FindAsync(r => r.Name == request.Role)).FirstOrDefault();
            if (role == null)
                return BadRequest(new { message = "Vai trò không tồn tại" });

            user.RoleId = role.Id;
            await _unitOfWork.Users.UpdateAsync(user);
            await _unitOfWork.SaveChangesAsync();
            
            return Ok(new { message = "Cập nhật vai trò thành công" });
        }
        catch (Exception ex)
        {
            return BadRequest(new { message = ex.Message });
        }
    }

    [HttpPut("profile")]
    public async Task<IActionResult> UpdateProfile([FromBody] UpdateProfileRequest request)
    {
        try
        {
            var userId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value ?? "0");
            var user = await _unitOfWork.Users.GetByIdAsync(userId);
            
            if (user == null)
                return NotFound();

            user.FullName = request.FullName;
            user.PhoneNumber = request.PhoneNumber;
            user.Address = request.Address;
            user.UpdatedAt = DateTime.UtcNow;

            await _unitOfWork.Users.UpdateAsync(user);
            await _unitOfWork.SaveChangesAsync();
            
            return Ok(new { message = "Cập nhật thông tin thành công" });
        }
        catch (Exception ex)
        {
            return BadRequest(new { message = ex.Message });
        }
    }

    [HttpDelete("{id}")]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> Delete(int id)
    {
        try
        {
            var user = await _unitOfWork.Users.GetByIdAsync(id);
            if (user == null)
                return NotFound();

            // Prevent deleting admin accounts
            var role = await _unitOfWork.Roles.GetByIdAsync(user.RoleId);
            if (role?.Name == "Admin")
                return BadRequest(new { message = "Không thể xóa tài khoản Admin" });

            await _unitOfWork.Users.DeleteAsync(id);
            await _unitOfWork.SaveChangesAsync();
            
            return Ok(new { message = "Xóa người dùng thành công" });
        }
        catch (Exception ex)
        {
            return BadRequest(new { message = ex.Message });
        }
    }
}

public record UpdateRoleRequest(string Role);
public record UpdateProfileRequest(string FullName, string? PhoneNumber, string? Address);
