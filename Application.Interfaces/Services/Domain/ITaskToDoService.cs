using Application.Interfaces.Services.Standard;
using Domain.Entities;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Application.Interfaces.Services.Domain
{
    public interface ITaskToDoService : IServiceBase<TaskToDo>
    {
        Task UpdateStatusAsync(Guid id, bool status);
        Task<IEnumerable<TaskToDo>> GetAllIncludingUserAsync();
        Task<TaskToDo> GetByIdIncludingUserAsync(Guid id);
    }
}
