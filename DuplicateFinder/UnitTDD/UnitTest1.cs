using System;
using System.IO;
using Xunit;
using System.Collections.Generic; //to use IEnumerate
using System.Linq; //ToList
using DuplicateFinder.Logic; //using namespace from DuplicateFinder
using DuplicateFinder.Logic.Interface;
using DuplicateFinder.Logic.Model;

namespace DuplicateFinder.Logic
{ 
    public class NewFeatures
    {
        [Fact]
        public void WeCanReadBlackList()
        {
            ///This unit test checks if the function is able to correctly read
            ///the Files Blacklist.txt within TestGrounds folder. It takes an array with
            ///the same contents and does an equal assertion
            ///To do: have it write a new txt file to serve as the dummy blackfileslist instead of using File Blacklist.txt



            //arrange
            var processor = new FileRenamer();
            string path = Directory.GetParent(Directory.GetCurrentDirectory()).Parent.Parent.Parent.FullName;
            string pathBL = path + ".\\UnitTDD\\TestingGround\\Files Blacklist.txt";
            var blLibrary = new List<string>(5);

            blLibrary.Add(@"C:\Users\guest\Documents\Software\ReactiveUI\LICENSE.txt");
            blLibrary.Add(@"C:\Users\guest\Documents\Software\ReactiveUI\src\global.json");
            blLibrary.Add(@"C:\Users\guest\Desktop\Critical Software Standard\Done\Barmsnes et al, 1997 - Implementation of Graphical User Interfaces in Nuclear Applications.pdf");

            //act
            List<string> response = processor.ReadTxtListFetch(pathBL);

            //assert
            Assert.Equal(blLibrary,response);
        }

        [Fact]
        public void InputEqualBlacklist() 
        {
            ///Now that we have a method to read the blacklist,
            ///we need to compare user input with the blacklist
            ///so that we do not touch prohibited files
            ///We take a good and bad input requested by user, and output a boolean
            ///True is in blacklist, false is not.
            ///to do: have the test use a created input in a created dummy blacklist file 
            ///and anohter input not in the dummy blacklist file


            //arrange
            var processor = new FileRenamer();
            var badRequest = @"C:\Users\guest\Documents\Software\ReactiveUI\LICENSE.txt";
            var goodRequest = @"C:\Users\guest\Documents\Software\ReactiveUI\LICENSES.txt";
            string path = Directory.GetParent(Directory.GetCurrentDirectory()).Parent.Parent.Parent.FullName;
            string pathBL = path + ".\\UnitTDD\\TestingGround\\Files Blacklist.txt";
            //List<string> blLibrary = processor.ReadBlackListFetch(pathBL); //this is now exectued within the function below

            //act
            bool blacklisted = processor.FileInList(badRequest, pathBL);
            bool notBlacklisted = processor.FileInList(goodRequest, pathBL);

            //assert
            Assert.Equal(blacklisted, true);
            Assert.Equal(notBlacklisted, false);
        }

        [Fact]
        public void CheckCollectionDuplicates()
        {
            ///This test makes sure we can keep an array with the duplicate filepaths 
            ///from a known test folder with known contents. We compare the function ComplteListGroup
            ///to the answer to make sure it works properly

            //arrange
            var processor = new FileRenamer();
            string path = Directory.GetParent(Directory.GetCurrentDirectory()).Parent.Parent.Parent.FullName;
            string pathFolder = path + ".\\UnitTDD\\TestingGround\\Name1"; //folder with known duplicate pdfs
            List<string> request = new List<string>();
            request.Add(pathFolder +"\\AChina -healthcare-market-an-introduction - Copy (3).pdf");
            request.Add(pathFolder + "\\China-healthcare-market-an-introduction - Copy (2).pdf");
            request.Add(pathFolder + "\\China-healthcare-market-an-introduction - Copy.pdf");
            request.Add(pathFolder + "\\ZChina-healthcare-market-an-introduction.pdf");
            request.Add(pathFolder + "\\Bemerging-trends-in-chinese-healthcare - Copy.pdf");
            request.Add(pathFolder + "\\emerging-trends-in-chinese-healthcare.pdf");
            request.Add(pathFolder + "\\emerging-trends-in-chinese-healthcare.txt");


            //var finder = new DuplicateFinder();
            var duplicatesBySize = processor.CollectCandidates(pathFolder, Model.CompareMode.Size).ToList();

            //act
            List<string> groups = processor.CompleteListGroups(duplicatesBySize);
            int i = 0;
            foreach (var filePath in request) //testing to see if both lists have every single element of the other and vice-versa
            {
                bool exists = groups.Any(s => s.Contains(filePath));
                if (exists) {
                    i++;
                }
            }
            int j = 0;
            foreach (var filePath in groups) //testing to see if both lists have every single element of the other and vice-versa
            {
                bool exists = request.Any(s => s.Contains(filePath));
                if (exists) {
                    j++;
                }
            }
            //assert //if both counters are the same, the two groups have ghe same size and have the same contents
            Assert.Equal(i, j);
        }

