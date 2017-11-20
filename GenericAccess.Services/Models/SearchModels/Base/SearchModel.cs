using System.Linq;
using GenericAccess.Mappings.Base;

namespace GenericAccess.Models.SearchModels.Base
{
    public interface IFilterable<TEntity>
    {
        IQueryable<TEntity> Filter(IQueryable<TEntity> query);
    }

    public abstract class SearchModel<TEntity> : SearchModel<TEntity, TEntity>
    {
        protected override IQueryable<TEntity> BuildView(IQueryable<TEntity> query)
        {
            return query;
        }
    }

    public abstract class SearchModel<TEntity, TView>: IFilterable<TView>
    {
        public int Skip { get; set; } = 0;

        public virtual int Take { get; set; } = 1000;

        protected abstract IQueryable<TView> Ordering(IQueryable<TView> query);

        public virtual SearchResult<TView> Find(IQueryable<TEntity> query)
        {
            var view = BuildView(query);
            view = Filter(view);

            return new SearchResult<TView>
            {
                Query = Ordering(view).Skip(Skip).Take(Take),
                Count = view.Count()
            };
        }

        protected virtual IQueryable<TView> BuildView(IQueryable<TEntity> query)
        {
            return query.AsQuery<TEntity, TView>();
        }

        public virtual IQueryable<TView> Filter(IQueryable<TView> view)
        {
            return view;
        }
    }
}