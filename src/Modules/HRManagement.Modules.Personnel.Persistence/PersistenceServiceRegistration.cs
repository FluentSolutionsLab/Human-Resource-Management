using System;
using System.Threading.Tasks;
using HRManagement.Common.Application.Contracts;
using HRManagement.Common.Application.Models;
using HRManagement.Common.Pertinence.Repositories;
using HRManagement.Modules.Personnel.Application;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;

namespace HRManagement.Modules.Personnel.Persistence;

public static class PersistenceServiceRegistration
{
    public static void AddModulePersonnelManagement(this IServiceCollection services)
    {
        services.AddApplicationServices();
        services.AddDbContext<PersonnelDbContext>();
        services.AddScoped<IUnitOfWork, UnitOfWork>(provider =>
        {
            var personnelDbContext = ResolveService<PersonnelDbContext>(provider);
            return new UnitOfWork(personnelDbContext);
        });
    }

    public static async Task DatabaseInitializer(this IServiceProvider provider, bool isDevelopment)
    {
        var resetDbOnStart = ResolveService<IOptions<AppSettings>>(provider).Value.Database.ResetDbOnStart;

        if (isDevelopment && resetDbOnStart)
        {
            var personnelDbContext = ResolveService<PersonnelDbContext>(provider);
            await Persistence.DatabaseInitializer.InitializeAsync(personnelDbContext);
        }
    }

    private static T ResolveService<T>(IServiceProvider provider)
    {
        var serviceScope = provider.CreateScope();
        var services = serviceScope.ServiceProvider;
        return services.GetRequiredService<T>();
    }
}