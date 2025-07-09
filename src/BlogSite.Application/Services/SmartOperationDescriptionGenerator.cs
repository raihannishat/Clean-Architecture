using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Microsoft.Extensions.Options;
using BlogSite.Application.Attributes;
using BlogSite.Application.Configuration;

namespace BlogSite.Application.Services;

/// <summary>
/// Smart operation description generator that uses attributes, templates, and conventions
/// </summary>
public class SmartOperationDescriptionGenerator : IOperationDescriptionGenerator
{
    private readonly IPluralizationService _pluralizationService;
    private readonly IPatternMatcher _patternMatcher;
    private readonly OperationDescriptionConfig _config;
    
    public SmartOperationDescriptionGenerator(
        IPluralizationService pluralizationService,
        IPatternMatcher patternMatcher,
        IOptions<OperationDescriptionConfig> config)
    {
        _pluralizationService = pluralizationService;
        _patternMatcher = patternMatcher;
        _config = config.Value;
    }
    
    public string GetDescription(Type requestType, string operationType, string entityType, string action)
    {
        // 1. Check for explicit attribute first
        var attr = requestType.GetCustomAttribute<OperationDescriptionAttribute>();
        if (attr != null && !string.IsNullOrEmpty(attr.Description))
            return attr.Description;
            
        // 2. Generate from convention
        return GenerateFromConvention(operationType, entityType, action);
    }
    
    public string GetShortDescription(Type requestType, string operationType, string entityType, string action)
    {
        // 1. Check for explicit short description in attribute
        var attr = requestType.GetCustomAttribute<OperationDescriptionAttribute>();
        if (attr != null && !string.IsNullOrEmpty(attr.ShortDescription))
            return attr.ShortDescription;
            
        // 2. Use regular description as fallback
        return GetDescription(requestType, operationType, entityType, action);
    }
    
    public string GenerateFromConvention(string operationType, string entityType, string action)
    {
        var key = $"{operationType.ToLower()}.{action.ToLower()}";
        
        // Try exact template match first
        if (_config.Templates.TryGetValue(key, out var template))
        {
            return ApplyTemplate(template, entityType);
        }
        
        // Smart pattern matching for GetBy* queries
        if (action.StartsWith("GetBy", StringComparison.OrdinalIgnoreCase))
        {
            return HandleGetByPattern(action, entityType);
        }
        
        // Pattern matching for other common patterns
        if (operationType.Equals("Query", StringComparison.OrdinalIgnoreCase))
        {
            return HandleQueryPatterns(action, entityType);
        }
        
        if (operationType.Equals("Command", StringComparison.OrdinalIgnoreCase))
        {
            return HandleCommandPatterns(action, entityType);
        }
        
        // Default fallback using configured template
        var fallbackTemplate = _config.FallbackTemplates.DefaultOperation;
        return _patternMatcher.ApplyTemplate(fallbackTemplate, entityType, _pluralizationService)
            .Replace("{action}", action)
            .Replace("{operationType}", operationType.ToLower());
    }
    
    private string HandleGetByPattern(string action, string entityType)
    {
        var field = _patternMatcher.ExtractFieldFromGetBy(action);
        if (string.IsNullOrEmpty(field))
            return _config.FallbackTemplates.GetByDefault.Replace("{entity}", entityType.ToLower()).Replace("{field}", "unknown");
        
        var fieldLower = field.ToLower();
        
        // Check if we have a specific field description configured
        if (_config.FieldDescriptions.TryGetValue(fieldLower, out var fieldConfig))
        {
            return _patternMatcher.ApplyTemplate(fieldConfig.Template, entityType, _pluralizationService, fieldConfig.UsePlural, field);
        }
        
        // Use default GetBy template
        return _patternMatcher.ApplyTemplate(_config.FallbackTemplates.GetByDefault, entityType, _pluralizationService, false, field);
    }
    
    private string HandleQueryPatterns(string action, string entityType)
    {
        // Check each configured query pattern
        foreach (var (key, pattern) in _config.QueryPatterns)
        {
            if (_patternMatcher.IsMatch(action, pattern))
            {
                return _patternMatcher.ApplyTemplate(pattern.Template, entityType, _pluralizationService, pattern.UsePlural);
            }
        }
        
        // Use default query fallback
        return _patternMatcher.ApplyTemplate(_config.FallbackTemplates.DefaultQuery, entityType, _pluralizationService)
            .Replace("{action}", action);
    }
    
    private string HandleCommandPatterns(string action, string entityType)
    {
        // Check each configured command pattern
        foreach (var (key, pattern) in _config.CommandPatterns)
        {
            if (_patternMatcher.IsMatch(action, pattern))
            {
                return _patternMatcher.ApplyTemplate(pattern.Template, entityType, _pluralizationService, pattern.UsePlural);
            }
        }
        
        // Use default command fallback
        return _patternMatcher.ApplyTemplate(_config.FallbackTemplates.DefaultCommand, entityType, _pluralizationService)
            .Replace("{action}", action);
    }
    
    private string ApplyTemplate(DescriptionTemplateConfig template, string entityType)
    {
        var entityValue = template.EntityForm switch
        {
            "{entity}" => entityType.ToLower(),
            "{entities}" => _pluralizationService.Pluralize(entityType.ToLower()),
            _ => entityType.ToLower()
        };
        
        return template.Template.Replace(template.EntityForm, entityValue);
    }
}

