using Microsoft.AspNetCore.Mvc;
using LogAnalyzerLibrary;
using static LogAnalyzerLibrary.getfiles;

namespace LogAnalyzerApi.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class LogAnalyzerController : ControllerBase
    {
        private readonly getfiles _fileAnalyzer;

        public LogAnalyzerController()
        {
            _fileAnalyzer = new getfiles();
        }

        [HttpGet("getallfiles")]
        public IActionResult GetAllFiles()
        {
            var directory = "c:/Logs"; // Default directory
            var files = _fileAnalyzer.GetAllFiles(directory);
            return Ok(files);
        }

        [HttpGet("countduplicatederrors")]
        public IActionResult CountDuplicatedErrors()
        {
            var directory = "c:/Logs"; // Directory containing log files
            var duplicatedErrorsPerFile = new Dictionary<string, int>();

            try
            {
                var logFiles = Directory.GetFiles(directory, "*.log", SearchOption.AllDirectories);

                foreach (var filePath in logFiles)
                {
                    var errorsInFile = _fileAnalyzer.CountDuplicatedErrors(filePath);
                    duplicatedErrorsPerFile.Add(filePath, errorsInFile);
                }

                return Ok(duplicatedErrorsPerFile);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Error counting duplicated errors: {ex.Message}");
            }
        }


        [HttpGet("counterrors")]
        public IActionResult CountErrors()
        {
            var directory = "c:/Logs"; // Directory containing log files
            var errorsPerFile = new Dictionary<string, int>();

            try
            {
                var logFiles = Directory.GetFiles(directory, "*.log", SearchOption.AllDirectories);

                foreach (var filePath in logFiles)
                {
                    var errorCount = _fileAnalyzer.CountErrors(filePath);
                    errorsPerFile.Add(filePath, errorCount);
                }

                return Ok(errorsPerFile);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Error counting errors: {ex.Message}");
            }
        }


        [HttpPost("archiveLogsToZip")]
        public IActionResult ArchiveLogsToZip([FromBody] ArchiveLogsToZipRequest request)
        {
            try
            {
                if (request == null || request.StartDate == default || request.EndDate == default)
                {
                    return BadRequest("Invalid request. Please provide valid start and end dates.");
                }

                string sourceDirectory = "c:/Logs"; // Source directory containing log files
                string zipDirectory = "c:/Logs/Zip"; // Zip directory

                bool success = _fileAnalyzer.ArchiveLogsToZip(sourceDirectory, zipDirectory, request.StartDate, request.EndDate);

                if (success)
                    return Ok("Logs archived to zip file successfully.");
                else
                    return StatusCode(500, "Error archiving logs to zip. Please check the server logs.");
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Error archiving logs to zip: {ex.Message}");
            }
        }


        [HttpPost("deleteArchivesByPeriod")]
        public IActionResult DeleteArchivesByPeriod([FromBody] DeleteArchivesByPeriodRequest request)
        {
            try
            {
                if (request == null || string.IsNullOrEmpty(request.Period))
                {
                    return BadRequest("Invalid request. Please provide a valid period.");
                }

                string zipDirectory = "c:/Logs/Zip"; // Zip directory

                bool success = _fileAnalyzer.DeleteArchivesByPeriod(zipDirectory, request.Period);

                if (success)
                    return Ok($"Archives for period {request.Period} deleted successfully.");
                else
                    return NotFound($"No archives found for period {request.Period}.");
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Error deleting archives by period: {ex.Message}");
            }
        }

        public class DeleteArchivesByPeriodRequest
        {
            public string Period { get; set; }
        }


        [HttpPost("deleteLogsByPeriod")]
        public IActionResult DeleteLogsByPeriod([FromBody] DeleteLogsByPeriodRequest request)
        {
            try
            {
                if (request == null || request.StartDate == default || request.EndDate == default)
                {
                    return BadRequest("Invalid request. Please provide valid start and end dates.");
                }

                string sourceDirectory = "c:/Logs"; // Source directory containing log files

                bool success = _fileAnalyzer.DeleteLogsByPeriod(sourceDirectory, request.StartDate, request.EndDate);

                if (success)
                    return Ok("Logs deleted successfully.");
                else
                    return StatusCode(500, "Error deleting logs. Please check the server logs.");
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Error deleting logs: {ex.Message}");
            }
        }



        [HttpPost("countLogsByPeriod")]
        public IActionResult CountLogsByPeriod([FromBody] CountLogsByPeriodRequest request)
        {
            try
            {
                if (request == null || request.StartDate == default || request.EndDate == default)
                {
                    return BadRequest("Invalid request. Please provide valid start and end dates.");
                }

                string sourceDirectory = "c:/Logs"; // Source directory containing log files

                int totalCount = _fileAnalyzer.CountLogsByPeriod(sourceDirectory, request.StartDate, request.EndDate);

                return Ok($"Total log files found: {totalCount}");
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Error counting logs: {ex.Message}");
            }
        }


        [HttpPost("searchLogsBySize")]
        public IActionResult SearchLogsBySize([FromBody] SearchLogsBySizeRequest request)
        {
            try
            {
                if (request == null || request.MinSizeKB < 0 || request.MaxSizeKB <= 0 || request.MinSizeKB >= request.MaxSizeKB)
                {
                    return BadRequest("Invalid request. Please provide valid size range.");
                }

                string sourceDirectory = "c:/Logs"; // Source directory containing log files

                List<string> matchingFiles = _fileAnalyzer.SearchLogsBySize(sourceDirectory, request.MinSizeKB, request.MaxSizeKB);

                return Ok(matchingFiles);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Error searching logs by size: {ex.Message}");
            }
        }


        [HttpPost("searchLogsByDirectory")]
        public IActionResult SearchLogsByDirectory([FromBody] SearchLogsByDirectoryRequest request)
        {
            try
            {
                if (string.IsNullOrEmpty(request.DirectoryPath))
                {
                    return BadRequest("Invalid request. Please provide a directory path.");
                }

                List<string> matchingFiles = _fileAnalyzer.SearchLogsByDirectory(request.DirectoryPath);

                return Ok(matchingFiles);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Error searching logs by directory: {ex.Message}");
            }
        }


        [HttpPost("uploadLogsToServer")]
        public async Task<IActionResult> UploadLogsToServer([FromBody] UploadLogsToServerRequest request)
        {
            try
            {
                if (string.IsNullOrEmpty(request.ServerUrl) || string.IsNullOrEmpty(request.DirectoryPath))
                {
                    return BadRequest("Invalid request. Please provide a server URL and directory path.");
                }

                bool success = await _fileAnalyzer.UploadLogsToServer(request.ServerUrl, request.DirectoryPath);

                if (success)
                    return Ok("Log files uploaded successfully.");
                else
                    return StatusCode(500, "Error uploading log files. Please check the server logs.");
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Error uploading log files: {ex.Message}");
            }
        }

    }
}


    public class ArchiveLogsToZipRequest
    {
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
    }


    public class DeleteLogsByPeriodRequest
    {
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
    }


    public class CountLogsByPeriodRequest
    {
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
    }

    public class SearchLogsBySizeRequest
    {
        public int MinSizeKB { get; set; }
        public int MaxSizeKB { get; set; }
    }

    public class SearchLogsByDirectoryRequest
    {
        public string DirectoryPath { get; set; }
    }


    public class UploadLogsToServerRequest
    {
        public string ServerUrl { get; set; }
        public string DirectoryPath { get; set; }
    }
