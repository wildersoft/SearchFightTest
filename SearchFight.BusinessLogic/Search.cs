using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

using Microsoft.EntityFrameworkCore.Internal;

using SearchFight.Model;
using SearchFight.Data;

namespace SearchFight.BusinessLogic
{
    public class Search : ISearch
    {
        private readonly SearchFightContext context;

        public Search(SearchFightContext searchfighterContext)
        {
            this.context = searchfighterContext;
        }

        public Task<IEnumerable<SearchResult>> SearchEngines(params string[] languages)
        {
            //recovering data by languages
            var results = context.ProgrammingLanguagesPopularity
                .Join(
                    context.ProgrammingLanguages,
                    left => left.ProgrammingLanguageID,
                    right => right.ProgrammingLanguageID,
                    (left, right) => new
                    {
                        left.SearchEngineID,
                        left.Rating,
                        ProgrammingLanguages = right
                    }
                )
                .Join(
                    context.SearchEngines,
                    left => left.SearchEngineID,
                    right => right.SearchEngineID,
                    (left, right) => new
                    {
                        ProgrammingLanguages = left.ProgrammingLanguages,
                        left.Rating,
                        SearchEngines = right.Name
                    }
                )
                .Where(item => languages.Contains(item.ProgrammingLanguages.Name))
                .Select(item =>
                    new SearchResult()
                    {
                        ProgrammingLanguage = item.ProgrammingLanguages.Name,
                        SearchEngine =  item.SearchEngines,
                        Rating = item.Rating
                    }
                )
                .AsEnumerable();

            return Task.FromResult(results);
        }
    }
}