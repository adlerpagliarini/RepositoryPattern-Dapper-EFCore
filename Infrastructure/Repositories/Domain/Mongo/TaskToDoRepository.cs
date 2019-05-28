using Domain.Entities;
using Infrastructure.DBConfiguration.Mongo;
using Infrastructure.Interfaces.Repositories.Domain;
using MongoDB.Driver;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Infrastructure.Repositories.Domain.Mongo
{
    public class TaskToDoRepository : ITaskToDoRepository
    {
        protected readonly MongoContext dbContext;
        protected IClientSessionHandle mongoSession;
        protected readonly IMongoCollection<User> dbSet;

        public TaskToDoRepository(MongoContext mongoContext)
        {
            dbContext = mongoContext;
            mongoSession = mongoContext.MongoSession;
            dbSet = dbContext.GetCollection<User>(typeof(User).Name);
        }

        public async Task<TaskToDo> AddAsync(TaskToDo obj)
        {
            var filter = new FilterDefinitionBuilder<User>().Eq(e => e.Id, obj.UserId);
            var updateCommand = Builders<User>.Update.AddToSet(nameof(TaskToDo), obj);
            var result = await dbSet.UpdateOneAsync(mongoSession, filter, updateCommand);
            return result.ModifiedCount > 0 ? obj : new TaskToDo();
        }

        public async Task<int> AddRangeAsync(IEnumerable<TaskToDo> entities)
        {
            var filter = new FilterDefinitionBuilder<User>().Eq(e => e.Id, entities.FirstOrDefault()?.UserId);
            var updateCommand = Builders<User>.Update.AddToSetEach(nameof(TaskToDo), entities);
            var result = await dbSet.UpdateOneAsync(mongoSession, filter, updateCommand);
            return (result.ModifiedCount > 0) ? entities.Count() : 0;
        }

        public async Task<int> UpdateAsync(TaskToDo obj)
        {
            var filter = new FilterDefinitionBuilder<User>().And(
                new FilterDefinitionBuilder<User>().Eq(e => e.Id, obj.UserId),
                new FilterDefinitionBuilder<User>().Eq($"{nameof(TaskToDo)}._id", obj.Id));

            var updateCommand = Builders<User>.Update.Set($"{nameof(TaskToDo)}.$", obj);
            var result = await dbSet.UpdateOneAsync(mongoSession, filter, updateCommand);
            return (int)result.ModifiedCount;
        }

        public async Task<int> UpdateRangeAsync(IEnumerable<TaskToDo> entities)
        {
            return await Task.FromResult(entities.Aggregate(0, (acc, task) =>
            {
                var filter = new FilterDefinitionBuilder<User>().And(
                new FilterDefinitionBuilder<User>().Eq(e => e.Id, task.UserId),
                new FilterDefinitionBuilder<User>().Eq($"{nameof(TaskToDo)}._id", task.Id));

                var updateCommand = Builders<User>.Update.Set($"{nameof(TaskToDo)}.$", task);
                var result = dbSet.UpdateOneAsync(mongoSession, filter, updateCommand).Result;
                return acc += (int)result.ModifiedCount;
            }));
        }

        public async Task<bool> RemoveAsync(object id)
        {
            var filter = new FilterDefinitionBuilder<User>().And(
                new FilterDefinitionBuilder<User>().Eq($"{nameof(TaskToDo)}._id", (Guid)id));
            var updateCommand = Builders<User>.Update.PullFilter(nameof(TaskToDo), Builders<TaskToDo>.Filter.Eq(e => e.Id, (Guid)id));
            var result = await dbSet.UpdateOneAsync(mongoSession, filter, updateCommand);
            return (int)result.ModifiedCount > 0 ? true : false;
        }

        public async Task<int> RemoveAsync(TaskToDo obj)
        {
            var filter = new FilterDefinitionBuilder<User>().And(
                new FilterDefinitionBuilder<User>().Eq(e => e.Id, obj.UserId),
                new FilterDefinitionBuilder<User>().Eq($"{nameof(TaskToDo)}._id", obj.Id));

            var updateCommand = Builders<User>.Update.Pull(nameof(TaskToDo), obj);
            var result = await dbSet.UpdateOneAsync(mongoSession, filter, updateCommand);
            return (int)result.ModifiedCount;
        }

        public async Task<int> RemoveRangeAsync(IEnumerable<TaskToDo> entities)
        {
            var filter = new FilterDefinitionBuilder<User>().And(
                new FilterDefinitionBuilder<User>().Eq(e => e.Id, entities.FirstOrDefault()?.UserId),
                new FilterDefinitionBuilder<User>().In($"{nameof(TaskToDo)}._id", entities.Select(e => e.Id)));

            var updateCommand = Builders<User>.Update.PullAll(nameof(TaskToDo), entities);
            var result = await dbSet.UpdateManyAsync(mongoSession, filter, updateCommand);
            return (int)result.ModifiedCount;
        }

        public async Task<IEnumerable<TaskToDo>> GetAllAsync()
        {
            var users = await dbSet.FindAsync(mongoSession, e => true);
            var tasks = new List<TaskToDo>();
            users.ToList().All(user =>
            {
                tasks.AddRange(user.TasksToDo);
                return true;
            });
            return tasks;
        }

        public async Task<TaskToDo> GetByIdAsync(object id)
        {
            var filter = Builders<User>.Filter.Eq($"{nameof(TaskToDo)}._id", ((Guid)id));
            var user = (await dbSet.FindAsync(mongoSession, filter)).FirstOrDefault();
            var taskToDo = user.TasksToDo.Where(task => task.Id == (Guid)id);
            return taskToDo.FirstOrDefault();
        }

        public async Task<IEnumerable<TaskToDo>> GetAllIncludingUserAsync()
        {
            var users = await dbSet.FindAsync(mongoSession, e => true);
            var tasks = new List<TaskToDo>();
            users.ToList().All(user =>
            {
                user.TasksToDo.All(t =>
                {
                    t.User = user;
                    return true;
                });
                tasks.AddRange(user.TasksToDo);
                return true;
            });
            return tasks;
        }

        public async Task<TaskToDo> GetByIdIncludingUserAsync(Guid id)
        {
            var filter = Builders<User>.Filter.Eq($"{nameof(TaskToDo)}._id", ((Guid)id));
            var user = (await dbSet.FindAsync(mongoSession, filter)).FirstOrDefault();
            var taskToDo = user.TasksToDo.Where(task => task.Id == (Guid)id).FirstOrDefault();
            taskToDo.User = user;
            return taskToDo;
        }

        public void Dispose()
        {
            GC.SuppressFinalize(this);
        }
    }
}
