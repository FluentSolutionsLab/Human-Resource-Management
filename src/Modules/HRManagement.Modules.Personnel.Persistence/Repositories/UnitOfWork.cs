using System;
using System.Threading.Tasks;
using HRManagement.Modules.Personnel.Application.Contracts;

namespace HRManagement.Modules.Personnel.Persistence.Repositories;

public class UnitOfWork : IUnitOfWork, IDisposable
{
    private readonly PersonnelDbContext _dbContext;
    private bool _disposed;

    public UnitOfWork(PersonnelDbContext dbContext)
    {
        _dbContext = dbContext;
        Roles = new RoleRepository(dbContext);
        Employees = new EmployeeRepository(dbContext);
    }

    public void Dispose()
    {
        Dispose(true);
        GC.SuppressFinalize(this);
    }

    public IRoleRepository Roles { get; }
    public IEmployeeRepository Employees { get; }

    public async Task<bool> SaveChangesAsync()
    {
        var isSaved = true;
        await using var transaction = await _dbContext.Database.BeginTransactionAsync();
        try
        {
            await _dbContext.SaveChangesAsync();
            await transaction.CommitAsync();
        }
        catch (Exception)
        {
            isSaved = false;
            await transaction.RollbackAsync();
        }

        return isSaved;
    }

    private void Dispose(bool disposing)
    {
        if (!_disposed && disposing)
            _dbContext.Dispose();
        _disposed = true;
    }
}