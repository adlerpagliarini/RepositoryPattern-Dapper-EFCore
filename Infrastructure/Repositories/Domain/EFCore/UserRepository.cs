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
        public UserRepository(ApplicationContext dbContext) : base(dbContext)
        {

        }

        public async Task<IEnumerable<User>> GetAllIncludingTasksAsync()
        {
            IQueryable<User> query = await Task.FromResult(GenerateQuery(filter: null,
                                                                         orderBy: null, 
                                                                         includeProperties: nameof(User.TasksToDo)));
            return query.AsEnumerable();
        }

        public async Task<User> GetByIdIncludingTasksAsync(int id)
        {
            IQueryable<User> query = await Task.FromResult(GenerateQuery(filter: (user => user.Id == id),
                                                                         orderBy: null,
                                                                         includeProperties: nameof(User.TasksToDo)));
            return query.SingleOrDefault();
        }
    }
}
