using System.Reflection;
using BlogSite.Domain.Entities;

namespace BlogSite.Application.Services;

public interface IEntityDiscoveryService
{
    IEnumerable<Type> GetEntityTypes();
    IEnumerable<string> GetEntityNames();
    bool IsValidEntity(string entityName);
    string GetProperEntityName(string entityName);
}

public class EntityDiscoveryService : IEntityDiscoveryService
{
    private readonly IPluralizationService _pluralizationService;
    private readonly Lazy<IEnumerable<Type>> _entityTypes;
    private readonly Lazy<Dictionary<string, string>> _entityNameMappings;

    public EntityDiscoveryService(IPluralizationService pluralizationService)
    {
        _pluralizationService = pluralizationService;
        _entityTypes = new Lazy<IEnumerable<Type>>(DiscoverEntityTypes);
        _entityNameMappings = new Lazy<Dictionary<string, string>>(BuildEntityNameMappings);
    }

    public IEnumerable<Type> GetEntityTypes()
    {
        return _entityTypes.Value;
    }

    public IEnumerable<string> GetEntityNames()
    {
        return _entityTypes.Value.Select(t => t.Name);
    }

    public bool IsValidEntity(string entityName)
    {
        if (string.IsNullOrEmpty(entityName))
            return false;

        var lower = entityName.ToLowerInvariant();
        return _entityNameMappings.Value.ContainsKey(lower);
    }

    public string GetProperEntityName(string entityName)
    {
        if (string.IsNullOrEmpty(entityName))
            return entityName;

        var lower = entityName.ToLowerInvariant();
        
        // First check if it's a direct entity name match
        if (_entityNameMappings.Value.TryGetValue(lower, out var properName))
        {
            return properName;
        }

        // If not found, try to singularize and check again (in case it's plural)
        var singular = _pluralizationService.Singularize(lower);
        if (_entityNameMappings.Value.TryGetValue(singular, out var singularProperName))
        {
            return singularProperName;
        }

        // Fallback: capitalize first letter
        return char.ToUpperInvariant(entityName[0]) + entityName[1..].ToLowerInvariant();
    }

    private IEnumerable<Type> DiscoverEntityTypes()
    {
        // Get all types that inherit from BaseEntity
        return AppDomain.CurrentDomain
            .GetAssemblies()
            .Where(a => !a.IsDynamic)
            .SelectMany(a =>
            {
                try
                {
                    return a.GetTypes();
                }
                catch (ReflectionTypeLoadException ex)
                {
                    // Handle assemblies that fail to load all types
                    return ex.Types.Where(t => t != null)!;
                }
            })
            .Where(t => t != null && 
                       t.IsClass && 
                       !t.IsAbstract && 
                       t.IsSubclassOf(typeof(BaseEntity)))
            .ToList();
    }

    private Dictionary<string, string> BuildEntityNameMappings()
    {
        var mappings = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase);

        foreach (var entityType in _entityTypes.Value)
        {
            var entityName = entityType.Name;
            var lowerName = entityName.ToLowerInvariant();
            
            // Add the entity name itself
            mappings[lowerName] = entityName;
            
            // Add plural forms
            var plural = _pluralizationService.Pluralize(lowerName);
            mappings[plural] = entityName;
        }

        return mappings;
    }
}