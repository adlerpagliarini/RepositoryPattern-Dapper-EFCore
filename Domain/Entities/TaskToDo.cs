using System;

namespace Domain.Entities
{
    public class TaskToDo : IIdentityEntity
    {
        public int Id { get; set; }

        public string Title { get; set; }

        public DateTime Start { get; set; }

        public DateTime DeadLine { get; set; }

        public bool Status { get; set; }

        public int UserId { get; set; }
        public virtual User User { get; set; }
    }
}
