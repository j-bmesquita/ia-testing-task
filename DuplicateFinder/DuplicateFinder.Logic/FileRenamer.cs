using System;
using System.Collections.Generic;
using System.IO;
using System.Security.Cryptography;
using DuplicateFinder.Logic.Interface;
using DuplicateFinder.Logic.Model;
using System.Linq; //used for List<string>any

namespace DuplicateFinder.Logic
{

    ///Classes and Methods added for the new features:
    ///The following methods were implemented along the unit test cases in UnitTDD project,
    ///within its UnitTest1.cs file

    public class FileRenamer : DuplicateFinder //FileRenamer is an extension of Duplicate Finder for the new features
    {
        public List<string> ReadTxtListFetch(string pathBL)
        {
            var blLibrary = new List<string>();
            string[] blReader = System.IO.File.ReadAllLines(pathBL);
            //System.Console.WriteLine("Contents of Blacklist.txt = ");
            foreach (string line in blReader)
            {
                // Use a tab to indent each line of the file.
                blLibrary.Add(line);
                //Console.WriteLine("\t" + line);
            }

            return blLibrary;
        }
        public bool FileInList(string filename, string pathFolder)//, bool state)
        {
            //if (state == true) //if we are searching for a filepath within a txt file
            //{
                List<string> blReader = ReadTxtListFetch(pathFolder);
                bool exists = blReader.Any(s => s.Contains(filename));
                if (exists)
                {
                    return true;
                }
                else
                {
                    return false;
                }
            //}
            //else //we are comparing a filepath to a list
            //{
                //List<string> BLReader = ReadTxtListFetch(pathFolder);
                //bool exists = BLReader.Any(s => s.Contains(filename));
                //if (exists)
                //{
                //    return true;
                //}
                //else
                //{
                //    return false;
                //}
            }

        
        public List<string> CompleteListGroups(IEnumerable<IDuplicate> duplicates) //returns list of strings with the paths. Within the list, paths are already grouped by repetition groups if
        {
            List<string> grp = new List<string>();
            foreach (var duplicate in duplicates)
            {
                foreach (var filePath in duplicate.FilePaths)
                {
                    grp.Add(filePath);
                }
            }
            //Console.WriteLine(Grp[0]);

            return grp;
        }
        public bool CheckDuplicateName(string newFilenameWithPath, string oldfilenamewithpath) //we have to compare to the whole file, not just those repeated//, List<string> groups)
        {
            string pathnofile = Path.GetDirectoryName(oldfilenamewithpath);
            List<string> allFiles = Directory.GetFiles(pathnofile, "*").ToList(); //, SearchOption.AllDirectories);
            
            //string newfilenamenopath = Path.GetFileName(newfilenamewithpath);
            //string oldfilenamenopath = Path.GetFileName(oldfilenamewithpath);

            bool exists = allFiles.Any(s => s.Contains(newFilenameWithPath));

            if (exists)
            {
                return true;
            }
            else
            {
                return false;
            }
        }
        public int RenameDuplicate(string newFilename, string folderDuplicatePath, string blacklistPath) //int serves to code for why something happened
        {
            string pathNoFile = Path.GetDirectoryName(folderDuplicatePath); //gets path to be plugged in with the newfilename
                                                                            //string newfilenamenopath = Path.GetFileName(newfilenamewithpath);
                                                                            //string oldfilenamenopath = Path.GetFileName(oldfilenamewithpath);

            bool check1 = FileInList(folderDuplicatePath, blacklistPath);

            if (check1) //in blacklist
            {
                return -1; //file is blacklisted!
            }
            else //not in blacklist
            {
                bool check2 = CheckDuplicateName(pathNoFile + "\\" + newFilename, folderDuplicatePath);

                if (!check2) //chosen name doesn't exist already
                {
                    try //for redundancy
                    {
                        System.IO.File.Move(folderDuplicatePath, pathNoFile + "\\" + newFilename);
                        return 0; //operation executed
                    }
                    catch
                    {
                        return -2; //Something went wrong trying to write the file
                    }

                } else
                {
                    return -3; //new filename already exists, cannot be used or file will be overwritten
                }
            }
        }
    }
}