using System.Reflection;
using HRManagement.BuildingBlocks.Contracts;
using HRManagement.BuildingBlocks.Models;
using HRManagement.BuildingBlocks.Repositories;
using HRManagement.Modules.Staff.Data;
using HRManagement.Modules.Staff.Features.Services;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;

namespace HRManagement.Modules.Staff;

public static class StaffManagementModule
{
    public static void AddStaffManagementModule(this IServiceCollection services)
    {
        services.AddMediatR(Assembly.GetExecutingAssembly());
        services.AddDbContext<StaffDbContext>(options =>
        {
            var connectionString = ResolveService<IOptions<AppSettings>>(services.BuildServiceProvider()).Value
                .ConnectionStrings.Default;
            options.UseSqlServer(connectionString);
        });
        services.AddScoped<IUnitOfWork, UnitOfWork>(provider =>
        {
            var personnelDbContext = ResolveService<StaffDbContext>(provider);
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