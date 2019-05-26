using Domain.Entities;
using Infrastructure.Interfaces.Repositories.Standard;
using Microsoft.Extensions.Options;
using MongoDB.Bson;
using MongoDB.Bson.Serialization;
using MongoDB.Driver;
using System;
using System.Collections.ObjectModel;
using System.Linq;
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
            RegisterConventions();
        }

        private void RegisterConventions()
        {
            MongoDefaults.GuidRepresentation = GuidRepresentation.Standard;
        }

        public static void ConfigureClassMaps()
        {
            BsonClassMap.RegisterClassMap<User>(map =>
            {
                map.AutoMap();
                map.MapCreator(x => User.UserFactory.NewUserFactory(x.Id, x.TasksToDo.ToArray()));
                map.MapIdMember(x => x.Id);
                map.MapMember(x => x.Name).SetIsRequired(true);
                //map.SetIgnoreExtraElements(true);
                map.MapMember(x => x.TasksToDo).SetElementName(nameof(TaskToDo));
            });

            BsonClassMap.RegisterClassMap<TaskToDo>(map =>
            {
                map.AutoMap();
                //map.UnmapMember(x => x.Id);
                map.UnmapMember(x => x.User);
                map.UnmapMember(x => x.UserId);
            });
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
