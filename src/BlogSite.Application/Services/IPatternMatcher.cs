using BlogSite.Application.Configuration;

namespace BlogSite.Application.Services;

/// <summary>
/// Interface for pattern matching operations
/// </summary>
public interface IPatternMatcher
{
    /// <summary>
    /// Checks if an action matches a pattern rule
    /// </summary>
    bool IsMatch(string action, PatternRuleConfig rule);
    
    /// <summary>
    /// Applies a template with entity and field replacements
    /// </summary>
    string ApplyTemplate(string template, string entityType, IPluralizationService pluralizationService, bool usePlural = false, string? field = null);
    
    /// <summary>
    /// Extracts field name from GetBy* patterns
    /// </summary>
    string ExtractFieldFromGetBy(string action);
}