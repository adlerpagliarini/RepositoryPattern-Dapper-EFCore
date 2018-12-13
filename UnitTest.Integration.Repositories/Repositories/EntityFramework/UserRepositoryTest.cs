using Domain.Entities;
using Infrastructure.DBConfiguration.EFCore;
using Infrastructure.Interfaces.Repositories.Domain;
using Infrastructure.Repositories.Domain.EFCore;
using Microsoft.EntityFrameworkCore.Storage;
using NUnit.Framework;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnitTest.Integration.Repositories.DBConfiguration.EFCore;
using UnitTest.Integration.Repositories.Repositories.DataBuilder;
using System.Linq;

namespace UnitTest.Integration.Repositories.Repositories.EntityFramework
{
    [TestFixture]
    public class UserRepositoryTest
    {
        private ApplicationContext dbContext;
        private IDbContextTransaction transaction;

        private IUserRepository userEntityFramework;
        private UserBuilder builder;

        [OneTimeSetUp]
        public void GlobalPrepare()
        {
            dbContext = new EntityFrameworkConnection().DataBaseConfiguration();
        }

        [SetUp]
        public void Inicializa()
        {
            userEntityFramework = new UserRepository(dbContext);
            builder = new UserBuilder();
            transaction = dbContext.Database.BeginTransaction();
        }

        [TearDown]
        public void ExecutadoAposExecucaoDeCadaTeste()
        {
            transaction.Rollback();
        }

        [Test]
        public async Task AddAsync()
        {
            var result = await userEntityFramework.AddAsync(builder.CreateUser());
            Assert.Greater(result.Id, 0);
        }

        [Test]
        public async Task AddRangeAsync()
        {
            var result = await userEntityFramework.AddRangeAsync(builder.CreateUserList(3));
            Assert.AreEqual(3, result);
        }

        [Test]
        public async Task RemoveAsync()
        {
            var inserted = await userEntityFramework.AddAsync(builder.CreateUser());
            var result = await userEntityFramework.RemoveAsync(inserted.Id);
            Assert.AreEqual(true, result);
        }

        [Test]
        public async Task RemoveAsyncObj()
        {
            var inserted = await userEntityFramework.AddAsync(builder.CreateUser());
            var result = await userEntityFramework.RemoveAsync(inserted);
            Assert.AreEqual(1, result);
        }

        [Test]
        public async Task RemoveRangeAsync()
        {
            var inserted1 = await userEntityFramework.AddAsync(builder.CreateUser());
            var inserted2 = await userEntityFramework.AddAsync(builder.CreateUser());
            var usersRange = new List<User>()
            {
                inserted1, inserted2
            };
            var result = await userEntityFramework.RemoveRangeAsync(usersRange);
            Assert.AreEqual(2, result);
        }

        [Test]
        public async Task UpdateAsync()
        {
            var inserted = await userEntityFramework.AddAsync(builder.CreateUser());
            inserted.Name = "Update";
            var result = await userEntityFramework.UpdateAsync(inserted);
            Assert.AreEqual(1, result);
        }

        [Test]
        public async Task UpdateRangeAsync()
        {
            var inserted1 = await userEntityFramework.AddAsync(builder.CreateUser());
            var inserted2 = await userEntityFramework.AddAsync(builder.CreateUser());
            inserted1.Name = "Update1";
            inserted2.Name = "Update2";
            var usersRange = new List<User>()
            {
                inserted1, inserted2
            };
            var result = await userEntityFramework.UpdateRangeAsync(usersRange);
            Assert.AreEqual(2, result);
        }

        [Test]
        public async Task GetByIdAsync()
        {
            var user = await userEntityFramework.AddAsync(builder.CreateUser());
            var result = await userEntityFramework.GetByIdAsync(user.Id);
            Assert.AreEqual(result.Id, user.Id);
        }

        [Test]
        public async Task GetAllAsync()
        {
            var user1 = await userEntityFramework.AddAsync(builder.CreateUser());
            var user2 = await userEntityFramework.AddAsync(builder.CreateUser());
            var result = await userEntityFramework.GetAllAsync();
            Assert.AreEqual(result.OrderBy(u => u.Id).FirstOrDefault().Id, user1.Id);
            Assert.AreEqual(result.OrderBy(u => u.Id).LastOrDefault().Id, user2.Id);
        }

        [Test]
        public async Task GetAllIncludingTasksAsync()
        {
            var user1 = await userEntityFramework.AddAsync(builder.CreateUserWithTasks(1));
            var user2 = await userEntityFramework.AddAsync(builder.CreateUserWithTasks(2));
            var result = await userEntityFramework.GetAllIncludingTasksAsync();

            Assert.AreEqual(result.OrderBy(u => u.Id).FirstOrDefault().Id, user1.Id);
            Assert.AreEqual(result.OrderBy(u => u.Id).LastOrDefault().Id, user2.Id);

            Assert.AreEqual(result.OrderBy(u => u.Id).FirstOrDefault().TasksToDo.Count(), 1);
            Assert.AreEqual(result.OrderBy(u => u.Id).LastOrDefault().TasksToDo.Count(), 2);
        }

        [Test]
        public async Task GetByIdIncludingTasksAsync()
        {
            var user = await userEntityFramework.AddAsync(builder.CreateUserWithTasks(3));
            var result = await userEntityFramework.GetByIdAsync(user.Id);

            Assert.AreEqual(result.Id, user.Id);

            Assert.AreEqual(result.TasksToDo.Count(), 3);
        }
    }
}
