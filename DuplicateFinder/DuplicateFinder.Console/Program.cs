using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using DuplicateFinder.Logic.Interface;
using DuplicateFinder.Logic.Model;

namespace DuplicateFinder
{
    internal class Program
    {
        public static void Main(string[] args)
        {
            string BLPath = @"C:\Users\jbmes\Documents\Files Blacklist.txt"; //set within the program
            List<string> grp = new List<string>();
            ConsoleKeyInfo runAgain;
            do
            {
                string folderPath;
                do
                {
                    Console.Clear();
                    Console.Write("Please enter a folder path to search for duplicate files: ");
                    folderPath = Console.ReadLine();
                } while (!Directory.Exists(folderPath));

                var finder = new Logic.DuplicateFinder();

                var sw = new Stopwatch();
                sw.Restart();

                var duplicatesBySize = finder.CollectCandidates(folderPath, CompareMode.Size).ToList();
                var duplicatesBySizeAndName = finder.CollectCandidates(folderPath, CompareMode.SizeAndName).ToList();
                var duplicatesByHash = finder.CheckCandidates(duplicatesBySizeAndName).ToList();

                sw.Stop();

                Console.WriteLine("These are potential duplicates by size:");
                PrintDuplicates(duplicatesBySize);

                Console.WriteLine("These are potential duplicates by size and name:");
                PrintDuplicates(duplicatesBySizeAndName);

                Console.WriteLine("These are actual duplicates by md5 hash:");
                PrintDuplicates(duplicatesByHash);

                Console.WriteLine("Execution time:" + sw.ElapsedMilliseconds + "ms");

                //addition
                //Read from blacklist
                List<string> BLl = new List<string>();
                BlackListReader(BLPath, BLl);
                Console.WriteLine(BLl[0]);
                grp = CompleteListGroups(duplicatesBySize);
                Console.WriteLine(grp[0]);

                
                do
                {
                    Console.WriteLine("Select File: ");
                    grp.ForEach(Console.WriteLine);
                    string fileselected = Console.ReadLine();

                    //is it blacklisted?
                    bool blexists = BLl.Any(s => s.Contains(fileselected));
                    if (!blexists) {
                        Console.WriteLine("Allowed!");
                    } else
                    {
                        Console.WriteLine("The file you've tried accessing is blacklisted!");
                    }


                } while (!Directory.Exists(folderPath));

                //addition

                Console.WriteLine("Run again [y/n]?");
                runAgain = Console.ReadKey();
            } while (runAgain.Key == ConsoleKey.Y);
            
        }

        private static void PrintDuplicates(IEnumerable<IDuplicate> duplicates)
        {
            var i = 1;
            foreach (var duplicate in duplicates)
            {
                Console.WriteLine($"Group{i++}:");
                PrintDuplicate(duplicate);
                Console.WriteLine();
            }
        }

        private static void PrintDuplicate(IDuplicate duplicate)
        {
            foreach (var filePath in duplicate.FilePaths)
            {
                Console.WriteLine(filePath);
            }
        }
        static List<string> BlackListReader(string BLPath, List<string> list) //returns list of strings with the paths. Within the list, paths are already grouped by repetition groups if
        {
            string[] BLReader = System.IO.File.ReadAllLines(BLPath);
            //System.Console.WriteLine("Contents of Blacklist.txt = ");

            foreach (string line in BLReader)
            {
                // Use a tab to indent each line of the file.
                //Console.WriteLine("\t" + line);
                list.Add(line);
            }
            return list;
        }
        private static List<string> CompleteListGroups(IEnumerable<IDuplicate> duplicates) //returns list of strings with the paths. Within the list, paths are already grouped by repetition groups if
        {
            List<string> Grp = new List<string>();
            foreach (var duplicate in duplicates)
            {
                foreach (var filePath in duplicate.FilePaths)
                {
                   // Grp.Add(filePath);
                }
            }
            //Console.WriteLine(Grp[0]);

            return Grp;
        }
    }
}