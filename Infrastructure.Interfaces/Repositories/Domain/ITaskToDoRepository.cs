using Domain.Entities;
using Infrastructure.Interfaces.Repositories.Domain.Standard;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Infrastructure.Interfaces.Repositories.Domain
{
    public interface ITaskToDoRepository : IDomainRepository<TaskToDo>
    {
        Task<IEnumerable<TaskToDo>> GetAllIncludingUserAsync();
        Task<TaskToDo> GetByIdIncludingUserAsync(int id);
    }
}
