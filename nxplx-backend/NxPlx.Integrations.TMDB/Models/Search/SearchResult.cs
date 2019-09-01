using System.Collections.Generic;

namespace NxPlx.Integrations.TMDBApi.Models.Search
{
    public class SearchResult<TResult> where TResult : ResultBase
    {
        public int page { get; set; }
        public List<TResult> results { get; set; }
        public int total_results { get; set; }
        public int total_pages { get; set; }
    }
}