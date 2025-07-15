using BlogApp.API.Application.CQRS;
using BlogApp.API.Application.Common;
using BlogApp.API.Core.Interfaces;
using FluentValidation;
using AutoRegister;

namespace BlogApp.API.Application.Features.Auth.Commands;

public record LoginCommand(string Email, string Password) : ICommand<BaseResponse<LoginResponse>>;

[Register(ServiceLifetime.Scoped)]
public class LoginCommandHandler : ICommandHandler<LoginCommand, BaseResponse<LoginResponse>>
{
    private readonly IAuthService _authService;
    private readonly IOutboxService _outboxService;

    public LoginCommandHandler(IAuthService authService, IOutboxService outboxService)
    {
        _authService = authService;
        _outboxService = outboxService;
    }

    public async Task<BaseResponse<LoginResponse>> HandleAsync(LoginCommand command, CancellationToken cancellationToken = default)
    {
        var result = await _authService.LoginAsync(command.Email, command.Password);
        
        if (result.IsSuccess)
        {
            await _outboxService.AddAsync(nameof(LoginCommand), command, cancellationToken);
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