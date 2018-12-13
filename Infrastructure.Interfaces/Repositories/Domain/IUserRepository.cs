using Domain.Entities;
using Infrastructure.Interfaces.Repositories.Domain.Standard;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Infrastructure.Interfaces.Repositories.Domain
{
    public interface IUserRepository : IDomainRepository<User>
    {
        Task<IEnumerable<User>> GetAllIncludingTasksAsync();
        Task<User> GetByIdIncludingTasksAsync(int id);
    }
}
