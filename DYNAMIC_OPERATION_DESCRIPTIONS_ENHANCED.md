# Dynamic OperationDescription System - Enhanced Implementation

## Overview

The OperationDescription system has been significantly enhanced to provide truly dynamic, context-aware, and intelligent operation descriptions. The system now supports:

- **Context-Aware Templates**: Descriptions that adapt based on user role, time, environment
- **Conditional Logic**: Templates with if/else logic for dynamic content
- **Localization Support**: Multi-language descriptions
- **Business Rule Integration**: Context-aware descriptions based on entity-specific business rules
- **Time and User-Based Rules**: Dynamic modifications based on current conditions
- **Advanced Template Processing**: Support for complex placeholders and expressions

## Key Features

### 1. Dynamic Context Variables
Templates can now use contextual information:
- `{ApplicationName}`, `{Version}`, `{Environment}`
- `{UserName}`, `{UserRole}`, `{CurrentTime}`
- `{Hour}`, `{DayOfWeek}`, `{IsBusinessHours}`, `{IsWeekend}`

### 2. Conditional Logic in Templates
Support for conditional expressions in templates:
```json
{
  "Template": "Creates a new {entity} {?IsValidationRequired ? 'with validation' : 'without validation'}"
}
```

### 3. Business Rule Context
Automatic detection and integration of business rules:
- `RequiresApproval`: Entities that need approval
- `IsValidationRequired`: Entities requiring validation
- `IsCached`: Entities that are cached
- `RequiresTransaction`: Operations requiring transactions
- `IsAsync`: Asynchronous operations

### 4. Multiple Template Types
Different template priorities:
1. **Localized**: Language-specific templates
2. **BusinessRuleTemplate**: Business logic aware templates
3. **ConditionalTemplate**: Templates with conditional logic
4. **TemplateWithContext**: Context-aware templates
5. **Template**: Basic fallback templates

### 5. Time and User-Based Rules
Dynamic modifications based on:
- Business hours vs. off-hours
- Weekend vs. weekday
- User role (Admin, User, Guest)
- Environment (Production, Development, etc.)

## Configuration Examples

### Enhanced appsettings.json Structure

```json
{
  "OperationDescription": {
    "EnableDynamicContext": true,
    "DefaultLanguage": "en",
    "SupportedLanguages": ["en", "bn"],
    "ContextVariables": {
      "ApplicationName": "BlogSite",
      "Version": "1.0.0",
      "Environment": "Development"
    },
    "Templates": {
      "command.create": {
        "Template": "Creates a new {entity}",
        "TemplateWithContext": "Creates a new {entity} in {ApplicationName}",
        "BusinessRuleTemplate": "Creates a new {entity} {?RequiresApproval ? 'pending approval' : 'immediately available'}",
        "ConditionalTemplate": "Creates {?IsValidationRequired ? 'a validated' : 'a new'} {entity}",
        "Localized": {
          "en": "Creates a new {entity}",
          "bn": "একটি নতুন {entity} তৈরি করে"
        }
      }
    },
    "DynamicRules": {
      "TimeBasedTemplates": {
        "Enabled": true,
        "Rules": [
          {
            "Condition": "Hour >= 9 && Hour <= 17",
            "TemplateSuffix": " during business hours"
          },
          {
            "Condition": "DayOfWeek == 'Saturday' || DayOfWeek == 'Sunday'",
            "TemplateSuffix": " on weekend"
          }
        ]
      },
      "UserBasedTemplates": {
        "Enabled": true,
        "Rules": [
          {
            "Condition": "UserRole == 'Admin'",
            "TemplatePrefix": "[Admin Operation] "
          }
        ]
      }
    },
    "BusinessContextRules": {
      "RequiresApproval": ["User", "Author", "Category"],
      "IsValidationRequired": ["BlogPost", "Comment", "User"],
      "IsCached": ["BlogPost", "Category", "Tag"],
      "RequiresTransaction": ["User", "Author", "BlogPost"],
      "IsAsync": ["Email", "Notification", "Export"]
    }
  }
}
```

## New Services and Components

### 1. IOperationContext Service
Provides contextual information for dynamic template generation:
- User context (role, permissions, authentication status)
- Time context (current time, business hours, weekend detection)
- Environment context (application name, version, environment)
- Business rule context (entity-specific rules)

