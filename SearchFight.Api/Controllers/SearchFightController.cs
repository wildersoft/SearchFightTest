using System;
using System.Linq;
using System.Threading.Tasks;

using Microsoft.AspNetCore.Mvc;

using SearchFight.BusinessLogic;

namespace SearchFighter.Api.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class SearchFightController : ControllerBase
    {
        private static readonly log4net.ILog log = log4net.LogManager.GetLogger(typeof(SearchFightController));
        private readonly ISearch _search;

        public SearchFightController(ISearch search)
        {
            _search = search;
        }

        [HttpPost]
        [Route("Search")]
        public async Task<IActionResult> SearchEngines([FromBody] string[] parameters)
        {
            try
            {
                var results = await _search.SearchEngines(parameters);

                if (results == null || !results.Any())
                {
                    return NotFound();
                }

                return Ok(results);
            }
            catch (Exception ex)
            {
                log.Warn(ex);
                return NotFound();
            }
        }
    }
}