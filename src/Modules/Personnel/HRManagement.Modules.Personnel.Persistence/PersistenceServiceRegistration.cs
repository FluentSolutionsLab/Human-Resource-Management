using HRManagement.Modules.Personnel.Application;
using HRManagement.Modules.Personnel.Application.Contracts;
using HRManagement.Modules.Personnel.Persistence.Repositories;
using Microsoft.Extensions.DependencyInjection;

namespace HRManagement.Modules.Personnel.Persistence;

public static class PersistenceServiceRegistration
{
    public static void AddPersistenceServices(this IServiceCollection services)
    {
        services.AddApplicationServices();
        services.AddDbContext<PersonnelDbContext>();
        services.AddScoped<IEmployeeRepository, EmployeeRepository>();
    }
}