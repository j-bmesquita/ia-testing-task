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


                Console.WriteLine("Run again [y/n]?");
                runAgain = Console.ReadKey();
            } while (runAgain.Key == ConsoleKey.Y);
            
        }

        private static void DeepDirectorySearch(string folderPath)
        {
            throw new NotImplementedException();
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
        
    }
}