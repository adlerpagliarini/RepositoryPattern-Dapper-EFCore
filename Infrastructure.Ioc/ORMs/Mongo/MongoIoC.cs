using Infrastructure.DBConfiguration.Mongo;
using Infrastructure.Interfaces.Repositories.Domain;
using Infrastructure.Interfaces.Repositories.Standard;
using Infrastructure.Repositories.Domain.Mongo;
using Infrastructure.Repositories.Standard.Mongo;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Infrastructure.IoC
{
    public class MongoIoC : OrmTypes
    {
        internal override IServiceCollection AddOrm(IServiceCollection services, IConfiguration configuration = null)
        {
            IConfiguration dbConnectionSettings = ResolveConfiguration.GetConnectionSettings(configuration);
            var mongoSettings = dbConnectionSettings.GetSection("MongoDB");

            services.Configure<MongoSettings>(mongoSettings);
            services.AddScoped<MongoContext>();

            MongoSettings.ConfigureMongoConventions();

            services.AddScoped(typeof(IRepositoryAsync<>), typeof(RepositoryAsync<>));
            services.AddScoped<IUserRepository, UserRepository>();

            services.AddScoped<ITaskToDoRepository, TaskToDoRepository>();

            return services;
        }
    }
}
