using System.Text.Json;
using MediatR;
using System.Text.RegularExpressions;

namespace BlogSite.Application.Dispatcher;

public class Dispatcher
{
    private readonly IServiceProvider _provider;
    private readonly string[] _knownEntities = { "Author", "BlogPost", "Category", "Comment" };
    private readonly Dictionary<string, string> _entityPlurals = new()
    {
        { "author", "authors" },
        { "blogpost", "blogposts" },
        { "category", "categories" },
        { "comment", "comments" }
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

        // Parse action dynamically
        string className = ParseActionDynamically(normalizedAction, suffix);

        var type = FindTypeByName(className);
        if (type is null)
            throw new Exception($"Handler type '{className}' not found for action '{action}'");

        var instance = payload.Deserialize(type)!;

        var resultType = type.GetInterfaces()
            .First(i => i.IsGenericType && i.GetGenericTypeDefinition() == typeof(IRequest<>))
            .GetGenericArguments()[0];

        var handlerType = typeof(IRequestHandler<,>).MakeGenericType(type, resultType);

        dynamic handler = _provider.GetRequiredService(handlerType);
        return await handler.Handle((dynamic)instance, CancellationToken.None);
    }

    private string ParseActionDynamically(string normalizedAction, string suffix)
    {
        // Handle special patterns first
        if (HandleSpecialPatterns(normalizedAction, suffix, out string? specialResult))
        {
            return specialResult;
        }

        // Handle "get by entity" patterns (e.g., "getbyauthor")
        if (normalizedAction.StartsWith("getby"))
        {
            var entityPart = normalizedAction.Substring(5); // Remove "getby"
            return HandleGetByEntityPattern(entityPart, suffix);
        }

        // Handle "get entity by field" patterns (e.g., "getauthorbyid", "getauthorbyemail")
        if (normalizedAction.StartsWith("get") && normalizedAction.Contains("by"))
        {
            return HandleGetEntityByFieldPattern(normalizedAction, suffix);
        }

        // Handle "get all entities" patterns (e.g., "getallauthors", "getallcategories")
        if (normalizedAction.StartsWith("getall"))
        {
            var entityPart = normalizedAction.Substring(6); // Remove "getall"
            var entityName = GetEntityNameFromPlural(entityPart);
            return $"GetAll{ToPascalCase(entityName)}{suffix}";
        }

        // Handle "get published/specific entity" patterns (e.g., "getpublishedblogposts")
        if (normalizedAction.StartsWith("get") && !normalizedAction.Contains("by"))
        {
            return HandleGetSpecificPattern(normalizedAction, suffix);
        }

        // Handle command patterns (create, update, delete, publish)
        if (normalizedAction.StartsWith("create") || 
            normalizedAction.StartsWith("update") || 
            normalizedAction.StartsWith("delete") ||
            normalizedAction.StartsWith("publish"))
        {
            return HandleCommandPattern(normalizedAction, suffix);
        }

        // Fallback: convert entire action to PascalCase
        return $"{ToPascalCase(normalizedAction)}{suffix}";
    }

    private bool HandleSpecialPatterns(string normalizedAction, string suffix, out string? result)
    {
        result = null;

        // Special cases that don't follow standard patterns
        var specialMappings = new Dictionary<string, string>
        {
            { "getbyauthor", "GetBlogPostsByAuthor" },
        };

        if (specialMappings.TryGetValue(normalizedAction, out var mapping))
        {
            result = $"{mapping}{suffix}";
            return true;
        }

        return false;
    }

    private string HandleGetByEntityPattern(string entityPart, string suffix)
    {
        // For patterns like "getbyauthor" -> "GetBlogPostsByAuthor"
        var entityName = ToPascalCase(entityPart);
        
        // Determine what entity type we're getting based on the "by" entity
        string targetEntity = entityPart.ToLower() switch
        {
            "author" => "BlogPosts",
            "category" => "BlogPosts", 
            _ => entityName
        };

        return $"Get{targetEntity}By{entityName}{suffix}";
    }

