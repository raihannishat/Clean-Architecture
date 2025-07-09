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
    private readonly ITemplateProcessor _templateProcessor;
    private readonly IOperationContext _operationContext;
    private readonly OperationDescriptionConfig _config;
    
    public SmartOperationDescriptionGenerator(
        IPluralizationService pluralizationService,
        IPatternMatcher patternMatcher,
        ITemplateProcessor templateProcessor,
        IOperationContext operationContext,
        IOptions<OperationDescriptionConfig> config)
    {
        _pluralizationService = pluralizationService;
        _patternMatcher = patternMatcher;
        _templateProcessor = templateProcessor;
        _operationContext = operationContext;
        _config = config.Value;
    }
    
    public string GetDescription(Type requestType, string operationType, string entityType, string action)
    {
        // 1. Check for explicit attribute first
        var attr = requestType.GetCustomAttribute<OperationDescriptionAttribute>();
        if (attr != null && !string.IsNullOrEmpty(attr.Description))
            return attr.Description;
            
        // 2. Generate from convention with enhanced context
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
        var context = CreateTemplateContext(operationType, entityType, action);
        var key = $"{operationType.ToLower()}.{action.ToLower()}";
        
        // Try exact template match first
        if (_config.Templates.TryGetValue(key, out var templateConfig))
        {
            return _templateProcessor.ProcessTemplate(templateConfig, context);
        }
        
        // Smart pattern matching for GetBy* queries
        if (action.StartsWith("GetBy", StringComparison.OrdinalIgnoreCase))
        {
            return HandleGetByPattern(action, entityType, context);
        }
        
        // Pattern matching for other common patterns
        if (operationType.Equals("Query", StringComparison.OrdinalIgnoreCase))
        {
            return HandleQueryPatterns(action, entityType, context);
        }
        
        if (operationType.Equals("Command", StringComparison.OrdinalIgnoreCase))
        {
            return HandleCommandPatterns(action, entityType, context);
        }
        
        // Default fallback using configured template with context
        var fallbackTemplate = _config.EnableDynamicContext && _operationContext != null
            ? _config.FallbackTemplates.ContextualOperation ?? _config.FallbackTemplates.DefaultOperation
            : _config.FallbackTemplates.DefaultOperation;
            
        return _templateProcessor.ProcessTemplate(fallbackTemplate, context);
    }
    
    private string HandleGetByPattern(string action, string entityType, TemplateContext context)
    {
        var field = _patternMatcher.ExtractFieldFromGetBy(action);
        if (string.IsNullOrEmpty(field))
        {
            context.Field = "unknown";
            return _templateProcessor.ProcessTemplate(_config.FallbackTemplates.GetByDefault, context);
        }
        
        context.Field = field;
        var fieldLower = field.ToLower();
        
        // Check if we have a specific field description configured
        if (_config.FieldDescriptions.TryGetValue(fieldLower, out var fieldConfig))
        {
            context.UsePlural = fieldConfig.UsePlural;
            return _templateProcessor.ProcessTemplate(fieldConfig.Template, context);
        }
        
        // Use default GetBy template
        return _templateProcessor.ProcessTemplate(_config.FallbackTemplates.GetByDefault, context);
    }
    
    private string HandleQueryPatterns(string action, string entityType, TemplateContext context)
    {
        // Check each configured query pattern
        foreach (var (key, pattern) in _config.QueryPatterns)
        {
            if (_patternMatcher.IsMatch(action, pattern))
            {
                context.UsePlural = pattern.UsePlural;
                return _templateProcessor.ProcessTemplate(pattern.Template, context);
            }
        }
        
        // Use default query fallback with context awareness
        var fallbackTemplate = _config.EnableDynamicContext && _operationContext != null
            ? _config.FallbackTemplates.ContextualQuery ?? _config.FallbackTemplates.DefaultQuery
            : _config.FallbackTemplates.DefaultQuery;
            
        return _templateProcessor.ProcessTemplate(fallbackTemplate, context);
    }
    
    private string HandleCommandPatterns(string action, string entityType, TemplateContext context)
    {
        // Check each configured command pattern
        foreach (var (key, pattern) in _config.CommandPatterns)
        {
            if (_patternMatcher.IsMatch(action, pattern))
            {
                context.UsePlural = pattern.UsePlural;
                return _templateProcessor.ProcessTemplate(pattern.Template, context);
            }
        }
        
        // Use default command fallback with context awareness
        var fallbackTemplate = _config.EnableDynamicContext && _operationContext != null
            ? _config.FallbackTemplates.ContextualCommand ?? _config.FallbackTemplates.DefaultCommand
            : _config.FallbackTemplates.DefaultCommand;
            
        return _templateProcessor.ProcessTemplate(fallbackTemplate, context);
    }
    
    private TemplateContext CreateTemplateContext(string operationType, string entityType, string action)
    {
        var context = new TemplateContext
        {
            EntityType = entityType,
            Action = action,
            OperationType = operationType,
            Language = _config.DefaultLanguage,
            OperationContext = _operationContext
        };
        
        // Get business rule context if operation context is available
        if (_operationContext != null)
        {
            context.BusinessRules = _operationContext.GetBusinessRuleContext(entityType);
        }
        
        return context;
    }
}

