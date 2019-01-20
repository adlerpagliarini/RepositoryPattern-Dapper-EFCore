using System.Collections.Generic;
using System.Threading.Tasks;

namespace Application.Interfaces.Services.Standard
{
    public interface IServiceBase<TEntity> where TEntity : class
    {
        Task<TEntity> AddAsync(TEntity obj);
        Task AddRangeAsync(IEnumerable<TEntity> entities);

        Task<TEntity> GetByIdAsync(object id);
        Task<IEnumerable<TEntity>> GetAllAsync();

        Task UpdateAsync(TEntity obj);
        Task UpdateRangeAsync(IEnumerable<TEntity> entities);

        Task<bool> RemoveAsync(object id);
        Task RemoveAsync(TEntity obj);
        Task RemoveRangeAsync(IEnumerable<TEntity> entities);
    }
}