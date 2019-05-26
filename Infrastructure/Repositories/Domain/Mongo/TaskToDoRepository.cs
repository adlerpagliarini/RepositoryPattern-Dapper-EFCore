using Domain.Entities;
using Infrastructure.DBConfiguration.Mongo;
using Infrastructure.Interfaces.Repositories.Domain;
using Infrastructure.Interfaces.Repositories.Domain.Standard;
using Infrastructure.Repositories.Standard.Mongo;
using MongoDB.Driver;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Infrastructure.Repositories.Domain.Mongo
{
    public class TaskToDoRepository : DomainRepository<User>, IDomainRepository<TaskToDo>, ITaskToDoRepository
    {
        public TaskToDoRepository(MongoContext mongoContext) : base(mongoContext)
        {
        }

        public async Task<TaskToDo> AddAsync(TaskToDo obj)
        {
            var filter = new FilterDefinitionBuilder<User>().Eq(e => e.Id, obj.UserId);
            var updateCommand = Builders<User>.Update.Set(nameof(TaskToDo), obj);
            var result = await dbSet.UpdateOneAsync(mongoSession, filter, updateCommand);
            return result.ModifiedCount > 0 ? obj : new TaskToDo();
        }

        public async Task<int> AddRangeAsync(IEnumerable<TaskToDo> entities)
        {
            var filter = new FilterDefinitionBuilder<User>().Eq(e => e.Id, entities.FirstOrDefault()?.UserId);
            var updateCommand = Builders<User>.Update.Set(nameof(TaskToDo), entities);
            var result = await dbSet.UpdateOneAsync(mongoSession, filter, updateCommand);
            return (result.ModifiedCount > 0) ? entities.Count() : 0;
        }

        public async Task<int> UpdateAsync(TaskToDo obj)
        {
            var filter = new FilterDefinitionBuilder<User>().And(
                new FilterDefinitionBuilder<User>().Eq(e => e.Id, obj.UserId),
                new FilterDefinitionBuilder<User>().Eq($"{nameof(TaskToDo)}._id", obj.Id));

            var updateCommand = Builders<User>.Update.Set(nameof(TaskToDo), obj);
            var result = await dbSet.UpdateOneAsync(mongoSession, filter, updateCommand);
            return (int)result.ModifiedCount;
        }

        public async Task<int> UpdateRangeAsync(IEnumerable<TaskToDo> entities)
        {
            var filter = new FilterDefinitionBuilder<User>().And(
                new FilterDefinitionBuilder<User>().Eq(e => e.Id, entities.FirstOrDefault()?.UserId),
                new FilterDefinitionBuilder<User>().In($"{nameof(TaskToDo)}._id", entities.Select(e => e.Id)));

            var updateCommand = Builders<User>.Update.Set(nameof(TaskToDo), entities);
            var result = await dbSet.UpdateManyAsync(mongoSession, filter, updateCommand);
            return (int)result.ModifiedCount;
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

        public async new Task<IEnumerable<TaskToDo>> GetAllAsync()
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

        public async new Task<TaskToDo> GetByIdAsync(object id)
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
    }
}
