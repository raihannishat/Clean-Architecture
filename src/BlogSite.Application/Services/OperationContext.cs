using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Options;
using BlogSite.Application.Configuration;

namespace BlogSite.Application.Services;

/// <summary>
/// Implementation of operation context service
/// </summary>
public class OperationContext : IOperationContext
{
    private readonly IHttpContextAccessor _httpContextAccessor;
    private readonly OperationDescriptionConfig _config;
    private readonly IExpressionEvaluator _expressionEvaluator;

    public OperationContext(
        IHttpContextAccessor httpContextAccessor,
        IOptions<OperationDescriptionConfig> config,
        IExpressionEvaluator expressionEvaluator)
    {
        _httpContextAccessor = httpContextAccessor;
        _config = config.Value;
        _expressionEvaluator = expressionEvaluator;
    }

    public UserContextInfo UserContext => GetUserContext();
    public TimeContextInfo TimeContext => new();
    public EnvironmentContextInfo EnvironmentContext => GetEnvironmentContext();

    public BusinessRuleContext GetBusinessRuleContext(string entityType)
    {
        var context = new BusinessRuleContext();
        
        if (_config.BusinessContextRules.TryGetValue("RequiresApproval", out var approvalEntities))
            context.RequiresApproval = approvalEntities.Contains(entityType);
            
        if (_config.BusinessContextRules.TryGetValue("IsValidationRequired", out var validationEntities))
            context.IsValidationRequired = validationEntities.Contains(entityType);
            
        if (_config.BusinessContextRules.TryGetValue("IsCached", out var cachedEntities))
            context.IsCached = cachedEntities.Contains(entityType);
            
        if (_config.BusinessContextRules.TryGetValue("RequiresTransaction", out var transactionEntities))
            context.RequiresTransaction = transactionEntities.Contains(entityType);
            
        if (_config.BusinessContextRules.TryGetValue("IsAsync", out var asyncEntities))
            context.IsAsync = asyncEntities.Contains(entityType);

        return context;
    }

    public Dictionary<string, object> GetCustomContext()
    {
        var context = new Dictionary<string, object>();
        
        foreach (var (key, value) in _config.ContextVariables)
        {
            context[key] = value;
        }
        
        // Add runtime context
        context["CurrentUser"] = UserContext;
        context["CurrentTime"] = TimeContext;
        context["Environment"] = EnvironmentContext;
        
        return context;
    }

    public bool EvaluateCondition(string condition)
    {
        if (string.IsNullOrWhiteSpace(condition))
            return true;
            
        try
        {
            var context = GetCustomContext();
            context["Hour"] = TimeContext.Hour;
            context["DayOfWeek"] = TimeContext.DayOfWeek;
            context["UserRole"] = UserContext.UserRole ?? "Guest";
            context["Environment"] = EnvironmentContext.Environment;
            
            return _expressionEvaluator.Evaluate(condition, context);
        }
        catch
        {
            return false;
        }
    }

    private UserContextInfo GetUserContext()
    {
        var httpContext = _httpContextAccessor.HttpContext;
        var userContext = new UserContextInfo();
        
        if (httpContext?.User?.Identity?.IsAuthenticated == true)
        {
            userContext.IsAuthenticated = true;
            userContext.UserName = httpContext.User.Identity.Name;
            userContext.UserId = httpContext.User.FindFirst("sub")?.Value ?? 
                                httpContext.User.FindFirst("id")?.Value;
            userContext.UserRole = httpContext.User.FindFirst("role")?.Value ?? "User";
            
            userContext.Permissions = httpContext.User.Claims
                .Where(c => c.Type == "permission")
                .Select(c => c.Value)
                .ToList();
        }
        else
        {
            userContext.UserRole = "Guest";
        }
        
        return userContext;
    }

    private EnvironmentContextInfo GetEnvironmentContext()
    {
        var envContext = new EnvironmentContextInfo();
        
        if (_config.ContextVariables.TryGetValue("Environment", out var env))
            envContext.Environment = env;
            
        if (_config.ContextVariables.TryGetValue("Version", out var version))
            envContext.Version = version;
            
        if (_config.ContextVariables.TryGetValue("ApplicationName", out var appName))
            envContext.ApplicationName = appName;
            
        return envContext;
    }
}

/// <summary>
/// Interface for evaluating conditional expressions in templates
/// </summary>
public interface IExpressionEvaluator
{
    bool Evaluate(string expression, Dictionary<string, object> context);
    object EvaluateValue(string expression, Dictionary<string, object> context);
}

/// <summary>
/// Simple expression evaluator for conditional templates
/// </summary>
public class SimpleExpressionEvaluator : IExpressionEvaluator
{
    public bool Evaluate(string expression, Dictionary<string, object> context)
    {
        try
        {
            var result = EvaluateValue(expression, context);
            return result is bool boolResult ? boolResult : Convert.ToBoolean(result);
        }
        catch
        {
            return false;
        }
    }

    public object EvaluateValue(string expression, Dictionary<string, object> context)
    {
        // Simple expression evaluation for basic conditions
        // This is a simplified implementation for demonstration
        
        if (expression.Contains("=="))
        {
            var parts = expression.Split("==", 2, StringSplitOptions.TrimEntries);
            if (parts.Length == 2)
            {
                var left = EvaluateOperand(parts[0], context);
                var right = EvaluateOperand(parts[1], context);
                return Equals(left?.ToString(), right?.ToString());
            }
        }
        
        if (expression.Contains(">="))
        {
            var parts = expression.Split(">=", 2, StringSplitOptions.TrimEntries);
            if (parts.Length == 2)
            {
                var left = EvaluateOperand(parts[0], context);
                var right = EvaluateOperand(parts[1], context);
                if (int.TryParse(left?.ToString(), out var leftInt) && 
                    int.TryParse(right?.ToString(), out var rightInt))
                {
                    return leftInt >= rightInt;
                }
            }
        }
        
        if (expression.Contains("<="))
        {
            var parts = expression.Split("<=", 2, StringSplitOptions.TrimEntries);
            if (parts.Length == 2)
            {
                var left = EvaluateOperand(parts[0], context);
                var right = EvaluateOperand(parts[1], context);
                if (int.TryParse(left?.ToString(), out var leftInt) && 
                    int.TryParse(right?.ToString(), out var rightInt))
                {
                    return leftInt <= rightInt;
                }
            }
        }
        
        if (expression.Contains("&&"))
        {
            var parts = expression.Split("&&", StringSplitOptions.TrimEntries);
            return parts.All(part => Evaluate(part.Trim(), context));
        }
        
        if (expression.Contains("||"))
        {
            var parts = expression.Split("||", StringSplitOptions.TrimEntries);
            return parts.Any(part => Evaluate(part.Trim(), context));
        }
        
        // Single operand evaluation
        return EvaluateOperand(expression, context);
    }

    private object? EvaluateOperand(string operand, Dictionary<string, object> context)
    {
        operand = operand.Trim().Trim('\'', '"');
        
        if (context.TryGetValue(operand, out var value))
            return value;
            
        if (int.TryParse(operand, out var intValue))
            return intValue;
            
        if (bool.TryParse(operand, out var boolValue))
            return boolValue;
            
        return operand;
    }
}