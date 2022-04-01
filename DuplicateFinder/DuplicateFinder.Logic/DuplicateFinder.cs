using System;
using System.Collections.Generic;
using System.IO;
using System.Security.Cryptography;
using DuplicateFinder.Logic.Interface;
using DuplicateFinder.Logic.Model;

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
    //Classes and Methods added for the new features:

    public class FileRenamer : DuplicateFinder //FileRenamer is an extension of Duplicate Finder
    {
        public List<string> ReadBlackListFetch(string pathBL)
        {
            var blLibrary = new List<string>(5);
            throw new NotImplementedException();
            return blLibrary;
        }
        public bool FileInBlackList(string filename, string pathFolder)
        {
            throw new NotImplementedException();
            if (true)
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        public bool CheckDuplicateName(string newfilenamewithpath, string oldfilenamewithpath)
        {
            //string pathnofile = Path.GetDirectoryName(oldfilenamewithpath);
            //string newfilenamenopath = Path.GetFileName(newfilenamewithpath);
            //string oldfilenamenopath = Path.GetFileName(oldfilenamewithpath);

            if (newfilenamewithpath == oldfilenamewithpath)
            {
                return true;
            }
            else 
            {
                return false;
            }
        }
        public void RenameDuplicate(string newfilenamewithpath, string oldfilenamewithpath)
        {
            //string pathnofile = Path.GetDirectoryName(oldfilenamewithpath);
            //string newfilenamenopath = Path.GetFileName(newfilenamewithpath);
            //string oldfilenamenopath = Path.GetFileName(oldfilenamewithpath);

            if (true)
            {
                //return newfilenamewithpath;
                //return true;
            }
            if (false) //in blacklist
            {
                //throw new InvalidOperationException(newfilename + " is in Files Blacklist.txt");
                //return oldfilenamewithpath;
                //return false;
            }
            if (false) //name already exists
            {
                //throw new InvalidOperationException(newfilename + " already exists);
                //return oldfilenamewithpath;
                //return false;
            }
        }
    }
}