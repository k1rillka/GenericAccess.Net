using System;
using System.Collections.Generic;
using System.Data.Entity.Migrations;
using System.Data.Entity.Validation;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;
using GenericAccess.Data.Common;
using GenericAccess.Exceptions;

namespace GenericAccess.Repositories
{
    public class Repository : IRepository
    {
        protected readonly IContext Context;

        public Repository(IContext context)
        {
            Context = context;
        }

        public virtual IQueryable<TEntity> Query<TEntity>() where TEntity : class
        {
            var query = Context.Set<TEntity>().AsQueryable();
            return typeof(IDeleted).IsAssignableFrom(typeof(TEntity))
                ? query.Where(ExpressionHelper.GetInvertedBoolExpression<TEntity>("IsDeleted"))
                : query;
        }

        public virtual TEntity Find<TEntity>(object id) where TEntity : class
        {
            return Context.Set<TEntity>().Find(id);
        }

        public virtual TEntity Get<TEntity>(int id) where TEntity : class, IEntity
        {
            var pkExpression = ExpressionHelper.PkFilterExpression<TEntity>(id);
            var entity = Get(pkExpression);
            return entity;
        }

        public virtual TEntity Get<TEntity>(Expression<Func<TEntity, bool>> predicate = null) where TEntity : class
        {
            var result = GetList(predicate).FirstOrDefault();
            return result;
        }

        public virtual TEntity Get<TEntity>(Expression<Func<TEntity, bool>> predicate,
            Func<IQueryable<TEntity>, IQueryable<TEntity>> entityInclude) where TEntity : class
        {
            var result = GetList(predicate, entityInclude).FirstOrDefault();
            return result;
        }

        public IQueryable<TEntity> GetQueryById<TEntity>(int id)
            where TEntity : class
        {
            var pkExpression = ExpressionHelper.PkFilterExpression<TEntity>(id);
            return GetList(pkExpression);
        }

        public IQueryable<TEntity> GetList<TEntity>(List<int> ids) where TEntity : class
        {
            var filterExpression = ExpressionHelper.FilterExpressionByIds<TEntity>(ids);
            return GetList(filterExpression);
        }

        public IQueryable<TEntity> GetList<TEntity>(Expression<Func<TEntity, bool>> predicate = null) where TEntity : class
        {
            return predicate == null ? Query<TEntity>() : Query<TEntity>().Where(predicate);
        }

        public IQueryable<TEntity> GetList<TEntity>(Expression<Func<TEntity, bool>> predicate,
            Func<IQueryable<TEntity>, IQueryable<TEntity>> entityInclude) where TEntity : class
        {
            var result = GetList(predicate);

            return entityInclude == null
                ? result
                : entityInclude(result);
        }

        public TProperty GetProperty<TEntity, TProperty>(int id, Expression<Func<TEntity, TProperty>> selector)
            where TEntity : class, IEntity
        {
            var pkExpression = ExpressionHelper.PkFilterExpression<TEntity>(id);
            return GetProperty(pkExpression, selector);
        }

        public TProperty GetProperty<TEntity, TProperty>(Expression<Func<TEntity, bool>> predicate,
            Expression<Func<TEntity, TProperty>> selector)
            where TEntity : class
        {
            return GetList(predicate).Select(selector).FirstOrDefault();
        }

        public bool Any<TEntity>(Expression<Func<TEntity, bool>> predicate = null) where TEntity : class
        {
            return predicate != null ? Query<TEntity>().Any(predicate) : Query<TEntity>().Any();
        }

        public void Add<TEntity>(TEntity entity) where TEntity : class
        {
            Context.Set<TEntity>().Add(entity);
        }

        public void AddRange<TEntity>(List<TEntity> entities) where TEntity : class
        {
            Context.Set<TEntity>().AddRange(entities);
        }

        public void Update<TEntity>(TEntity model) where TEntity : class
        {
            var entity = model as IEntity;
            if (entity != null)
            {
                var id = entity.PrimaryKey;
                if (Context.UpdateEntry(id, model)) return;
            }

            Context.MarkEntryModified(model);
        }

        public void UpdateProperty<TEntity, TProperty>(int id, Expression<Func<TEntity, TProperty>> propertySelector, TProperty value)
            where TEntity : class
        {
            var pkSelector = ExpressionHelper.PkFilterExpression<TEntity>(id);
            UpdateProperty(pkSelector, propertySelector, value);
        }

        public void UpdateProperty<TEntity, TProperty>(Expression<Func<TEntity, bool>> selector, Expression<Func<TEntity, TProperty>> propertySelector, TProperty value)
            where TEntity : class
        {
            var entity = Get(selector);
            Context.UpdateEntryProperty(entity, propertySelector, value);
        }

        public void UpdateProperty<TEntity>(Expression<Func<TEntity, bool>> selector, string propertyName, object value)
            where TEntity : class
        {
            var entity = Get(selector);
            Context.UpdateEntryProperty(entity, propertyName, value);
        }

        public void UpdateProperty<TEntity>(int id, string propertyName, object value)
            where TEntity : class
        {
            var pkSelector = ExpressionHelper.PkFilterExpression<TEntity>(id);
            UpdateProperty(pkSelector, propertyName, value);
        }

        public void AddOrUpdate<TEntity>(TEntity entity)
            where TEntity : class
        {
            Context.Set<TEntity>().AddOrUpdate(entity);
        }

        public void Remove<TEntity>(int id) where TEntity : class, IEntity
        {
            var entity = Get<TEntity>(id);
            Remove(entity);
        }

        public void Remove<TEntity>(Expression<Func<TEntity, bool>> predicate) where TEntity : class
        {
            var entity = Get(predicate);
            Remove(entity);
        }

        public void Remove<TEntity>(TEntity entity) where TEntity : class
        {
            if (entity == null) return;
            Context.Set<TEntity>().Remove(entity);
        }

        public void RemoveRange<TEntity>(Expression<Func<TEntity, bool>> predicate) where TEntity : class
        {
            var entities = GetList(predicate);
            RemoveRange(entities);
        }

        public void RemoveRange<TEntity>(IEnumerable<TEntity> entities) where TEntity : class
        {
            Context.Set<TEntity>().RemoveRange(entities);
        }

        public IList<TEntity> SqlQuery<TEntity>(string sql)
        {
            return Context.Database.SqlQuery<TEntity>(sql).ToList();
        }

        public IList<TEntity> SqlQuery<TEntity>(string sql, params object[] paramsObjects)
        {
            return Context.Database.SqlQuery<TEntity>(sql, paramsObjects).ToList();
        }

        public IQueryable<TEntity> SqlQueryAsQueryable<TEntity>(string sql, params object[] paramsObjects)
        {
            return Context.Database.SqlQuery<TEntity>(sql, paramsObjects).AsQueryable();
        }

        public int ExecuteSqlCommand(string sql, params object[] paramsObjects)
        {
            return Context.ExecuteSqlCommand(sql, paramsObjects);
        }

        public void Transaction(Action<ITransaction> action)
        {
            using (var transaction = Context.BeginTransaction())
            {
                try
                {
                    action(transaction);

                    transaction.Commit();
                }
                catch (Exception exception)
                {
                    transaction.Rollback();

                    throw new TransactionException(exception);
                }
            }
        }

        public void Reload<TEntity>(TEntity entity)
            where TEntity : class
        {
            Context.Reload(entity);
        }

        public void Commit()
        {
            try
            {
                Context.SaveChanges();
            }
            catch (DbEntityValidationException validationException)
            {
                var validationErrors = validationException.EntityValidationErrors;

                //empty catch here so its easy to look at validation errors
            }
        }

    }
}
