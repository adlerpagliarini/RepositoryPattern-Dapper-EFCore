using System;
using System.Collections.Generic;
using System.Linq.Expressions;


// https://github.com/ardalis/Specification
// https://github.com/dotnet-architecture/eShopOnWeb
namespace Infrastructure.Interfaces.Repositories.Standard
{
    public interface ISpecification<T, TResult> : ISpecification<T>
    {
        Expression<Func<T, TResult>> Selector { get; set; }
    }

    public interface ISpecification<T>
    {
        IEnumerable<Expression<Func<T, bool>>> Criterias { get; }
        IEnumerable<Expression<Func<T, object>>> Includes { get; }
        List<string> IncludeStrings { get; }
        Expression<Func<T, object>> OrderBy { get; }
        Expression<Func<T, object>> OrderByDescending { get; }
        Expression<Func<T, object>> GroupBy { get; }

        int Take { get; }
        int Skip { get; }
        bool IsPagingEnabled { get; }
    }
}
