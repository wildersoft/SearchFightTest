using System.Collections.Generic;
using System.Threading.Tasks;

using SearchFight.Model;

namespace SearchFight.BusinessLogic
{
    public interface ISearch
    {
        Task<IEnumerable<SearchResult>> SearchEngines(params string[] languages);
    }
}