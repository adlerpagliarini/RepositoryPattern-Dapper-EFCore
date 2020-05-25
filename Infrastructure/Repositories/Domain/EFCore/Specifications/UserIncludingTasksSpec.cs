using Domain.Entities;
using Infrastructure.Interfaces.Repositories.EFCore;
using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Text;
using Infrastructure.Interfaces.Repositories.EFCore.Query;

namespace Infrastructure.Repositories.Domain.EFCore.Specifications
{
    public class UserIncludingTasksSpec : BaseSpecification<User>
    {
        public UserIncludingTasksSpec(int skip = 0, int take = 0) : base(u => true)
        {
            AddIncludes(i => i
                .Include(e => e.TasksToDo)
            // .ThenInclude(e => e)
            );

            if (take > 0)
                ApplyPaging(skip, take);
        }
    }
}
