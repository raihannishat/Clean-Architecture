# Complete Dynamic System Documentation

## সংক্ষিপ্ত বর্ণনা

এই প্রোজেক্টটি একটি সম্পূর্ণ **Dynamic Approach** ব্যবহার করে তৈরি করা হয়েছে যেখানে:

- ✅ **কোনো hardcoded value নেই**
- ✅ **appsettings.json সম্পূর্ণ clean (শুধু `{}` আছে)**
- ✅ **Single Endpoint with Dynamic Dispatching Pattern**
- ✅ **Runtime এ সব কিছু discover হয়**
- ✅ **Dynamic service registration**

## 🏗️ Architecture Overview

### Core Components

1. **Dynamic Discovery Service** - Runtime এ operations discover করে
2. **Dynamic Dispatcher** - সব operations handle করে 
3. **Single Dynamic Endpoint** - একটিমাত্র endpoint যা সব request handle করে
4. **Auto-Configuration** - সব configuration runtime এ হয়

## 🚀 Key Features

### 1. **Complete Dynamic Discovery**
```csharp
// Automatically discovers:
// - MediatR Commands & Queries
// - Service methods
// - Repository interfaces
// - Database entities
// - AutoMapper profiles
```

### 2. **Single Universal Endpoint**
```
POST /api
{
  "action": "create.author",
  "payload": { ... }
}
```

### 3. **Zero Configuration**
- No appsettings.json entries needed
- No hardcoded connection strings
- No hardcoded service registrations
- Everything discovered at runtime

## 📋 API Endpoints

### 1. **Main Dynamic API**
```http
POST /api
Content-Type: application/json

{
  "action": "operation.name",
  "payload": { /* your data */ },
  "metadata": { /* optional */ }
}
```

### 2. **Operations Discovery**
```http
GET /api/operations
```
Returns all available operations with metadata.

### 3. **Operation Metadata**
```http
GET /api/operations/{action}
```
Returns detailed metadata for a specific operation.

## 🔧 Dynamic Features

### Auto-Discovery Capabilities

#### **1. Operations Discovery**
- ✅ MediatR Commands
- ✅ MediatR Queries  
- ✅ Service Methods
- ✅ Repository Methods
- ✅ Custom Handlers

#### **2. Service Registration**
- ✅ Auto-register repositories
- ✅ Auto-register services
- ✅ Auto-register MediatR handlers
- ✅ Auto-configure dependencies

#### **3. Database Configuration**
- ✅ Auto-detect existing database files
- ✅ Auto-create database if not found
- ✅ Dynamic connection string generation

#### **4. Documentation Generation**
- ✅ Auto-generate Swagger docs
- ✅ Dynamic operation descriptions
- ✅ Runtime metadata discovery

## 📝 Usage Examples

### Creating a Blog Post
```http
POST /api
{
  "action": "create.blog.post",
  "payload": {
    "title": "My Blog Post",
    "content": "Content here...",
    "authorId": 1
  }
}
```

### Getting All Authors
```http
POST /api
{
  "action": "get.all.authors",
  "payload": {}
}
```

### Custom Service Method
```http
POST /api
{
  "action": "publish.blog.post",
  "payload": {
    "id": 1
  }
}
```

## 🏃‍♂️ How to Run

### 1. **Start the Application**
```bash
cd src/BlogSite.API
dotnet run
```

### 2. **Access Swagger UI**
```
http://localhost:5000
```

### 3. **Discover Available Operations**
```http
GET http://localhost:5000/api/operations
```

### 4. **Execute Operations**
```http
POST http://localhost:5000/api
{
  "action": "your.operation",
  "payload": { ... }
}
```

## 🔍 Dynamic Operation Naming

The system automatically generates operation names:

| Class/Method | Generated Action |
|-------------|------------------|
| `CreateAuthorCommand` | `create.author` |
| `GetAllBlogPostsQuery` | `get.all.blog.posts` |
| `BlogPostService.PublishAsync()` | `publish.blog.post` |
| `UpdateCategoryCommand` | `update.category` |

## 📊 Response Format

### Success Response
```json
{
  "success": true,
  "action": "create.author",
  "result": { /* operation result */ },
  "timestamp": "2024-01-01T00:00:00Z"
}
```

### Error Response
```json
{
  "type": "https://tools.ietf.org/html/rfc7231#section-6.5.1",
  "title": "Operation Failed",
  "status": 500,
  "detail": "Error details...",
  "instance": "/api?action=create.author"
}
```

## 🛠️ Advanced Features

