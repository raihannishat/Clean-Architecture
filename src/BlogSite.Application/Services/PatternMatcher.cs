using System;
using System.Text.RegularExpressions;
using BlogSite.Application.Configuration;

namespace BlogSite.Application.Services;

/// <summary>
/// Service for pattern matching and template application
/// </summary>
public class PatternMatcher : IPatternMatcher
{
    public bool IsMatch(string action, PatternRuleConfig rule)
    {
        var actionLower = action.ToLower();
        var patternLower = rule.Pattern.ToLower();
        
        return rule.MatchType switch
        {
            PatternMatchType.Contains => actionLower.Contains(patternLower),
            PatternMatchType.StartsWith => actionLower.StartsWith(patternLower),
            PatternMatchType.EndsWith => actionLower.EndsWith(patternLower),
            PatternMatchType.Exact => actionLower.Equals(patternLower),
            PatternMatchType.Regex => Regex.IsMatch(action, rule.Pattern, RegexOptions.IgnoreCase),
            _ => false
        };
    }
    
    public string ApplyTemplate(string template, string entityType, IPluralizationService pluralizationService, bool usePlural = false, string? field = null)
    {
        var result = template;
        
        // Replace entity placeholders
        var entityValue = usePlural 
            ? pluralizationService.Pluralize(entityType.ToLower())
            : entityType.ToLower();
            
        result = result.Replace("{entity}", entityType.ToLower());
        result = result.Replace("{entities}", pluralizationService.Pluralize(entityType.ToLower()));
        result = result.Replace("{entityType}", entityType.ToLower());
        
        // Replace field placeholder if provided
        if (!string.IsNullOrEmpty(field))
        {
            result = result.Replace("{field}", SplitCamelCase(field).ToLower());
            result = result.Replace("{fieldName}", field.ToLower());
        }
        
        return result;
    }
    
    public string ExtractFieldFromGetBy(string action)
    {
        if (!action.StartsWith("GetBy", StringComparison.OrdinalIgnoreCase) || action.Length <= 5)
            return string.Empty;
            
        return action[5..]; // Remove "GetBy" prefix
    }
    
    private string SplitCamelCase(string input)
    {
        if (string.IsNullOrEmpty(input))
            return string.Empty;
            
        return Regex.Replace(input, "([A-Z])", " $1").Trim();
    }
}