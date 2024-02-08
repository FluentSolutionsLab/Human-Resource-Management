using System;
using HRManagement.Common.Application.Contracts;
using HRManagement.Common.Application.Models;
using HRManagement.Common.Pertinence.Repositories;
using HRManagement.Modules.Personnel.Application;
using HRManagement.Modules.Personnel.Application.UseCases.Services;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace HRManagement.Modules.Personnel.Persistence;

public static class PersistenceServiceRegistration
{
    public static void AddModulePersonnelManagement(this IServiceCollection services, bool isDevelopment)
    {
        services.AddApplicationServices();
        services.AddDbContext<PersonnelDbContext>(options =>
        {
            var connectionString = ResolveService<IOptions<AppSettings>>(services.BuildServiceProvider()).Value
                .ConnectionStrings.Default;
            options.UseSqlServer(connectionString);
            if (isDevelopment)
            {
                var loggerFactory = LoggerFactory.Create(builder =>
                {
                    builder
                        .AddFilter((category, level) =>
                            category == DbLoggerCategory.Database.Command.Name && level == LogLevel.Information)
                        .AddConsole();
                });
                options.UseLoggerFactory(loggerFactory).EnableSensitiveDataLogging();
            }
        });
        services.AddScoped<IUnitOfWork, UnitOfWork>(provider =>
        {
            var personnelDbContext = ResolveService<PersonnelDbContext>(provider);
            return new UnitOfWork(personnelDbContext);
        });
        services.AddScoped<IEmployeeService, EmployeeService>();
        services.AddScoped<IRoleService, RoleService>();
    }

    private static T ResolveService<T>(IServiceProvider provider)
    {
        var serviceScope = provider.CreateScope();
        var services = serviceScope.ServiceProvider;
        return services.GetRequiredService<T>();
    }
}