using System;
using System.Threading.Tasks;
using HRManagement.Common.Application.Contracts;
using HRManagement.Common.Pertinence.Repositories;
using HRManagement.Modules.Personnel.Application;
using HRManagement.Modules.Personnel.Application.Contracts;
using HRManagement.Modules.Personnel.Persistence.Repositories;
using Microsoft.Extensions.DependencyInjection;

namespace HRManagement.Modules.Personnel.Persistence;

public static class PersistenceServiceRegistration
{
    public static void AddModulePersonnelManagement(this IServiceCollection services)
    {
        services.AddApplicationServices();
        services.AddDbContext<PersonnelDbContext>();
        services.AddScoped<IUnitOfWork, UnitOfWork>(provider =>
        {
            var personnelDbContext = ResolveDbContext(provider);
            return new UnitOfWork(personnelDbContext);
        });
    }

    public static async Task ModulePersonnelManagementDatabaseInitializer(this IServiceProvider provider)
    {
        var personnelDbContext = ResolveDbContext(provider);
        await DatabaseInitializer.InitializeAsync(personnelDbContext);
    }

    private static PersonnelDbContext ResolveDbContext(IServiceProvider provider)
    {
        var serviceScope = provider.CreateScope();
        var services = serviceScope.ServiceProvider;
        var referenceDbContext = services.GetRequiredService<PersonnelDbContext>();
        return referenceDbContext;
    }
}