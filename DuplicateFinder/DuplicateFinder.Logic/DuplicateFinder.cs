using System;
using System.Collections.Generic;
using System.IO;
using System.Security.Cryptography;
using DuplicateFinder.Logic.Interface;
using DuplicateFinder.Logic.Model;
using System.Linq; //used for List<string>any

namespace DuplicateFinder.Logic
{
    public class DuplicateFinder : IDuplicateFinder
    {
        private DirectoryInfo _rootDirectory;
        private List<FileInfo> _fileList;

        public DirectoryInfo RootDirectory
        {
            get { return _rootDirectory; }
            set { _rootDirectory = value; }
        }
        public List<FileInfo> FileList
        {
            get { return _fileList; }
            set { _fileList = value; }
        }

        public List<Duplicate> CollectEqualNameAndSize(List<Duplicate> duplicatesList, List<int> ignoreIndexList)
        {
            for (int i = 0; i < FileList.Count; i++)
            {
                var pathList = new List<string>();
                pathList.Add(FileList[i].FullName); 

                // iterate over the rest of the files 
                for (int j = i + 1; j < FileList.Count; j++) 
                {
                    if (ignoreIndexList.Contains(j)) continue;

                    if (FileList[i].Length == FileList[j].Length && // two elements have the same length
                        FileList[i].Name == FileList[j].Name) // two elements have the same name
                    {
                        pathList.Add(FileList[j].FullName);
                        ignoreIndexList.Add(j);
                    }
                }
                if (pathList.Count > 1)
                {
                    duplicatesList.Add(new Duplicate(pathList));
                }
            }
            return duplicatesList;
        }

        public List<Duplicate> CollectEqualSize(List<Duplicate> duplicatesList, List<int> ignoreIndexList)
        {
            for (int i = 0; i < FileList.Count; i++)
            {
                var paths = new List<string>();
                paths.Add(FileList[i].FullName);

                for (int j = i + 1; j < FileList.Count; j++)
                {
                    if (ignoreIndexList.Contains(j)) continue; 

                    if (FileList[i].Length == FileList[j].Length)
                    {
                        paths.Add(FileList[j].FullName);
                        ignoreIndexList.Add(j);
                    }
                }

                if (paths.Count > 1)
                {
                    duplicatesList.Add(new Duplicate(paths));
                }
            }
            return duplicatesList;
        }

        public IEnumerable<IDuplicate> CollectCandidates(string rootPath)
        {
            try
            {
                FileList = DeepDirectorySearch(new DirectoryInfo(rootPath));
            } catch (ArgumentException)
            {
                Console.WriteLine("The path given is invalid. Please try another path");
            }

            return CollectEqualNameAndSize(new List<Duplicate>(), new List<int>());
        }

        public IEnumerable<IDuplicate> CollectCandidates(string rootPath, CompareMode compareMode)
        {
            if (compareMode == CompareMode.SizeAndName)
            {
                return CollectCandidates(rootPath);
            }
            else
            {
                try
                {
                    FileList = DeepDirectorySearch(new DirectoryInfo(rootPath));
                }
                catch (ArgumentException)
                {
                    Console.WriteLine("The path given is invalid. Please try another path");
                }
                var duplicatesList = new List<Duplicate>();

                return CollectEqualSize(duplicatesList, new List<int>());
            }
        }

        public IEnumerable<IDuplicate> CheckCandidates(IEnumerable<IDuplicate> duplicates)
        {
            var duplicatesByHash = new Dictionary<string, List<string>>();
            foreach (var duplicate in duplicates)
            {
                foreach (var filePath in duplicate.FilePaths)
                {
                    var md5Provider = new MD5CryptoServiceProvider();
                    var hash = BitConverter.ToString(md5Provider.ComputeHash(File.ReadAllBytes(filePath)));
                    if (duplicatesByHash.ContainsKey(hash))
                        duplicatesByHash[hash].Add(filePath);
                    else
                    {
                        duplicatesByHash.Add(hash, new List<string>());
                        duplicatesByHash[hash].Add(filePath);
                    }
                }
            }
            List<IDuplicate> selectedDuplicates = new List<IDuplicate>();
            foreach (var duplicateByHash in duplicatesByHash)
            {
                if (duplicateByHash.Value.Count > 1)
                    selectedDuplicates.Add(new Duplicate(duplicateByHash.Value));
            }
            return selectedDuplicates;
        }

        public List<FileInfo> DeepDirectorySearch(DirectoryInfo rootDir)
        {
            FileList = new List<FileInfo>();
            var directoryInfoList = new List<DirectoryInfo>() { rootDir };
            var subdirectoryInfoList = new List<DirectoryInfo>();

            while (directoryInfoList.Count > 0)
            {
                
                foreach (DirectoryInfo directory in directoryInfoList)
                {
                    FileList.AddRange(FindFilesAndSubDirectories(directory, out DirectoryInfo[] subdirectory));
                    subdirectoryInfoList.AddRange(subdirectory);
                }
                directoryInfoList.Clear();

                foreach (DirectoryInfo directory in subdirectoryInfoList)
                {
                    FileList.AddRange(FindFilesAndSubDirectories(directory, out DirectoryInfo[] subdirectory));
                    directoryInfoList.AddRange(subdirectory);
                }
                subdirectoryInfoList.Clear();
            }

            return FileList;
        }

        public List<FileInfo> FindFilesAndSubDirectories(DirectoryInfo directoryInfo, out DirectoryInfo[] subDirectories)
        {
            subDirectories = directoryInfo.GetDirectories();

            FileList = new List<FileInfo>();
            for (int fileIndex = 0; fileIndex < directoryInfo.GetFiles().Length; fileIndex++)
            {
                FileList.Add(directoryInfo.GetFiles()[fileIndex]);
            }
            return FileList;
        }
    }


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
        public bool CheckDuplicateName(string newFilenameWithPath, List<string> groups)
        {
            //string pathnofile = Path.GetDirectoryName(oldfilenamewithpath);
            //string newfilenamenopath = Path.GetFileName(newfilenamewithpath);
            //string oldfilenamenopath = Path.GetFileName(oldfilenamewithpath);

            bool exists = groups.Any(s => s.Contains(newFilenameWithPath));

            if (exists)
            {
                return true;
            }
            else
            {
                return false;
            }
        }
        public void RenameDuplicate(string newFilename, string folderDuplicatePath, string blacklistPath, List<string> groups)
        {
            string pathNoFile = Path.GetDirectoryName(folderDuplicatePath); //gets path to be plugged in with the newfilename
                                                                            //string newfilenamenopath = Path.GetFileName(newfilenamewithpath);
                                                                            //string oldfilenamenopath = Path.GetFileName(oldfilenamewithpath);

            bool check1 = FileInList(pathNoFile + "\\" + newFilename, blacklistPath);

            if (check1) //in blacklist
            {
                //return newfilenamewithpath;
                //return true;
            }
            if (!check1) //not in blacklist
            {
                bool check2 = CheckDuplicateName(pathNoFile + "\\" + newFilename, groups);

                if (!check2) //chosen name doesn't exist already
                {
                    System.IO.File.Move(folderDuplicatePath, pathNoFile +"\\" + newFilename);

                } else
                {
                    //raise exception
                }
            }
        }
    }
}