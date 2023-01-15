using System;
using System.Text.RegularExpressions;
using System.Text;

namespace MyApp // Note: actual namespace depends on the project name.
{
    internal class Program
    {
        static void Main(string[] args)
        {
            AdressExtractor ad = new AdressExtractor();
            var csvFilesFound = ad.GetAllCsvPaths();
            if (!csvFilesFound.Any())
            {
                Console.WriteLine("No csv files found. Exit progam on any key");
                Console.ReadKey();
                return;
            }


            Console.WriteLine("Extracting email adresses from following files:\n");
            csvFilesFound.ForEach(e => Console.WriteLine(e));
            //FileHandling.GetAllEmailAdresses(ad.GetAllCsvPaths());

            MailExtracter.ExtractEmails(ad.GetAllCsvPaths(),ad.OutputFilePath);

            Console.WriteLine("\nPress any key to close this window");
            Console.ReadKey();

        }
    }

    public class AdressExtractor
    {
        string currentPath;
        string inputPath;
        string outputFileName;
        public string CurrentPath { get => currentPath; set => currentPath = value; }
        public string OutputFilePath { get => outputFileName; set => outputFileName = value; }
        public string InputPath { get => inputPath; set => inputPath = value; }

        public AdressExtractor()
        {
            CurrentPath = Directory.GetCurrentDirectory();
            InputPath = Path.Combine(CurrentPath, "Input");
            if (!Directory.Exists(InputPath)) Directory.CreateDirectory(InputPath);

            OutputFilePath = Path.Combine(CurrentPath,"Result", string.Format(DateTime.Now.ToString("yyyy-dd-M--HH-mm-ss") + ".csv"));
        }
    }

    public static class FileHandling
    {
        public static List<string> GetAllCsvPaths(this AdressExtractor ad)
        {
            if(!Directory.Exists(ad.InputPath))return new List<string>();
            return Directory.EnumerateFiles(ad.InputPath, "*.csv",SearchOption.TopDirectoryOnly).ToList();
        }
    }


    class MailExtracter
    {
        public static void ExtractEmails(List<string> csvFilePaths, string outputFilePath)
        {

            StringBuilder sb = new StringBuilder();

            csvFilePaths.ForEach(csvFile =>
            {
                string data = File.ReadAllText(csvFile); //read File 
                                                            //instantiate with this pattern 
                Regex emailRegex = new Regex(@"\w+([-+.]\w+)*@\w+([-.]\w+)*\.\w+([-.]\w+)*",
                    RegexOptions.IgnoreCase);
                //find items that matches with our pattern
                MatchCollection emailMatches = emailRegex.Matches(data);



                foreach (Match emailMatch in emailMatches)
                {
                    sb.AppendLine(emailMatch.Value);
                }
            });

            List<string> lstring = sb.ToString().Split(Environment.NewLine).OrderBy(i => i).ToList().GroupBy(i => i).Select(y => y.First()).ToList();

            Console.WriteLine($"\nFound {lstring.Count} email adresses and write them to {outputFilePath}");

            if(!Directory.Exists(Path.GetDirectoryName(outputFilePath)))
                Directory.CreateDirectory(Path.GetDirectoryName(outputFilePath));
            File.WriteAllLines(outputFilePath, lstring);
        }


    }











}