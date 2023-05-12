using System.Linq.Expressions;
using HRManagement.Common.Application.Contracts;
using HRManagement.Common.Application.Models;
using HRManagement.Common.Domain.Models;
using Microsoft.EntityFrameworkCore;

namespace HRManagement.Common.Pertinence.Repositories;

public class GenericRepository<TEntity, TId> : IGenericRepository<TEntity, TId>
    where TEntity : Entity<TId> where TId : struct
{
    private readonly DbContext _dbContext;

    private readonly DbSet<TEntity> _dbSet;

    public GenericRepository(DbContext dbContext)
    {
        _dbContext = dbContext;
        _dbSet = dbContext.Set<TEntity>();
    }

    public async Task<TEntity> GetByIdAsync(TId id)
    {
        return await _dbSet.FindAsync(id);
    }

    public async Task<PagedList<TEntity>> GetAsync(
        Expression<Func<TEntity, bool>> filter = null,
        Func<IQueryable<TEntity>, IOrderedQueryable<TEntity>> orderBy = null, 
        string includeProperties = "",
        int pageNumber = 1,
        int pageSize = 10)
    {
        IQueryable<TEntity> query = _dbSet;

        if (filter != null)
            query = query.Where(filter);

        foreach (var property in includeProperties.Split(new[] {','}, StringSplitOptions.RemoveEmptyEntries))
            query = query.Include(property);

        if (orderBy != null)
            query = orderBy(query);

        return await PagedList<TEntity>.CreateAsync(query, pageNumber, pageSize);
    }

    public async Task AddAsync(TEntity entity)
    {
        await _dbSet.AddAsync(entity);
    }

    public void Update(TEntity entity)
    {
        _dbSet.Attach(entity);
        _dbContext.Entry(entity).State = EntityState.Modified;
    }

    public void Delete(TEntity entity)
    {
        if (_dbContext.Entry(entity).State == EntityState.Detached)
            _dbSet.Attach(entity);
        _dbSet.Remove(entity);
    }
}