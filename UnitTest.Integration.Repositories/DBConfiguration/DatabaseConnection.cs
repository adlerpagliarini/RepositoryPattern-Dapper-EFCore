using Infrastructure.DBConfiguration.Mongo;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Options;
using System.IO;

namespace UnitTest.Integration.Repositories.DBConfiguration
{
    public class DatabaseConnection
    {
        public static IOptions<DataOptionFactory> ConnectionConfiguration
        {
            get
            {
                IConfigurationRoot Configuration = new ConfigurationBuilder()
                    .SetBasePath(Directory.GetCurrentDirectory())
                    .AddJsonFile("appsettings.test.json")
                    .Build();               
                return Options.Create(Configuration.GetSection("ConnectionStrings").Get<DataOptionFactory>());        
            }
        }

        public static IOptions<MongoSettings> MongoDBConfiguration
        {
            get
            {
                IConfigurationRoot Configuration = new ConfigurationBuilder()
                    .SetBasePath(Directory.GetCurrentDirectory())
                    .AddJsonFile("appsettings.test.json")
                    .Build();
                return Options.Create(Configuration.GetSection("MongoDB").Get<MongoSettings>());
            }
        }
    }
}
