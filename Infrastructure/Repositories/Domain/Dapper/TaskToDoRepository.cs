using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using Dapper;
using Domain.Entities;
using Infrastructure.Interfaces.DBConfiguration;
using Infrastructure.Interfaces.Repositories.Domain;
using Infrastructure.Repositories.Standard.Dapper;

namespace Infrastructure.Repositories.Domain.Dapper
{
    public class TaskToDoRepository : DomainRepository<TaskToDo>,
                                      ITaskToDoRepository
    {
        public TaskToDoRepository(IDatabaseFactory databaseOptions) : base(databaseOptions)
        {
        }

        public TaskToDoRepository(IDbConnection databaseConnection, IDbTransaction transaction = null) : base(databaseConnection, transaction)
        {
        }

        protected override string InsertQuery => $"INSERT INTO [{nameof(TaskToDo)}] VALUES (@{nameof(TaskToDo.Title)}, @{nameof(TaskToDo.Start)}, @{nameof(TaskToDo.DeadLine)}, @{nameof(TaskToDo.Status)}, @{nameof(TaskToDo.UserId)})";
        protected override string InsertQueryReturnInserted => $"INSERT INTO [{nameof(TaskToDo)}] OUTPUT INSERTED.* VALUES (@{nameof(TaskToDo.Title)}, @{nameof(TaskToDo.Start)}, @{nameof(TaskToDo.DeadLine)}, @{nameof(TaskToDo.Status)}, @{nameof(TaskToDo.UserId)})";
        protected override string UpdateByIdQuery => $"UPDATE [{nameof(TaskToDo)}] SET {nameof(TaskToDo.Title)} = @{nameof(TaskToDo.Title)}, {nameof(TaskToDo.Start)} = @{nameof(TaskToDo.Start)}, {nameof(TaskToDo.DeadLine)} = @{nameof(TaskToDo.DeadLine)}, {nameof(TaskToDo.Status)} = @{nameof(TaskToDo.Status)} WHERE {nameof(TaskToDo.Id)} = @{nameof(TaskToDo.Id)}";
        protected override string DeleteByIdQuery => $"DELETE FROM [{nameof(TaskToDo)}] WHERE {nameof(TaskToDo.Id)} = @{nameof(TaskToDo.Id)}";
        protected override string SelectAllQuery => $"SELECT * FROM [{nameof(TaskToDo)}]";
        protected override string SelectByIdQuery => $"SELECT t.* FROM [{nameof(TaskToDo)}] t WHERE t.{nameof(TaskToDo.Id)} = @{nameof(TaskToDo.Id)}";

        private string SelectAllIncludingRelation => $"SELECT t.*, u.* FROM [{nameof(TaskToDo)}] t INNER JOIN [{nameof(User)}] u ON u.{nameof(User.Id)} = t.{nameof(TaskToDo.UserId)}";
        private string SelectByIdIncludingRelation => $"SELECT t.*, u.* FROM [{nameof(TaskToDo)}] t INNER JOIN [{nameof(User)}] u ON u.{nameof(User.Id)} = t.{nameof(TaskToDo.UserId)} WHERE t.{nameof(TaskToDo.Id)} = @{nameof(TaskToDo.Id)}";

        public async Task<IEnumerable<TaskToDo>> GetAllIncludingUserAsync()
        {
            var queryResult = await dbConn.QueryAsync<TaskToDo, User, TaskToDo>(SelectAllIncludingRelation, transaction: dbTransaction,
                map: (taskToDo, user) => FuncMapRelation(taskToDo, user));

            return queryResult.Distinct();
        }

        public async Task<TaskToDo> GetByIdIncludingUserAsync(int id)
        {
            var queryResult = await dbConn.QueryAsync<TaskToDo, User, TaskToDo>(SelectByIdIncludingRelation, param: new { Id = id }, transaction: dbTransaction,
                map: (taskToDo, user) => FuncMapRelation(taskToDo, user));

            return queryResult.Distinct().FirstOrDefault();
        }

        private readonly Func<TaskToDo, User, TaskToDo> FuncMapRelation = (taskToDo, user) =>
        {
            taskToDo.User = user;
            return taskToDo;
        };
    }
}
