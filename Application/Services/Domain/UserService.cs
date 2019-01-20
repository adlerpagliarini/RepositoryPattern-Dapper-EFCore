using System.Collections.Generic;
using System.Threading.Tasks;
using Application.Interfaces.Services.Domain;
using Application.Services.Standard;
using Domain.Entities;
using Infrastructure.Interfaces.Repositories.Domain;

namespace Application.Services.Domain
{
    public class UserService : ServiceBase<User>, 
                               IUserService
    {
        private readonly IUserRepository _repository;

        public UserService(IUserRepository repository) : base(repository)
        {
            _repository = repository;
        }

        public async Task<IEnumerable<User>> GetAllIncludingTasksAsync()
        {
            return await _repository.GetAllIncludingTasksAsync();
        }

        public async Task<User> GetByIdIncludingTasksAsync(int id)
        {
            return await _repository.GetByIdIncludingTasksAsync(id);
        }
    }
}
