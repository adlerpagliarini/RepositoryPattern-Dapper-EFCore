using Domain.Entities;
using Infrastructure.DBConfiguration.Mongo;
using Infrastructure.Interfaces.Repositories.Standard;
using MongoDB.Driver;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Infrastructure.Repositories.Standard.Mongo
{
    public class RepositoryAsync<TEntity> : IRepositoryAsync<TEntity> where TEntity : class, IIdentityEntity
    {
        protected readonly MongoContext dbContext;
        protected IClientSessionHandle mongoSession;
        protected readonly IMongoCollection<TEntity> dbSet;

        protected RepositoryAsync(MongoContext mongoContext)
        {
            dbContext = mongoContext;
            mongoSession = mongoContext.MongoSession;
            dbSet = dbContext.GetCollection<TEntity>(typeof(TEntity).Name);
        }

        public virtual async Task<TEntity> AddAsync(TEntity obj)
        {
            await dbSet.InsertOneAsync(mongoSession, obj);
            return obj;
        }

        public virtual async Task<int> AddRangeAsync(IEnumerable<TEntity> entities)
        {
            await dbSet.InsertManyAsync(mongoSession, entities);
            return entities.Count();
        }

        public virtual async Task<IEnumerable<TEntity>> GetAllAsync()
        {
            var result = await dbSet.FindAsync(mongoSession, e => true);
            return await result.ToListAsync();
        }

        public virtual async Task<TEntity> GetByIdAsync(object id)
        {
            var result = await dbSet.FindAsync(mongoSession, e => e.Id == ((Guid)id));
            return result.FirstOrDefault();
        }

        public virtual async Task<int> UpdateAsync(TEntity obj)
        {
            var filter = new FilterDefinitionBuilder<TEntity>().Eq(e => e.Id, obj.Id);
            var result = await dbSet.FindOneAndReplaceAsync(mongoSession, filter, obj);
            return result.Id == obj.Id ? 1 : 0;
        }

        public virtual async Task<int> UpdateRangeAsync(IEnumerable<TEntity> entities)
        {
            var updates = new List<WriteModel<TEntity>>();
            entities.All(entity =>
            {
                var filter = new FilterDefinitionBuilder<TEntity>().Eq(e => e.Id, entity.Id);
                updates.Add(new ReplaceOneModel<TEntity>(filter, entity));
                return true;
            });

            var result = await dbSet.BulkWriteAsync(mongoSession, updates);
            return (int) result.ModifiedCount;
        }

        public virtual async Task<bool> RemoveAsync(object id)
        {
            var filter = new FilterDefinitionBuilder<TEntity>().Eq(e => e.Id, ((Guid)id));
            var deletedResult = await dbSet.DeleteOneAsync(mongoSession, filter);
            return deletedResult.DeletedCount > 0 ? true : false;
        }

        public virtual async Task<int> RemoveAsync(TEntity obj)
        {
            var deletedResult = await dbSet.DeleteOneAsync(mongoSession, e => e.Id == obj.Id);
            return (int)deletedResult.DeletedCount;
        }

        public virtual async Task<int> RemoveRangeAsync(IEnumerable<TEntity> entities)
        {
            var deletedEntitiesResult = await dbSet.DeleteManyAsync(mongoSession, Builders<TEntity>.Filter.In("Id", entities.Select(e => e.Id)));
            return (int) deletedEntitiesResult.DeletedCount;
        }

        public void Dispose()
        {
            GC.SuppressFinalize(this);
        }
    }
}
