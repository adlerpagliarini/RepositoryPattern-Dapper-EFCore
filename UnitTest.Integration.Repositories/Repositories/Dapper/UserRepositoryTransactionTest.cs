using Domain.Entities;
using Infrastructure.Interfaces.Repositories.Domain;
using Infrastructure.Repositories.Domain.Dapper;
using NUnit.Framework;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnitTest.Integration.Repositories.Repositories.DataBuilder;
using System.Linq;
using System.Data;
using UnitTest.Integration.Repositories.DBConfiguration.Dapper;
using Infrastructure.Interfaces.DBConfiguration;

namespace UnitTest.Integration.Repositories.Repositories.Dapper
{
    [TestFixture]
    public class UserRepositoryTransactionTest
    {
        private IDatabaseFactory databaseOptions;
        private IDbTransaction transaction;

        private IUserRepository userDapper;
        private ITaskToDoRepository taskToDoDapper;
        private UserBuilder userBuilder;
        private TaskToDoBuilder taskToDoBuilder;

        [OneTimeSetUp]
        public void GlobalPrepare()
        {
            databaseOptions = new DapperConnection().DatabaseFactory();
        }

        [SetUp]
        public void Inicializa()
        {
            var conn = databaseOptions.GetDbConnection;
            conn.Open();
            transaction = conn.BeginTransaction();
            userDapper = new UserRepository(conn, transaction);
            taskToDoDapper = new TaskToDoRepository(conn, transaction);
            userBuilder = new UserBuilder();
            taskToDoBuilder = new TaskToDoBuilder();
        }

        [TearDown]
        public void ExecutadoAposExecucaoDeCadaTeste()
        {
            transaction.Rollback();
        }

        [Test]
        public async Task AddAsync()
        {
            User result;
            result = await userDapper.AddAsync(userBuilder.CreateUser());
            Assert.Greater(result.Id, 0);
        }

        [Test]
        public async Task AddRangeAsync()
        {
            int result;
            result = await userDapper.AddRangeAsync(userBuilder.CreateUserList(3));
            Assert.AreEqual(3, result);
        }

        [Test]
        public async Task RemoveAsync()
        {
            var inserted = await userDapper.AddAsync(userBuilder.CreateUser());
            var result = await userDapper.RemoveAsync(inserted.Id);
            Assert.AreEqual(true, result);
        }

        [Test]
        public async Task RemoveAsyncObj()
        {
            var inserted = await userDapper.AddAsync(userBuilder.CreateUser());
            var result = await userDapper.RemoveAsync(inserted);
            Assert.AreEqual(1, result);
        }

        [Test]
        public async Task RemoveRangeAsync()
        {
            var inserted1 = await userDapper.AddAsync(userBuilder.CreateUser());
            var inserted2 = await userDapper.AddAsync(userBuilder.CreateUser());
            var usersRange = new List<User>()
            {
                inserted1, inserted2
            };
            var result = await userDapper.RemoveRangeAsync(usersRange);
            Assert.AreEqual(2, result);
        }

        [Test]
        public async Task UpdateAsync()
        {
            var inserted = await userDapper.AddAsync(userBuilder.CreateUser());
            inserted.Name = "Update";
            var result = await userDapper.UpdateAsync(inserted);
            Assert.AreEqual(1, result);
        }

        [Test]
        public async Task UpdateRangeAsync()
        {
            var inserted1 = await userDapper.AddAsync(userBuilder.CreateUser());
            var inserted2 = await userDapper.AddAsync(userBuilder.CreateUser());
            inserted1.Name = "Update1";
            inserted2.Name = "Update2";
            var usersRange = new List<User>()
            {
                inserted1, inserted2
            };
            var result = await userDapper.UpdateRangeAsync(usersRange);
            Assert.AreEqual(2, result);
        }

        [Test]
        public async Task GetByIdAsync()
        {
            var user = await userDapper.AddAsync(userBuilder.CreateUser());
            var result = await userDapper.GetByIdAsync(user.Id);
            Assert.AreEqual(result.Id, user.Id);
        }

        [Test]
        public async Task GetAllAsync()
        {
            var user1 = await userDapper.AddAsync(userBuilder.CreateUser());
            var user2 = await userDapper.AddAsync(userBuilder.CreateUser());
            var result = await userDapper.GetAllAsync();
            Assert.AreEqual(result.OrderBy(u => u.Id).FirstOrDefault().Id, user1.Id);
            Assert.AreEqual(result.OrderBy(u => u.Id).LastOrDefault().Id, user2.Id);
        }

        [Test]
        public async Task GetAllIncludingTasksAsync()
        {
            var user1 = await userDapper.AddAsync(userBuilder.CreateUser());
            var user2 = await userDapper.AddAsync(userBuilder.CreateUser());
            var task1 = await taskToDoDapper.AddRangeAsync(taskToDoBuilder.CreateTaskToDoListWithUser(1, user1.Id));
            var task2 = await taskToDoDapper.AddRangeAsync(taskToDoBuilder.CreateTaskToDoListWithUser(2, user2.Id));

            var result = await userDapper.GetAllIncludingTasksAsync();

            var result1 = result.OrderBy(u => u.Id).FirstOrDefault();
            var result2 = result.OrderBy(u => u.Id).LastOrDefault();

            Assert.AreEqual(result1.Id, user1.Id);
            Assert.AreEqual(result2.Id, user2.Id);

            Assert.AreEqual(result1.TasksToDo.Count(), 1);
            Assert.AreEqual(result2.TasksToDo.Count(), 2);
        }

        [Test]
        public async Task GetByIdIncludingTasksAsync()
        {
            var user = await userDapper.AddAsync(userBuilder.CreateUser());
            var task = await taskToDoDapper.AddRangeAsync(taskToDoBuilder.CreateTaskToDoListWithUser(3, user.Id));

            var result = await userDapper.GetByIdIncludingTasksAsync(user.Id);

            Assert.AreEqual(result.Id, user.Id);

            Assert.AreEqual(result.TasksToDo.Count(), 3);
        }
    }
}
