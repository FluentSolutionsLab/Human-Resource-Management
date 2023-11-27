using System.Linq.Expressions;
using CSharpFunctionalExtensions;
using HRManagement.Common.Application.Models;

namespace HRManagement.Common.Application.Contracts;

public interface IGenericRepository<TEntity, TId> where TEntity : Domain.Models.Entity<TId> where TId : struct
{
    Task<Maybe<TEntity>> GetByIdAsync(TId id);
    Task<Maybe<TEntity>> GetByIdAsync(TId id, string includeProperties);
    Task<Result<bool>> HasMatches(Expression<Func<TEntity, bool>> predicate);

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