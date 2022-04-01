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
            ///the Files Blacklist.txt within resources It takes an array with
            ///the same contents and does an equal assertion



            //arrange
            var processor = new FileRenamer();
            string path = Directory.GetParent(Directory.GetCurrentDirectory()).Parent.Parent.Parent.FullName;
            string pathBL = path + @"/DuplicateFinderWPF/Resources/Files Blacklist.txt";
            var blLibrary = new List<string>(5);

            blLibrary.Add(@"C:\Users\guest\Documents\Software\ReactiveUI\LICENSE.txt");
            blLibrary.Add(@"C:\Users\guest\Documents\Software\ReactiveUI\src\global.json");
            blLibrary.Add(@"C:\Users\guest\Desktop\Critical Software Standard\Done\Barmsnes et al, 1997 - Implementation of Graphical User Interfaces in Nuclear Applications.pdf");

            //act
            List<string> response = processor.ReadBlackListFetch(path);

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


            //arrange
            var processor = new FileRenamer();
            var badrequest = @"C:\Users\guest\Documents\Software\ReactiveUI\LICENSE.txt";
            var goodrequest = @"C:\Users\guest\Documents\Software\ReactiveUI\LICENSES.txt";
            var pathBL = @"./DuplicateFinder/DuplicateFinderWPF/Resources/Files Blacklist.txt";
            List<string> blLibrary = processor.ReadBlackListFetch(pathBL);

            //act
            bool blacklisted = processor.FileInBlackList(badrequest, pathBL);
            bool notblacklisted = processor.FileInBlackList(goodrequest, pathBL);

            //assert
            Assert.Equal(blacklisted, true);
            Assert.Equal(notblacklisted, false);
        }

        [Fact]
        public void CheckPossibleDuplicate()
        {
            ///This test just checks the functionality of the duplicate new filename checker
            ///making sure we aren't using a path that already exists

            //arrange
            var processor = new FileRenamer();
            var chosenfile = @""; //includes path
            var input = @"";

            //act
            bool check = processor.CheckDuplicateName(input, chosenfile);

            Assert.Equal(check, false);
        }

        [Fact]
        public void RenameFolderWithNewName() 
        {
            ///We can rename a folder with a new nonexisting name and not in blacklist
            ///The user choses a file to rename, chooses a new name, and the function checks
            ///if it is possible to rename the file (if it is in the Blacklist or if the new name already exists)
            ///Whether the file is renamed or not is asserted at the end

            //arrange
            var processor = new FileRenamer();
            var chosenfile = @""; //includes path
            var input = @"";
            var pathBL = @"./DuplicateFinder/DuplicateFinderWPF/Resources/Files Blacklist.txt";
            bool blacklisted = processor.FileInBlackList(input, pathBL);
            bool check = processor.CheckDuplicateName(input, chosenfile);

            //act
            //bool faiL = processor.RenameDuplicate(badinput, chosenfile); //test that both blacklisted and existing names cannot be used

            if (!check) //if path/file name is not being used
            {
                if (!blacklisted) //if path.file name isn't in the blacklist
                System.IO.File.Move(chosenfile, input);
                processor.RenameDuplicate(input, chosenfile);
            }
            

            //assert
            Assert.Equal(File.Exists(input), true);

        }

        
    }

}
