using Infrastructure.Interfaces.DBConfiguration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using System;

namespace UnitTest.Integration.Repositories.DBConfiguration.Dapper
{
    public class DapperConnection
    {
        private IServiceProvider _provider;

        public IOptions<Infrastructure.DBConfiguration.Dapper.DataSettings> DataSettings()
        {
            var services = new ServiceCollection();
            services.AddTransient(
                provider => Options.Create(
                        new Infrastructure.DBConfiguration.Dapper.DataSettings
                        {
                            DefaultConnection = DatabaseConnection.ConnectionConfiguration.Value.DefaultConnection
                        }
             ));
            _provider = services.BuildServiceProvider();
            return _provider.GetService<IOptions<Infrastructure.DBConfiguration.Dapper.DataSettings>>();
        }

        public IDatabaseFactory DatabaseFactory()
        {
            var services = new ServiceCollection();
            services.AddTransient<IDatabaseFactory, Infrastructure.DBConfiguration.Dapper.DatabaseFactory>(_ => new
                                    Infrastructure.DBConfiguration.Dapper.DatabaseFactory(this.DataSettings()));
            _provider = services.BuildServiceProvider();
            return _provider.GetService<IDatabaseFactory>();
        }
    }
}
