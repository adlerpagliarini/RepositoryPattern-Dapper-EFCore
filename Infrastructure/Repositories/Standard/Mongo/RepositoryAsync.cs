using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Domain.Entities;
using Infrastructure.DBConfiguration.Mongo;
using Infrastructure.Interfaces.Repositories.Mongo;
using MongoDB.Bson;
using MongoDB.Driver;

namespace Infrastructure.Repositories.Standard.Mongo
{
    public class RepositoryAsync<TEntity> : IMongoRepositoryAsync<TEntity> where TEntity : class, IIdentityEntity
    {
        protected readonly MongoContext dbContext;
        protected IClientSessionHandle mongoSession;
        protected readonly IMongoCollection<TEntity> dbSet;

        protected RepositoryAsync(MongoContext mongoContext)
        {
            dbContext = mongoContext;
            mongoSession = dbContext.MongoClient.StartSession();
            dbSet = dbContext.GetCollection<TEntity>(typeof(TEntity).Name);
        }

        public async Task<TEntity> AddAsync(TEntity obj)
        {
            await dbSet.InsertOneAsync(mongoSession, obj);       
            return obj;
        }

        public async Task<int> AddRangeAsync(IEnumerable<TEntity> entities)
        {
            await dbSet.InsertManyAsync(mongoSession, entities);
            return entities.Count();
        }

        public async Task<IEnumerable<TEntity>> GetAllAsync()
        {
            var result = await dbSet.FindAsync(mongoSession, e => true);
            return await result.ToListAsync();
        }

        public async Task<TEntity> GetByIdAsync(object id)
        {
            var result = await dbSet.FindAsync(mongoSession, e => e.Id == ((Guid)id));
            return result.FirstOrDefault();
        }

        public async Task<int> UpdateAsync(TEntity obj)
        {
            var filter = new FilterDefinitionBuilder<TEntity>().Eq(e => e.Id, obj.Id);
            var result = await dbSet.FindOneAndReplaceAsync(mongoSession, filter, obj);
            return result.Id == obj.Id ? 1 : 0;
        }

        public async Task<int> UpdateRangeAsync(IEnumerable<TEntity> entities)
        {
            int count = 0;
            foreach (var entity in entities)
            {
                var filter = new FilterDefinitionBuilder<TEntity>().Eq(e => e.Id, entity.Id);
                var result = await dbSet.FindOneAndReplaceAsync(mongoSession, filter, entity);
                count = result.Id == entity.Id ? count + 1 : count;
            }
            return count;
        }

        public async Task<bool> RemoveAsync(object id)
        {
            var filter = new FilterDefinitionBuilder<TEntity>().Eq(e => e.Id, ((Guid)id));
            var deletedResult = await dbSet.DeleteOneAsync(mongoSession, filter);
            return deletedResult.DeletedCount > 0 ? true : false;
        }

        public async Task<int> RemoveAsync(TEntity obj)
        {
            var deletedResult = await dbSet.DeleteOneAsync(mongoSession, e => e.Id == obj.Id);
            return (int) deletedResult.DeletedCount;
        }

        public async Task<int> RemoveRangeAsync(IEnumerable<TEntity> entities)
        {
            int deletedCount = 0;
            foreach (var entity in entities)
            {
                var deletedEntity = await dbSet.DeleteOneAsync(mongoSession, Builders<TEntity>.Filter.Eq("Id", entity.Id));
                deletedCount += (int) deletedEntity.DeletedCount;
            }
            return deletedCount;
        }

        public void StartTransaction()
        {
            mongoSession.StartTransaction();
        }

        public async Task AbortTransactionAsync()
        {
            await mongoSession.AbortTransactionAsync();
        }

        public async Task CommitTransactionAsync()
        {
            await mongoSession.CommitTransactionAsync();
        }

        public void Dispose()
        {
            GC.SuppressFinalize(this);
            mongoSession.Dispose();
        }
    }
}
