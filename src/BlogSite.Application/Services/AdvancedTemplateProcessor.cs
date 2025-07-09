using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using Microsoft.Extensions.Options;
using BlogSite.Application.Configuration;

namespace BlogSite.Application.Services;

/// <summary>
/// Advanced template processor with support for conditional logic, localization, and dynamic context
/// </summary>
public class AdvancedTemplateProcessor : ITemplateProcessor
{
    private readonly IPluralizationService _pluralizationService;
    private readonly IExpressionEvaluator _expressionEvaluator;
    private readonly OperationDescriptionConfig _config;

    public AdvancedTemplateProcessor(
        IPluralizationService pluralizationService,
        IExpressionEvaluator expressionEvaluator,
        IOptions<OperationDescriptionConfig> config)
    {
        _pluralizationService = pluralizationService;
        _expressionEvaluator = expressionEvaluator;
        _config = config.Value;
    }

    public string ProcessTemplate(string template, TemplateContext context)
    {
        if (string.IsNullOrEmpty(template))
            return string.Empty;

        var result = template;

        // Process conditional expressions first
        result = ProcessConditionalTemplate(result, context);

        // Replace basic placeholders
        result = ReplaceBasicPlaceholders(result, context);

        // Replace context variables
        result = ReplaceContextVariables(result, context);

        // Apply dynamic rules
        result = ApplyDynamicRules(result, context);

        return result.Trim();
    }

    public string ProcessTemplate(DescriptionTemplateConfig templateConfig, TemplateContext context)
    {
        var bestTemplate = GetBestTemplate(templateConfig, context);
        return ProcessTemplate(bestTemplate, context);
    }

    public string ProcessConditionalTemplate(string template, TemplateContext context)
    {
        if (string.IsNullOrEmpty(template))
            return string.Empty;

        // Pattern for conditional expressions: {?condition ? trueValue : falseValue}
        var conditionalPattern = @"\{\?([^?]+)\?\s*([^:]+)\s*:\s*([^}]+)\}";
        
        return Regex.Replace(template, conditionalPattern, match =>
        {
            var condition = match.Groups[1].Value.Trim();
            var trueValue = match.Groups[2].Value.Trim().Trim('\'', '"');
            var falseValue = match.Groups[3].Value.Trim().Trim('\'', '"');

            var contextDict = BuildContextDictionary(context);
            var conditionResult = _expressionEvaluator.Evaluate(condition, contextDict);

            return conditionResult ? trueValue : falseValue;
        });
    }

    public string GetBestTemplate(DescriptionTemplateConfig templateConfig, TemplateContext context)
    {
        // Priority order: Localized -> BusinessRuleTemplate -> ConditionalTemplate -> TemplateWithContext -> Template

        // 1. Check for localized template
        if (templateConfig.Localized.TryGetValue(context.Language, out var localizedTemplate) &&
            !string.IsNullOrEmpty(localizedTemplate))
        {
            return localizedTemplate;
        }

        // 2. Check for business rule template if business rules are provided
        if (!string.IsNullOrEmpty(templateConfig.BusinessRuleTemplate) && HasBusinessRuleContext(context))
        {
            return templateConfig.BusinessRuleTemplate;
        }

        // 3. Check for conditional template
        if (!string.IsNullOrEmpty(templateConfig.ConditionalTemplate))
        {
            return templateConfig.ConditionalTemplate;
        }

        // 4. Check for context template if context is enabled
        if (_config.EnableDynamicContext && !string.IsNullOrEmpty(templateConfig.TemplateWithContext))
        {
            return templateConfig.TemplateWithContext;
        }

        // 5. Fallback to basic template
        return templateConfig.Template;
    }

    private string ReplaceBasicPlaceholders(string template, TemplateContext context)
    {
        var result = template;

        // Entity placeholders
        var entityValue = context.UsePlural 
            ? _pluralizationService.Pluralize(context.EntityType.ToLower())
            : context.EntityType.ToLower();

        result = result.Replace("{entity}", context.EntityType.ToLower());
        result = result.Replace("{entities}", _pluralizationService.Pluralize(context.EntityType.ToLower()));
        result = result.Replace("{entityType}", context.EntityType.ToLower());

        // Action and operation placeholders
        result = result.Replace("{action}", context.Action);
        result = result.Replace("{operationType}", context.OperationType.ToLower());

        // Field placeholder
        if (!string.IsNullOrEmpty(context.Field))
        {
            result = result.Replace("{field}", SplitCamelCase(context.Field).ToLower());
            result = result.Replace("{fieldName}", context.Field.ToLower());
        }

        return result;
    }

