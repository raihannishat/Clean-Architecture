namespace BlogSite.Application.Configuration;

/// <summary>
/// Configuration options for entity discovery
/// </summary>
public class EntityDiscoveryOptions
{
    public const string SectionName = "EntityDiscovery";
    
    /// <summary>
    /// List of fallback entities to use when no entities can be discovered dynamically
    /// </summary>
    public List<string> FallbackEntities { get; set; } = new();
}