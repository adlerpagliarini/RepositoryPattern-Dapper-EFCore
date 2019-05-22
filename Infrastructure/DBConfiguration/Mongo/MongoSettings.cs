using Infrastructure.Interfaces.DBConfiguration;

namespace Infrastructure.DBConfiguration.Mongo
{
    public class MongoSettings : IMongoSettings
    {
        public string DefaultConnection { get; set; }
        public string DBName { get; set; }
    }
}
