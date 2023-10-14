using HRManagement.Common.Application.Contracts;
using Microsoft.Extensions.DependencyInjection;

namespace HRManagement.Common.Infrastructure;

public static class InfrastructureServicesRegistration
{
    public static void AddInfrastructureServices(this IServiceCollection services)
    {
        services.AddSingleton<ICacheService, MemoryCacheService>();
    }
}