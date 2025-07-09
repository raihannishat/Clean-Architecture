using System.Reflection;
using System.Collections.Concurrent;
using System.Text.Json;
using Microsoft.Extensions.Options;
using BlogSite.Domain.Entities;
using BlogSite.Application.Configuration;

namespace BlogSite.Application.Services;

public interface IEntityDiscoveryService
{
    IEnumerable<Type> GetEntityTypes();
    IEnumerable<string> GetEntityNames();
    bool IsValidEntity(string entityName);
    string GetProperEntityName(string entityName);
    void RegisterEntity(string entityName);
    void RegisterEntityType(Type entityType);
    Task<IEnumerable<string>> DiscoverEntitiesFromRequestTypesAsync();
}

public class EntityDiscoveryService : IEntityDiscoveryService
{
    private readonly IPluralizationService _pluralizationService;
    private readonly EntityDiscoveryOptions _options;
    private readonly Lazy<IEnumerable<Type>> _reflectionBasedEntityTypes;
    private readonly Lazy<Dictionary<string, string>> _entityNameMappings;
    private readonly ConcurrentDictionary<string, string> _runtimeDiscoveredEntities;
    private readonly ConcurrentHashSet<Type> _registeredEntityTypes;
    private readonly object _discoveryLock = new();

    public EntityDiscoveryService(IPluralizationService pluralizationService, IOptions<EntityDiscoveryOptions>? options = null)
    {
        _pluralizationService = pluralizationService;
        _options = options?.Value ?? new EntityDiscoveryOptions();
        
        _reflectionBasedEntityTypes = new Lazy<IEnumerable<Type>>(DiscoverEntityTypesFromReflection);
        _entityNameMappings = new Lazy<Dictionary<string, string>>(BuildEntityNameMappings);
        
        // Use caching if enabled, otherwise use regular dictionaries
        _runtimeDiscoveredEntities = _options.EnableEntityCaching 
            ? new ConcurrentDictionary<string, string>(StringComparer.OrdinalIgnoreCase)
            : new ConcurrentDictionary<string, string>(StringComparer.OrdinalIgnoreCase);
            
        _registeredEntityTypes = new ConcurrentHashSet<Type>();
        
        // Auto-discover entities from request types on initialization if enabled
        if (_options.EnableRequestTypeDiscovery)
        {
            _ = Task.Run(async () => await DiscoverEntitiesFromRequestTypesAsync());
        }
    }

    public IEnumerable<Type> GetEntityTypes()
    {
        var types = new HashSet<Type>(_reflectionBasedEntityTypes.Value);
        types.UnionWith(_registeredEntityTypes);
        return types;
    }

    public IEnumerable<string> GetEntityNames()
    {
        var names = new HashSet<string>(StringComparer.OrdinalIgnoreCase);
        
        // Add reflection-based entity names
        names.UnionWith(_reflectionBasedEntityTypes.Value.Select(t => t.Name));
        
        // Add registered entity type names
        names.UnionWith(_registeredEntityTypes.Select(t => t.Name));
        
        // Add runtime discovered entity names
        names.UnionWith(_runtimeDiscoveredEntities.Values);
        
        return names;
    }

    public bool IsValidEntity(string entityName)
    {
        if (string.IsNullOrEmpty(entityName))
            return false;

        var lower = entityName.ToLowerInvariant();
        
        // Check in compiled mappings
        if (_entityNameMappings.Value.ContainsKey(lower))
            return true;
            
        // Check in runtime discovered entities
        if (_runtimeDiscoveredEntities.ContainsKey(lower))
            return true;
            
        // Try plural/singular variations
        var singular = _pluralizationService.Singularize(lower);
        var plural = _pluralizationService.Pluralize(lower);
        
        return _entityNameMappings.Value.ContainsKey(singular) ||
               _entityNameMappings.Value.ContainsKey(plural) ||
               _runtimeDiscoveredEntities.ContainsKey(singular) ||
               _runtimeDiscoveredEntities.ContainsKey(plural);
    }

    public string GetProperEntityName(string entityName)
    {
        if (string.IsNullOrEmpty(entityName))
            return entityName;

        var lower = entityName.ToLowerInvariant();
        
        // First check compiled mappings
        if (_entityNameMappings.Value.TryGetValue(lower, out var properName))
            return properName;
            
        // Check runtime discovered entities
        if (_runtimeDiscoveredEntities.TryGetValue(lower, out var runtimeName))
            return runtimeName;

        // Try singular/plural variations
        var singular = _pluralizationService.Singularize(lower);
        if (_entityNameMappings.Value.TryGetValue(singular, out var singularProperName))
            return singularProperName;
            
        if (_runtimeDiscoveredEntities.TryGetValue(singular, out var runtimeSingularName))
            return runtimeSingularName;

        var plural = _pluralizationService.Pluralize(lower);
        if (_entityNameMappings.Value.TryGetValue(plural, out var pluralProperName))
            return pluralProperName;
            
        if (_runtimeDiscoveredEntities.TryGetValue(plural, out var runtimePluralName))
            return runtimePluralName;

        // Auto-register this entity if it looks valid
        var properCased = CapitalizeEntityName(entityName);
        RegisterEntity(properCased);
        
        return properCased;
    }

