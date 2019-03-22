using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Threading.Tasks;
using CentralConfiguration.Cms.Data.Entity;
using Microsoft.Extensions.Configuration;
using MongoDB.Driver;

namespace CentralConfiguration.Cms.Data.Repository
{
    // ReSharper disable once UnusedTypeParameter
    public class BaseMongoRepository<TEntity, TId> : IRepository<TEntity, string> where TEntity : BaseMongoEntity
    {
        private readonly IMongoCollection<TEntity> _collection;

        public BaseMongoRepository(IConfiguration configuration)
        {
            IMongoClient client = new MongoClient(configuration["MongoConnection:ConnectionString"]);
            _collection = client.GetDatabase(configuration["MongoConnection:Database"]).GetCollection<TEntity>($"{typeof(TEntity).Name}s");
        }

        public async Task<TEntity> GetOneAsync(Expression<Func<TEntity, bool>> predicate)
        {
            return await _collection.Find(predicate).FirstOrDefaultAsync();
        }

        public async Task<List<TEntity>> GetListAsync(Expression<Func<TEntity, bool>> predicate)
        {
            return await _collection.Find(predicate).ToListAsync();
        }

        public async Task<TEntity> UpdateOneAsync(TEntity entity)
        {
            await _collection.ReplaceOneAsync(x => x.Id.Equals(entity.Id), entity);
            return entity;
        }
        public async Task<TEntity> InsertOneAsync(TEntity entity)
        {
            await _collection.InsertOneAsync(entity);
            return entity;
        }
        public async Task DeleteAsync(string id)
        {
            await _collection.DeleteOneAsync(x => x.Id.Equals(id));
        }

    }
}
