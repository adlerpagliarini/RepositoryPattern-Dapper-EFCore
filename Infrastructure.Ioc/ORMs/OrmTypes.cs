using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Infrastructure.IoC
{
    public abstract class OrmTypes : IOrmTypes
    {
        internal abstract IServiceCollection AddOrm(IServiceCollection services, IConfiguration configuration = null);

        public IServiceCollection ResolveOrm(IServiceCollection services, IConfiguration configuration = null)
        {
            return AddOrm(services, configuration);
        }
    }
}
