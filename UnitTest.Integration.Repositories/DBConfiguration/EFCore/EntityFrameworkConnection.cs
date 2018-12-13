using Infrastructure.DBConfiguration.EFCore;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using System;

namespace UnitTest.Integration.Repositories.DBConfiguration.EFCore
{
    public class EntityFrameworkConnection
    {
        private IServiceProvider _provider;

        public ApplicationContext DataBaseConfiguration()
        {            
            var services = new ServiceCollection();
            services.AddDbContext<ApplicationContext>(options => options.UseSqlServer(DatabaseConnection.ConnectionConfiguration.Value.DefaultConnection));
            _provider = services.BuildServiceProvider();
            return _provider.GetService<ApplicationContext>();
        }
    }
}
