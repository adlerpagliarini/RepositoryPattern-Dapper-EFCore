using Domain.Entities;
using Infrastructure.Interfaces.Repositories.EFCore;
using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Text;
using Infrastructure.Interfaces.Repositories.EFCore.Query;

namespace Infrastructure.Repositories.Domain.EFCore.Specifications
{
    public class UserByIdIncludingTasksSpec : BaseSpecification<User>
    {
        public UserByIdIncludingTasksSpec(int id, int skip = 0, int take = 0) : base(u => u.Id == id)
        {
            AddIncludes(i => i.Include(e => e.TasksToDo)); // .ThenInclude(e => e)

            if (take > 0)
                ApplyPaging(skip, take);
        }

        public UserByIdIncludingTasksSpec(int id, string name) : base(u => u.Id == id)
        {
            AddCriteria(e => e.Name == name);
            AddIncludes(i => i.Include(e => e.TasksToDo));
            ApplyOrderByDescending(e => e.Id);
        }
    }
}
