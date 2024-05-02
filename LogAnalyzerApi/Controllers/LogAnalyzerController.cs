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

        // Count Total available logs in a period API Controller
        // GET localhost:5215/LogAnalyzer/CountTotalLogs?startDate=2019-10-19&endDate=2024-04-07
        [HttpGet("CountTotalLogs")]
        public ActionResult<int> CountTotalLogs(DateTime startDate, DateTime endDate)
        {
            var totalLogs = _logAnalyzer.CountTotalLogs(startDate, endDate);
            return Ok(totalLogs);
        }

        // Search Logs in directories API Controller
        // GET localhost:5215/LogAnalyzer/SearchLogs?searchPattern=*.log
        [HttpGet(Name = "SearchLogs")]
        public ActionResult<List<string>> SearchLogs(string searchPattern)
        {
            var foundLogs = _logAnalyzer.SearchLogs(searchPattern);
            return Ok(foundLogs);
        }

        
    }
}
