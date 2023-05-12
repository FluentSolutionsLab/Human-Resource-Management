using System;
using System.Collections;
using System.Threading.Tasks;
using HRManagement.Common.Application.Contracts;
using HRManagement.Common.Domain.Models;
using HRManagement.Common.Pertinence.Repositories;
using HRManagement.Modules.Personnel.Application.Contracts;

namespace HRManagement.Modules.Personnel.Persistence.Repositories;

public class GenericUnitOfWork : IGenericUnitOfWork, IDisposable
{
    private readonly PersonnelDbContext _dbContext;
    private bool _disposed;
    private Hashtable _repositories;

    public GenericUnitOfWork(PersonnelDbContext dbContext)
    {
        _dbContext = dbContext ?? throw new ArgumentNullException(nameof(dbContext));
    }

    public IGenericRepository<TEntity, TId> GetRepository<TEntity, TId>() where TEntity : Entity<TId> where TId : struct
    {
        _repositories ??= new Hashtable();

        var type = typeof(TEntity).Name;

        if (_repositories.ContainsKey(type))
        {
            return (IGenericRepository<TEntity, TId>)_repositories[type];
        }

        var repositoryType = typeof(GenericRepository<,>);
        _repositories.Add(type, Activator.CreateInstance(repositoryType.MakeGenericType(typeof(TEntity), typeof(TId)), _dbContext));

        return (IGenericRepository<TEntity, TId>)_repositories[type];
    }

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
    
    public void Dispose()
    {
        Dispose(true);
        GC.SuppressFinalize(this);
    }

    private void Dispose(bool disposing)
    {
        if (!_disposed && disposing)
            _dbContext.Dispose();
        _disposed = true;
    }
}