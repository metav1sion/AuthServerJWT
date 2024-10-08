﻿using System.Linq.Expressions;

namespace AuthServer.Core.Repositories;

public interface IGenericRepository<TEntity> where TEntity : class
{
    Task<TEntity> GetByIdAsync(int id);
    Task<IEnumerable<TEntity>> GetAllAsync();
    IQueryable<TEntity> Where(Expression<Func<TEntity, bool>>predicate); //TEntity alan ve bool dönen bir metot girilecek içine.
    Task AddAsync(TEntity entity);
    void Remove(TEntity entity);
    TEntity Update(TEntity entity);
}