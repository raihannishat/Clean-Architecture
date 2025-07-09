using System;

namespace BlogSite.Application.Attributes;

/// <summary>
/// Attribute to provide custom descriptions for CQRS operations
/// </summary>
[AttributeUsage(AttributeTargets.Class)]
public class OperationDescriptionAttribute : Attribute
{
    public string Description { get; }
    public string? ShortDescription { get; }
    
    public OperationDescriptionAttribute(string description, string? shortDescription = null)
    {
        Description = description;
        ShortDescription = shortDescription;
    }
}