using HRManagement.Modules.Personnel.Persistence;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace HRManagement.Api.IntegrationTests;

public class TestWebApplicationFactory<TProgram> : WebApplicationFactory<TProgram> where TProgram : class
{
    protected override IHost CreateHost(IHostBuilder builder)
    {
        builder.ConfigureServices(services =>
        {
            var descriptor = services.SingleOrDefault(d => d.ServiceType == typeof(DbContextOptions<PersonnelDbContext>));

            if (descriptor != null)
            {
                services.Remove(descriptor);
            }

            services.AddDbContext<PersonnelDbContext>(options =>
            {
                options.UseInMemoryDatabase("PersonnelDbContextInMemoryTest");
            });

            var sp = services.BuildServiceProvider();

            using var scope = sp.CreateScope();
            var scopedServices = scope.ServiceProvider;
            var context = scopedServices.GetRequiredService<PersonnelDbContext>();
            var logger = scopedServices.GetRequiredService<ILogger<WebApplicationFactory<TProgram>>>();

            context.Database.EnsureCreated();

            try
            {
                Task.FromResult(() => DatabaseInitializer.InitializeAsync(context));
            }
            catch (Exception ex)
            {
                logger.LogError(ex, $"An error occurred seeding the database with test messages. Error: {ex.Message}");
            }
        });

        return base.CreateHost(builder);
    }
}