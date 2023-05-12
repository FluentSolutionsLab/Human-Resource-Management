namespace HRManagement.Modules.Personnel.Application.Contracts;

public interface IGenericUnitOfWork
{
    IGenericRepository<TEntity, TId> GetRepository<TEntity, TId>() where TEntity : Common.Domain.Models.Entity<TId> where TId : struct;
    Task<bool> SaveChangesAsync();
}