using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using GenericAccess.Data.Common;

namespace GenericAccess.Repositories
{
    public interface IRepository
    {
        IQueryable<TEntity> Query<TEntity>() 
            where TEntity : class;

        TEntity Find<TEntity>(object id)
            where TEntity : class;

        TEntity Get<TEntity>(int id) 
            where TEntity : class, IEntity;

        TEntity Get<TEntity>(Expression<Func<TEntity, bool>> predicate = null) 
            where TEntity : class;

        TEntity Get<TEntity>(Expression<Func<TEntity, bool>> predicate,
            Func<IQueryable<TEntity>, IQueryable<TEntity>> entityInclude) 
            where TEntity : class;

        IQueryable<TEntity> GetQueryById<TEntity>(int id)
            where TEntity : class;

        IQueryable<TEntity> GetList<TEntity>(List<int> ids) where TEntity : class;

        IQueryable<TEntity> GetList<TEntity>(Expression<Func<TEntity, bool>> predicate = null) 
            where TEntity : class;

        IQueryable<TEntity> GetList<TEntity>(Expression<Func<TEntity, bool>> predicate,
            Func<IQueryable<TEntity>, IQueryable<TEntity>> entityInclude) 
            where TEntity : class;

        TProperty GetProperty<TEntity, TProperty>(int id, Expression<Func<TEntity, TProperty>> selector)
            where TEntity : class, IEntity;

        TProperty GetProperty<TEntity, TProperty>(Expression<Func<TEntity, bool>> predicate, 
                                                  Expression<Func<TEntity, TProperty>> selector) 
            where TEntity : class;

        bool Any<TEntity>(Expression<Func<TEntity, bool>> predicate = null) 
            where TEntity : class;

        void Add<TEntity>(TEntity entity) 
            where TEntity : class;

        void AddRange<TEntity>(List<TEntity> entities) 
            where TEntity : class;

        void Update<TEntity>(TEntity model) 
            where TEntity : class;

        void UpdateProperty<TEntity, TProperty>(int id, Expression<Func<TEntity, TProperty>> propertySelector, TProperty value)
            where TEntity : class;

        void UpdateProperty<TEntity, TProperty>(Expression<Func<TEntity, bool>> selector, Expression<Func<TEntity, TProperty>> propertySelector, TProperty value)
            where TEntity : class;

        void UpdateProperty<TEntity>(Expression<Func<TEntity, bool>> selector, string propertyName, object value)
            where TEntity : class;

        void UpdateProperty<TEntity>(int id, string propertyName, object value)
            where TEntity : class;

        void AddOrUpdate<TEntity>(TEntity entity)
            where TEntity: class;

        void Remove<TEntity>(int id) where TEntity : class, IEntity;

        void Remove<TEntity>(Expression<Func<TEntity, bool>> predicate) where TEntity : class;

        void Remove<TEntity>(TEntity entity) 
            where TEntity : class;

        void RemoveRange<TEntity>(IEnumerable<TEntity> entities) where TEntity : class;

        void RemoveRange<TEntity>(Expression<Func<TEntity, bool>> predicate) where TEntity : class;

        void Reload<TEntity>(TEntity entity)
            where TEntity: class;

        int ExecuteSqlCommand(string sql, params object[] paramsObjects);

        IList<TEntity> SqlQuery<TEntity>(string sql);

        IList<TEntity> SqlQuery<TEntity>(string sql, params object[] paramsObjects);

        IQueryable<TEntity> SqlQueryAsQueryable<TEntity>(string sql, params object[] paramsObjects);
        
        void Transaction(Action<ITransaction> action);

        void Commit();
    }
}