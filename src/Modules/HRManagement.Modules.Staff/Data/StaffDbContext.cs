using HRManagement.Modules.Staff.Models;
using Microsoft.EntityFrameworkCore;

namespace HRManagement.Modules.Staff.Data;

public class StaffDbContext(DbContextOptions<StaffDbContext> options) : DbContext(options)
{
    public DbSet<Employee> Employees { get; set; }
    public DbSet<Role> Roles { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.ApplyConfigurationsFromAssembly(typeof(StaffDbContext).Assembly);
    }
}