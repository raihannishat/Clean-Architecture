using BlogApp.API.Application.Common;

namespace BlogApp.API.Core.Interfaces;

public interface IAuthService
{
    Task<BaseResponse<LoginResponse>> LoginAsync(string email, string password);
    Task<BaseResponse<RegisterResponse>> RegisterAsync(string firstName, string lastName, string email, string userName, string password, string confirmPassword);
}

public class LoginResponse
{
    public string Token { get; set; } = string.Empty;
    public string UserId { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string UserName { get; set; } = string.Empty;
}

public class RegisterResponse
{
    public string UserId { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string UserName { get; set; } = string.Empty;
} 