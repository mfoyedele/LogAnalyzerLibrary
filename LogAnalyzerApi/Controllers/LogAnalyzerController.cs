using Microsoft.AspNetCore.Mvc;
using LogAnalyzerLibrary;

namespace LogAnalyzerApi.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class LogAnalyzerController : ControllerBase
    {
        private readonly AccessLog _logAnalyzer;        

        public LogAnalyzerController()
        {
            _logAnalyzer = new AccessLog("C:\\AmadeoLogs", "C:\\AWIErrors", "C:\\Loggings");
        }

        [HttpGet(Name = "SearchLogs")]
        public ActionResult<List<string>> SearchLogs(string searchPattern)
        {
            var foundLogs = _logAnalyzer.SearchLogs(searchPattern);
            return Ok(foundLogs);
        }

    }
}
