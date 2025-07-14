using System.Reflection;
using AutoRegister;

namespace BlogApp.API.Application.CQRS;

[Register(ServiceLifetime.Scoped)]
public class Mediator : IMediator
{
    private readonly IServiceProvider _serviceProvider;

    public Mediator(IServiceProvider serviceProvider)
    {
        _serviceProvider = serviceProvider;
    }

    public async Task<TResponse> SendAsync<TRequest, TResponse>(TRequest request, CancellationToken cancellationToken = default) 
        where TRequest : class
    {
        Type handlerType;
        
        if (request is ICommand<TResponse>)
        {
            handlerType = typeof(ICommandHandler<,>).MakeGenericType(typeof(TRequest), typeof(TResponse));
        }
        else if (request is IQuery<TResponse>)
        {
            handlerType = typeof(IQueryHandler<,>).MakeGenericType(typeof(TRequest), typeof(TResponse));
        }
        else
        {
            throw new InvalidOperationException($"Request must implement either ICommand<{typeof(TResponse).Name}> or IQuery<{typeof(TResponse).Name}>");
        }
        
        var handler = _serviceProvider.GetService(handlerType);
        
        if (handler == null)
            throw new InvalidOperationException($"No handler found for {typeof(TRequest).Name}");
        
        var method = handlerType.GetMethod("HandleAsync");
        return await (Task<TResponse>)method!.Invoke(handler, new object[] { request, cancellationToken })!;
    }
} 