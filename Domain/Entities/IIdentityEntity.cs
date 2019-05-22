using System;

namespace Domain.Entities
{
    public interface IIdentityEntity
    {
        Guid Id { get; set; }
    }
}
