using System.Linq.Expressions;
using HRManagement.Common.Application.Models;
using HRManagement.Common.Domain.Models;

namespace HRManagement.Common.Application.Contracts;

public interface IGenericRepository<TEntity, TId> where TEntity : Entity<TId> where TId : struct
{
    Task<TEntity> GetByIdAsync(TId id);
    Task<TEntity> GetByIdAsync(TId id, string includeProperties = "");

    Task<PagedList<TEntity>> GetAsync(
        Expression<Func<TEntity, bool>> filter = null,
        Func<IQueryable<TEntity>, IOrderedQueryable<TEntity>> orderBy = null,
        string includeProperties = "",
        int pageNumber = 1,
        int pageSize = 10);

    Task AddAsync(TEntity entity);
    void Update(TEntity entity);
    void Delete(TEntity entity);
}