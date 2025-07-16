namespace BlogApp.API.Application.Features.Auth.Commands;

public record RegisterCommand(
    string FirstName,
    string LastName,
    string Email,
    string UserName,
    string Password,
    string ConfirmPassword
) : BlogApp.API.Application.CQRS.ICommand<BaseResponse<RegisterResponse>>;

[Register(ServiceLifetime.Scoped)]
public class RegisterCommandHandler : BlogApp.API.Application.CQRS.ICommandHandler<RegisterCommand, BaseResponse<RegisterResponse>>
{
    private readonly IAuthService _authService;
    private readonly IOutboxService _outboxService;

    public RegisterCommandHandler(IAuthService authService, IOutboxService outboxService)
    {
        _authService = authService;
        _outboxService = outboxService;
    }

    public async Task<BaseResponse<RegisterResponse>> HandleAsync(RegisterCommand command, CancellationToken cancellationToken = default)
    {
        var result = await _authService.RegisterAsync(
            command.FirstName,
            command.LastName,
            command.Email,
            command.UserName,
            command.Password,
            command.ConfirmPassword
        );
        
        if (result.IsSuccess)
        {
            await _outboxService.AddAsync(nameof(RegisterCommand), command, cancellationToken);
            return BaseResponse<RegisterResponse>.Success(result.Data!, "Registration successful");
        }
        
        return BaseResponse<RegisterResponse>.ValidationError(result.Errors, result.Message);
    }
} 