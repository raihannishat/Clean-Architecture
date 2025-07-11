using BlogApp.API.Application.CQRS;
using BlogApp.API.Application.Common;
using BlogApp.API.Core.Interfaces;
using FluentValidation;
using AutoRegister;

namespace BlogApp.API.Application.Features.Auth.Commands;

public class LoginCommand : ICommand<BaseResponse<LoginResponse>>
{
    public string Email { get; set; } = string.Empty;
    public string Password { get; set; } = string.Empty;
}

[Register(ServiceLifetime.Scoped)]
public class LoginCommandHandler : ICommandHandler<LoginCommand, BaseResponse<LoginResponse>>
{
    private readonly IAuthService _authService;

    public LoginCommandHandler(IAuthService authService)
    {
        _authService = authService;
    }

    public async Task<BaseResponse<LoginResponse>> HandleAsync(LoginCommand command, CancellationToken cancellationToken = default)
    {
        var result = await _authService.LoginAsync(command.Email, command.Password);
        
        if (result.IsSuccess)
        {
            return BaseResponse<LoginResponse>.Success(result.Data!, "Login successful");
        }
        
        return BaseResponse<LoginResponse>.Unauthorized(result.Message);
    }
}

[Register(ServiceLifetime.Scoped)]
public class LoginCommandValidator : AbstractValidator<LoginCommand>
{
    public LoginCommandValidator()
    {
        RuleFor(x => x.Email)
            .NotEmpty().WithMessage("Email is required")
            .EmailAddress().WithMessage("Invalid email format");

        RuleFor(x => x.Password)
            .NotEmpty().WithMessage("Password is required")
            .MinimumLength(6).WithMessage("Password must be at least 6 characters");
    }
} 