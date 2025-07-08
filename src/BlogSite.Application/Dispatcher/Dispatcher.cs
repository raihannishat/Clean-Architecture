using System.Text.Json;
using MediatR;
using Microsoft.Extensions.DependencyInjection;

namespace BlogSite.Application.Dispatcher;

public class Dispatcher
{
    private readonly IServiceProvider _provider;

    public Dispatcher(IServiceProvider provider)
    {
        _provider = provider;
    }

    public async Task<object?> DispatchAsync(string rawType, JsonElement payload)
    {
        string suffix = rawType.StartsWith("Get", StringComparison.OrdinalIgnoreCase)
            ? "Query"
            : "Command";

        string fullTypeName = $"{rawType}{suffix}";

        var type = AppDomain.CurrentDomain
            .GetAssemblies()
            .SelectMany(a => a.GetTypes())
            .FirstOrDefault(t => t.Name.Equals(fullTypeName, StringComparison.Ordinal));

        if (type is null)
            throw new Exception($"Handler type '{fullTypeName}' not found");

        var instance = payload.Deserialize(type)!;

        var resultType = type.GetInterfaces()
            .First(i => i.IsGenericType && i.GetGenericTypeDefinition() == typeof(IRequest<>))
            .GetGenericArguments()[0];

        var handlerType = typeof(IRequestHandler<,>).MakeGenericType(type, resultType);

        dynamic handler = _provider.GetRequiredService(handlerType);
        return await handler.Handle((dynamic)instance, CancellationToken.None);
    }
}