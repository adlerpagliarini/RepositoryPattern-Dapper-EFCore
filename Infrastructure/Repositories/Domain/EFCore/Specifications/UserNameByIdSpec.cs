using Domain.Entities;
using Infrastructure.Interfaces.Repositories.EFCore;
using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Text;
using Infrastructure.Interfaces.Repositories.EFCore.Query;

namespace Infrastructure.Repositories.Domain.EFCore.Specifications
{
    public class UserNameByIdSpec : BaseSpecification<User, string>
    {
        public UserNameByIdSpec(int id) : base(u => u.Id == id)
        {
            Expression<Func<User, string>> selector = e => e.Name;
            Selector = selector;
        }
    }
}