### 2. ITemplateProcessor Service
Advanced template processing with:
- Conditional expression evaluation
- Context variable replacement
- Dynamic rule application
- Localization support

### 3. IExpressionEvaluator Service
Simple expression evaluator for conditional logic:
- Boolean expressions (`==`, `>=`, `<=`, `&&`, `||`)
- Context variable evaluation
- Safe expression parsing

## Template Processing Priority

The system uses the following priority order when selecting templates:

1. **Localized Template** (based on user's language preference)
2. **Business Rule Template** (if business rules are applicable)
3. **Conditional Template** (if conditional logic is present)
4. **Context Template** (if dynamic context is enabled)
5. **Basic Template** (fallback)

## Example Dynamic Descriptions

### Context-Aware Examples
- **Admin User**: "[Admin Operation] Creates a new user with validation during business hours"
- **Guest User**: "[Guest Operation] Creates a new user with validation on weekend"
- **Production**: "Creates a new user"
- **Development**: "Creates a new user in BlogSite (Development)"

### Business Rule Examples
- **User Entity**: "Creates a new user with validation pending approval"
- **BlogPost Entity**: "Creates a new blogpost with validation from cache"
- **Category Entity**: "Creates a new category pending approval within transaction"

### Conditional Logic Examples
- **With Validation**: "Creates a validated user"
- **Without Validation**: "Creates a new user"
- **Partial Update**: "Updates specific fields of an existing user"
- **Full Update**: "Updates all data for an existing user"

### Localization Examples
- **English**: "Creates a new user"
- **Bengali**: "একটি নতুন user তৈরি করে"

## Benefits

### 1. Intelligence
- Descriptions automatically adapt to context
- Business logic is reflected in descriptions
- Time and user-specific modifications

### 2. Flexibility
- Multiple template types for different scenarios
- Configurable rules without code changes
- Extensible expression evaluation

### 3. Maintainability
- Centralized configuration
- Template inheritance and fallbacks
- Clear separation of concerns

### 4. User Experience
- Contextually relevant descriptions
- Localized content
- Role-appropriate information

### 5. Scalability
- Automatic discovery and application
- Caching of compiled expressions
- Minimal performance overhead

## Technical Implementation

### Service Registration
```csharp
// Enhanced template processing services
services.AddSingleton<IExpressionEvaluator, SimpleExpressionEvaluator>();
services.AddSingleton<ITemplateProcessor, AdvancedTemplateProcessor>();
services.AddScoped<IOperationContext, OperationContext>();
services.AddHttpContextAccessor();
```

### Usage in Code
```csharp
public class SmartOperationDescriptionGenerator : IOperationDescriptionGenerator
{
    public string GetDescription(Type requestType, string operationType, string entityType, string action)
    {
        var context = CreateTemplateContext(operationType, entityType, action);
        
        if (_config.Templates.TryGetValue($"{operationType.ToLower()}.{action.ToLower()}", out var templateConfig))
        {
            return _templateProcessor.ProcessTemplate(templateConfig, context);
        }
        
        return GenerateFromConvention(operationType, entityType, action);
    }
}
```

## Future Enhancements

### 1. Advanced Expression Engine
- Support for more complex expressions
- Custom function support
- Mathematical operations

### 2. Template Caching
- Pre-compiled template caching
- Performance optimizations
- Memory usage optimization

### 3. External Template Storage
- Database-stored templates
- Remote template configuration
- Template versioning

### 4. AI-Generated Descriptions
- LLM integration for description generation
- Natural language processing
- Contextual semantic understanding

### 5. Analytics and Insights
- Template usage analytics
- Performance metrics
- User interaction tracking

## Migration Guide

### From Basic to Enhanced System

1. **Update Configuration**: Add new configuration sections to appsettings.json
2. **Service Registration**: Register new services in Program.cs
3. **Template Migration**: Convert existing templates to new format
4. **Testing**: Verify dynamic behavior in different contexts

### Backward Compatibility
- Existing basic templates continue to work
- Gradual migration path available
- Fallback mechanisms ensure stability

## Conclusion

The enhanced OperationDescription system provides a powerful, flexible, and intelligent way to generate dynamic operation descriptions. It combines the simplicity of template-based generation with the power of contextual awareness, conditional logic, and business rule integration, making it suitable for complex enterprise applications while remaining easy to configure and maintain.