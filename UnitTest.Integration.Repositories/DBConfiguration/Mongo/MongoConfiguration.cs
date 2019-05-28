using Domain.Entities;
using Infrastructure.DBConfiguration.Mongo;
using Microsoft.Extensions.DependencyInjection;
using System;

namespace UnitTest.Integration.Repositories.DBConfiguration.Mongo
{
    public static class MongoConfiguration
    {
        private static IServiceProvider _provider;

        public static MongoContext DataBaseConfiguration()
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
                        MongoSettings.ConfigureMongoConventions();
                        var mongoContext = MongoConfiguration.DataBaseConfiguration();
                        mongoContext.MongoDatabase.DropCollection(nameof(User));
                        mongoContext.MongoDatabase.CreateCollection(nameof(User));
                    }
                }
        }
    }
}
