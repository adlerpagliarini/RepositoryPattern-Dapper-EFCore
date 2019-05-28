using NUnit.Framework;
using System.Threading.Tasks;
using UnitTest.Integration.Repositories.Repositories.DataBuilder;
using System.Linq;
using Infrastructure.DBConfiguration.Mongo;
using Infrastructure.Repositories.Domain.Mongo;
using System.Collections.Generic;
using Domain.Entities;
using UnitTest.Integration.Repositories.DBConfiguration.Mongo;

namespace UnitTest.Integration.Repositories.Repositories.Mongo
{
    [TestFixture]
    public class TaskToDoRepositoryTest
    {
        private MongoContext dbContext;        
        private TaskToDoRepository taskToDoMongoRepository;
        private UserRepository userMongoRepository;

        private UserBuilder userBuilder;
        private TaskToDoBuilder taskToDoBuilder;


        [OneTimeSetUp]
        public void GlobalPrepare()
        {
            dbContext = MongoConfiguration.DataBaseConfiguration();
            MongoSetUpConfiguration.SetUpConfiguration();
        }

        [SetUp]
        public void Inicializa()
        {
            userBuilder = new UserBuilder();
            taskToDoBuilder = new TaskToDoBuilder();
            userMongoRepository = new UserRepository(dbContext);
            taskToDoMongoRepository = new TaskToDoRepository(dbContext);
            dbContext.StartTransaction();
        }

        [TearDown]
        public async Task ExecutadoAposExecucaoDeCadaTeste()
        {
            await dbContext.AbortTransactionAsync();
        }

        [Test]
        public async Task AddAsync()
        {
            var user = userBuilder.CreateUser();
            var taskToDo = taskToDoBuilder.CreateTaskToDoWithUser(user.Id);
            var inserted = await userMongoRepository.AddAsync(user);
            await taskToDoMongoRepository.AddAsync(taskToDo);
            var result = await userMongoRepository.GetByIdAsync(user.Id);
            Assert.AreEqual(1, result.TasksToDo.Count());
        }

        [Test]
        public async Task AddRangeAsync()
        {
            var user = userBuilder.CreateUser();
            var taskToDo = taskToDoBuilder.CreateTaskToDoListWithUser(3, user.Id);
            var inserted = await userMongoRepository.AddAsync(user);
            await taskToDoMongoRepository.AddRangeAsync(taskToDo);
            var result = await userMongoRepository.GetByIdAsync(user.Id);
            Assert.AreEqual(3, result.TasksToDo.Count());
        }

        [Test]
        public async Task UpdateAsync()
        {
            var user = userBuilder.CreateUserWithTasks(2);
            var inserted = await userMongoRepository.AddAsync(user);

            var taskToDo1 = user.TasksToDo.FirstOrDefault();
            taskToDo1.Title = "update 1";

            await taskToDoMongoRepository.UpdateAsync(taskToDo1);

            var foundTheFirst = await taskToDoMongoRepository.GetByIdAsync(taskToDo1.Id);

            Assert.IsNotNull(foundTheFirst);
        }

        [Test]
        public async Task UpdateAsync_Twice()
        {
            var user = userBuilder.CreateUserWithTasks(2);
            var inserted = await userMongoRepository.AddAsync(user);

            var taskToDo1 = user.TasksToDo.FirstOrDefault();
            taskToDo1.Title = "update 1";
            var taskToDo2 = user.TasksToDo.LastOrDefault();
            taskToDo2.Title = "update 2";

            await taskToDoMongoRepository.UpdateAsync(taskToDo1);
            await taskToDoMongoRepository.UpdateAsync(taskToDo2);

            var foundTheFirst = await taskToDoMongoRepository.GetByIdAsync(taskToDo1.Id);

            Assert.IsNotNull(foundTheFirst);
        }

        [Test]
        public async Task UpdateRangeAsync()
        {
            var user = userBuilder.CreateUserWithTasks(2);
            var inserted = await userMongoRepository.AddAsync(user);

            var taskToDo1 = user.TasksToDo.FirstOrDefault();
            var taskToDo2 = user.TasksToDo.LastOrDefault();
            taskToDo1.Title = "update1";
            taskToDo2.Title = "update2";

            var tasks = new List<TaskToDo>() { taskToDo1, taskToDo2 };
            var result = await taskToDoMongoRepository.UpdateRangeAsync(tasks);
            
            var t1 = (await userMongoRepository.GetByIdAsync(user.Id)).TasksToDo.FirstOrDefault().Title;
            var t2 = (await userMongoRepository.GetByIdAsync(user.Id)).TasksToDo.LastOrDefault().Title;
            
            Assert.IsTrue(t1 == "update1");
            Assert.IsTrue(t2 == "update2");

            Assert.IsTrue(result == 2);
        }

