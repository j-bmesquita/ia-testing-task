using System;
//using Xunit;
using System.Collections.Generic; //to use IEnumerate
using System.Linq; //ToList
using DuplicateFinder.Logic; //using namespace from DuplicateFinder
using DuplicateFinder.Logic.Interface;
using DuplicateFinder.Logic.Model;

namespace DuplicateFinder
{
    public class UnitTest1
    {
        //[Fact]
        public static void GetCollectEqualNameAndSize()
        {
            //arrange
            string folderPath = "C:\\Users\\jbmes\\source\\repos\\j-bmesquita\\ia-testing-task\\DuplicateFinder\\TestingGround\\Name1"; //input here path to test folder

            //act
            var finder = new DuplicateFinder.Logic.DuplicateFinder();

            var duplicatesBySize = finder.CollectCandidates(folderPath, DuplicateFinder.Logic.Model.CompareMode.Size).ToList();
            var duplicatesBySizeAndName = finder.CollectCandidates(folderPath, DuplicateFinder.Logic.Model.CompareMode.SizeAndName).ToList();
            var duplicatesByHash = finder.CheckCandidates(duplicatesBySizeAndName).ToList();

            //assert
            Console.WriteLine("TestPrintDuplicates");
            TestPrintDuplicates(duplicatesBySize);
            Console.WriteLine("CountGroupsSize");
            CountGroupsSize(duplicatesBySize);
            Console.WriteLine("CompleteListGroups");
            CompleteListGroups(duplicatesBySize);
            Console.WriteLine("BlackListReader");
            string BLPath = @"C:\Users\jbmes\Documents\Files Blacklist.txt"; //It could not find path if Files Blacklisted were within the project?
            BlackListReader(BLPath);
            //Console.WriteLine(folderPath);
            //Console.WriteLine(finder);
            //Console.WriteLine(duplicatesBySize);
            //Console.WriteLine(duplicatesBySizeAndName);
            //Console.WriteLine(duplicatesByHash);
            //Assert.Equal(2, TestPrintDuplicates(duplicatesBySize));
            //Assert.Equal(0, TestPrintDuplicates(duplicatesBySizeAndName));
            //Assert.Equal(0, TestPrintDuplicates(duplicatesByHash));

        }
        public static void Main(string[] args)
        {
            GetCollectEqualNameAndSize();

        }

        //So we can read the values
        private static int TestPrintDuplicates(IEnumerable<IDuplicate> duplicates)
        {
            var i = 1;
            foreach (var duplicate in duplicates)
            {
                Console.WriteLine($"Group{i++}:");
                PrintDuplicate(duplicate); //prints strings to be changed
                Console.WriteLine(); //just skips a line
                //i++;
            }
            Console.WriteLine(i); //this value is the number of strings to be changed + 1
            return i;
        }

        private static void PrintDuplicate(IDuplicate duplicate)
        {   
            foreach (var filePath in duplicate.FilePaths)
            {
                Console.WriteLine(filePath);
            }
        }
        private static List<string> CompleteListGroups(IEnumerable<IDuplicate> duplicates) //returns list of strings with the paths. Within the list, paths are already grouped by repetition groups if
        {                                                                  
            List<string> Grp = new List<string>();
            // List<int> GrpInt = new List<int> { };
            foreach (var duplicate in duplicates)
            {
                //Console.WriteLine("Group Checkpoint");
                //int i = 0;
                foreach (var filePath in duplicate.FilePaths)
                {
                    Grp.Add(filePath);
                    //Console.WriteLine(filePath);
                    //i++; //goes to array
                }
                //GrpInt.Add(i);
            }
            //Console.WriteLine("Size of List ", Grp.Count);
            Console.WriteLine(Grp[0]); //gives back the individual pieces
            //foreach (string k in Grp) //prints element by element
            //{
            //    Console.WriteLine(k);
            //}

            return Grp;
        }
        private static List<int> CountGroupsSize(IEnumerable<IDuplicate> duplicates) //returns list of strings and group number for splits
        {                                                                  // example, we have three repetitions followed by 2, we have then an array [3,2]
            //List<string> Grp = new List<string>();
            List<int> GrpInt = new List<int> { };
            foreach (var duplicate in duplicates)
            {
                //Console.WriteLine("Group Checkpoint");
                int i = 0;
                foreach (var filePath in duplicate.FilePaths)
                {
                //    Grp.Add(filePath);
                //    //Console.WriteLine(filePath);
                    i++; //goes to array
                }
                GrpInt.Add(i);
            }
            //Console.WriteLine("Size of List ", Grp.Count);
            //Console.WriteLine(GrpInt); 
            foreach (int k in GrpInt) //prints element by element
            {
                Console.WriteLine(k);
            }
            return GrpInt;
        }
        private static void BlackListReader(string BLPath) //returns list of strings with the paths. Within the list, paths are already grouped by repetition groups if
        {
            string[] BLReader = System.IO.File.ReadAllLines(BLPath);
            System.Console.WriteLine("Contents of Blacklist.txt = ");
            foreach (string line in BLReader)
            {
                // Use a tab to indent each line of the file.
                Console.WriteLine("\t" + line);
            }
        }

        //Function to read Line by Line from Blacklist.txt
    }
}