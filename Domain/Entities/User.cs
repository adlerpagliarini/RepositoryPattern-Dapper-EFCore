using System.Collections.Generic;
using System.Collections.ObjectModel;

namespace Domain.Entities
{
    public class User : IIdentityEntity
    {
        public int Id { get; set; }

        public string Name { get; set; }

        private ICollection<TaskToDo> _tasksToDo { get; set; }
        public virtual IReadOnlyCollection<TaskToDo> TasksToDo { get { return _tasksToDo as Collection<TaskToDo>; }}

        public User()
        {
            this._tasksToDo = new Collection<TaskToDo>();
        }

        public void AddItemToDo(TaskToDo todo)
        {
            _tasksToDo.Add(todo);
        }
    }
}
