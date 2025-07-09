# Dynamic Entity Discovery System

## Overview

The entity discovery system has been enhanced to completely eliminate the need for manually maintaining hardcoded entity lists. The system now uses multiple automatic discovery strategies to dynamically identify and register entities at runtime.

## Key Features

### 🔄 **Fully Dynamic Discovery**
- No more manual entity list maintenance
- Automatic discovery from multiple sources
- Runtime entity registration and learning
- Zero configuration required for most scenarios

### 🎯 **Multiple Discovery Strategies**

1. **Reflection-Based Discovery**
   - Scans assemblies for classes inheriting from `BaseEntity`
   - Identifies entities by naming patterns (`*Entity`, `*Model`)
   - Analyzes properties to detect entity characteristics (Id, timestamps)

2. **Request Type Pattern Analysis**
   - Extracts entities from Command/Query type names
   - Analyzes `GetAuthorQuery`, `CreateBlogPostCommand` patterns
   - Automatically registers discovered entities

3. **Database Context Discovery**
   - Scans `DbContext` classes for `DbSet<T>` properties
   - Automatically discovers entity types from EF configurations
   - Works with any ORM that follows similar patterns

4. **Runtime Registration**
   - Allows manual registration of new entities
   - Automatically registers entities when referenced
   - Learns from usage patterns

### ⚡ **Performance Optimized**
- Lazy loading of entity types
- Concurrent collections for thread safety
- Configurable caching
- Background discovery tasks

## Configuration Options

```json
{
  "EntityDiscovery": {
    "EnableRequestTypeDiscovery": true,
    "EnableDbContextDiscovery": true,
    "EnableReflectionDiscovery": true,
    "EnableEntityCaching": true,
    "FallbackEntities": []  // Optional, only used as last resort
  }
}
```

### Configuration Properties

| Property | Default | Description |
|----------|---------|-------------|
| `EnableRequestTypeDiscovery` | `true` | Discover entities from Command/Query patterns |
| `EnableDbContextDiscovery` | `true` | Discover entities from DbContext/DbSet analysis |
| `EnableReflectionDiscovery` | `true` | Discover entities via reflection scanning |
| `EnableEntityCaching` | `true` | Cache discovered entities for performance |
| `FallbackEntities` | `[]` | **Optional** fallback list (rarely needed) |

## How It Works

### 1. Initialization Phase
```csharp
// EntityDiscoveryService automatically:
1. Scans assemblies for BaseEntity inheritors
2. Analyzes existing Command/Query types
3. Discovers DbContext entity mappings
4. Builds comprehensive entity registry
```

### 2. Runtime Discovery
```csharp
// When processing requests:
1. Extract entity from request pattern
2. Check known entity registry
3. Auto-register if valid but unknown
4. Update internal mappings
```

### 3. Learning System
```csharp
// System learns from usage:
dispatcher.ProcessRequest("GetNewEntityQuery") 
// → Automatically discovers "NewEntity"
// → Registers for future use
// → No configuration needed
```

## API Usage

### Basic Usage (Automatic)
```csharp
// No setup required - works automatically
var entity = entityDiscoveryService.GetProperEntityName("blogpost");
// Returns: "BlogPost" (auto-discovered)

var isValid = entityDiscoveryService.IsValidEntity("Author");
// Returns: true (discovered from reflection/patterns)
```

### Manual Registration
```csharp
// Register custom entities if needed
entityDiscoveryService.RegisterEntity("CustomEntity");
entityDiscoveryService.RegisterEntityType(typeof(MyCustomEntity));
```

### Async Discovery
```csharp
// Trigger comprehensive discovery
var entities = await entityDiscoveryService.DiscoverEntitiesFromRequestTypesAsync();
```

## Migration from Hardcoded Lists

### Before (Old System)
```json
{
  "EntityDiscovery": {
    "FallbackEntities": [
      "Author",
      "BlogPost", 
      "Category",
      "Comment",
      "User",
      "Tag"
    ]
  }
}
```

### After (New System)
```json
{
  "EntityDiscovery": {
    // Everything is automatic!
    // FallbackEntities can be removed entirely
  }
}
```