### 1. **Operation Metadata**
```json
{
  "action": "create.author",
  "name": "CreateAuthorCommand",
  "type": "Command",
  "description": "Executes create.author command on Author",
  "requestType": {
    "name": "CreateAuthorCommand",
    "properties": [
      {
        "name": "Name",
        "type": "String",
        "required": true
      }
    ]
  },
  "tags": {
    "assembly": "BlogSite.Application",
    "type": "Command"
  }
}
```

### 2. **Dynamic Service Registration**
```csharp
// Automatically registers:
services.AddScoped<IBlogPostService, BlogPostService>();
services.AddScoped<IAuthorService, AuthorService>();
// ... and many more
```

### 3. **Auto-Database Configuration**
```csharp
// Searches for database in:
// - BlogSite.db
// - blogsite.db
// - database.db
// - Data/BlogSite.db
// - App_Data/BlogSite.db
```

## 🌟 Benefits

### **For Developers**
- ⚡ **Zero Configuration** - Just write your classes
- 🔄 **Auto-Discovery** - New operations automatically available
- 🛡️ **Type Safety** - Full compile-time checking
- 📚 **Self-Documenting** - Auto-generated documentation

### **For Architecture**
- 🎯 **Single Responsibility** - One endpoint, one purpose
- 🔌 **Loose Coupling** - No hardcoded dependencies
- 📈 **Scalability** - Easy to add new operations
- 🧪 **Testability** - Dynamic mocking and testing

### **For Maintenance**
- 🔧 **No Configuration Hell** - Everything is discovered
- 📦 **Plugin Architecture** - Drop in new assemblies
- 🚀 **Hot Swapping** - Runtime operation updates
- 🎨 **Clean Code** - No boilerplate configuration

## 💡 How It Works

### 1. **Application Startup**
```
1. Load all assemblies
2. Discover all operations
3. Register services dynamically
4. Configure database automatically
5. Setup single endpoint
6. Generate documentation
```

### 2. **Request Processing**
```
1. Receive request with action name
2. Look up operation metadata
3. Deserialize payload to correct type
4. Execute operation via dispatcher
5. Return typed result
```

### 3. **Operation Discovery**
```
1. Scan assemblies for MediatR requests
2. Scan for service classes and methods
3. Generate action names automatically
4. Create operation metadata
5. Register handlers dynamically
```

## 🎯 Design Principles

### **1. Convention over Configuration**
- Follow naming conventions, get automatic registration
- No need to manually configure anything

### **2. Runtime Discovery**
- Everything discovered at application startup
- No compile-time binding to specific operations

### **3. Single Point of Entry**
- One endpoint handles all operations
- Unified request/response format

### **4. Zero Hardcoding**
- No hardcoded strings, paths, or configurations
- Everything derived from code structure

## 🚀 Future Enhancements

### Planned Features
- 🔐 **Dynamic Authentication** - Operation-level auth discovery
- 🌍 **Multi-language Support** - Dynamic localization
- 📊 **Performance Metrics** - Auto-generated performance tracking
- 🔄 **Caching Layer** - Intelligent operation caching
- 📡 **Real-time Updates** - SignalR integration
- 🧩 **Plugin System** - Hot-swappable operation modules

## ⚡ Performance

### Optimizations
- ✅ **Cached Operation Discovery** - Operations cached after first discovery
- ✅ **Compiled Expressions** - Fast reflection-based execution
- ✅ **Minimal Serialization** - Efficient JSON handling
- ✅ **Connection Pooling** - Optimized database connections

## 🛡️ Security Considerations

### Built-in Security
- 🔒 **Type Safety** - Strong typing prevents injection
- 🛡️ **Validation** - Automatic model validation
- 🚫 **Operation Filtering** - Only registered operations allowed
- 📝 **Audit Trail** - All operations logged automatically

## 📈 Monitoring & Logging

### Automatic Logging
```csharp
// All operations automatically logged:
logger.LogInformation("Dispatching action: {Action}", action);
logger.LogInformation("Successfully executed operation: {Action}", action);
logger.LogError(ex, "Error dispatching action: {Action}", action);
```

## 🎉 Conclusion

এই সিস্টেমটি একটি **Revolutionary Approach** যেখানে:

- 🚫 **No Configuration Files**
- 🔄 **Complete Automation** 
- ⚡ **Maximum Flexibility**
- 🎯 **Minimal Code**

সবকিছু **Runtime এ discover** হয় এবং **Single Endpoint** দিয়ে সব operation handle করা যায়।

---

## 🤝 Contributing

নতুন operations যোগ করতে:

1. MediatR Command/Query তৈরি করুন
2. অথবা Service class এ method যোগ করুন  
3. Application restart করুন
4. আপনার operation automatically available হবে!

**এত সহজ! 🎉**