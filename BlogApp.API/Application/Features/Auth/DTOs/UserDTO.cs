namespace BlogApp.API.Application.Features.Auth.DTOs;

public record UserDTO(
    string Id,
    string UserName,
    string Email,
    string FirstName,
    string LastName
)
{
    public UserDTO() : this(string.Empty, string.Empty, string.Empty, string.Empty, string.Empty) {}
} 