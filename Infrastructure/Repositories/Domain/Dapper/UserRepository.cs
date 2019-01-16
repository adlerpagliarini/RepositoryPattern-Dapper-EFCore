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
    public class UserRepository : DomainRepository<User>,
                                  IUserRepository
    {
        public UserRepository(IDatabaseFactory databaseOptions) : base(databaseOptions)
        {
        }

        public UserRepository(IDbConnection databaseConnection, IDbTransaction transaction = null) : base(databaseConnection, transaction)
        {
        }

        protected override string InsertQuery => $"INSERT INTO [{nameof(User)}] VALUES (@{nameof(User.Name)})";
        protected override string InsertQueryReturnInserted => $"INSERT INTO [{nameof(User)}] OUTPUT INSERTED.* VALUES (@{nameof(User.Name)})";
        protected override string UpdateByIdQuery => $"UPDATE [{nameof(User)}] SET {nameof(User.Name)} = @{nameof(User.Name)} WHERE {nameof(User.Id)} = @{nameof(User.Id)}";
        protected override string DeleteByIdQuery => $"DELETE FROM [{nameof(User)}] WHERE {nameof(User.Id)} = @{nameof(User.Id)}";
        protected override string SelectAllQuery => $"SELECT * FROM [{nameof(User)}]";
        protected override string SelectByIdQuery => $"SELECT * FROM [{nameof(User)}] WHERE {nameof(User.Id)} = @{nameof(User.Id)}";

        private string SelectAllIncludingRelation => $"SELECT u.*, t.* FROM [{nameof(User)}] u LEFT JOIN [{nameof(TaskToDo)}] t ON t.{nameof(TaskToDo.UserId)} = u.{nameof(User.Id)}";
        private string SelectByIdIncludingRelation => $"SELECT u.*, t.* FROM [{nameof(User)}] u LEFT JOIN [{nameof(TaskToDo)}] t ON t.{nameof(TaskToDo.UserId)} = u.{nameof(User.Id)} WHERE u.{nameof(User.Id)} = @{nameof(User.Id)}";

        public async Task<IEnumerable<User>> GetAllIncludingTasksAsync()
        {
            var userDictionary = new Dictionary<int, User>();
            var queryResult = await dbConn.QueryAsync<User, TaskToDo, User>(SelectAllIncludingRelation, transaction: dbTransaction,
                map: (user, toDoList) => FuncMapRelation(user, toDoList, userDictionary));

            return queryResult.Distinct();
        }

        public async Task<User> GetByIdIncludingTasksAsync(int id)
        {
            var userDictionary = new Dictionary<int, User>();
            var queryResult = await dbConn.QueryAsync<User, TaskToDo, User>(SelectByIdIncludingRelation, param: new { Id = id }, transaction: dbTransaction,
                map: (user, toDoList) => FuncMapRelation(user, toDoList, userDictionary));

            return queryResult.Distinct().FirstOrDefault();
        }

        private readonly Func<User, TaskToDo, Dictionary<int, User>, User> FuncMapRelation = (user, tasksToDo, userDictionary) =>
        {
            if (!userDictionary.TryGetValue(user.Id, out User userEntry))
            {
                userEntry = user;
                userDictionary.Add(userEntry.Id, userEntry);
            }

            userEntry.AddItemToDo(tasksToDo);
            return userEntry;
        };
    }
}
