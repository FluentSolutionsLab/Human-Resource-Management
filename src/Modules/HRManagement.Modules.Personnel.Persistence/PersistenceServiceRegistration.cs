using System;
using System.Threading.Tasks;
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
        services.AddScoped<IGenericUnitOfWork, GenericUnitOfWork>();
    }
    
    public static async Task ModulePersonnelManagementDatabaseInitializer(this IServiceProvider provider)
    {
        using var serviceScope = provider.CreateScope();
        var services = serviceScope.ServiceProvider;
        var referenceDbContext = services.GetRequiredService<PersonnelDbContext>();
        await DatabaseInitializer.InitializeAsync(referenceDbContext);
    }

}