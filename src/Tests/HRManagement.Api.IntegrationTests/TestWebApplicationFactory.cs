using HRManagement.Modules.Personnel.Persistence;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.Extensions.Hosting;

namespace HRManagement.Api.IntegrationTests;

public class TestWebApplicationFactory<TProgram> : WebApplicationFactory<TProgram> where TProgram : class
{
    protected override IHost CreateHost(IHostBuilder builder)
    {
        builder.ConfigureServices(services =>
        {
            services.RemoveAll(typeof(PersonnelDbContext));
            services.AddDbContext<PersonnelDbContext>(options =>
            {
                options.UseInMemoryDatabase("PersonnelDbContextInMemoryTest");
                options.UseLazyLoadingProxies();
                options.EnableSensitiveDataLogging();
            });

            var sp = services.BuildServiceProvider();

            using var scope = sp.CreateScope();
            var scopedServices = scope.ServiceProvider;
            var context = scopedServices.GetRequiredService<PersonnelDbContext>();
            context.Database.EnsureCreated();
            Task.FromResult(() => DatabaseInitializer.InitializeAsync(context));
        });

        return base.CreateHost(builder);
    }
}