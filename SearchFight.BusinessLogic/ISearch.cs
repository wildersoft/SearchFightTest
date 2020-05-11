using System.Collections.Generic;
using System.Threading.Tasks;

using SearchFight.BusinessLogic.Model;

namespace SearchFight.BusinessLogic
{
    public interface ISearch
    {
        List<SearchEngines> GetSearchEngines();
        Task<IEnumerable<SearchResult>> SearchEngines(params string[] languages);
    }
}