using Infrastructure.Interfaces.Repositories.Standard;
using Microsoft.Extensions.Options;
using MongoDB.Driver;
using System;
using System.Threading.Tasks;

namespace Infrastructure.DBConfiguration.Mongo
{
    public class MongoContext : IRepositoryTransaction, IDisposable
    {
        public IMongoClient MongoClient { get; private set; }
        public IMongoDatabase MongoDatabase { get; private set; }
        public IClientSessionHandle MongoSession { get; private set; }

        public MongoContext()
            => Init(DatabaseConnection.ConnectionConfiguration.GetSection("MongoDB")["DefaultConnection"],
                    DatabaseConnection.ConnectionConfiguration.GetSection("MongoDB")["DBName"]);

        public MongoContext(IOptions<MongoSettings> mongoSettings)
            => Init(mongoSettings.Value.DefaultConnection, mongoSettings.Value.DBName);

        public IMongoCollection<T> GetCollection<T>(string name) => MongoDatabase.GetCollection<T>(name);

        private void Init(string connectionString, string database)
        {
            MongoClient = new MongoClient(connectionString);
            MongoDatabase = MongoClient.GetDatabase(database);
            MongoSession = MongoClient.StartSession();
        }

        public void StartTransaction()
        {
            MongoSession.StartTransaction();
        }

        public async Task AbortTransactionAsync()
        {
            await MongoSession.AbortTransactionAsync();
        }

        public async Task CommitTransactionAsync()
        {
            await MongoSession.CommitTransactionAsync();
        }

        public void Dispose()
        {
            MongoSession.Dispose();
        }
    }
}
