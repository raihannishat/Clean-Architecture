using System;

namespace BlogSite.Application.Services;

/// <summary>
/// Service for generating dynamic operation descriptions
/// </summary>
public interface IOperationDescriptionGenerator
{
    /// <summary>
    /// Gets the full description for an operation
    /// </summary>
    string GetDescription(Type requestType, string operationType, string entityType, string action);
    
    /// <summary>
    /// Gets a short description for an operation
    /// </summary>
    string GetShortDescription(Type requestType, string operationType, string entityType, string action);
    
    /// <summary>
    /// Generates description from convention when type is not available
    /// </summary>
    string GenerateFromConvention(string operationType, string entityType, string action);
}