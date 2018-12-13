using Infrastructure.DBConfiguration.EFCore;
using Infrastructure.Interfaces.Repositories.Domain;
using Infrastructure.Repositories.Domain.EFCore;
using Microsoft.EntityFrameworkCore.Storage;
using NUnit.Framework;
using System.Threading.Tasks;
using UnitTest.Integration.Repositories.DBConfiguration.EFCore;
using UnitTest.Integration.Repositories.Repositories.DataBuilder;
using System.Linq;

namespace UnitTest.Integration.Repositories.Repositories.EntityFramework
{
    [TestFixture]
    public class TaskToDoRepositoryTest
    {
        private ApplicationContext dbContext;
        private IDbContextTransaction transaction;

        private IUserRepository userEntityFramework;
        private ITaskToDoRepository taskToDoEntityFramework;
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
            taskToDoEntityFramework = new TaskToDoRepository(dbContext);
            builder = new UserBuilder();
            transaction = dbContext.Database.BeginTransaction();
        }

        [TearDown]
        public void ExecutadoAposExecucaoDeCadaTeste()
        {
            transaction.Rollback();
        }

        [Test]
        public async Task GetAllIncludingUserAsync()
        {
            var user = await userEntityFramework.AddAsync(builder.CreateUserWithTasks(2));
            var tasks = user.TasksToDo;
            var result = await taskToDoEntityFramework.GetAllIncludingUserAsync();

            Assert.AreEqual(result.FirstOrDefault().UserId, user.Id);
            Assert.AreEqual(result.LastOrDefault().UserId, user.Id);
        }

        [Test]
        public async Task GetByIdIncludingUserAsync()
        {
            var user = await userEntityFramework.AddAsync(builder.CreateUserWithTasks(2));
            var tasks = user.TasksToDo;
            var result = await taskToDoEntityFramework.GetByIdIncludingUserAsync(tasks.FirstOrDefault().Id);

            Assert.AreEqual(result.UserId, user.Id);
        }
    }
}
