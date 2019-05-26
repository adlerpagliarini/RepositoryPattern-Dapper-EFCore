using Domain.Entities;
using Infrastructure.DBConfiguration.Mongo;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace UnitTest.Integration.Repositories.DBConfiguration.Mongo
{
    public class MongoConfiguration
    {
        private IServiceProvider _provider;

        public MongoContext DataBaseConfiguration()
        {
            var services = new ServiceCollection();
            services.AddTransient(_ => new MongoContext(DatabaseConnection.MongoDBConfiguration));
            _provider = services.BuildServiceProvider();
            return _provider.GetService<MongoContext>();
        }
    }

    public static class MongoSetUpConfiguration
    {
        private static bool instance;
        private static object syncRoot = new object();

        public static void SetUpConfiguration()
        {
            if (instance == false)
                lock (syncRoot)
                {
                    if (instance == false)
                    {
                        instance = true;
                        MongoContext.ConfigureClassMaps();
                        var mongoContext = new MongoConfiguration().DataBaseConfiguration();
                        mongoContext.MongoDatabase.DropCollection(nameof(User));
                        mongoContext.MongoDatabase.CreateCollection(nameof(User));
                    }
                }
        }
    }
}
