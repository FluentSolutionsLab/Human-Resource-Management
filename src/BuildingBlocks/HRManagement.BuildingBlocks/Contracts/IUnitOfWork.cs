using HRManagement.BuildingBlocks.Models;

namespace HRManagement.BuildingBlocks.Contracts;

public interface IUnitOfWork
{
    IGenericRepository<TEntity, TId> GetRepository<TEntity, TId>() where TEntity : Entity<TId> where TId : struct;
    Task<bool> SaveChangesAsync();
}