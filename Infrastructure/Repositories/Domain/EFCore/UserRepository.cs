using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Domain.Entities;
using Infrastructure.DBConfiguration.EFCore;
using Infrastructure.Interfaces.Repositories.Domain;
using Infrastructure.Repositories.Standard.EFCore;

namespace Infrastructure.Repositories.Domain.EFCore
{
    public class UserRepository : DomainRepository<User>,
                                  IUserRepository
    {
        private readonly ApplicationContext _dbContext;

        public UserRepository(ApplicationContext dbContext) : base(dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task<IEnumerable<User>> GetAllIncludingTasksAsync()
        {
            IQueryable<User> query = await Task.FromResult(GenerateQuery(filter: null,
                                                                         orderBy: null, 
                                                                         includeProperties: nameof(User.TasksToDo)));
            return query.AsEnumerable();
        }

        public async Task<User> GetByIdIncludingTasksAsync(Guid id)
        {
            IQueryable<User> query = await Task.FromResult(GenerateQuery(filter: (user => user.Id == id),
                                                                         orderBy: null,
                                                                         includeProperties: nameof(User.TasksToDo)));
            return query.SingleOrDefault();
        }

        //public async Task<TaskToDo> GetTaskByIdAsync(Guid id) => await Task.FromResult(_dbContext.TaskToDo.Where(t => t.Id == id).SingleOrDefault());
    }
}
