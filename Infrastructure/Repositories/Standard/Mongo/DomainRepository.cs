using Domain.Entities;
using Infrastructure.DBConfiguration.Mongo;
using Infrastructure.Interfaces.Repositories.Domain.Standard;

namespace Infrastructure.Repositories.Standard.Mongo
{
    public class DomainRepository<TEntity> : RepositoryAsync<TEntity>,
                                             IMongoDomainRepository<TEntity> where TEntity : class, IIdentityEntity
    {
        public DomainRepository(MongoContext mongoContext) : base(mongoContext)
        {
        }
    }
}
