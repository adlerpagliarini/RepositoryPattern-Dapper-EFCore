using Domain.Entities;
using Infrastructure.Interfaces.Repositories.Standard;
using System.Threading.Tasks;

namespace Infrastructure.Interfaces.Repositories.Mongo
{
    public interface IMongoRepositoryAsync<TEntity> : IRepositoryTransaction, IRepositoryAsync<TEntity> where TEntity : class, IIdentityEntity
    {
    }
}
