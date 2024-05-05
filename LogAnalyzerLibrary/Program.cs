using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LogAnalyzerLibrary
{
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
}
