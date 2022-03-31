using System;
using Xunit;
using System.Collections.Generic; //to use IEnumerate
using System.Linq; //ToList
using DuplicateFinder.Logic; //using namespace from DuplicateFinder
using DuplicateFinder.Logic.Interface;
using DuplicateFinder.Logic.Model;

namespace DuplicateFinder
{
    public class UnitTest1
    {
        [Fact]
        public void GetCollectEqualNameAndSize()
        {
            //arrange
            string folderPath = "C:\\Users\\jbmes\\source\\repos\\j-bmesquita\\ia-testing-task\\DuplicateFinder\\TestingGround\\Name1"; //input here path to test folder

            //act
            var finder = new DuplicateFinder.Logic.DuplicateFinder();

            var duplicatesBySize = finder.CollectCandidates(folderPath, DuplicateFinder.Logic.Model.CompareMode.Size).ToList();
            var duplicatesBySizeAndName = finder.CollectCandidates(folderPath, DuplicateFinder.Logic.Model.CompareMode.SizeAndName).ToList();
            var duplicatesByHash = finder.CheckCandidates(duplicatesBySizeAndName).ToList();

            //assert we detect the right number of repetition groups for all functions
            //Assert.Equal(2, NumberRepetitions(duplicatesBySize));
            //Assert.Equal(0, NumberRepetitions(duplicatesBySizeAndName));
            //Assert.Equal(0, NumberRepetitions(duplicatesByHash));

            //assert we count correctly how many repetitions of each in all functions
            List<int> SizeArray = new List<int>(2);
            SizeArray.Add(3);
            SizeArray.Add(2);
            //Assert.Equal(SizeArray, CountGroupsSize(duplicatesBySize));
            List<string> pathArray = new List<string>(5);
            pathArray.Add("C:\\Users\\jbmes\\source\\repos\\j-bmesquita\\ia-testing-task\\DuplicateFinder\\TestingGround\\Name1\\China-healthcare-market-an-introduction - Copy (2).pdf");
            pathArray.Add("C:\\Users\\jbmes\\source\\repos\\j-bmesquita\\ia-testing-task\\DuplicateFinder\\TestingGround\\Name1\\China-healthcare-market-an-introduction - Copy.pdf");
            pathArray.Add("C:\\Users\\jbmes\\source\\repos\\j-bmesquita\\ia-testing-task\\DuplicateFinder\\TestingGround\\Name1\\China-healthcare-market-an-introduction.pdf");
            pathArray.Add("C:\\Users\\jbmes\\source\\repos\\j-bmesquita\\ia-testing-task\\DuplicateFinder\\TestingGround\\Name1\\emerging-trends-in-chinese-healthcare - Copy.pdf");
            pathArray.Add("C:\\Users\\jbmes\\source\\repos\\j-bmesquita\\ia-testing-task\\DuplicateFinder\\TestingGround\\Name1\\emerging-trends-in-chinese-healthcare.pdf");

            Assert.Equal(pathArray, CompleteListGroups(duplicatesBySize));
            //Assert.Equal(null, CountGroupsSize(duplicatesBySizeAndName));
            //Assert.Equal(null, CountGroupsSize(duplicatesByHash));

        }

        //So we can read the values
        private static int NumberRepetitions(IEnumerable<IDuplicate> duplicates)
        {
            var i = 1;
            foreach (var duplicate in duplicates)
            {
                Console.WriteLine($"Group{i++}:");
                PrintDuplicate(duplicate);
                Console.WriteLine();
                //i++;
            }
            return i-1;    
        }

        private static void PrintDuplicate(IDuplicate duplicate)
        {
            foreach (var filePath in duplicate.FilePaths)
            {
                Console.WriteLine(filePath);
            }
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
            //Console.WriteLine(Grp); //does not print actual list
            //foreach (string k in Grp) //prints element by element
            //{
            //    Console.WriteLine(k);
            //}
            return GrpInt;
        }
        private static List<string> CompleteListGroups(IEnumerable<IDuplicate> duplicates) //returns list of strings and group number for splits
        {                                                                  // example, we have three repetitions followed by 2, we have then an array [3,2]
            List<string> Grp = new List<string>();
            List<int> GrpInt = new List<int> { };
            foreach (var duplicate in duplicates)
            {
                Console.WriteLine("Group Checkpoint");
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
            //Console.WriteLine(Grp); //does not print actual list
            //foreach (string k in Grp) //prints element by element
            //{
            //    Console.WriteLine(k);
            //}
            return Grp;
        }
    }
}
