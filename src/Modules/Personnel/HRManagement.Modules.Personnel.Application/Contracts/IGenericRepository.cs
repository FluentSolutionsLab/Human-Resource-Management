using System.Linq.Expressions;
using HRManagement.Common.Domain.Models;

namespace HRManagement.Modules.Personnel.Application.Contracts;

public interface IGenericRepository<TEntity, TId> where TEntity : Entity<TId> where TId : struct
{
    Task<TEntity> GetByIdAsync(TId id);

    Task<IReadOnlyList<TEntity>> GetAsync(Expression<Func<TEntity, bool>>? filter = null,
        Func<IQueryable<TEntity>, IOrderedQueryable<TEntity>>? orderBy = null, string includeProperties = "");

    Task AddAsync(TEntity entity);
    void Update(TEntity entity);
    void Delete(TEntity entity);
    Task CommitAsync();
}