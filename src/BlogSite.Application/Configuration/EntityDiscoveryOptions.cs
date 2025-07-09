namespace BlogSite.Application.Configuration;

/// <summary>
/// Configuration options for entity discovery.
/// The system now supports multiple dynamic discovery strategies and this configuration
/// is only used as a last resort fallback when all automatic discovery methods fail.
/// </summary>
public class EntityDiscoveryOptions
{
    public const string SectionName = "EntityDiscovery";
    
    /// <summary>
    /// Optional list of fallback entities to use ONLY when no entities can be discovered dynamically.
    /// 
    /// The system will automatically discover entities through:
    /// 1. Reflection-based discovery (scanning for classes inheriting from BaseEntity)
    /// 2. Pattern analysis of existing Command/Query types
    /// 3. DbContext/DbSet analysis
    /// 4. Runtime registration of new entities
    /// 
    /// This fallback list is only used if ALL automatic discovery methods fail.
    /// In most cases, this can be left empty or removed entirely.
    /// </summary>
    public List<string> FallbackEntities { get; set; } = new();
    
    /// <summary>
    /// Enable or disable automatic entity discovery from request types.
    /// Default: true
    /// </summary>
    public bool EnableRequestTypeDiscovery { get; set; } = true;
    
    /// <summary>
    /// Enable or disable automatic entity discovery from DbContext types.
    /// Default: true
    /// </summary>
    public bool EnableDbContextDiscovery { get; set; } = true;
    
    /// <summary>
    /// Enable or disable reflection-based entity discovery.
    /// Default: true
    /// </summary>
    public bool EnableReflectionDiscovery { get; set; } = true;
    
    /// <summary>
    /// Cache discovered entities for improved performance.
    /// Default: true
    /// </summary>
    public bool EnableEntityCaching { get; set; } = true;
}