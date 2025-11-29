using ECommerce.Application.DTOs.Auth;
using ECommerce.Application.DTOs.User;
using ECommerce.Application.Interfaces;
using ECommerce.Domain.Entities;
using ECommerce.Domain.Interfaces;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using BCrypt.Net;

namespace ECommerce.Infrastructure.Services;

public class AuthService : IAuthService
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IConfiguration _configuration;
    private readonly IEmailService _emailService;
    private readonly IFileStorageService _fileStorage;

    public AuthService(
        IUnitOfWork unitOfWork,
        IConfiguration configuration,
        IEmailService emailService,
        IFileStorageService fileStorage)
    {
        _unitOfWork = unitOfWork;
        _configuration = configuration;
        _emailService = emailService;
        _fileStorage = fileStorage;
    }

    public async Task<LoginResponse> RegisterAsync(RegisterRequest request)
    {
        // Check if email exists
        if (await _unitOfWork.Users.EmailExistsAsync(request.Email))
        {
            throw new Exception("Email already exists");
        }

        // Get User role (default)
        var userRole = (await _unitOfWork.Roles.FindAsync(r => r.Name == "User")).FirstOrDefault();
        if (userRole == null)
        {
            throw new Exception("User role not found");
        }

        // Create user
        var user = new User
        {
            Email = request.Email,
            PasswordHash = BCrypt.Net.BCrypt.HashPassword(request.Password),
            FullName = request.FullName,
            PhoneNumber = request.PhoneNumber,
            RoleId = userRole.Id,
            EmailConfirmed = false
        };

        await _unitOfWork.Users.AddAsync(user);
        await _unitOfWork.SaveChangesAsync();
        
        // Create empty cart for user after user is saved
        var cart = new Cart { UserId = user.Id };
        await _unitOfWork.Carts.AddAsync(cart);
        await _unitOfWork.SaveChangesAsync();

        // Try to send confirmation email (don't fail registration if email fails)
        try
        {
            await _emailService.SendRegistrationConfirmationAsync(user.Email, user.FullName);
        }
        catch (Exception)
        {
            // Log error but continue with registration
            // Email will be sent later or user can request resend
        }

        // Generate JWT token
        var token = GenerateJwtToken(user, userRole.Name);

        return new LoginResponse
        {
            Token = token,
            Email = user.Email,
            FullName = user.FullName,
            Role = userRole.Name,
            UserId = user.Id
        };
    }

    public async Task<LoginResponse> LoginAsync(LoginRequest request)
        {
            var user = await _unitOfWork.Users.GetByEmailAsync(request.Email);
            
            if (user == null)
            {
                throw new Exception("Login credentials not found");
            }

            if (!BCrypt.Net.BCrypt.Verify(request.Password, user.PasswordHash))
            {
                throw new Exception("Password is incorrect");
            }

            var token = GenerateJwtToken(user, user.Role.Name);

            return new LoginResponse
            {
                Token = token,
                Email = user.Email,
                FullName = user.FullName,
                Role = user.Role.Name,
                UserId = user.Id
            };
        }    public async Task ChangePasswordAsync(int userId, ChangePasswordRequest request)
    {
        var user = await _unitOfWork.Users.GetByIdAsync(userId);
        if (user == null)
        {
            throw new Exception("User not found");
        }

        if (!BCrypt.Net.BCrypt.Verify(request.OldPassword, user.PasswordHash))
        {
            throw new Exception("Old password is incorrect");
        }

        if (BCrypt.Net.BCrypt.Verify(request.NewPassword, user.PasswordHash))
        {
            throw new Exception("New password must be different from old password");
        }

        user.PasswordHash = BCrypt.Net.BCrypt.HashPassword(request.NewPassword);
        await _unitOfWork.Users.UpdateAsync(user);
        await _unitOfWork.SaveChangesAsync();

        // Send password changed notification email
        try
        {
            await _emailService.SendPasswordChangedAsync(user.Email, user.FullName);
        }
        catch (Exception)
        {
            // Log error but don't fail the password change
        }
    }

    public async Task ForgotPasswordAsync(ForgotPasswordRequest request)
    {
        var user = await _unitOfWork.Users.GetByEmailAsync(request.Email);
        if (user == null)
        {
            // Don't reveal that user doesn't exist
            return;
        }

        // Generate reset token
        var resetToken = Guid.NewGuid().ToString();
        user.ResetPasswordToken = resetToken;
        user.ResetPasswordExpiry = DateTime.UtcNow.AddHours(1);

        await _unitOfWork.Users.UpdateAsync(user);
        await _unitOfWork.SaveChangesAsync();

        // Send reset email
        await _emailService.SendPasswordResetEmailAsync(user.Email, resetToken);
    }

    public async Task ResetPasswordAsync(ResetPasswordRequest request)
    {
        var user = await _unitOfWork.Users.GetByResetTokenAsync(request.Token);
        if (user == null)
        {
            throw new Exception("Invalid or expired reset token");
        }

        user.PasswordHash = BCrypt.Net.BCrypt.HashPassword(request.NewPassword);
        user.ResetPasswordToken = null;
        user.ResetPasswordExpiry = null;

        await _unitOfWork.Users.UpdateAsync(user);
        await _unitOfWork.SaveChangesAsync();
    }

    public async Task<string> UpdateAvatarAsync(int userId, Stream fileStream, string fileName)
    {
        var user = await _unitOfWork.Users.GetByIdAsync(userId);
        if (user == null)
        {
            throw new Exception("User not found");
        }

        // Delete old avatar if exists
        if (!string.IsNullOrEmpty(user.AvatarUrl))
        {
            await _fileStorage.DeleteFileAsync(user.AvatarUrl);
        }

        // Upload new avatar
        var avatarUrl = await _fileStorage.UploadFileAsync(fileStream, fileName, "avatars");
        user.AvatarUrl = avatarUrl;

        await _unitOfWork.Users.UpdateAsync(user);
        await _unitOfWork.SaveChangesAsync();

        return avatarUrl;
    }

    public async Task<UserDto> GetUserByIdAsync(int userId)
    {
        var user = await _unitOfWork.Users.GetByIdAsync(userId);
        if (user == null)
            throw new Exception("User not found");

        return new UserDto
        {
            Id = user.Id,
            Email = user.Email,
            FullName = user.FullName,
            PhoneNumber = user.PhoneNumber,
            Address = user.Address,
            AvatarUrl = user.AvatarUrl,
            CreatedAt = user.CreatedAt,
            UpdatedAt = user.UpdatedAt,
            Role = new RoleDto
            {
                Id = user.Role.Id,
                Name = user.Role.Name
            }
        };
    }

    private string GenerateJwtToken(User user, string roleName)
    {
        var tokenHandler = new JwtSecurityTokenHandler();
        var key = Encoding.ASCII.GetBytes(_configuration["Jwt:Secret"] ?? "YourSuperSecretKeyThatIsAtLeast32CharactersLong!");
        
        var tokenDescriptor = new SecurityTokenDescriptor
        {
            Subject = new ClaimsIdentity(new[]
            {
                new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
                new Claim(ClaimTypes.Email, user.Email),
                new Claim(ClaimTypes.Name, user.FullName),
                new Claim(ClaimTypes.Role, roleName)
            }),
            Expires = DateTime.UtcNow.AddDays(7),
            SigningCredentials = new SigningCredentials(
                new SymmetricSecurityKey(key),
                SecurityAlgorithms.HmacSha256Signature),
            Issuer = _configuration["Jwt:Issuer"] ?? "ECommerceAPI",
            Audience = _configuration["Jwt:Audience"] ?? "ECommerceClient"
        };

        var token = tokenHandler.CreateToken(tokenDescriptor);
        return tokenHandler.WriteToken(token);
    }
}
