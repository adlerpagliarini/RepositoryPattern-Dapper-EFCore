using Application.Interfaces.Services.Standard;
using Domain.Entities;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Application.Interfaces.Services.Domain
{
    public interface IUserService : IServiceBase<User>
    {
        Task<IEnumerable<User>> GetAllIncludingTasksAsync();
        Task<User> GetByIdIncludingTasksAsync(int id);
    }
}
