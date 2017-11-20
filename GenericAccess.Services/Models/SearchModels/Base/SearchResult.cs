using System.Linq;

namespace GenericAccess.Models.SearchModels.Base
{
    public class SearchResult<TEntity>
    {
        public IQueryable<TEntity> Query { get; set; }
        public int Count { get; set; }
    }
}