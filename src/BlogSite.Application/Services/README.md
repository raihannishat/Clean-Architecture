# Dynamic Entity System

This document describes the dynamic entity system that automatically handles entity discovery and pluralization.

## Overview

The system has been refactored to automatically discover entities and handle pluralization without requiring manual updates to hardcoded dictionaries.

## Key Components

### PluralizationService
- **Interface**: `IPluralizationService`
- **Implementation**: `PluralizationService`
- **Purpose**: Automatically converts between singular and plural forms of words using English grammar rules
- **Features**:
  - Handles irregular plurals (person/people, child/children, etc.)
  - Applies standard pluralization rules (category → categories, blog → blogs, etc.)
  - Preserves original casing
  - Supports both pluralization and singularization

### EntityDiscoveryService
- **Interface**: `IEntityDiscoveryService`
- **Implementation**: `EntityDiscoveryService`
- **Purpose**: Automatically discovers all entities that inherit from `BaseEntity`
- **Features**:
  - Uses reflection to find all entity types
  - Builds entity name mappings (both singular and plural forms)
  - Validates entity names
  - Returns proper casing for entity names

## Benefits

### Before (Manual System)
```csharp
private readonly Dictionary<string, string> _entityPlurals = new()
{
    { "author", "authors" },
    { "blogpost", "blogposts" },
    { "category", "categories" },
    { "comment", "comments" }
};
```

**Problems with the old approach:**
- Required manual updates for each new entity
- Hardcoded mappings that could become out of sync
- No automatic pluralization rules
- Maintenance overhead

### After (Dynamic System)
The system now:
- **Automatically discovers** all entities that inherit from `BaseEntity`
- **Automatically generates** plural forms using English grammar rules
- **Requires no manual updates** when adding new entities
- **Handles edge cases** with irregular plurals
- **Scales infinitely** as new entities are added

## Usage Example

When you add a new entity like `Product.cs`:

```csharp
public class Product : BaseEntity
{
    public string Name { get; set; } = string.Empty;
    public decimal Price { get; set; }
}
```

The system will automatically:
1. Discover the `Product` entity
2. Generate the plural form `Products`
3. Handle API calls like:
   - `getallproducts` → `GetAllProductsQuery`
   - `createproduct` → `CreateProductCommand`
   - `getproductbyid` → `GetProductByIdQuery`

No manual configuration required!

## Adding New Entities

To add a new entity:

1. Create your entity class inheriting from `BaseEntity`
2. Place it in the `BlogSite.Domain.Entities` namespace
3. That's it! The system will automatically discover and handle it

## Supported Pluralization Rules

The system handles:
- Regular plurals: `blog` → `blogs`
- Y-ending words: `category` → `categories`
- S/X/Z/CH/SH endings: `class` → `classes`
- F/FE endings: `life` → `lives`
- Irregular plurals: `person` → `people`, `child` → `children`

## Performance

- Entity discovery is done once at startup using lazy initialization
- Pluralization rules are applied on-demand
- Both services are registered as singletons for optimal performance