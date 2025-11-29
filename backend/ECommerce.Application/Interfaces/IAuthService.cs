using ECommerce.Application.DTOs.Auth;
using ECommerce.Application.DTOs.User;

namespace ECommerce.Application.Interfaces;

public interface IAuthService
{
    Task<LoginResponse> RegisterAsync(RegisterRequest request);
    Task<LoginResponse> LoginAsync(LoginRequest request);
    Task ChangePasswordAsync(int userId, ChangePasswordRequest request);
    Task ForgotPasswordAsync(ForgotPasswordRequest request);
    Task ResetPasswordAsync(ResetPasswordRequest request);
    Task<string> UpdateAvatarAsync(int userId, Stream fileStream, string fileName);
    Task<UserDto> GetUserByIdAsync(int userId);
}