    private string ReplaceContextVariables(string template, TemplateContext context)
    {
        var result = template;

        // Replace configured context variables
        foreach (var (key, value) in _config.ContextVariables)
        {
            result = result.Replace($"{{{key}}}", value);
        }

        // Replace operation context variables
        if (context.OperationContext != null)
        {
            var userContext = context.OperationContext.UserContext;
            var timeContext = context.OperationContext.TimeContext;
            var envContext = context.OperationContext.EnvironmentContext;

            result = result.Replace("{UserName}", userContext.UserName ?? "Unknown");
            result = result.Replace("{UserRole}", userContext.UserRole ?? "Guest");
            result = result.Replace("{CurrentTime}", timeContext.CurrentTime.ToString("yyyy-MM-dd HH:mm"));
            result = result.Replace("{Environment}", envContext.Environment);
            result = result.Replace("{Version}", envContext.Version);
            result = result.Replace("{ApplicationName}", envContext.ApplicationName);
        }

        return result;
    }

    private string ApplyDynamicRules(string template, TemplateContext context)
    {
        if (!_config.EnableDynamicContext || context.OperationContext == null)
            return template;

        var result = template;

        // Apply time-based rules
        if (_config.DynamicRules.TimeBasedTemplates.Enabled)
        {
            foreach (var rule in _config.DynamicRules.TimeBasedTemplates.Rules)
            {
                if (context.OperationContext.EvaluateCondition(rule.Condition))
                {
                    if (!string.IsNullOrEmpty(rule.TemplatePrefix))
                        result = rule.TemplatePrefix + result;
                    if (!string.IsNullOrEmpty(rule.TemplateSuffix))
                        result = result + rule.TemplateSuffix;
                    if (!string.IsNullOrEmpty(rule.TemplateReplacement))
                        result = rule.TemplateReplacement;
                }
            }
        }

        // Apply user-based rules
        if (_config.DynamicRules.UserBasedTemplates.Enabled)
        {
            foreach (var rule in _config.DynamicRules.UserBasedTemplates.Rules)
            {
                if (context.OperationContext.EvaluateCondition(rule.Condition))
                {
                    if (!string.IsNullOrEmpty(rule.TemplatePrefix))
                        result = rule.TemplatePrefix + result;
                    if (!string.IsNullOrEmpty(rule.TemplateSuffix))
                        result = result + rule.TemplateSuffix;
                    if (!string.IsNullOrEmpty(rule.TemplateReplacement))
                        result = rule.TemplateReplacement;
                }
            }
        }

        return result;
    }

    private Dictionary<string, object> BuildContextDictionary(TemplateContext context)
    {
        var contextDict = new Dictionary<string, object>(context.AdditionalContext);

        // Add business rule context
        contextDict["RequiresApproval"] = context.BusinessRules.RequiresApproval;
        contextDict["IsValidationRequired"] = context.BusinessRules.IsValidationRequired;
        contextDict["IsCached"] = context.BusinessRules.IsCached;
        contextDict["RequiresTransaction"] = context.BusinessRules.RequiresTransaction;
        contextDict["IsAsync"] = context.BusinessRules.IsAsync;
        contextDict["IsPartialUpdate"] = context.BusinessRules.IsPartialUpdate;
        contextDict["IncludeDeleted"] = context.BusinessRules.IncludeDeleted;

        // Add operation context if available
        if (context.OperationContext != null)
        {
            var operationContext = context.OperationContext.GetCustomContext();
            foreach (var (key, value) in operationContext)
            {
                contextDict[key] = value;
            }

            // Add specific context values
            contextDict["Hour"] = context.OperationContext.TimeContext.Hour;
            contextDict["DayOfWeek"] = context.OperationContext.TimeContext.DayOfWeek;
            contextDict["UserRole"] = context.OperationContext.UserContext.UserRole ?? "Guest";
            contextDict["Environment"] = context.OperationContext.EnvironmentContext.Environment;
            contextDict["IsProduction"] = context.OperationContext.EnvironmentContext.IsProduction;
            contextDict["IsBusinessHours"] = context.OperationContext.TimeContext.IsBusinessHours;
            contextDict["IsWeekend"] = context.OperationContext.TimeContext.IsWeekend;
        }

        return contextDict;
    }

    private bool HasBusinessRuleContext(TemplateContext context)
    {
        return context.BusinessRules.RequiresApproval ||
               context.BusinessRules.IsValidationRequired ||
               context.BusinessRules.IsCached ||
               context.BusinessRules.RequiresTransaction ||
               context.BusinessRules.IsAsync ||
               context.BusinessRules.IsPartialUpdate ||
               context.BusinessRules.IncludeDeleted ||
               context.BusinessRules.CustomRules.Any();
    }

    private string SplitCamelCase(string input)
    {
        if (string.IsNullOrEmpty(input))
            return string.Empty;

        return Regex.Replace(input, "([A-Z])", " $1").Trim();
    }
}