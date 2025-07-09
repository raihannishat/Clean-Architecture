using System;
using System.Collections.Generic;

namespace BlogSite.Application.Services;

/// <summary>
/// Provides contextual information for dynamic operation description generation
/// </summary>
public interface IOperationContext
{
    /// <summary>
    /// Gets the current user context
    /// </summary>
    UserContextInfo UserContext { get; }
    
    /// <summary>
    /// Gets the current time context
    /// </summary>
    TimeContextInfo TimeContext { get; }
    
    /// <summary>
    /// Gets the current environment context
    /// </summary>
    EnvironmentContextInfo EnvironmentContext { get; }
    
    /// <summary>
    /// Gets business rule context for an entity
    /// </summary>
    BusinessRuleContext GetBusinessRuleContext(string entityType);
    
    /// <summary>
    /// Gets custom context variables
    /// </summary>
    Dictionary<string, object> GetCustomContext();
    
    /// <summary>
    /// Evaluates a conditional expression
    /// </summary>
    bool EvaluateCondition(string condition);
}

/// <summary>
/// User context information
/// </summary>
public class UserContextInfo
{
    public string? UserId { get; set; }
    public string? UserName { get; set; }
    public string? UserRole { get; set; }
    public List<string> Permissions { get; set; } = new();
    public bool IsAuthenticated { get; set; }
}

/// <summary>
/// Time context information
/// </summary>
public class TimeContextInfo
{
    public DateTime CurrentTime { get; set; } = DateTime.Now;
    public int Hour => CurrentTime.Hour;
    public string DayOfWeek => CurrentTime.DayOfWeek.ToString();
    public bool IsBusinessHours => Hour >= 9 && Hour <= 17;
    public bool IsWeekend => CurrentTime.DayOfWeek == System.DayOfWeek.Saturday || CurrentTime.DayOfWeek == System.DayOfWeek.Sunday;
}

/// <summary>
/// Environment context information
/// </summary>
public class EnvironmentContextInfo
{
    public string Environment { get; set; } = "Development";
    public string Version { get; set; } = "1.0.0";
    public string ApplicationName { get; set; } = "BlogSite";
    public bool IsProduction => Environment.Equals("Production", StringComparison.OrdinalIgnoreCase);
}

/// <summary>
/// Business rule context for an entity
/// </summary>
public class BusinessRuleContext
{
    public bool RequiresApproval { get; set; }
    public bool IsValidationRequired { get; set; }
    public bool IsCached { get; set; }
    public bool RequiresTransaction { get; set; }
    public bool IsAsync { get; set; }
    public bool IsPartialUpdate { get; set; }
    public bool IncludeDeleted { get; set; }
    public Dictionary<string, object> CustomRules { get; set; } = new();
}