        [Test]
        public async Task UpdateRangeAsync_Twice()
        {
            var user = userBuilder.CreateUserWithTasks(1);
            var inserted = await userMongoRepository.AddAsync(user);
            await taskToDoMongoRepository.UpdateRangeAsync(user.TasksToDo);
            var tasks = taskToDoBuilder.CreateTaskToDoListWithUser(2, user.Id);
            await taskToDoMongoRepository.AddRangeAsync(tasks);
            await taskToDoMongoRepository.UpdateRangeAsync(tasks);

            var result = await taskToDoMongoRepository.GetByIdAsync(user.TasksToDo.FirstOrDefault().Id);
            
            Assert.IsNotNull(result);
        }

        [Test]
        public async Task RemoveAsync()
        {
            var user = userBuilder.CreateUser();
            var tasksToDo = taskToDoBuilder.CreateTaskToDoListWithUser(3, user.Id);
            var inserted = await userMongoRepository.AddAsync(user);
            var deleteTaskTodo = taskToDoBuilder.CreateTaskToDoWithUser(user.Id);
            deleteTaskTodo.Title = "Deletar";
            tasksToDo.Add(deleteTaskTodo);
            await taskToDoMongoRepository.AddRangeAsync(tasksToDo);

            var tempUser = await userMongoRepository.GetByIdAsync(user.Id);
            Assert.IsNotNull(tempUser.TasksToDo.Where(t => t.Title == "Deletar").FirstOrDefault());

            var result = await taskToDoMongoRepository.RemoveAsync(deleteTaskTodo.Id);
            tempUser = await userMongoRepository.GetByIdAsync(user.Id);
            Assert.IsNull(tempUser.TasksToDo.Where(t => t.Title == "Deletar").FirstOrDefault());

            Assert.IsTrue(tempUser.TasksToDo.Count() == 3);
            Assert.IsTrue(result);
        }

        [Test]
        public async Task RemoveAsyncObj()
        {
            var user = userBuilder.CreateUser();
            var tasksToDo = taskToDoBuilder.CreateTaskToDoListWithUser(3, user.Id);
            var inserted = await userMongoRepository.AddAsync(user);
            var deleteTaskTodo = taskToDoBuilder.CreateTaskToDoWithUser(user.Id);
            deleteTaskTodo.Title = "Deletar";
            tasksToDo.Add(deleteTaskTodo);
            await taskToDoMongoRepository.AddRangeAsync(tasksToDo);

            var tempUser = await userMongoRepository.GetByIdAsync(user.Id);
            Assert.IsNotNull(tempUser.TasksToDo.Where(t => t.Title == "Deletar").FirstOrDefault());

            var result = await taskToDoMongoRepository.RemoveAsync(deleteTaskTodo);
            tempUser = await userMongoRepository.GetByIdAsync(user.Id);
            Assert.IsNull(tempUser.TasksToDo.Where(t => t.Title == "Deletar").FirstOrDefault());

            Assert.IsTrue(tempUser.TasksToDo.Count() == 3);
            Assert.IsTrue(result == 1);
        }

        [Test]
        public async Task RemoveRangeAsync()
        {
            var user = userBuilder.CreateUser();
            var taskToDo = taskToDoBuilder.CreateTaskToDoListWithUser(3, user.Id);
            var inserted = await userMongoRepository.AddAsync(user);
            await taskToDoMongoRepository.AddRangeAsync(taskToDo);

            var tempUser = await userMongoRepository.GetByIdAsync(user.Id);
            var result = await taskToDoMongoRepository.RemoveRangeAsync(tempUser.TasksToDo);
            tempUser = await userMongoRepository.GetByIdAsync(user.Id);

            Assert.IsTrue(tempUser.TasksToDo.Count() == 0);
            Assert.IsTrue(result == 1);
        }

        [Test]
        public async Task GetAllAsync()
        {
            var user = await userMongoRepository.AddAsync(userBuilder.CreateUserWithTasks(1));
            var tasks = user.TasksToDo;
            var result = await taskToDoMongoRepository.GetAllAsync();

            Assert.AreEqual(result.FirstOrDefault().Id, tasks.FirstOrDefault().Id);
        }

        [Test]
        public async Task GetByIdAsync()
        {
            var user = await userMongoRepository.AddAsync(userBuilder.CreateUserWithTasks(2));
            var tasks = user.TasksToDo;
            var result = await taskToDoMongoRepository.GetByIdAsync(tasks.FirstOrDefault().Id);

            Assert.AreEqual(result.Id, tasks.FirstOrDefault().Id);
        }

        [Test]
        public async Task GetAllIncludingUserAsync()
        {
            var user = await userMongoRepository.AddAsync(userBuilder.CreateUserWithTasks(2));
            var tasks = user.TasksToDo;
            var result = await taskToDoMongoRepository.GetAllIncludingUserAsync();

            Assert.IsNotNull(result.FirstOrDefault().User);
            Assert.IsNotNull(result.LastOrDefault().User);
        }

        [Test]
        public async Task GetByIdIncludingUserAsync()
        {
            var user = await userMongoRepository.AddAsync(userBuilder.CreateUserWithTasks(2));
            var tasks = user.TasksToDo;
            var result = await taskToDoMongoRepository.GetByIdIncludingUserAsync(tasks.FirstOrDefault().Id);

            Assert.AreEqual(result.UserId, user.Id);
        }
    }
}
