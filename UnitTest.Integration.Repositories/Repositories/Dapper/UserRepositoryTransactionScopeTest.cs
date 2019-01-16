using Domain.Entities;
using Infrastructure.Interfaces.Repositories.Domain;
using Infrastructure.Repositories.Domain.Dapper;
using NUnit.Framework;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnitTest.Integration.Repositories.Repositories.DataBuilder;
using System.Linq;
using UnitTest.Integration.Repositories.DBConfiguration.Dapper;
using System.Transactions;
using Infrastructure.Interfaces.DBConfiguration;

namespace UnitTest.Integration.Repositories.Repositories.Dapper
{
    [TestFixture]
    public class UserRepositoryTransactionScopeTest
    {
        private IDatabaseFactory databaseOptions;

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
            userBuilder = new UserBuilder();
            taskToDoBuilder = new TaskToDoBuilder();
        }

        [TearDown]
        public void ExecutadoAposExecucaoDeCadaTeste()
        {
        }

        [Test]
        public async Task AddAsync()
        {
            using (var scope = new TransactionScope(TransactionScopeAsyncFlowOption.Enabled))
            {
                userDapper = new UserRepository(databaseOptions);
                var result = await userDapper.AddAsync(userBuilder.CreateUser());
                Assert.Greater(result.Id, 0);
            }
        }

        [Test]
        public async Task AddRangeAsync()
        {
            using (var scope = new TransactionScope(TransactionScopeAsyncFlowOption.Enabled))
            {
                userDapper = new UserRepository(databaseOptions);
                var result = await userDapper.AddRangeAsync(userBuilder.CreateUserList(3));
                Assert.AreEqual(3, result);
            }
        }

        [Test]
        public async Task RemoveAsync()
        {
            using (var scope = new TransactionScope(TransactionScopeAsyncFlowOption.Enabled))
            {
                userDapper = new UserRepository(databaseOptions);
                var inserted = await userDapper.AddAsync(userBuilder.CreateUser());
                var result = await userDapper.RemoveAsync(inserted.Id);
                Assert.AreEqual(true, result);
            }
        }

        [Test]
        public async Task RemoveAsyncObj()
        {
            using (var scope = new TransactionScope(TransactionScopeAsyncFlowOption.Enabled))
            {
                userDapper = new UserRepository(databaseOptions);
                var inserted = await userDapper.AddAsync(userBuilder.CreateUser());
                var result = await userDapper.RemoveAsync(inserted);
                Assert.AreEqual(1, result);
            }
        }

        [Test]
        public async Task RemoveRangeAsync()
        {
            using (var scope = new TransactionScope(TransactionScopeAsyncFlowOption.Enabled))
            {
                userDapper = new UserRepository(databaseOptions);
                var inserted1 = await userDapper.AddAsync(userBuilder.CreateUser());
                var inserted2 = await userDapper.AddAsync(userBuilder.CreateUser());
                var usersRange = new List<User>()
                {
                    inserted1, inserted2
                };
                var result = await userDapper.RemoveRangeAsync(usersRange);
                Assert.AreEqual(2, result);
            }
        }

        [Test]
        public async Task UpdateAsync()
        {
            using (var scope = new TransactionScope(TransactionScopeAsyncFlowOption.Enabled))
            {
                userDapper = new UserRepository(databaseOptions);
                var inserted = await userDapper.AddAsync(userBuilder.CreateUser());
                inserted.Name = "Update";
                var result = await userDapper.UpdateAsync(inserted);
                Assert.AreEqual(1, result);
            }
        }

        [Test]
        public async Task UpdateRangeAsync()
        {
            using (var scope = new TransactionScope(TransactionScopeAsyncFlowOption.Enabled))
            {
                userDapper = new UserRepository(databaseOptions);
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
        }

        [Test]
        public async Task GetByIdAsync()
        {
            using (var scope = new TransactionScope(TransactionScopeAsyncFlowOption.Enabled))
            {
                userDapper = new UserRepository(databaseOptions);
                var user = await userDapper.AddAsync(userBuilder.CreateUser());
                var result = await userDapper.GetByIdAsync(user.Id);
                Assert.AreEqual(result.Id, user.Id);
            }
        }

        [Test]
        public async Task GetAllAsync()
        {
            using (var scope = new TransactionScope(TransactionScopeAsyncFlowOption.Enabled))
            {
                userDapper = new UserRepository(databaseOptions);
                var user1 = await userDapper.AddAsync(userBuilder.CreateUser());
                var user2 = await userDapper.AddAsync(userBuilder.CreateUser());
                var result = await userDapper.GetAllAsync();
                Assert.AreEqual(result.OrderBy(u => u.Id).FirstOrDefault().Id, user1.Id);
                Assert.AreEqual(result.OrderBy(u => u.Id).LastOrDefault().Id, user2.Id);
            }
        }

        [Test]
        public async Task GetAllIncludingTasksAsync()
        {
            using (var scope = new TransactionScope(TransactionScopeAsyncFlowOption.Enabled))
            {
                var conn = databaseOptions.GetDbConnection;
                userDapper = new UserRepository(conn);
                taskToDoDapper = new TaskToDoRepository(conn);

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
        }

        [Test]
        public async Task GetByIdIncludingTasksAsync()
        {
            using (var scope = new TransactionScope(TransactionScopeAsyncFlowOption.Enabled))
            {
                var conn = databaseOptions.GetDbConnection;
                userDapper = new UserRepository(conn);
                taskToDoDapper = new TaskToDoRepository(conn);

                var user = await userDapper.AddAsync(userBuilder.CreateUser());
                var task = await taskToDoDapper.AddRangeAsync(taskToDoBuilder.CreateTaskToDoListWithUser(3, user.Id));

                var result = await userDapper.GetByIdIncludingTasksAsync(user.Id);

                Assert.AreEqual(result.Id, user.Id);

                Assert.AreEqual(result.TasksToDo.Count(), 3);
            }
        }
    }
}
