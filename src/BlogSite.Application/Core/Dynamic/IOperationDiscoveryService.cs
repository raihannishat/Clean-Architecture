using System.Reflection;
using Microsoft.Extensions.DependencyInjection;

namespace BlogSite.Application.Core.Dynamic;

/// <summary>
/// Dynamic service for discovering operations from assemblies
/// </summary>
public interface IOperationDiscoveryService
{
    /// <summary>
    /// Discovers all operations from loaded assemblies
    /// </summary>
    Task<IEnumerable<OperationMetadata>> DiscoverOperationsAsync();
    
    /// <summary>
    /// Discovers operations from specific assembly
    /// </summary>
    Task<IEnumerable<OperationMetadata>> DiscoverOperationsAsync(Assembly assembly);
    
    /// <summary>
    /// Registers discovered operations in the service container
    /// </summary>
    Task RegisterDiscoveredOperationsAsync(IServiceCollection services);
}