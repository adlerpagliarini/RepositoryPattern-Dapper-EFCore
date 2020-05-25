using Domain.Entities;
using Infrastructure.Interfaces.Repositories.Standard;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Infrastructure.Interfaces.Repositories.EFCore
{
    public class EfSpecificationEvaluator<T, TResult> where T : class, IIdentityEntity
    {
        public static IQueryable<TResult> GetQuery(IQueryable<T> inputQuery, ISpecification<T, TResult> specification)
        {
            var query = EfSpecificationEvaluator<T>.GetQuery(inputQuery, specification);

            // Apply selector
            var selectQuery = query.Select(specification.Selector);

            return selectQuery;
        }
    }

    public class EfSpecificationEvaluator<T> where T : class, IIdentityEntity
    {
        public static IQueryable<T> GetQuery(IQueryable<T> inputQuery, ISpecification<T> specification)
        {
            var query = inputQuery;

            query = SpecificationEvaluator<T>.GetQuery(query, specification);

            // Includes all expression-based includes
            query = specification.Includes.Aggregate(query,
                                    (current, include) => current.Include(include));

            // Include any string-based include statements
            query = specification.IncludeStrings.Aggregate(query,
                                    (current, include) => current.Include(include));

            return query;
        }
    }
}
