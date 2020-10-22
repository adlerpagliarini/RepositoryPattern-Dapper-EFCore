using Domain.Entities;
using Infrastructure.Interfaces.Repositories.EFCore;
using Infrastructure.Interfaces.Repositories.Standard;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;

namespace Infrastructure.Repositories.Standard.EFCore
{
    public class RepositoryAsync<TEntity> : SpecificMethods<TEntity>, IRepositoryAsync<TEntity> where TEntity : class, IIdentityEntity
    {

        protected readonly DbContext dbContext;
        protected readonly DbSet<TEntity> dbSet;

        protected RepositoryAsync(DbContext dbContext)
        {
            this.dbContext = dbContext;
            this.dbSet = this.dbContext.Set<TEntity>();
        }

        public void Dispose()
        {
            dbContext.Dispose();
            GC.SuppressFinalize(this);
        }

        public virtual async Task<TEntity> AddAsync(TEntity obj)
        {
            var r = await dbSet.AddAsync(obj);
            await CommitAsync();
            return r.Entity;
        }

        public virtual async Task<int> AddRangeAsync(IEnumerable<TEntity> entities)
        {
            await dbSet.AddRangeAsync(entities);
            return await CommitAsync();
        }

        public virtual async Task<IEnumerable<TEntity>> GetAllAsync()
        {
            return await Task.FromResult(dbSet);
        }

        public virtual async Task<TEntity> GetByIdAsync(object id)
        {
            return await dbSet.FindAsync(id);
        }

        public virtual async Task<bool> RemoveAsync(object id)
        {
            TEntity entity = await GetByIdAsync(id);

            if (entity == null) return false;

            return await RemoveAsync(entity) > 0 ? true : false;
        }

        public virtual async Task<int> RemoveAsync(TEntity obj)
        {
            dbSet.Remove(obj);
            return await CommitAsync();
        }

        public virtual async Task<int> RemoveRangeAsync(IEnumerable<TEntity> entities)
        {
            dbSet.RemoveRange(entities);
            return await CommitAsync();
        }

        public virtual async Task<int> UpdateAsync(TEntity obj)
        {
            var avoidingAttachedEntity = await GetByIdAsync(obj.Id);
            dbContext.Entry(avoidingAttachedEntity).State = EntityState.Detached;

            var entry = dbContext.Entry(obj);
            if (entry.State == EntityState.Detached) dbContext.Attach(obj);

            dbContext.Entry(obj).State = EntityState.Modified;
            return await CommitAsync();
        }

        public virtual async Task<int> UpdateRangeAsync(IEnumerable<TEntity> entities)
        {
            dbSet.UpdateRange(entities);
            return await CommitAsync();
        }

        public async Task<IReadOnlyList<TEntity>> ApplySpecification(ISpecification<TEntity> spec)
        {
            return await Specification(spec).ToListAsync();
        }

        private IQueryable<TEntity> Specification(ISpecification<TEntity> spec)
        {
            // Wont load includes
            // return SpecificationEvaluator<TEntity>.GetQuery(dbSet.AsQueryable(), spec);
            return EfSpecificationEvaluator<TEntity>.GetQuery(dbSet, spec);
        }

        public async Task<IReadOnlyList<TResult>> ApplySpecification<TResult>(ISpecification<TEntity, TResult> spec)
        {
            if (spec is null) throw new ArgumentNullException("spec is required");
            if (spec.Selector is null) throw new Exception("Specification must have Selector defined.");

            return await Specification(spec).ToListAsync();
        }

        private IQueryable<TResult> Specification<TResult>(ISpecification<TEntity, TResult> spec)
        {
            return EfSpecificationEvaluator<TEntity, TResult>.GetQuery(dbSet, spec);
        }

        private async Task<int> CommitAsync()
        {
            return await dbContext.SaveChangesAsync();
        }

        #region ProtectedMethods
        protected override IQueryable<TEntity> GenerateQuery(Expression<Func<TEntity, bool>> filter = null,
            Func<IQueryable<TEntity>, IOrderedQueryable<TEntity>> orderBy = null,
            params string[] includeProperties)
        {
            IQueryable<TEntity> query = dbSet;
            query = GenerateQueryableWhereExpression(query, filter);
            query = GenerateIncludeProperties(query, includeProperties);

            if (orderBy != null)
                return orderBy(query);

            return query;
        }
        private IQueryable<TEntity> GenerateQueryableWhereExpression(IQueryable<TEntity> query,
            Expression<Func<TEntity, bool>> filter)
        {
            if (filter != null)
                return query.Where(filter);

            return query;
        }

        private IQueryable<TEntity> GenerateIncludeProperties(IQueryable<TEntity> query, params string[] includeProperties)
        {
            foreach (string includeProperty in includeProperties)
                query = query.Include(includeProperty);

            return query;
        }

        protected override IEnumerable<TEntity> GetYieldManipulated(IEnumerable<TEntity> entities, Func<TEntity, TEntity> DoAction)
        {
            foreach (var e in entities)
            {
                yield return DoAction(e);
            }
            yield break;
        }

        public override async Task<int> UpdateTrackedAsync(TEntity obj)
        {
            var avoidingAttachedEntity = await GetByIdAsync(obj.Id);
            DetachTracked(avoidingAttachedEntity);
            dbContext.Update(obj);
            return await CommitAsync();
        }

        public override async Task<int> UpdateRelatedAsync(TEntity obj)
        {
            dbContext.Attach(obj);
            var unchangedEntities = dbContext.ChangeTracker.Entries().Where(e => e.State == EntityState.Unchanged);

            foreach (EntityEntry ee in unchangedEntities)
                ee.State = EntityState.Modified;

            return await CommitAsync();
        }

        public override void DetachTracked(object obj)
        {
            foreach(var entry in dbContext.Entry(obj).Navigations)
            {
                if (entry is ReferenceEntry && entry.CurrentValue is object entityObject)
                    dbContext.Entry(entityObject).State = EntityState.Detached;

                if (entry is CollectionEntry && entry.CurrentValue is IEnumerable<object> relatedCollection)
                    foreach (var collection in relatedCollection)
                        DetachTracked(collection);
            }
            dbContext.Entry(obj).State = EntityState.Detached;
        }
        #endregion ProtectedMethods
    }
}
