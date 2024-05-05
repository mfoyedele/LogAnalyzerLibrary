using System;
using System.Collections.Generic;
using System.IO.Compression;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using System.Net.Http;

namespace LogAnalyzerLibrary
{    
    public class getfiles
    {
        public List<string> GetAllFiles(string sDirt)
        {
            List<string> files = new List<string>();

            try
            {
                foreach (string file in Directory.GetFiles(sDirt))
                {
                    files.Add(file);
                }
                foreach (string fl in Directory.GetDirectories(sDirt))
                {
                    files.AddRange(GetAllFiles(fl));
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }

            return files;
        }

        public int CountDuplicatedErrors(string filePath)
        {
            int duplicatedErrors = 0;
            Dictionary<string, int> errorCounts = new Dictionary<string, int>();

            try
            {
                string[] lines = File.ReadAllLines(filePath);

                foreach (string line in lines)
                {
                    // Assuming each error is identified by a unique string in the log file
                    if (!errorCounts.ContainsKey(line))
                    {
                        errorCounts[line] = 1;
                    }
                    else
                    {
                        errorCounts[line]++;
                        if (errorCounts[line] == 2) // Count as duplicated if it occurs more than once
                        {
                            duplicatedErrors++;
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error counting duplicated errors in file {filePath}: {ex.Message}");
            }

            return duplicatedErrors;
        }

        public int CountErrors(string filePath)
        {
            int errorCount = 0;

            try
            {
                string[] lines = File.ReadAllLines(filePath);

                foreach (string line in lines)
                {
                    // Example: Counting errors if each line contains "ERROR"
                    if (line.Contains("ERROR"))
                    {
                        errorCount++;
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error counting errors in file {filePath}: {ex.Message}");
            }

            return errorCount;
        }

        public bool DeleteArchivesByPeriod(string zipDirectory, string period)
        {
            try
            {
                string periodZipDirectory = Path.Combine(zipDirectory, period);

                if (Directory.Exists(periodZipDirectory))
                {
                    DeleteDirectory(periodZipDirectory);
                    Console.WriteLine($"Archives for period {period} deleted successfully.");
                    return true;
                }
                else
                {
                    Console.WriteLine($"No archives found for period {period}.");
                    return false;
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error deleting archives for period {period}: {ex.Message}");
                // You might want to log this error to a log file.
                return false;
            }
        }

        private void DeleteDirectory(string targetDir)
        {
            foreach (string file in Directory.GetFiles(targetDir))
            {
                File.Delete(file);
            }
            foreach (string subDir in Directory.GetDirectories(targetDir))
            {
                DeleteDirectory(subDir);
            }
            Directory.Delete(targetDir);
        }

        public bool ArchiveLogsToZip(string sourceDirectory, string zipDirectory, DateTime startDate, DateTime endDate)
        {
            try
            {
                if (!Directory.Exists(sourceDirectory))
                {
                    Console.WriteLine("Source directory does not exist.");
                    return false;
                }

                if (!Directory.Exists(zipDirectory))
                {
                    Directory.CreateDirectory(zipDirectory);
                }

                string zipFileName = $"Logs_{startDate:yyyyMMdd}_{endDate:yyyyMMdd}.zip";
                string period = $"{startDate:yyyy.MM.dd}-{endDate:yyyy.MM.dd}";
                string periodZipDirectory = Path.Combine(zipDirectory, period);
                string zipFilePath = Path.Combine(periodZipDirectory, zipFileName);

                // Create the period directory
                if (!Directory.Exists(periodZipDirectory))
                {
                    Directory.CreateDirectory(periodZipDirectory);
                }

                using (var archive = ZipFile.Open(zipFilePath, ZipArchiveMode.Create))
                {
                    foreach (var filePath in Directory.GetFiles(sourceDirectory, "*.log", SearchOption.AllDirectories))
                    {
                        var fileInfo = new FileInfo(filePath);
                        if (fileInfo.CreationTime >= startDate && fileInfo.CreationTime <= endDate)
                        {
                            // Add file to the zip archive preserving directory structure
                            var entryName = filePath.Substring(sourceDirectory.Length + 1); // Remove the source directory path
                            archive.CreateEntryFromFile(filePath, Path.Combine(period, entryName));
                        }
                    }
                }

                Console.WriteLine("Logs archived to zip file successfully.");
                return true;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error archiving logs to zip: {ex.Message}");
                // You might want to log this error to a log file.
                return false;
            }
        }


        public bool DeleteLogsByPeriod(string sourceDirectory, DateTime startDate, DateTime endDate)
        {
            try
            {
                // Ensure source directory exists
                if (!Directory.Exists(sourceDirectory))
                {
                    Console.WriteLine("Source directory does not exist.");
                    return false;
                }

                // Get all log files in the source directory and its subdirectories
                string[] logFiles = Directory.GetFiles(sourceDirectory, "*.log", SearchOption.AllDirectories);

                // Iterate through each log file
                foreach (string logFile in logFiles)
                {
                    var fileInfo = new FileInfo(logFile);
                    // Check if the file's creation date is within the specified period
                    if (fileInfo.CreationTime >= startDate && fileInfo.CreationTime <= endDate)
                    {
                        // Delete the file
                        File.Delete(logFile);
                        Console.WriteLine($"Deleted log file: {logFile}");
                    }
                }

                Console.WriteLine("Logs deleted successfully.");
                return true;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error deleting logs: {ex.Message}");
                // You might want to log this error to a log file.
                return false;
            }
        }


        public int CountLogsByPeriod(string sourceDirectory, DateTime startDate, DateTime endDate)
        {
            try
            {
                // Ensure source directory exists
                if (!Directory.Exists(sourceDirectory))
                {
                    Console.WriteLine("Source directory does not exist.");
                    return 0;
                }

                // Get all log files in the source directory and its subdirectories
                string[] logFiles = Directory.GetFiles(sourceDirectory, "*.log", SearchOption.AllDirectories);

                // Counter for the total log files
                int totalCount = 0;

                // Iterate through each log file
                foreach (string logFile in logFiles)
                {
                    var fileInfo = new FileInfo(logFile);
                    // Check if the file's creation date is within the specified period
                    if (fileInfo.CreationTime >= startDate && fileInfo.CreationTime <= endDate)
                    {
                        // Increment the total count
                        totalCount++;
                    }
                }

                Console.WriteLine($"Total log files found: {totalCount}");
                return totalCount;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error counting logs: {ex.Message}");
                // You might want to log this error to a log file.
                return 0;
            }
        }


        public List<string> SearchLogsBySize(string sourceDirectory, int minSizeKB, int maxSizeKB)
        {
            try
            {
                // Ensure source directory exists
                if (!Directory.Exists(sourceDirectory))
                {
                    Console.WriteLine("Source directory does not exist.");
                    return new List<string>();
                }

                // Get all log files in the source directory and its subdirectories
                string[] logFiles = Directory.GetFiles(sourceDirectory, "*.log", SearchOption.AllDirectories);

                // List to store log files within the size range
                List<string> matchingFiles = new List<string>();

                // Iterate through each log file
                foreach (string logFile in logFiles)
                {
                    var fileInfo = new FileInfo(logFile);
                    // Calculate file size in kilobytes
                    long fileSizeKB = fileInfo.Length / 1024; // Convert bytes to kilobytes
                                                              // Check if the file's size is within the specified range
                    if (fileSizeKB >= minSizeKB && fileSizeKB <= maxSizeKB)
                    {
                        // Add the file to the list
                        matchingFiles.Add(logFile);
                    }
                }

                Console.WriteLine($"Total matching log files found: {matchingFiles.Count}");
                return matchingFiles;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error searching logs by size: {ex.Message}");
                // You might want to log this error to a log file.
                return new List<string>();
            }
        }


        public List<string> SearchLogsByDirectory(string directoryPath)
        {
            try
            {
                // Ensure directory exists
                if (!Directory.Exists(directoryPath))
                {
                    Console.WriteLine($"Directory {directoryPath} does not exist.");
                    return new List<string>();
                }

                // Get all log files in the directory
                string[] logFiles = Directory.GetFiles(directoryPath, "*.log");

                // List to store log files
                List<string> matchingFiles = new List<string>(logFiles);

                Console.WriteLine($"Total log files found in {directoryPath}: {matchingFiles.Count}");
                return matchingFiles;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error searching logs in directory {directoryPath}: {ex.Message}");
                // You might want to log this error to a log file.
                return new List<string>();
            }
        }


        public async Task<bool> UploadLogsToServer(string serverUrl, string directoryPath)
        {
            try
            {
                // Ensure directory exists
                if (!Directory.Exists(directoryPath))
                {
                    Console.WriteLine($"Directory {directoryPath} does not exist.");
                    return false;
                }


                using (HttpClient client = new HttpClient())
                {
                    using (MultipartFormDataContent formData = new MultipartFormDataContent())
                    {
                        // Get all log files in the directory
                        string[] logFiles = Directory.GetFiles(directoryPath, "*.log");

                        // Add each log file to the form data
                        foreach (string logFile in logFiles)
                        {
                            byte[] fileBytes = await File.ReadAllBytesAsync(logFile);
                            formData.Add(new ByteArrayContent(fileBytes), "files", Path.GetFileName(logFile));
                        }

                        // Send the form data to the server
                        HttpResponseMessage response = await client.PostAsync(serverUrl, formData);
                        if (response.IsSuccessStatusCode)
                        {
                            Console.WriteLine("Log files uploaded successfully.");
                            return true;
                        }
                        else
                        {
                            Console.WriteLine($"Error uploading log files: {response.StatusCode}");
                            return false;
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error uploading log files: {ex.Message}");
                // You might want to log this error to a log file.
                return false;
            }
        }
    }
 }
