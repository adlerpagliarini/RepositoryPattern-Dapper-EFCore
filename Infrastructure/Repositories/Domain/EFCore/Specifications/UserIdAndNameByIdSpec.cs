using Domain.Entities;
using Infrastructure.Interfaces.Repositories.EFCore;
using System;
using System.Linq.Expressions;

namespace Infrastructure.Repositories.Domain.EFCore.Specifications
{
    public class UserIdAndNameByIdSpec : BaseSpecification<User, UserIdentifierDto>
    {
        public UserIdAndNameByIdSpec(int id) : base(u => u.Id == id)
        {
            Expression<Func<User, UserIdentifierDto>> selector = e => new UserIdentifierDto() { Id = e.Id, Name = e.Name };

            Selector = selector;
        }
    }

    public class UserIdentifierDto
    {
        public int Id { get; set; }
        public string Name { get; set; }
    }
}
