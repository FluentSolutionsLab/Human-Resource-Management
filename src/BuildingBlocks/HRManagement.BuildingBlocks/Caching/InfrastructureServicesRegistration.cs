using HRManagement.BuildingBlocks.Contracts;
using Microsoft.Extensions.DependencyInjection;

namespace HRManagement.BuildingBlocks.Caching;

public static class InfrastructureServicesRegistration
{
    public static void AddInfrastructureServices(this IServiceCollection services)
    {
        services.AddSingleton<ICacheService, MemoryCacheService>();
    }
}