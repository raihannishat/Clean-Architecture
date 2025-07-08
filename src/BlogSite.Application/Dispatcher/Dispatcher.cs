using System.Text.Json;
using MediatR;

namespace BlogSite.Application.Dispatcher;

public class Dispatcher
{
    private readonly IServiceProvider _provider;
    private readonly string[] _knownEntities = { "Author", "BlogPost", "Category", "Comment" };
    private readonly Dictionary<string, string> _actionMappings = new()
    {
        // Query mappings
        { "getbyauthor", "GetBlogPostsByAuthor" },
        { "getauthorbyid", "GetAuthorById" },
        { "getauthorbyemail", "GetAuthorByEmail" },
        { "getallauthors", "GetAllAuthors" },
        { "getblogpostsbycategory", "GetBlogPostsByCategory" },
        { "getpublishedblogposts", "GetPublishedBlogPosts" },
        { "getallcategories", "GetAllCategories" },
        
        // Command mappings  
        { "createauthor", "CreateAuthor" },
        { "updateauthor", "UpdateAuthor" },
        { "deleteauthor", "DeleteAuthor" },
        { "createblogpost", "CreateBlogPost" },
        { "publishblogpost", "PublishBlogPost" },
        { "createcategory", "CreateCategory" }
    };

    public Dispatcher(IServiceProvider provider)
    {
        _provider = provider;
    }

    public async Task<object?> DispatchAsync(string action, JsonElement payload)
    {
        // Normalize action to lowercase for comparison
        var normalizedAction = action.ToLowerInvariant();
        
        // Determine operation type based on action prefix
        bool isQuery = normalizedAction.StartsWith("get");
        string suffix = isQuery ? "Query" : "Command";

        // Try to get mapped action name or parse dynamically
        string className = GetClassName(normalizedAction, suffix);

        var type = FindTypeByName(className);
        if (type is null)
            throw new Exception($"Handler type '{className}' not found. Available actions: {string.Join(", ", _actionMappings.Keys)}");

        var instance = payload.Deserialize(type)!;

        var resultType = type.GetInterfaces()
            .First(i => i.IsGenericType && i.GetGenericTypeDefinition() == typeof(IRequest<>))
            .GetGenericArguments()[0];

        var handlerType = typeof(IRequestHandler<,>).MakeGenericType(type, resultType);

        dynamic handler = _provider.GetRequiredService(handlerType);
        return await handler.Handle((dynamic)instance, CancellationToken.None);
    }

    private string GetClassName(string normalizedAction, string suffix)
    {
        // First try direct mapping
        if (_actionMappings.TryGetValue(normalizedAction, out var mappedName))
        {
            return $"{mappedName}{suffix}";
        }

        // If no direct mapping, try to parse dynamically
        return ParseActionDynamically(normalizedAction, suffix);
    }

    private string ParseActionDynamically(string normalizedAction, string suffix)
    {
        // Try to parse patterns like:
        // "getbyauthor" → "GetByAuthor" + suffix
        // "createauthor" → "CreateAuthor" + suffix
        // "getauthorbyid" → "GetAuthorById" + suffix

        // Convert to PascalCase
        var parts = normalizedAction.Split(new[] { "by" }, StringSplitOptions.RemoveEmptyEntries);
        
        if (parts.Length == 2)
        {
            // Handle patterns like "getbyauthor" or "getauthorbyid"
            var firstPart = ToPascalCase(parts[0]);
            var secondPart = ToPascalCase(parts[1]);
            
            // Check if second part is an entity
            if (_knownEntities.Any(e => e.Equals(secondPart, StringComparison.OrdinalIgnoreCase)))
            {
                return $"{firstPart}By{secondPart}{suffix}";
            }
            else
            {
                return $"{firstPart}{secondPart}{suffix}";
            }
        }
        
        // Handle simple patterns like "createauthor", "getallauthors"
        var actionName = ToPascalCase(normalizedAction);
        return $"{actionName}{suffix}";
    }

    private string ToPascalCase(string input)
    {
        if (string.IsNullOrEmpty(input))
            return input;

        // Split camelCase words and convert to PascalCase
        var result = string.Concat(input.Split(' ', '-', '_')
            .Select(word => char.ToUpperInvariant(word[0]) + word[1..].ToLowerInvariant()));
            
        return result;
    }

    private Type? FindTypeByName(string className)
    {
        return AppDomain.CurrentDomain
            .GetAssemblies()
            .SelectMany(a => a.GetTypes())
            .FirstOrDefault(t => t.Name.Equals(className, StringComparison.OrdinalIgnoreCase));
    }
}