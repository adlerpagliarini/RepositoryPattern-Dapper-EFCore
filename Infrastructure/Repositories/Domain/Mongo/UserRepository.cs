using Domain.Entities;
using Infrastructure.DBConfiguration.Mongo;
using Infrastructure.Interfaces.Repositories.Domain;
using Infrastructure.Interfaces.Repositories.Standard;
using Infrastructure.Repositories.Standard.Mongo;
using MongoDB.Bson;
using MongoDB.Driver;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Infrastructure.Repositories.Domain.Mongo
{
    public class UserRepository : DomainRepository<User>, IUserRepository
    {
        public UserRepository(MongoContext mongoContext) : base(mongoContext) { }

        public async Task<IEnumerable<User>> GetAllIncludingTasksAsync() => await GetAllAsync();

        public async Task<User> GetByIdIncludingTasksAsync(Guid id) => await GetByIdAsync(id);
    }
}
