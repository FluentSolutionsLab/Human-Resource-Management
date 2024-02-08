using System.Reflection;
using MediatR;
using Microsoft.Extensions.DependencyInjection;

namespace HRManagement.Modules.Staff.Application;

public static class ApplicationServicesRegistration
{
    public static void AddApplicationServices(this IServiceCollection services)
    {
        services.AddMediatR(Assembly.GetExecutingAssembly());
    }
}