    public void RegisterEntity(string entityName)
    {
        if (string.IsNullOrEmpty(entityName))
            return;
            
        var properName = CapitalizeEntityName(entityName);
        var lowerName = properName.ToLowerInvariant();
        
        _runtimeDiscoveredEntities.TryAdd(lowerName, properName);
        
        // Also add plural form
        var plural = _pluralizationService.Pluralize(lowerName);
        _runtimeDiscoveredEntities.TryAdd(plural, properName);
    }

    public void RegisterEntityType(Type entityType)
    {
        if (entityType == null || !entityType.IsClass || entityType.IsAbstract)
            return;
            
        _registeredEntityTypes.Add(entityType);
        RegisterEntity(entityType.Name);
    }

    public async Task<IEnumerable<string>> DiscoverEntitiesFromRequestTypesAsync()
    {
        return await Task.Run(() =>
        {
            var discoveredEntities = new HashSet<string>(StringComparer.OrdinalIgnoreCase);
            
            if (!_options.EnableRequestTypeDiscovery)
                return discoveredEntities;
            
            try
            {
                var assemblies = GetRelevantAssemblies();
                
                // Discover from request types
                var requestTypes = GetRequestTypes(assemblies);
                foreach (var requestType in requestTypes)
                {
                    var entityName = ExtractEntityFromRequestType(requestType);
                    if (!string.IsNullOrEmpty(entityName))
                    {
                        discoveredEntities.Add(entityName);
                        RegisterEntity(entityName);
                    }
                }
                
                // Also discover from DbContext or similar patterns if enabled
                if (_options.EnableDbContextDiscovery)
                {
                    DiscoverFromDataContexts(assemblies, discoveredEntities);
                }
            }
            catch (Exception)
            {
                // Gracefully handle discovery failures
            }
            
            return discoveredEntities;
        });
    }

    private IEnumerable<Type> DiscoverEntityTypesFromReflection()
    {
        var entityTypes = new List<Type>();
        
        if (!_options.EnableReflectionDiscovery)
            return entityTypes;
        
        try
        {
            var assemblies = GetRelevantAssemblies();
            
            foreach (var assembly in assemblies)
            {
                try
                {
                    var types = assembly.GetTypes()
                        .Where(t => t != null && 
                                   t.IsClass && 
                                   !t.IsAbstract && 
                                   IsEntityType(t))
                        .ToList();
                        
                    entityTypes.AddRange(types);
                }
                catch (ReflectionTypeLoadException ex)
                {
                    // Handle assemblies that fail to load all types
                    var loadedTypes = ex.Types.Where(t => t != null && IsEntityType(t));
                    entityTypes.AddRange(loadedTypes!);
                }
            }
        }
        catch (Exception)
        {
            // Gracefully handle reflection failures
        }
        
        return entityTypes;
    }

    private Assembly[] GetRelevantAssemblies()
    {
        var assemblies = new HashSet<Assembly>();
        
        try
        {
            // Add current domain assemblies
            assemblies.UnionWith(AppDomain.CurrentDomain.GetAssemblies()
                .Where(a => !a.IsDynamic && !IsSystemAssembly(a)));
                
            // Add specific assemblies by name patterns
            var domainAssemblies = AppDomain.CurrentDomain.GetAssemblies()
                .Where(a => a.FullName?.Contains("BlogSite") == true ||
                           a.FullName?.Contains("Domain") == true ||
                           a.FullName?.Contains("Application") == true ||
                           a.FullName?.Contains("Infrastructure") == true)
                .ToArray();
                
            assemblies.UnionWith(domainAssemblies);
        }
        catch (Exception)
        {
            // Fallback to minimal assembly set
            assemblies.Add(Assembly.GetExecutingAssembly());
        }
        
        return assemblies.ToArray();
    }

    private static bool IsSystemAssembly(Assembly assembly)
    {
        var name = assembly.FullName ?? "";
        return name.StartsWith("System.") ||
               name.StartsWith("Microsoft.") ||
               name.StartsWith("netstandard") ||
               name.StartsWith("mscorlib");
    }

    private static bool IsEntityType(Type type)
    {
        // Check if inherits from BaseEntity
        if (typeof(BaseEntity).IsAssignableFrom(type))
            return true;
            
        // Check for common entity patterns
        if (type.Name.EndsWith("Entity") || 
            type.Name.EndsWith("Model") ||
            type.Namespace?.Contains("Entities") == true ||
            type.Namespace?.Contains("Models") == true)
            return true;
            
        // Check for properties that suggest an entity (Id, CreatedAt, etc.)
        var properties = type.GetProperties();
        var hasIdProperty = properties.Any(p => p.Name.Equals("Id", StringComparison.OrdinalIgnoreCase));
        var hasTimestampProperties = properties.Any(p => 
            p.Name.Contains("Created") || 
            p.Name.Contains("Updated") || 
            p.Name.Contains("Modified"));
            
        return hasIdProperty && hasTimestampProperties;
    }

