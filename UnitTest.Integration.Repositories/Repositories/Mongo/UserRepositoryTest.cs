using Domain.Entities;
using Microsoft.EntityFrameworkCore.Storage;
using NUnit.Framework;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnitTest.Integration.Repositories.Repositories.DataBuilder;
using System.Linq;
using Infrastructure.DBConfiguration.Mongo;
using Infrastructure.Repositories.Standard.Mongo;
using Infrastructure.Interfaces.Repositories.Domain.Standard;
using UnitTest.Integration.Repositories.DBConfiguration;
using System;
using MongoDB.Driver;
using Infrastructure.Interfaces.Repositories.Domain;
using Infrastructure.Repositories.Domain.Mongo;

namespace UnitTest.Integration.Repositories.Repositories.Mongo
{
    [TestFixture]
    public class UserRepositoryTest
    {
        private MongoContext dbContext;

        private IMongoDomainRepository<User> userMongoRepository;
        private UserBuilder builder;

        [OneTimeSetUp]
        public void GlobalPrepare()
        {
            dbContext = new MongoContext(DatabaseConnection.MongoDBConfiguration);
            MongoContext.ConfigureClassMaps();
        }

        [SetUp]
        public void Inicializa()
        {
            builder = new UserBuilder();
            userMongoRepository = new UserRepository(dbContext);
            userMongoRepository.StartTransaction();
        }

        [TearDown]
        public async Task ExecutadoAposExecucaoDeCadaTeste()
        {
            await userMongoRepository.AbortTransactionAsync();
        }

        [Test]
        public async Task AddAsync()
        {            
            var user = builder.CreateUser();
            var inserted = await userMongoRepository.AddAsync(user);
            var result = await userMongoRepository.GetByIdAsync(inserted.Id);
            Assert.AreNotEqual(default(Guid), result.Id);
        }

        [Test]
        public async Task TestMethodAddAsync()
        {

            var client = new MongoClient(DatabaseConnection.MongoDBConfiguration.Value.DefaultConnection);
            var session = client.StartSession();
            var users = session.Client.GetDatabase(DatabaseConnection.MongoDBConfiguration.Value.DBName).GetCollection<User>("User");

            var user1 = builder.CreateUser();
            var user2 = builder.CreateUser();

            try
            {
                //Begin transaction
                session.StartTransaction(new TransactionOptions(
                             readConcern: ReadConcern.Snapshot,
                             writeConcern: WriteConcern.WMajority));
                //Insert the sample data 
                await users.InsertOneAsync(session, user1);
                await users.InsertOneAsync(session, user2);


                var filter = new FilterDefinitionBuilder<User>().Empty;
                var results = await users.Find<User>(session, filter).ToListAsync();
                var count = results.Count();
                Assert.IsTrue(count == 2);
                session.AbortTransaction();

            }
            catch (Exception e)
            {
                session.AbortTransaction();
            }            
        }

        [Test]
        public async Task AddRangeAsync()
        {
            var result = await userMongoRepository.AddRangeAsync(builder.CreateUserList(3));
            Assert.AreEqual(3, result);
        }

        [Test]
        public async Task RemoveAsync()
        {
            var inserted = await userMongoRepository.AddAsync(builder.CreateUser());
            var found = await userMongoRepository.GetByIdAsync(inserted.Id);
            var result = await userMongoRepository.RemoveAsync(found.Id);
            Assert.AreEqual(true, result);
        }

        [Test]
        public async Task RemoveAsyncObj()
        {
            var inserted = await userMongoRepository.AddAsync(builder.CreateUser());
            var result = await userMongoRepository.RemoveAsync(inserted);
            Assert.AreEqual(1, result);
        }

        [Test]
        public async Task RemoveRangeAsync()
        {
            var inserted1 = await userMongoRepository.AddAsync(builder.CreateUser());
            var inserted2 = await userMongoRepository.AddAsync(builder.CreateUser());
            var usersRange = new List<User>()
            {
                inserted1, inserted2
            };
            var result = await userMongoRepository.RemoveRangeAsync(usersRange);
            Assert.AreEqual(2, result);
        }

        [Test]
        public async Task UpdateAsync()
        {
            var inserted = await userMongoRepository.AddAsync(builder.CreateUser());
            inserted.Name = "Update";
            var result = await userMongoRepository.UpdateAsync(inserted);
            Assert.AreEqual(1, result);
        }

        [Test]
        public async Task UpdateRangeAsync()
        {
            var inserted1 = await userMongoRepository.AddAsync(builder.CreateUser());
            var inserted2 = await userMongoRepository.AddAsync(builder.CreateUser());
            inserted1.Name = "Update1";
            inserted2.Name = "Update2";
            var usersRange = new List<User>()
            {
                inserted1, inserted2
            };
            var result = await userMongoRepository.UpdateRangeAsync(usersRange);
            Assert.AreEqual(2, result);
        }

        [Test]
        public async Task GetByIdAsync()
        {
            var user = await userMongoRepository.AddAsync(builder.CreateUser());
            var result = await userMongoRepository.GetByIdAsync(user.Id);
            Assert.AreEqual(result.Id, user.Id);
        }

        [Test]
        public async Task GetAllAsync()
        {
            var user1 = await userMongoRepository.AddAsync(builder.CreateUser());
            var user2 = await userMongoRepository.AddAsync(builder.CreateUser());
            var result = await userMongoRepository.GetAllAsync();

            var sortUsers = new List<User>() { user1, user2 };
            sortUsers = sortUsers.OrderBy(u => u.Id).ToList();

            Assert.AreEqual(result.OrderBy(u => u.Id).FirstOrDefault().Id, sortUsers.FirstOrDefault().Id);
            Assert.AreEqual(result.OrderBy(u => u.Id).LastOrDefault().Id, sortUsers.LastOrDefault().Id);
        }

        [Test]
        public async Task GetAllIncludingTasksAsync()
        {
            var user = await userMongoRepository.AddAsync(builder.CreateUserWithTasks(2));
            var result = await ((UserRepository) userMongoRepository).GetAllIncludingTasksAsync();

            Assert.AreEqual(result.OrderBy(u => u.Id).FirstOrDefault().Id, user.Id);
            Assert.AreEqual(result.OrderBy(u => u.Id).LastOrDefault().TasksToDo.Count(), 2);
        }

        [Test]
        public async Task GetByIdIncludingTasksAsync()
        {
            var user = await userMongoRepository.AddAsync(builder.CreateUserWithTasks(3));
            var result = await userMongoRepository.GetByIdAsync(user.Id);

            Assert.AreEqual(result.Id, user.Id);
            Assert.AreEqual(result.TasksToDo.Count(), 3);
        }
    }
}
