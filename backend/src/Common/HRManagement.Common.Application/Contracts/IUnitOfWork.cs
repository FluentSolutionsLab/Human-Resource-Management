using HRManagement.Common.Domain.Models;

namespace HRManagement.Common.Application.Contracts;

public interface IUnitOfWork
{
    IGenericRepository<TEntity, TId> GetRepository<TEntity, TId>() where TEntity : Entity<TId> where TId : struct;
    Task<bool> SaveChangesAsync();
}