using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading.Tasks;

using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

using SearchFight.BusinessLogic;

namespace SearchFighter.Api.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class SearchFightController : ControllerBase
    {
        private readonly ILogger<SearchFightController> _logger;
        private readonly ISearch _search;

        public SearchFightController(ILogger<SearchFightController> logger, ISearch search)
        {
            _logger = logger;
            _search = search;
        }

        [HttpGet]
        public ActionResult Index()
        {
            string result = "algo";
            return Ok(result);
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
                _logger.LogWarning(ex.Message);
                return NotFound();
            }
        }

        public string GetJsonFromBody()
        {
            try
            {
                if (!this.Request.ContentLength.HasValue)
                {
                    return null;
                }
                int len = (int)(this.Request.ContentLength ?? 0);
                byte[] buff = new byte[len];
                this.Request.Body.Read(buff, 0, len);
                string result = System.Text.Encoding.Default.GetString(buff);
                return result;
            }
            catch (Exception ex)
            {
                return null;
            }
        }
    }
}