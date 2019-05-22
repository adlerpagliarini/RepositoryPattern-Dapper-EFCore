using Domain.Entities;
using Infrastructure.Interfaces.Repositories.Mongo;

namespace Infrastructure.Interfaces.Repositories.Domain.Standard
{
    public interface IMongoDomainRepository<TEntity> : IMongoRepositoryAsync<TEntity> where TEntity : class, IIdentityEntity
    {
    }
}