    private string HandleGetEntityByFieldPattern(string normalizedAction, string suffix)
    {
        // For patterns like "getauthorbyid" -> "GetAuthorById"
        var parts = normalizedAction.Split("by", 2);
        if (parts.Length == 2)
        {
            var entityPart = parts[0].Substring(3); // Remove "get"
            var fieldPart = parts[1];
            
            var entityName = GetEntityNameFromSingularOrPlural(entityPart);
            var fieldName = ToPascalCase(fieldPart);
            
            return $"Get{ToPascalCase(entityName)}By{fieldName}{suffix}";
        }

        return $"{ToPascalCase(normalizedAction)}{suffix}";
    }

    private string HandleGetSpecificPattern(string normalizedAction, string suffix)
    {
        // For patterns like "getpublishedblogposts" -> "GetPublishedBlogPosts"
        var actionPart = normalizedAction.Substring(3); // Remove "get"
        
        // Check for known patterns
        if (actionPart.Contains("published"))
        {
            var entityPart = actionPart.Replace("published", "");
            var entityName = GetEntityNameFromPlural(entityPart);
            return $"GetPublished{ToPascalCase(entityName)}{suffix}";
        }

        // Default handling
        var parsed = ParseCamelCaseWords(actionPart);
        return $"Get{parsed}{suffix}";
    }

    private string HandleCommandPattern(string normalizedAction, string suffix)
    {
        // For patterns like "createauthor" -> "CreateAuthor", "publishblogpost" -> "PublishBlogPost"
        string command = "";
        string entityPart = "";

        if (normalizedAction.StartsWith("create"))
        {
            command = "Create";
            entityPart = normalizedAction.Substring(6);
        }
        else if (normalizedAction.StartsWith("update"))
        {
            command = "Update";
            entityPart = normalizedAction.Substring(6);
        }
        else if (normalizedAction.StartsWith("delete"))
        {
            command = "Delete";
            entityPart = normalizedAction.Substring(6);
        }
        else if (normalizedAction.StartsWith("publish"))
        {
            command = "Publish";
            entityPart = normalizedAction.Substring(7);
        }

        var entityName = GetEntityNameFromSingularOrPlural(entityPart);
        return $"{command}{ToPascalCase(entityName)}{suffix}";
    }

    private string GetEntityNameFromPlural(string pluralForm)
    {
        var lower = pluralForm.ToLower();
        
        // Check if it's a known plural form
        var match = _entityPlurals.FirstOrDefault(x => x.Value == lower);
        if (match.Key != null)
        {
            return GetProperEntityName(match.Key);
        }

        // Try simple plural rules
        if (lower.EndsWith("ies"))
        {
            var singular = lower.Substring(0, lower.Length - 3) + "y";
            return GetProperEntityName(singular);
        }
        else if (lower.EndsWith("s"))
        {
            var singular = lower.Substring(0, lower.Length - 1);
            return GetProperEntityName(singular);
        }

        return GetProperEntityName(lower);
    }

    private string GetEntityNameFromSingularOrPlural(string entityPart)
    {
        var lower = entityPart.ToLower();
        
        // Check if it's already a known entity
        if (_knownEntities.Any(e => e.ToLower() == lower))
        {
            return GetProperEntityName(lower);
        }

        // Check if it might be plural
        return GetEntityNameFromPlural(entityPart);
    }

    private string GetProperEntityName(string entityName)
    {
        var lower = entityName.ToLower();
        return lower switch
        {
            "author" => "Author",
            "blogpost" => "BlogPost",
            "category" => "Category",
            "comment" => "Comment",
            _ => ToPascalCase(entityName)
        };
    }

    private string ParseCamelCaseWords(string input)
    {
        // Handle compound words like "publishedblogposts"
        var result = Regex.Replace(input, @"([a-z])([A-Z])", "$1 $2");
        return ToPascalCase(result);
    }

    private string ToPascalCase(string input)
    {
        if (string.IsNullOrEmpty(input))
            return input;

        // Handle space-separated words
        if (input.Contains(' '))
        {
            return string.Concat(input.Split(' ')
                .Select(word => char.ToUpperInvariant(word[0]) + word[1..].ToLowerInvariant()));
        }

        // Handle single word
        return char.ToUpperInvariant(input[0]) + input[1..].ToLowerInvariant();
    }

    private Type? FindTypeByName(string className)
    {
        return AppDomain.CurrentDomain
            .GetAssemblies()
            .SelectMany(a => a.GetTypes())
            .FirstOrDefault(t => t.Name.Equals(className, StringComparison.OrdinalIgnoreCase));
    }
}