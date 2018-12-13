using Domain.Entities;
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
            taskToDo = new TaskToDo() { Title = "Task from Builder" };
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
    }
}