        [Fact]
        public void CheckPossibleDuplicate()
        {
            ///This test just checks the functionality of the duplicate new filename checker
            ///making sure we aren't using a path that already exists

            //arrange
            var processor = new FileRenamer();
            string path = Directory.GetParent(Directory.GetCurrentDirectory()).Parent.Parent.Parent.FullName;
            string pathFolder = path + ".\\UnitTDD\\TestingGround\\Name1"; //folder with known duplicate pdfs
            var duplicatesBySize = processor.CollectCandidates(pathFolder, Model.CompareMode.Size).ToList();
            List<string> groups = processor.CompleteListGroups(duplicatesBySize);
            var inputValid = groups[0] + " .txt"; //adding an extra extension just to create a new string
            var inputinValid = groups[0];


            //act
            bool checkValid = processor.CheckDuplicateName(inputValid, groups);
            bool checkInvalid = processor.CheckDuplicateName(inputinValid, groups);

            Assert.Equal(checkValid, false);
            Assert.Equal(checkInvalid, true);
        }

        [Fact]
        public void RenameFileWithNewName()
        {
            ///We can rename a folder with a new nonexisting name and not in blacklist
            ///The user choses a file to rename, chooses a new name, and the function checks
            ///if it is possible to rename the file (if it is in the Blacklist or if the new name already exists)
            ///Whether the file is renamed or not is asserted at the end

            //arrange
            var processor = new FileRenamer();
            string path = Directory.GetParent(Directory.GetCurrentDirectory()).Parent.Parent.Parent.FullName;
            string pathFolder = path + ".\\UnitTDD\\TestingGround\\Name1"; //folder with known duplicate pdfs
            string pathFolderBL = path + ".\\UnitTDD\\TestingGround\\Files Blacklist.txt"; //folder with Blacklist


            var duplicatesBySize = processor.CollectCandidates(pathFolder, Model.CompareMode.Size).ToList();
            List<string> groups = processor.CompleteListGroups(duplicatesBySize);
            string input = "newFileName.pdf"; //just the filename, the path is used since we don't want the user to write the whole path //we need to get the extension
            string saveOldName = Path.GetFileName(groups[0]);

            //bool check = true; //dummy for now
            //bool check = processor.CheckDuplicateName();

            //act
            processor.RenameDuplicate(input, groups[0], pathFolderBL, groups); //picking Group[0] for testing purposes
            //bool faiL = processor.RenameDuplicate(badinput, chosenfile); //test that both blacklisted and existing names cannot be used


            //assert
            Assert.Equal(File.Exists(pathFolder +"\\" + input), true); //asserts we have the new file
            Assert.Equal(File.Exists(pathFolder + "\\" + saveOldName), false); //asserts the previous file doesn't exist anymore

            //resetting file to previous name:
            processor.RenameDuplicate(saveOldName, pathFolder + "\\" + input, pathFolderBL, groups);
            Assert.Equal(File.Exists(pathFolder + "\\" + input), false); //asserts we have the old file
            Assert.Equal(File.Exists(pathFolder + "\\" + saveOldName), true); //asserts the new filename is no longer there (resets Test Staging Area)

        }

        
    }

}
