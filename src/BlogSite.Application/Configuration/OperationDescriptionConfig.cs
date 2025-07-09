using System.Collections.Generic;

namespace BlogSite.Application.Configuration;

/// <summary>
/// Configuration for operation description generation
/// </summary>
public class OperationDescriptionConfig
{
    public bool EnableDynamicContext { get; set; } = true;
    public string DefaultLanguage { get; set; } = "en";
    public List<string> SupportedLanguages { get; set; } = new() { "en" };
    public Dictionary<string, string> ContextVariables { get; set; } = new();
    public Dictionary<string, DescriptionTemplateConfig> Templates { get; set; } = new();
    public Dictionary<string, FieldDescriptionConfig> FieldDescriptions { get; set; } = new();
    public Dictionary<string, PatternRuleConfig> QueryPatterns { get; set; } = new();
    public Dictionary<string, PatternRuleConfig> CommandPatterns { get; set; } = new();
    public FallbackTemplatesConfig FallbackTemplates { get; set; } = new();
    public DynamicRulesConfig DynamicRules { get; set; } = new();
    public Dictionary<string, List<string>> BusinessContextRules { get; set; } = new();
}

/// <summary>
/// Configuration for a description template
/// </summary>
public class DescriptionTemplateConfig
{
    public string EntityForm { get; set; } = "{entity}";
    public string Template { get; set; } = string.Empty;
    public string? TemplateWithContext { get; set; }
    public string? BusinessRuleTemplate { get; set; }
    public string? ConditionalTemplate { get; set; }
    public Dictionary<string, string> Localized { get; set; } = new();
    public int Priority { get; set; } = 0;
    public List<string> RequiredContext { get; set; } = new();
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
    public string ContextualQuery { get; set; } = "Executes {action} query on {entity}";
    public string ContextualCommand { get; set; } = "Executes {action} command on {entity}";
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

/// <summary>
/// Configuration for dynamic rules
/// </summary>
public class DynamicRulesConfig
{
    public TimeBasedTemplatesConfig TimeBasedTemplates { get; set; } = new();
    public UserBasedTemplatesConfig UserBasedTemplates { get; set; } = new();
}

/// <summary>
/// Configuration for time-based template modifications
/// </summary>
public class TimeBasedTemplatesConfig
{
    public bool Enabled { get; set; } = false;
    public List<TimeBasedRuleConfig> Rules { get; set; } = new();
}

/// <summary>
/// Configuration for user-based template modifications
/// </summary>
public class UserBasedTemplatesConfig
{
    public bool Enabled { get; set; } = false;
    public List<UserBasedRuleConfig> Rules { get; set; } = new();
}

/// <summary>
/// Configuration for a time-based rule
/// </summary>
public class TimeBasedRuleConfig
{
    public string Condition { get; set; } = string.Empty;
    public string? TemplatePrefix { get; set; }
    public string? TemplateSuffix { get; set; }
    public string? TemplateReplacement { get; set; }
}

/// <summary>
/// Configuration for a user-based rule
/// </summary>
public class UserBasedRuleConfig
{
    public string Condition { get; set; } = string.Empty;
    public string? TemplatePrefix { get; set; }
    public string? TemplateSuffix { get; set; }
    public string? TemplateReplacement { get; set; }
}