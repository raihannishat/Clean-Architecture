# Configurable Operation Description System

## Overview

The operation description generator has been refactored from a hardcoded implementation to a fully configurable system. All templates, patterns, and field descriptions are now defined in the `appsettings.json` configuration file, making it easy to customize without code changes.

## Configuration Structure

The configuration is defined in the `"OperationDescription"` section of `appsettings.json`:

```json
{
  "OperationDescription": {
    "Templates": { ... },           // Exact template matches
    "FieldDescriptions": { ... },   // GetBy field-specific descriptions  
    "QueryPatterns": { ... },       // Pattern-based query rules
    "CommandPatterns": { ... },     // Pattern-based command rules
    "FallbackTemplates": { ... }    // Default templates
  }
}
```

## Configuration Sections

### 1. Templates
Exact template matches for specific `operationType.action` combinations:

```json
"Templates": {
  "command.create": {
    "EntityForm": "{entity}",
    "Template": "Creates a new {entity}"
  },
  "query.getbyid": {
    "EntityForm": "{entity}",
    "Template": "Gets a {entity} by ID"
  }
}
```

### 2. FieldDescriptions
Special handling for `GetBy*` queries with field-specific descriptions:

```json
"FieldDescriptions": {
  "email": {
    "Template": "Gets a {entity} by email address",
    "UsePlural": false
  },
  "category": {
    "Template": "Gets {entities} by category", 
    "UsePlural": true
  }
}
```

### 3. QueryPatterns & CommandPatterns
Pattern-based rules for flexible matching:

```json
"QueryPatterns": {
  "search": {
    "Pattern": "search",
    "Template": "Searches {entities}",
    "UsePlural": true,
    "MatchType": "Contains"
  },
  "getPlural": {
    "Pattern": "^get.*s$",
    "Template": "Gets {entities}",
    "UsePlural": true,
    "MatchType": "Regex"
  }
}
```

### 4. FallbackTemplates
Default templates when no specific rules match:

```json
"FallbackTemplates": {
  "DefaultQuery": "Executes {action} query on {entity}",
  "DefaultCommand": "Executes {action} command on {entity}",
  "DefaultOperation": "Executes {action} {operationType} on {entity}",
  "GetByDefault": "Gets a {entity} by {field}"
}
```

## Pattern Matching Types

The system supports multiple pattern matching strategies:

- **Contains**: Matches if the pattern is contained anywhere in the action
- **StartsWith**: Matches if the action starts with the pattern
- **EndsWith**: Matches if the action ends with the pattern
- **Exact**: Matches only if the action exactly equals the pattern
- **Regex**: Matches using regular expressions

## Template Variables

Templates support several placeholder variables:

- `{entity}` - Singular entity name (e.g., "blog post")
- `{entities}` - Plural entity name (e.g., "blog posts")
- `{entityType}` - Original entity type (e.g., "BlogPost")
- `{field}` - Field name for GetBy patterns (e.g., "email")
- `{fieldName}` - Raw field name (e.g., "Email")
- `{action}` - Original action name
- `{operationType}` - Operation type (Query/Command)

## Processing Order

The system processes descriptions in this order:

1. **Attribute Override**: Check for explicit `[OperationDescription]` attribute
2. **Exact Template Match**: Look for `operationType.action` in Templates
3. **GetBy Pattern**: Handle `GetBy*` actions with field-specific rules
4. **Pattern Matching**: Apply QueryPatterns or CommandPatterns based on operation type
5. **Fallback**: Use appropriate fallback template

## Adding New Patterns

### Example: Adding Support for Bulk Operations

```json
"CommandPatterns": {
  "bulkCreate": {
    "Pattern": "bulk.*create",
    "Template": "Creates multiple {entities} in bulk",
    "UsePlural": true,
    "MatchType": "Regex"
  },
  "bulkDelete": {
    "Pattern": "bulk.*delete",
    "Template": "Deletes multiple {entities} in bulk", 
    "UsePlural": true,
    "MatchType": "Regex"
  }
}
```

### Example: Adding Custom Field Descriptions

```json
"FieldDescriptions": {
  "phoneNumber": {
    "Template": "Gets a {entity} by phone number",
    "UsePlural": false
  },
  "department": {
    "Template": "Gets {entities} by department",
    "UsePlural": true
  }
}
```

## Architecture Components

### Configuration Models
- `OperationDescriptionConfig` - Main configuration class
- `DescriptionTemplateConfig` - Template configuration
- `FieldDescriptionConfig` - Field-specific configuration
- `PatternRuleConfig` - Pattern matching rules
- `PatternMatchType` - Enumeration for match types

### Services
- `SmartOperationDescriptionGenerator` - Main service (now configuration-driven)
- `IPatternMatcher` / `PatternMatcher` - Pattern matching and template application
- Configuration is injected via `IOptions<OperationDescriptionConfig>`

### Benefits
- **No Code Changes**: Customize descriptions entirely through configuration
- **Flexible Patterns**: Support multiple matching strategies (regex, contains, etc.)
- **Easy Extension**: Add new patterns without touching code
- **Maintainable**: Centralized configuration in JSON files
- **Testable**: Configuration can be easily mocked for testing

## Migration from Hardcoded System

The previous hardcoded implementation has been completely replaced. All existing templates and patterns have been migrated to the configuration file with identical behavior. No breaking changes to the API.

## Environment-Specific Configuration

You can override descriptions per environment by using environment-specific configuration files:

- `appsettings.Development.json` - Development overrides
- `appsettings.Production.json` - Production overrides
- `appsettings.Staging.json` - Staging overrides

This allows for different description styles in different environments if needed.