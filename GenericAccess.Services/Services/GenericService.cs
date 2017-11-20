using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using AutoMapper;
using GenericAccess.Data.Common;
using GenericAccess.Mappings.Base;
using GenericAccess.Models;
using GenericAccess.Models.SearchModels.Base;
using GenericAccess.Repositories;

namespace GenericAccess.Services
{
    public class GenericService : IGenericService
    {
        private readonly IRepository _repository;

        public GenericService(IRepository repository)
        {
            _repository = repository;
        }

        public TModel Get<TEntity, TModel>(int id)
            where TEntity : class, IEntity
        {
            if (MappingsList.Exist<TEntity, TModel>())
            {
                var query = _repository.GetQueryById<TEntity>(id);
                return query.AsQuery<TEntity, TModel>().FirstOrDefault();
            }

            var entity = _repository.Get<TEntity>(id);
            return entity.To<TModel>();
        }

        public TModel Get<TEntity, TView, TModel>(int id)
            where TEntity : class, IEntity
        {
            var query = _repository.GetQueryById<TEntity>(id);
            return query.AsQuery<TEntity, TView, TModel>().FirstOrDefault();
        }

        public TModel Get<TEntity, TModel>(Expression<Func<TEntity, bool>> filterExpression = null)
            where TEntity : class
        {
            if (MappingsList.Exist<TEntity, TModel>())
            {
                var query = _repository.GetList(filterExpression);
                return query.AsQuery<TEntity, TModel>().FirstOrDefault();
            }

            var entity = _repository.Get(filterExpression);
            return entity.To<TModel>();
        }

        public TModel Get<TEntity, TView, TModel>(Expression<Func<TEntity, bool>> filterExpression = null)
            where TEntity : class, IEntity
        {
            var query = _repository.GetList(filterExpression);
            return query.AsQuery<TEntity, TView, TModel>().FirstOrDefault();
        }

        public List<TModel> GetList<TEntity, TModel>(Expression<Func<TEntity, bool>> filterExpression = null)
            where TEntity : class
        {
            var list = _repository.GetList(filterExpression).To<TEntity, TModel>();
            return list;
        }

        public List<TModel> GetList<TEntity, TView, TModel>(Expression<Func<TEntity, bool>> filterExpression = null)
            where TEntity : class, IEntity
        {
            var list = _repository.GetList(filterExpression).AsQuery<TEntity, TView>().AsQuery<TView, TModel>().ToList();
            return list;
        }

        public List<TModel> GetList<TEntity, TModel>(Expression<Func<TEntity, bool>> filterExpression,
                                                     Func<IQueryable<TEntity>, IQueryable<TEntity>> include)
            where TEntity : class
        {
            var list = _repository.GetList(filterExpression, include).ToList();
            return list.To<TModel>();
        }

        public SearchResultModel<TModel> Search<TEntity, TModel>(SearchModel<TEntity, TEntity> pagination)
            where TEntity : class
        {
            return Search<TEntity, TEntity, TModel>(pagination);
        }

        public SearchResultModel<TModel> Search<TEntity, TView, TModel>(SearchModel<TEntity, TView> pagination)
            where TEntity : class
            where TView : class
        {
            var query = _repository.Query<TEntity>();
            var model = pagination.Find(query);

            return new SearchResultModel<TModel>
            {
                List = model.Query.To<TView, TModel>(),
                Count = model.Count
            };
        }

        public IQueryable<TModel> Query<TEntity, TModel>(IFilterable<TEntity> searchModel)
            where TEntity: class
        {
            var query = _repository.Query<TEntity>();
            query = searchModel.Filter(query);
            return query.AsQuery<TEntity, TModel>();
        } 

        public List<TModel> GetQueryMappingList<TEntity, TModel>(Expression<Func<TEntity, bool>> filterExpression = null)
            where TEntity : class, IEntity
        {
            return _repository.GetList(filterExpression).AsQuery<TEntity, TModel>().ToList();
        }

        public int Add<TEntity>(object model)
            where TEntity : class, IEntity
        {
            var entity = Mapper.Map<TEntity>(model);
            _repository.Add(entity);
            _repository.Commit();
            return entity.PrimaryKey;
        }

        public void Update<TEntity>(int id, object model)
            where TEntity : class, IEntity
        {
            var entity = _repository.Get<TEntity>(id);
            Mapper.Map(model, entity);
            _repository.Update(entity);
            _repository.Commit();
        }
        
