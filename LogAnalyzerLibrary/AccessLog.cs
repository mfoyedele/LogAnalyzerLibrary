using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
using System.Linq;

namespace LogAnalyzerLibrary
{
    public class AccessLog
    {
    private string[] logDirectories;

    public AccessLog(params string[] directories)
    {
        logDirectories = directories;
    }


        // US-9: Search Logs in directories
        public List<string> SearchLogs(string searchPattern)
        {
            List<string> foundLogs = new List<string>();

            foreach (string directory in logDirectories)
            {
                try
                {
                    foundLogs.AddRange(Directory.GetFiles(directory, searchPattern, SearchOption.AllDirectories));
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Error accessing directory {directory}: {ex.Message}");
                }
            }

            return foundLogs;
        }

        

    }
}
