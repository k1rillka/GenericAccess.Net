using System.Collections.Generic;

namespace GenericAccess.Models.SearchModels.Base
{
    public class SearchResultModel<TModel> 
    {
        public SearchResultModel()
        {
            List = new List<TModel>();
        } 

        public List<TModel> List { get; set; }
        public int Count { get; set; }
    }
}