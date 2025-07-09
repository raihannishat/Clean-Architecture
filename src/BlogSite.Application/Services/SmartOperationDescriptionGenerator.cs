using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using BlogSite.Application.Attributes;

namespace BlogSite.Application.Services;

/// <summary>
/// Smart operation description generator that uses attributes, templates, and conventions
/// </summary>
public class SmartOperationDescriptionGenerator : IOperationDescriptionGenerator
{
    private readonly IPluralizationService _pluralizationService;
    private readonly Dictionary<string, DescriptionTemplate> _templates;
    
    public SmartOperationDescriptionGenerator(IPluralizationService pluralizationService)
    {
        _pluralizationService = pluralizationService;
        _templates = InitializeTemplates();
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
        if (_templates.TryGetValue(key, out var template))
        {
            return template.Apply(entityType, _pluralizationService);
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
        
        // Default fallback
        return $"Executes {action} {operationType.ToLower()} on {entityType.ToLower()}";
    }
    
    private string HandleGetByPattern(string action, string entityType)
    {
        var field = action[5..]; // Remove "GetBy"
        
        // Handle special cases
        return field.ToLower() switch
        {
            "id" => $"Gets a {entityType.ToLower()} by ID",
            "email" => $"Gets a {entityType.ToLower()} by email address",
            "name" => $"Gets a {entityType.ToLower()} by name",
            "title" => $"Gets a {entityType.ToLower()} by title",
            "slug" => $"Gets a {entityType.ToLower()} by URL slug",
            "username" => $"Gets a {entityType.ToLower()} by username",
            "category" => $"Gets {_pluralizationService.Pluralize(entityType.ToLower())} by category",
            "author" => $"Gets {_pluralizationService.Pluralize(entityType.ToLower())} by author",
            "user" => $"Gets {_pluralizationService.Pluralize(entityType.ToLower())} by user",
            "tag" => $"Gets {_pluralizationService.Pluralize(entityType.ToLower())} by tag",
            "status" => $"Gets {_pluralizationService.Pluralize(entityType.ToLower())} by status",
            _ => $"Gets a {entityType.ToLower()} by {SplitCamelCase(field).ToLower()}"
        };
    }
    
    private string HandleQueryPatterns(string action, string entityType)
    {
        var actionLower = action.ToLower();
        
        return actionLower switch
        {
            var a when a.Contains("search") => $"Searches {_pluralizationService.Pluralize(entityType.ToLower())}",
            var a when a.Contains("filter") => $"Filters {_pluralizationService.Pluralize(entityType.ToLower())}",
            var a when a.Contains("count") => $"Counts {_pluralizationService.Pluralize(entityType.ToLower())}",
            var a when a.Contains("exists") => $"Checks if {entityType.ToLower()} exists",
            var a when a.Contains("validate") => $"Validates {entityType.ToLower()} data",
            var a when a.StartsWith("get") && a.EndsWith("s") => $"Gets {actionLower[3..]} {_pluralizationService.Pluralize(entityType.ToLower())}",
            _ => $"Executes {action} query on {entityType.ToLower()}"
        };
    }
    
    private string HandleCommandPatterns(string action, string entityType)
    {
        var actionLower = action.ToLower();
        
        return actionLower switch
        {
            var a when a.Contains("import") => $"Imports {_pluralizationService.Pluralize(entityType.ToLower())}",
            var a when a.Contains("export") => $"Exports {_pluralizationService.Pluralize(entityType.ToLower())}",
            var a when a.Contains("sync") => $"Synchronizes {entityType.ToLower()} data",
            var a when a.Contains("migrate") => $"Migrates {entityType.ToLower()} data",
            var a when a.Contains("restore") => $"Restores a {entityType.ToLower()}",
            var a when a.Contains("backup") => $"Backs up {entityType.ToLower()} data",
            var a when a.Contains("refresh") => $"Refreshes {entityType.ToLower()} data",
            var a when a.Contains("reset") => $"Resets {entityType.ToLower()} data",
            var a when a.Contains("lock") => $"Locks a {entityType.ToLower()}",
            var a when a.Contains("unlock") => $"Unlocks a {entityType.ToLower()}",
            _ => $"Executes {action} command on {entityType.ToLower()}"
        };
    }
    
    private string SplitCamelCase(string input)
    {
        return string.Join(" ", SplitCamelCaseToWords(input));
    }
    
    private IEnumerable<string> SplitCamelCaseToWords(string input)
    {
        var currentWord = new System.Text.StringBuilder();
        
        for (int i = 0; i < input.Length; i++)
        {
            char c = input[i];
            
            if (char.IsUpper(c) && currentWord.Length > 0)
            {
                yield return currentWord.ToString();
                currentWord.Clear();
            }
            
            currentWord.Append(char.ToLower(c));
        }
        
        if (currentWord.Length > 0)
        {
            yield return currentWord.ToString();
        }
    }
    
    private Dictionary<string, DescriptionTemplate> InitializeTemplates()
    {
        return new Dictionary<string, DescriptionTemplate>
        {
            // Command templates
            ["command.create"] = new("{entity}", "Creates a new {entity}"),
            ["command.update"] = new("{entity}", "Updates an existing {entity}"),
            ["command.delete"] = new("{entity}", "Deletes a {entity}"),
            ["command.remove"] = new("{entity}", "Removes a {entity}"),
            ["command.publish"] = new("{entity}", "Publishes a {entity}"),
            ["command.unpublish"] = new("{entity}", "Unpublishes a {entity}"),
            ["command.archive"] = new("{entity}", "Archives a {entity}"),
            ["command.unarchive"] = new("{entity}", "Unarchives a {entity}"),
            ["command.activate"] = new("{entity}", "Activates a {entity}"),
            ["command.deactivate"] = new("{entity}", "Deactivates a {entity}"),
            ["command.enable"] = new("{entity}", "Enables a {entity}"),
            ["command.disable"] = new("{entity}", "Disables a {entity}"),
            ["command.approve"] = new("{entity}", "Approves a {entity}"),
            ["command.reject"] = new("{entity}", "Rejects a {entity}"),
            ["command.submit"] = new("{entity}", "Submits a {entity}"),
            ["command.cancel"] = new("{entity}", "Cancels a {entity}"),
            ["command.complete"] = new("{entity}", "Completes a {entity}"),
            ["command.start"] = new("{entity}", "Starts a {entity}"),
            ["command.stop"] = new("{entity}", "Stops a {entity}"),
            ["command.pause"] = new("{entity}", "Pauses a {entity}"),
            ["command.resume"] = new("{entity}", "Resumes a {entity}"),
            
            // Query templates
            ["query.getall"] = new("{entities}", "Gets all {entities}"),
            ["query.getbyid"] = new("{entity}", "Gets a {entity} by ID"),
            ["query.getbyemail"] = new("{entity}", "Gets a {entity} by email"),
            ["query.getbyname"] = new("{entity}", "Gets a {entity} by name"),
            ["query.getbytitle"] = new("{entity}", "Gets a {entity} by title"),
            ["query.getbyslug"] = new("{entity}", "Gets a {entity} by slug"),
            ["query.getbyusername"] = new("{entity}", "Gets a {entity} by username"),
            ["query.getbycategory"] = new("{entities}", "Gets {entities} by category"),
            ["query.getbyauthor"] = new("{entities}", "Gets {entities} by author"),
            ["query.getbyuser"] = new("{entities}", "Gets {entities} by user"),
            ["query.getbytag"] = new("{entities}", "Gets {entities} by tag"),
            ["query.getbystatus"] = new("{entities}", "Gets {entities} by status"),
            ["query.getpublished"] = new("{entities}", "Gets published {entities}"),
            ["query.getunpublished"] = new("{entities}", "Gets unpublished {entities}"),
            ["query.getactive"] = new("{entities}", "Gets active {entities}"),
            ["query.getinactive"] = new("{entities}", "Gets inactive {entities}"),
            ["query.getarchived"] = new("{entities}", "Gets archived {entities}"),
            ["query.getapproved"] = new("{entities}", "Gets approved {entities}"),
            ["query.getpending"] = new("{entities}", "Gets pending {entities}"),
            ["query.getrejected"] = new("{entities}", "Gets rejected {entities}"),
            ["query.search"] = new("{entities}", "Searches {entities}"),
            ["query.filter"] = new("{entities}", "Filters {entities}"),
            ["query.count"] = new("{entities}", "Counts {entities}"),
            ["query.exists"] = new("{entity}", "Checks if {entity} exists"),
        };
    }
}

/// <summary>
/// Template for generating operation descriptions
/// </summary>
public record DescriptionTemplate(string EntityForm, string Template)
{
    public string Apply(string entityType, IPluralizationService pluralizationService)
    {
        var entityValue = EntityForm switch
        {
            "{entity}" => entityType.ToLower(),
            "{entities}" => pluralizationService.Pluralize(entityType.ToLower()),
            _ => entityType.ToLower()
        };
        
        return Template.Replace(EntityForm, entityValue);
    }
}