    private IEnumerable<Type> GetRequestTypes(Assembly[] assemblies)
    {
        var requestTypes = new List<Type>();
        
        foreach (var assembly in assemblies)
        {
            try
            {
                var types = assembly.GetTypes()
                    .Where(t => t.IsClass && 
                               !t.IsAbstract && 
                               (t.Name.EndsWith("Command") || 
                                t.Name.EndsWith("Query") ||
                                t.Name.EndsWith("Request")))
                    .ToList();
                    
                requestTypes.AddRange(types);
            }
            catch (Exception)
            {
                // Continue with other assemblies
            }
        }
        
        return requestTypes;
    }

    private string? ExtractEntityFromRequestType(Type requestType)
    {
        var typeName = requestType.Name;
        
        // Remove common suffixes
        var suffixes = new[] { "Command", "Query", "Request" };
        foreach (var suffix in suffixes)
        {
            if (typeName.EndsWith(suffix))
            {
                typeName = typeName[..^suffix.Length];
                break;
            }
        }
        
        // Extract entity using camel case splitting
        var words = SplitCamelCase(typeName);
        
        // Entity is typically the last word or two
        for (int i = words.Count - 1; i >= Math.Max(0, words.Count - 2); i--)
        {
            var potentialEntity = string.Join("", words.Skip(i));
            if (potentialEntity.Length > 2 && char.IsUpper(potentialEntity[0]))
            {
                return potentialEntity;
            }
        }
        
        return null;
    }

    private void DiscoverFromDataContexts(Assembly[] assemblies, HashSet<string> discoveredEntities)
    {
        foreach (var assembly in assemblies)
        {
            try
            {
                var contextTypes = assembly.GetTypes()
                    .Where(t => t.Name.Contains("Context") || t.Name.Contains("DbContext"))
                    .ToList();
                    
                foreach (var contextType in contextTypes)
                {
                    var dbSetProperties = contextType.GetProperties()
                        .Where(p => p.PropertyType.IsGenericType && 
                                   p.PropertyType.Name.StartsWith("DbSet"))
                        .ToList();
                        
                    foreach (var dbSetProperty in dbSetProperties)
                    {
                        var entityType = dbSetProperty.PropertyType.GetGenericArguments().FirstOrDefault();
                        if (entityType != null)
                        {
                            discoveredEntities.Add(entityType.Name);
                            RegisterEntity(entityType.Name);
                        }
                    }
                }
            }
            catch (Exception)
            {
                // Continue with discovery
            }
        }
    }

    private Dictionary<string, string> BuildEntityNameMappings()
    {
        var mappings = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase);

        foreach (var entityType in _reflectionBasedEntityTypes.Value)
        {
            var entityName = entityType.Name;
            var lowerName = entityName.ToLowerInvariant();
            
            mappings[lowerName] = entityName;
            
            var plural = _pluralizationService.Pluralize(lowerName);
            mappings[plural] = entityName;
        }

        return mappings;
    }

    private static List<string> SplitCamelCase(string input)
    {
        var words = new List<string>();
        var currentWord = new System.Text.StringBuilder();

        for (int i = 0; i < input.Length; i++)
        {
            char c = input[i];
            
            if (char.IsUpper(c) && currentWord.Length > 0)
            {
                words.Add(currentWord.ToString());
                currentWord.Clear();
            }
            
            currentWord.Append(c);
        }

        if (currentWord.Length > 0)
        {
            words.Add(currentWord.ToString());
        }

        return words;
    }

    private static string CapitalizeEntityName(string entityName)
    {
        if (string.IsNullOrEmpty(entityName))
            return entityName;
            
        return char.ToUpperInvariant(entityName[0]) + entityName[1..].ToLowerInvariant();
    }
}

// Helper class for thread-safe HashSet
public class ConcurrentHashSet<T> : IDisposable
{
    private readonly HashSet<T> _hashSet = new();
    private readonly ReaderWriterLockSlim _lock = new();

    public bool Add(T item)
    {
        _lock.EnterWriteLock();
        try
        {
            return _hashSet.Add(item);
        }
        finally
        {
            _lock.ExitWriteLock();
        }
    }

    public bool Contains(T item)
    {
        _lock.EnterReadLock();
        try
        {
            return _hashSet.Contains(item);
        }
        finally
        {
            _lock.ExitReadLock();
        }
    }

    public IEnumerator<T> GetEnumerator()
    {
        _lock.EnterReadLock();
        try
        {
            return new List<T>(_hashSet).GetEnumerator();
        }
        finally
        {
            _lock.ExitReadLock();
        }
    }

    public void Dispose()
    {
        _lock?.Dispose();
    }
}