namespace HRManagement.Common.Application.Contracts;

public interface IUnitOfWork
{
    IGenericRepository<TEntity, TId> GetRepository<TEntity, TId>() where TEntity : Common.Domain.Models.Entity<TId> where TId : struct;
    Task<bool> SaveChangesAsync();
}