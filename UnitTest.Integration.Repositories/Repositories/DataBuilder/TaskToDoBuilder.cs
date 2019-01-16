using Domain.Entities;
using System;
using System.Collections.Generic;

namespace UnitTest.Integration.Repositories.Repositories.DataBuilder
{
    public class TaskToDoBuilder
    {
        private TaskToDo taskToDo;
        private List<TaskToDo> taskToDoList;

        public TaskToDoBuilder()
        {
        }

        public TaskToDo CreateTaskToDo()
        {
            taskToDo = new TaskToDo() { Title = "Task from Builder", Start = DateTime.Now, DeadLine = DateTime.Now };
            return taskToDo;
        }

        public TaskToDo CreateTaskToDoWithUser(int id)
        {
            taskToDo = new TaskToDo() { Title = "Task from Builder", Start = DateTime.Now, DeadLine = DateTime.Now, UserId = id };
            return taskToDo;
        }

        public List<TaskToDo> CreateTaskToDoList(int amount)
        {
            taskToDoList = new List<TaskToDo>();
            for (int i = 0; i < amount; i++)
            {
                taskToDoList.Add(CreateTaskToDo());
            }

            return taskToDoList;
        }

        public List<TaskToDo> CreateTaskToDoListWithUser(int amount, int id)
        {
            taskToDoList = new List<TaskToDo>();
            for (int i = 0; i < amount; i++)
            {
                taskToDoList.Add(CreateTaskToDoWithUser(id));
            }

            return taskToDoList;
        }
    }
}