        public void UpdateRange<TEntity, TModel>(List<TModel> models, Func<IQueryable<TEntity>, IQueryable<TEntity>> entityInclude = null) where TEntity : class, IEntity where TModel : class, IEntityModel
        {
            var ids = models.Select(x => x.Id).ToArray();
            var filterExpression = ExpressionHelper.FilterExpressionByIds<TEntity>(ids.ToArray());
            var entitiesToUpdate = _repository.GetList(filterExpression, entityInclude).ToList();
            foreach (var entity in entitiesToUpdate)
            {
                var updatedEntity = models.FirstOrDefault(x => x.Id == entity.PrimaryKey);
                if (updatedEntity != null)
                {
                    Mapper.Map(updatedEntity, entity);
                }
            }
            _repository.Commit();
        }

        public void Update<TEntity>(int id, object model, Func<IQueryable<TEntity>, IQueryable<TEntity>> entityInclude) where TEntity : class, IEntity
        {
            var pkExpression = ExpressionHelper.PkFilterExpression<TEntity>(id);
            var entity = _repository.Get(pkExpression, entityInclude);
            Mapper.Map(model, entity);
            _repository.Update(entity);
            _repository.Commit();
        }

        public void UpdateProperty<TEntity, TProperty>(int id, Expression<Func<TEntity, TProperty>> propertySelector, TProperty value)
            where TEntity : class
        {
            _repository.UpdateProperty(id, propertySelector, value);
            _repository.Commit();
        }

        public void UpdateProperty<TEntity, TProperty>(Expression<Func<TEntity, bool>> selector, Expression<Func<TEntity, TProperty>> propertySelector, TProperty value)
            where TEntity : class
        {
            _repository.UpdateProperty(selector, propertySelector, value);
            _repository.Commit();
        }

        public void UpdateProperty<TEntity>(Expression<Func<TEntity, bool>> selector, string propertyName, object value)
            where TEntity : class
        {
            _repository.UpdateProperty(selector, propertyName, value);
            _repository.Commit();
        }

        public void UpdateProperty<TEntity>(int id, string propertyName, object value)
            where TEntity : class
        {
            _repository.UpdateProperty<TEntity>(id, propertyName, value);
            _repository.Commit();
        }

        public void Sync<TFrom, TTo>(int id, bool reload = false)
            where TFrom : class, IEntity
            where TTo : class, IEntity
        {
            var from = _repository.Get<TFrom>(id);
            var to = _repository.Get<TTo>(id);

            if (reload)
            {
                _repository.Reload(from);
            }

            Mapper.Map(from, to);
        }

        public void SyncList<TFrom, TTo>(List<int> ids)
            where TFrom : class, IEntity
            where TTo : class, IEntity
        {
            var fromList = _repository.GetList<TFrom>(ids).ToList();
            var toList = _repository.GetList<TTo>(ids).ToList();

            foreach (var to in toList)
            {
                var from = fromList.FirstOrDefault(x => x.PrimaryKey == to.PrimaryKey);
                if (from == null) continue;
                Mapper.Map(from, to);
            }
        }

        public void SyncAndSave<TFrom, TTo>(int id, bool reload = false)
            where TFrom : class, IEntity
            where TTo : class, IEntity
        {
            Sync<TFrom, TTo>(id, reload);
            _repository.Commit();
        }

        public void SyncListAndSave<TFrom, TTo>(List<int> ids)
            where TFrom : class, IEntity
            where TTo : class, IEntity
        {
            SyncList<TFrom, TTo>(ids);
            _repository.Commit();
        }

        public void Delete<TEntity>(int id)
            where TEntity : class, IEntity
        {
            var entity = _repository.Get<TEntity>(id);
            Delete(entity);
        }

        public void Delete<TEntity>(Expression<Func<TEntity, bool>> filterExpression)
            where TEntity : class
        {
            var entity = _repository.Get(filterExpression);
            Delete(entity);
        }

        public void Delete<TEntity>(TEntity entity)
            where TEntity : class
        {
            _repository.Remove(entity);
            _repository.Commit();
        }

        public void DeleteRange<TEntity>(IEnumerable<TEntity> entities) where TEntity : class
        {
            _repository.RemoveRange(entities);
            _repository.Commit();
        }

        public void DeleteRange<TEntity>(Expression<Func<TEntity, bool>> filterExpression) where TEntity : class
        {
            _repository.RemoveRange(filterExpression);
            _repository.Commit();
        }

        public void AddRange<TEntity, TModel>(List<TModel> models) where TEntity : class, IEntity
        {
            foreach (var model in models)
            {
                var entity = Mapper.Map<TEntity>(model);
                _repository.Add(entity);
            }
            _repository.Commit();
        }
    }
}