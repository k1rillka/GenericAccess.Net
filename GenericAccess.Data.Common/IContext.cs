using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Data.Entity.Infrastructure;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace GenericAccess.Data.Common
{
    public interface IContext
    {
        DbSet<TEntity> Set<TEntity>()
            where TEntity : class;

        void MarkEntryModified<TEntity>(TEntity entity)
            where TEntity : class;

        bool UpdateEntry<TEntity>(int id, TEntity entity)
            where TEntity : class;

        void UpdateEntryProperty<TEntity, TProperty>(TEntity entity, Expression<Func<TEntity, TProperty>> propertySelector, TProperty value)
            where TEntity : class;

        void UpdateEntryProperty<TEntity>(TEntity entity, string propertyName, object value)
            where TEntity : class;

        void Reload<TEntity>(TEntity entity)
            where TEntity : class;

        ITransaction BeginTransaction();

        int ExecuteSqlCommand(string sql, params object[] paramsObjects);

        Database Database { get; }

        DbChangeTracker ChangeTracker { get; }

        int SaveChanges();
    }
}
