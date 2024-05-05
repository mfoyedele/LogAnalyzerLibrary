using System;
using System.Collections.Generic;
using System.IO;

class Program
{
    static void Main(string[] args)
    {
        getfiles get = new getfiles();
        List<string> files = get.GetAllFiles(@"c:\Logs");

        foreach (string file in files)
        {
            Console.WriteLine($"File: {file}");
            int duplicatedErrors = get.CountDuplicatedErrors(file);
            Console.WriteLine($"Number of duplicated errors: {duplicatedErrors}");
        }

        Console.Read();
    }
}

class getfiles
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
}
