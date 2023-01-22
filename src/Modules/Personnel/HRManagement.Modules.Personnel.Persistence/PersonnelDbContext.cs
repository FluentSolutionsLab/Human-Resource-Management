using HRManagement.Common.Domain.Models;
using HRManagement.Modules.Personnel.Domain.Employee;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace HRManagement.Modules.Personnel.Persistence;

public class PersonnelDbContext : DbContext
{
    private readonly string _connectionString;
    private readonly bool _isDevEnvironment;

    public PersonnelDbContext(IOptions<AppSettings> settings, IConfiguration configuration)
    {
        _connectionString = settings.Value.ConnectionStrings.PersonnelManagement ?? throw new ArgumentNullException(nameof(settings));
        _isDevEnvironment = configuration.GetValue<string>("ASPNETCORE_ENVIRONMENT") == "Development";
    }

    public DbSet<Employee> Employees { get; set; }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        optionsBuilder.UseSqlServer(_connectionString).UseLazyLoadingProxies();
        if (_isDevEnvironment)
        {
            var loggerFactory = LoggerFactory.Create(builder =>
            {
                builder
                    .AddFilter((category, level) =>
                        category == DbLoggerCategory.Database.Command.Name && level == LogLevel.Information)
                    .AddConsole();
            });
            optionsBuilder.UseLoggerFactory(loggerFactory).EnableSensitiveDataLogging();
        }
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.ApplyConfigurationsFromAssembly(typeof(PersonnelDbContext).Assembly);
    }
}