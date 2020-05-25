using Infrastructure.Interfaces.Repositories.EFCore.Query;
using System;
using System.Collections.Generic;
using System.Text;

namespace Infrastructure.Interfaces.Repositories.Standard
{
    public interface IIncludeQuery
    {
        Dictionary<IIncludeQuery, string> PathMap { get; }
        IncludeVisitor Visitor { get; }
        HashSet<string> Paths { get; }
    }

    public interface IIncludeQuery<TEntity, out TPreviousProperty> : IIncludeQuery
    {
    }
}
