using MongoDB.Bson.Serialization.Attributes;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;

namespace Domain.Entities
{
    public class User : IIdentityEntity
    {
        public Guid Id { get; set; }

        public string Name { get; set; }

        private ICollection<TaskToDo> _tasksToDo { get; set; }
        public virtual IReadOnlyCollection<TaskToDo> TasksToDo { get { return _tasksToDo.ToArray(); } }

        public User()
        {
            Id = Guid.NewGuid();
            _tasksToDo = new Collection<TaskToDo>();
        }

        public void AddItemToDo(TaskToDo todo)
        {
            _tasksToDo.Add(todo);
        }

        public static class UserFactory
        {
            public static User NewUserFactory(ICollection<TaskToDo> tasksToDo)
            {
                return new User() { _tasksToDo = tasksToDo };
            }
        }
    }
}
