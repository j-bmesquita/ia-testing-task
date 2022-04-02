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

                //including new features below:
                ConsoleKey response;
                do
                {
                    Console.Write("Do you wish to rename any files [y/n]?");
                    response = Console.ReadKey(false).Key;
                    if (response == ConsoleKey.Y)
                    {
                        string input;
                        bool confirmed = false;
                        bool cancel = false;
                        bool cancel2 = false;
                        string filePath;
                        do
                        {
                            Console.WriteLine();
                            Console.Write("Type folder FullPath with current filename. Write exit to cancel this operation: ");
                            filePath = Console.ReadLine();
                            if (filePath == "exit")
                            {
                                cancel2 = true;
                            }
                        } while (!File.Exists(filePath) && !cancel2);
                        if (!cancel2)
                        {
                            do
                            {
                                Console.WriteLine("Type new filename WITH EXTENSION (no need for path). Write 'exit' to cancel renaming this file: ");
                                input = Console.ReadLine();

                                if (input == "exit")
                                {
                                    cancel = true;
                                }
                                if (!cancel && !confirmed)
                                {
                                    do
                                    {
                                        Console.WriteLine("new name selected, " + input + ", are you sure [y/n]?");
                                        response = Console.ReadKey(false).Key;   // true is intercept key (dont show), false is show
                                        if (response != ConsoleKey.Enter)
                                            Console.WriteLine();
                                    } while (response != ConsoleKey.Y && response != ConsoleKey.N);

                                    var processor = new Logic.FileRenamer();
                                    string path = Directory.GetParent(Directory.GetCurrentDirectory()).Parent.Parent.FullName;
                                    string pathFolderBL = path + ".\\DuplicateFinder.Console\\TestingGround\\Files Blacklist.txt"; //folder with Blacklist
                                    
                                    int result = processor.RenameDuplicate(input, filePath, pathFolderBL);
                                    
                                    if (result == 0) //success
                                    {
                                        confirmed = response == ConsoleKey.Y;
                                        Console.WriteLine(result);
                                    }
                                    if (result == -1)
                                    {
                                        Console.WriteLine("The file chosen has been blacklisted!");
                                        cancel = true;
                                    }
                                    if (result == -3)
                                    {
                                        Console.WriteLine("The new file name already exists!");
                                        //cancel = true;
                                    }
                                }
                            } while (!confirmed && !cancel);
                        }
                    }

                } while (response != ConsoleKey.N);

                ////including new features above
                Console.WriteLine();
                Console.Write("Run again [y/n]?");
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