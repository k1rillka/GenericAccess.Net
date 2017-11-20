using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;
using GenericAccess.Data.Common;
using GenericAccess.Models.SearchModels.Base;

namespace GenericAccess.Services
{
    public interface IGenericService
    {
        TModel Get<TEntity, TModel>(int id)
            where TEntity : class, IEntity;

        TModel Get<TEntity, TView, TModel>(int id)
            where TEntity : class, IEntity;

        TModel Get<TEntity, TModel>(Expression<Func<TEntity, bool>> filterExpression)
            where TEntity : class;

        TModel Get<TEntity, TView, TModel>(Expression<Func<TEntity, bool>> filterExpression = null)
            where TEntity : class, IEntity;

        List<TModel> GetList<TEntity, TModel>(Expression<Func<TEntity, bool>> filterExpression = null)
            where TEntity : class;

        List<TModel> GetList<TEntity, TView, TModel>(Expression<Func<TEntity, bool>> filterExpression = null)
            where TEntity : class, IEntity;

        List<TModel> GetList<TEntity, TModel>(Expression<Func<TEntity, bool>> filterExpression,
            Func<IQueryable<TEntity>, IQueryable<TEntity>> include)
            where TEntity : class;

        SearchResultModel<TModel> Search<TEntity, TModel>(SearchModel<TEntity, TEntity> pagination)
            where TEntity : class;

        SearchResultModel<TModel> Search<TEntity, TView, TModel>(SearchModel<TEntity, TView> pagination)
            where TEntity : class
            where TView : class;

        IQueryable<TModel> Query<TEntity, TModel>(IFilterable<TEntity> searchModel)
            where TEntity : class;

        List<TModel> GetQueryMappingList<TEntity, TModel>(Expression<Func<TEntity, bool>> filterExpression = null)
            where TEntity : class, IEntity;

        int Add<TEntity>(object model)
            where TEntity : class, IEntity;

        void Update<TEntity>(int id, object model)
            where TEntity : class, IEntity;

        void Update<TEntity>(int id, object model, Func<IQueryable<TEntity>, IQueryable<TEntity>> entityInclude)
            where TEntity : class, IEntity;

        void UpdateProperty<TEntity, TProperty>(int id, Expression<Func<TEntity, TProperty>> propertySelector, TProperty value)
            where TEntity : class;

        void UpdateProperty<TEntity, TProperty>(Expression<Func<TEntity, bool>> selector, Expression<Func<TEntity, TProperty>> propertySelector, TProperty value)
            where TEntity : class;

        void UpdateProperty<TEntity>(Expression<Func<TEntity, bool>> selector, string propertyName, object value)
            where TEntity : class;

        void UpdateProperty<TEntity>(int id, string propertyName, object value)
            where TEntity : class;

        void Sync<TFrom, TTo>(int id, bool reload = false)
            where TFrom : class, IEntity
            where TTo : class, IEntity;

        void SyncList<TFrom, TTo>(List<int> ids)
            where TFrom : class, IEntity
            where TTo : class, IEntity;

        void SyncAndSave<TFrom, TTo>(int id, bool reload = false)
            where TFrom : class, IEntity
            where TTo : class, IEntity;

        void SyncListAndSave<TFrom, TTo>(List<int> ids)
            where TFrom : class, IEntity
            where TTo : class, IEntity;

        void Delete<TEntity>(int id)
            where TEntity : class, IEntity;

        void Delete<TEntity>(Expression<Func<TEntity, bool>> filterExpression)
            where TEntity : class;

        void Delete<TEntity>(TEntity entity)
            where TEntity : class;

        void DeleteRange<TEntity>(IEnumerable<TEntity> entities) where TEntity : class;

        void DeleteRange<TEntity>(Expression<Func<TEntity, bool>> filterExpression) where TEntity : class;

        void UpdateRange<TEntity, TModel>(List<TModel> models, Func<IQueryable<TEntity>, IQueryable<TEntity>> entityInclude = null) where TEntity : class, IEntity

            where TModel : class, IEntityModel;

        void AddRange<TEntity, TModel>(List<TModel> models) where TEntity : class, IEntity;
    }
}
