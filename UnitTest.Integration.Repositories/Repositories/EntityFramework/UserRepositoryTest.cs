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
using Infrastructure.Repositories.Domain.EFCore.Specifications;

namespace UnitTest.Integration.Repositories.Repositories.EntityFramework
{
    [TestFixture]
    public class UserRepositoryTest
    {
        private ApplicationContext dbContext;
        private IDbContextTransaction transaction;

        private IUserSpecificationRepository userEntityFramework;
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
        public async Task GetAllIncludingTasksSpecificationAsync()
        {
            // Arrange
            var _dbContext = new EntityFrameworkConnection().DataBaseConfiguration();
            var _userEntityFramework = new UserRepository(_dbContext);
            var _builder = new UserBuilder();
            var _transaction = _dbContext.Database.BeginTransaction();

            var user1 = await _userEntityFramework.AddAsync(_builder.CreateUserWithTasks(1));
            var user2 = await _userEntityFramework.AddAsync(_builder.CreateUserWithTasks(2));

            await _dbContext.SaveChangesAsync();

            _transaction.Commit();

            var result = await userEntityFramework.GetAllIncludingTasksBySpecAsync();

            _dbContext.User.Remove(user1);
            _dbContext.User.Remove(user2);
            await _dbContext.SaveChangesAsync();

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

        [Test]
        public async Task GetByIdIncludingTasksSpecificationAsync()
        {
            var user = await userEntityFramework.AddAsync(builder.CreateUserWithTasks(3));
            var result = await userEntityFramework.GetByIdIncludingTasksBySpecAsync(user.Id);

            Assert.AreEqual(result.Id, user.Id);

            Assert.AreEqual(result.TasksToDo.Count(), 3);
        }

        [Test]
        public async Task GetById_UserByIdIncludingTasksSpec()
        {
            var user = await userEntityFramework.AddAsync(builder.CreateUserWithTasks(3));
            var result = (await userEntityFramework.ApplySpecification(new UserByIdIncludingTasksSpec(user.Id, user.Name))).FirstOrDefault();

            Assert.AreEqual(result.Id, user.Id);

            Assert.AreEqual(result.TasksToDo.Count(), 3);
        }

        [Test]
        public async Task GetById_UserIdAndNameByIdSpec()
        {
            var _dbContext = new EntityFrameworkConnection().DataBaseConfiguration();
            var _userEntityFramework = new UserRepository(_dbContext);
            var _builder = new UserBuilder();
            var _transaction = _dbContext.Database.BeginTransaction();

            var user = await _userEntityFramework.AddAsync(_builder.CreateUserWithTasks(1));

            await _dbContext.SaveChangesAsync();

            _transaction.Commit();

            var result = (await userEntityFramework.ApplySpecification(new UserIdAndNameByIdSpec(user.Id))).FirstOrDefault();

            _dbContext.User.Remove(user);
            await _dbContext.SaveChangesAsync();

            Assert.AreEqual(result.Id, user.Id);
            Assert.AreEqual(result.Name, user.Name);
        }

        [Test]
        public async Task GetById_UserNameByIdSpec()
        {
            var user = await userEntityFramework.AddAsync(builder.CreateUserWithTasks(3));
            var result = (await userEntityFramework.ApplySpecification(new UserNameByIdSpec(user.Id))).FirstOrDefault();

            Assert.AreEqual(result, user.Name);            
        }
    }
}
