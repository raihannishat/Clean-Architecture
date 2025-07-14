namespace BlogApp.API.DTOs;

public class LoginDTO
{
    public string Email { get; set; } = string.Empty;
    public string Password { get; set; } = string.Empty;
}

public class RegisterDTO
{
    public string FirstName { get; set; } = string.Empty;
    public string LastName { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string Password { get; set; } = string.Empty;
    public string ConfirmPassword { get; set; } = string.Empty;
}

public class AuthResponseDTO
{
    public bool IsSuccess { get; set; }
    public string Message { get; set; } = string.Empty;
    public string Token { get; set; } = string.Empty;
    public DateTime ExpiresAt { get; set; }
    public UserDTO User { get; set; } = new UserDTO();
}

public class UserDTO
{
    public string Id { get; set; } = string.Empty;
    public string FirstName { get; set; } = string.Empty;
    public string LastName { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string Bio { get; set; } = string.Empty;
    public string ProfileImageUrl { get; set; } = string.Empty;
    public DateTime CreatedAt { get; set; }
} 