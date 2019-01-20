using Application.Interfaces.Services.Standard;
using Domain.Entities;
using Infrastructure.Interfaces.Repositories.Standard;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Application.Services.Standard
{
    public class ServiceBase<TEntity> : IServiceBase<TEntity> where TEntity : class, IIdentityEntity
    {
        protected readonly IRepositoryAsync<TEntity> repository;

        public ServiceBase(IRepositoryAsync<TEntity> repository)
        {
            this.repository = repository;
        }
        public virtual async Task<TEntity> AddAsync(TEntity obj)
        {
            return await repository.AddAsync(obj);
        }

        public virtual async Task AddRangeAsync(IEnumerable<TEntity> entities)
        {
            await repository.AddRangeAsync(entities);
        }

        public virtual async Task<IEnumerable<TEntity>> GetAllAsync()
        {
            return await repository.GetAllAsync();
        }

        public virtual async Task<TEntity> GetByIdAsync(object id)
        {
            return await repository.GetByIdAsync(id);
        }

        public virtual async Task<bool> RemoveAsync(object id)
        {
            return await repository.RemoveAsync(id);
        }

        public virtual async Task RemoveAsync(TEntity obj)
        {
            await repository.RemoveAsync(obj);
        }

        public virtual async Task RemoveRangeAsync(IEnumerable<TEntity> entities)
        {
            await repository.RemoveRangeAsync(entities);
        }

        public virtual async Task UpdateAsync(TEntity obj)
        {
            await repository.UpdateAsync(obj);
        }

        public virtual async Task UpdateRangeAsync(IEnumerable<TEntity> entities)
        {
            await repository.UpdateRangeAsync(entities);
        }
    }
}
