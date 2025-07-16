namespace BlogApp.API.Application.Features.Auth.Commands;

public record LoginCommand(string Email, string Password) : BlogApp.API.Application.CQRS.ICommand<BaseResponse<LoginResponse>>;

[Register(ServiceLifetime.Scoped)]
public class LoginCommandHandler : BlogApp.API.Application.CQRS.ICommandHandler<LoginCommand, BaseResponse<LoginResponse>>
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