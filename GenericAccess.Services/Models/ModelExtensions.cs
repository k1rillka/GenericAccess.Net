using System.Collections.Generic;
using System.Linq;
using AutoMapper;
using GenericAccess.Mappings.Base;

namespace GenericAccess.Models
{
    public static class ModelExtensions
    {
        public static TOutput To<TOutput>(this object input)
        {
            return Mapper.Map<TOutput>(input);
        }

        public static List<TOutput> To<TOutput>(this IEnumerable<object> input)
        {
            return input.Select(Mapper.Map<TOutput>).ToList();
        }

        public static List<TModel> To<TEntity, TModel>(this IQueryable<TEntity> input)
            where TEntity: class 
        {
            if (typeof (TEntity) == typeof (TModel))
            {
                return input.ToList().Cast<TModel>().ToList();
            }

            if (MappingsList.Exist<TEntity, TModel>())
            {
                return input.AsQuery<TEntity, TModel>().ToList();
            }
      
            return input.ToList().To<TModel>();
        }

        public static IEnumerable<TModel> ToEnumerable<TModel>(this TModel model)
        {
            yield return model;
        }

        public static IQueryable<TModel> ToQueryable<TModel>(this TModel model)
        {
            return model.ToEnumerable().AsQueryable();
        }
    }
}