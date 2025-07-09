using System.Collections.Generic;

namespace BlogSite.Application.Configuration;

/// <summary>
/// Configuration for operation description generation
/// </summary>
public class OperationDescriptionConfig
{
    public Dictionary<string, DescriptionTemplateConfig> Templates { get; set; } = new();
    public Dictionary<string, FieldDescriptionConfig> FieldDescriptions { get; set; } = new();
    public Dictionary<string, PatternRuleConfig> QueryPatterns { get; set; } = new();
    public Dictionary<string, PatternRuleConfig> CommandPatterns { get; set; } = new();
    public FallbackTemplatesConfig FallbackTemplates { get; set; } = new();
}

/// <summary>
/// Configuration for a description template
/// </summary>
public class DescriptionTemplateConfig
{
    public string EntityForm { get; set; } = "{entity}";
    public string Template { get; set; } = string.Empty;
}

/// <summary>
/// Configuration for field-specific descriptions (used in GetBy patterns)
/// </summary>
public class FieldDescriptionConfig
{
    public string Template { get; set; } = string.Empty;
    public bool UsePlural { get; set; } = false;
    public string? CustomDescription { get; set; }
}

/// <summary>
/// Configuration for pattern-based rules
/// </summary>
public class PatternRuleConfig
{
    public string Pattern { get; set; } = string.Empty;
    public string Template { get; set; } = string.Empty;
    public bool UsePlural { get; set; } = false;
    public PatternMatchType MatchType { get; set; } = PatternMatchType.Contains;
}

/// <summary>
/// Configuration for fallback templates
/// </summary>
public class FallbackTemplatesConfig
{
    public string DefaultQuery { get; set; } = "Executes {action} query on {entity}";
    public string DefaultCommand { get; set; } = "Executes {action} command on {entity}";
    public string DefaultOperation { get; set; } = "Executes {action} {operationType} on {entity}";
    public string GetByDefault { get; set; } = "Gets a {entity} by {field}";
}

/// <summary>
/// How to match patterns
/// </summary>
public enum PatternMatchType
{
    Contains,
    StartsWith,
    EndsWith,
    Exact,
    Regex
}