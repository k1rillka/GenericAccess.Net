using System.Linq;

namespace GenericAccess.Mappings.Base
{
    public static class MappingsExtensions
    {
        public static IQueryable<TModel> AsQuery<TEntity, TView, TModel>(this IQueryable<TEntity> query)
        {
            return query.AsQuery<TEntity, TView>().AsQuery<TView, TModel>();
        }

        public static IQueryable<TModel> AsQuery<TEntity, TModel>(this IQueryable<TEntity> query)
        {
            return MappingsList.Apply<TEntity, TModel>(query);
        }
    }
}