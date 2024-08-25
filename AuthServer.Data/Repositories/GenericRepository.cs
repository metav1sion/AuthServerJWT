using System.Linq.Expressions;
using AuthServer.Core.Repositories;
using Microsoft.EntityFrameworkCore;

namespace AuthServer.Data.Repositories;

public class GenericRepository<TEntity> : IGenericRepository<TEntity> where TEntity : class
{
    private readonly AppDbContext _appDbContext;
    private readonly DbSet<TEntity> _dbSet;
    public GenericRepository(AppDbContext appDbContext)
    {
        _appDbContext = appDbContext;
        _dbSet = appDbContext.Set<TEntity>();
    }

    public async Task<TEntity> GetByIdAsync(int id)
    {
        var entity = await _dbSet.FindAsync(id);
        if (entity != null)
        {
            _appDbContext.Entry(entity).State = EntityState.Detached; //memoryde takip edilmesini istemiyorum
        }

        return entity;
    }

    public async Task<IEnumerable<TEntity>> GetAllAsync()
    {
        return await _dbSet.ToListAsync();
    }

    public IQueryable<TEntity> Where(Expression<Func<TEntity, bool>> predicate)
    {
        var query = _dbSet.Where(predicate);
        return query;
    }

    public async Task AddAsync(TEntity entity)
    {
        await _dbSet.AddAsync(entity);
    }

    public void Remove(TEntity entity)
    {
        _dbSet.Remove(entity);
    }

    public TEntity Update(TEntity entity)
    {
        _appDbContext.Entry(entity).State = EntityState.Modified;
        return entity;
    }
}