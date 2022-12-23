using System.Reflection;
using MediatR;
using Microsoft.Extensions.DependencyInjection;

namespace HRManagement.Modules.Personnel.Application;

public static class ApplicationServicesRegistration
{
    public static void AddApplicationServices(this IServiceCollection services)
    {
        services.AddAutoMapper(Assembly.GetExecutingAssembly());
        services.AddMediatR(Assembly.GetExecutingAssembly());
    }
}