using Domain.Entities;
using Infrastructure.DBConfiguration.Mongo;
using Infrastructure.Interfaces.Repositories.Domain;
using Infrastructure.Repositories.Standard.Mongo;
using MongoDB.Driver;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Infrastructure.Repositories.Domain.Mongo
{
    public class UserRepository : DomainRepository<User>, IUserRepository
    {
        public UserRepository(MongoContext mongoContext) : base(mongoContext) { }

        public async Task<IEnumerable<User>> GetAllIncludingTasksAsync() => await GetAllAsync();

        public async Task<User> GetByIdIncludingTasksAsync(Guid id) => await GetByIdAsync(id);

        public async override Task<int> UpdateAsync(User obj)
        {
            var filter = new FilterDefinitionBuilder<User>().Eq(e => e.Id, obj.Id);
            var updateCommand = Builders<User>.Update.Set(nameof(User.Name), obj.Name);
            var result = await dbSet.UpdateOneAsync(mongoSession, filter, updateCommand);
            return (int)result.ModifiedCount;
        }

        public async override Task<int> UpdateRangeAsync(IEnumerable<User> entities)
        {
            var updates = new List<WriteModel<User>>();
            entities.All(entity =>
            {
                var filter = new FilterDefinitionBuilder<User>().Eq(e => e.Id, entity.Id);
                var updateCommand = Builders<User>.Update.Set(nameof(User.Name), entity.Name);
                updates.Add(new UpdateOneModel<User>(filter, updateCommand));
                return true;
            });

            var result = await dbSet.BulkWriteAsync(mongoSession, updates);
            return (int)result.ModifiedCount;
        }
    }
}
