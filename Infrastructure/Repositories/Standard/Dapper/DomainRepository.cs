using Domain.Entities;
using Infrastructure.Interfaces.DBConfiguration;
using Infrastructure.Interfaces.Repositories.Domain.Standard;
using System.Data;

namespace Infrastructure.Repositories.Standard.Dapper
{
    public abstract class DomainRepository<TEntity> : RepositoryAsync<TEntity>,
                                                      IDomainRepository<TEntity> where TEntity : class, IIdentityEntity
    {
        protected DomainRepository(IDatabaseFactory databaseOptions) : base(databaseOptions)
        {
        }

        protected DomainRepository(IDbConnection databaseConnection, IDbTransaction transaction = null) : base(databaseConnection, transaction)
        {
        }
    }
}
