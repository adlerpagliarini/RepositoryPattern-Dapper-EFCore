using Domain.Entities;
using System.Collections.Generic;

namespace UnitTest.Integration.Repositories.Repositories.DataBuilder
{
    public class UserBuilder
    {
        private User user;
        private List<User> userList;
        private readonly TaskToDoBuilder taskToDoBuilder;

        public UserBuilder()
        {
            taskToDoBuilder = new TaskToDoBuilder();
        }
        public User CreateUser()
        {
            user = new User() { Name = "User from Builder" };
            return user;
        }

        public List<User> CreateUserList(int amount)
        {
            userList = new List<User>();
            for (int i = 0; i < amount; i++)
            {
                userList.Add(CreateUser());
            }

            return userList;
        }

        public User CreateUserWithTasks(int amountOfTasks)
        {
            user = CreateUser();
            foreach (var item in taskToDoBuilder.CreateTaskToDoList(amountOfTasks))
            {
                user.AddItemToDo(item);
            }
            return user;
        }

        public List<User> CreateUserListWithTasks(int amountOfUsers, int amountOfTasks)
        {
            userList = CreateUserList(1);
            for (int i = 0; i < amountOfUsers; i++)
            {
                foreach (var item in taskToDoBuilder.CreateTaskToDoList(amountOfTasks))
                {
                    user.AddItemToDo(item);
                }
                userList.Add(user);
            }
            return userList;
        }

    }
}
