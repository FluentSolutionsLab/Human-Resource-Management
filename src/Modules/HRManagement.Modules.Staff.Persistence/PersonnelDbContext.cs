using HRManagement.Modules.Staff.Domain;
using Microsoft.EntityFrameworkCore;

namespace HRManagement.Modules.Staff.Persistence;

public class PersonnelDbContext : DbContext
{
    public PersonnelDbContext(DbContextOptions<PersonnelDbContext> options) : base(options)
    {
    }

    public DbSet<Employee> Employees { get; set; }
    public DbSet<Role> Roles { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.ApplyConfigurationsFromAssembly(typeof(PersonnelDbContext).Assembly);
    }
}