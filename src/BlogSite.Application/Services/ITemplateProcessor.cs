using System.Collections.Generic;
using BlogSite.Application.Configuration;

namespace BlogSite.Application.Services;

/// <summary>
/// Interface for processing templates with dynamic context and conditional logic
/// </summary>
public interface ITemplateProcessor
{
    /// <summary>
    /// Processes a template with the given context
    /// </summary>
    string ProcessTemplate(string template, TemplateContext context);
    
    /// <summary>
    /// Processes a template configuration with the given context
    /// </summary>
    string ProcessTemplate(DescriptionTemplateConfig templateConfig, TemplateContext context);
    
    /// <summary>
    /// Processes conditional templates
    /// </summary>
    string ProcessConditionalTemplate(string template, TemplateContext context);
    
    /// <summary>
    /// Gets the best template from configuration based on context
    /// </summary>
    string GetBestTemplate(DescriptionTemplateConfig templateConfig, TemplateContext context);
}

/// <summary>
/// Context information for template processing
/// </summary>
public class TemplateContext
{
    public string EntityType { get; set; } = string.Empty;
    public string Action { get; set; } = string.Empty;
    public string OperationType { get; set; } = string.Empty;
    public string? Field { get; set; }
    public string Language { get; set; } = "en";
    public bool UsePlural { get; set; }
    public IOperationContext OperationContext { get; set; } = null!;
    public BusinessRuleContext BusinessRules { get; set; } = new();
    public Dictionary<string, object> AdditionalContext { get; set; } = new();
}