using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Threading.Tasks;
using CentralConfiguration.Cms.Data.Entity;

namespace CentralConfiguration.Cms.Data.Repository
{
    public interface IRepository<TEntity, in TId> where TEntity : IEntity<TId>
    {
        Task<TEntity> GetOneAsync(Expression<Func<TEntity, bool>> predicate);
        Task<List<TEntity>> GetListAsync(Expression<Func<TEntity, bool>> predicate);
        Task<TEntity> InsertOneAsync(TEntity entity);
        Task<TEntity> UpdateOneAsync(TEntity entity);
        Task DeleteAsync(TId id);
    }
}