## Discovery Strategies in Detail

### 1. Reflection-Based Discovery

**Identifies entities by:**
- Inheritance from `BaseEntity`
- Naming patterns: `*Entity`, `*Model`
- Property analysis: entities typically have `Id` + timestamp properties

```csharp
// Automatically discovered:
public class BlogPost : BaseEntity { } // ✅ Inherits BaseEntity
public class UserModel { public int Id { get; set; } } // ✅ Has Id property
```

### 2. Request Type Pattern Analysis

**Extracts entities from:**
- Command type names: `CreateBlogPostCommand` → `BlogPost`
- Query type names: `GetAllAuthorsQuery` → `Author`
- Handles complex patterns: `UpdateUserProfileCommand` → `UserProfile`

```csharp
// These automatically register entities:
public class GetBlogPostQuery { } // → Registers "BlogPost"
public class CreateAuthorCommand { } // → Registers "Author"
```

### 3. Database Context Discovery

**Scans for:**
- `DbSet<T>` properties in context classes
- EF Core entity configurations
- Any ORM patterns

```csharp
public class BlogContext : DbContext
{
    public DbSet<Author> Authors { get; set; } // ✅ Auto-discovered
    public DbSet<BlogPost> BlogPosts { get; set; } // ✅ Auto-discovered
}
```

### 4. Runtime Learning

**Automatically handles:**
- Unknown entities in requests
- Dynamic entity registration
- Pattern learning from usage

```csharp
// First request with unknown entity:
dispatcher.Process("GetProductQuery"); // Unknown "Product"
// → System analyzes pattern
// → Registers "Product" entity
// → Future requests work automatically
```

## Benefits

### ✅ **Zero Maintenance**
- No hardcoded entity lists to maintain
- Automatic discovery of new entities
- Self-learning system

### ✅ **Developer Friendly**
- Works out of the box
- No configuration required
- Follows convention over configuration

### ✅ **Scalable**
- Handles growing entity counts
- Performance optimized
- Thread-safe operations

### ✅ **Robust**
- Multiple fallback strategies
- Graceful error handling
- Optional manual overrides

## Troubleshooting

### Entity Not Discovered?

1. **Check naming patterns**: Ensure entities follow conventions
2. **Verify inheritance**: Entities should inherit from `BaseEntity`
3. **Manual registration**: Use `RegisterEntity()` if needed
4. **Enable discovery**: Check configuration options

### Performance Concerns?

1. **Disable unused discovery**: Turn off unneeded strategies
2. **Enable caching**: Ensure `EnableEntityCaching` is true
3. **Lazy loading**: Discovery happens on-demand

### Migration Issues?

1. **Keep fallbacks temporarily**: Leave old config during transition
2. **Gradual migration**: Enable features one by one
3. **Monitoring**: Check logs for discovery activities

## Example Scenarios

### Scenario 1: Adding New Entity
```csharp
// 1. Create entity class
public class Product : BaseEntity 
{
    public string Name { get; set; }
}

// 2. Create commands/queries
public class GetProductQuery { }
public class CreateProductCommand { }

// 3. That's it! No configuration needed
// System automatically discovers "Product" entity
```

### Scenario 2: Legacy System Migration
```csharp
// Keep existing fallback during migration
"FallbackEntities": ["LegacyEntity"]

// System will:
1. Use dynamic discovery for new entities
2. Fall back to configured list for legacy ones
3. Gradually learn all entities
4. Eventually, fallback list can be removed
```

### Scenario 3: Custom Discovery
```csharp
// For special cases, manual registration:
public class CustomEntityService 
{
    public void RegisterCustomEntities(IEntityDiscoveryService service)
    {
        service.RegisterEntity("SpecialEntity");
        service.RegisterEntityType(typeof(CustomType));
    }
}
```

## Conclusion

The new dynamic entity discovery system eliminates the need for manual entity list maintenance while providing robust, performance-optimized entity management. The system learns and adapts automatically, making it perfect for growing applications where new entities are frequently added.

**Key Takeaway**: In most cases, you can completely remove the `FallbackEntities` configuration and let the system handle everything automatically!