using Caliburn.Micro;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DuplicateFinder.Logic;
using DuplicateFinderWPF.Views;
using System.ComponentModel;
using DuplicateFinder.Logic.Interface;
using System.IO;

namespace DuplicateFinderWPF.ViewModels
{

    public class DuplicateFinderViewModel : Conductor<object>
    {
        public DuplicateFinderViewModel() { }
        
        
        private string _path = "Please enter a folder path to search for duplicate files: ";
        private DuplicateFinder.Logic.DuplicateFinder _finder = new DuplicateFinder.Logic.DuplicateFinder();
        private string _duplicateBySize = "";
        private string _duplicateBySizeAndName = "";
        private string _duplicateByMD5 = "";

        // Model instance
        public DuplicateFinder.Logic.DuplicateFinder Finder
                {
                    get { return _finder; }
                    set { _finder = value; }
                }

        // Strings to be referenced in the View
        public string DuplicateByMD5
        {
            get { return _duplicateByMD5; }
            set 
            {
                _duplicateByMD5 = value;
                NotifyOfPropertyChange(() => DuplicateByMD5);
            }
        }
        public string DuplicateBySizeAndName
        {
            get { return _duplicateBySizeAndName; }
            set 
            { 
                _duplicateBySizeAndName = value;
                NotifyOfPropertyChange(() => DuplicateBySizeAndName);
            }
        }
        public string DuplicateBySize
        {
            get { return _duplicateBySize; }
            set 
            { 
                _duplicateBySize = value;
                NotifyOfPropertyChange(() => DuplicateBySize);
            }
        }
        public string Path
        {
            get 
            { 
                return _path; 
            }
            set 
            { 
                _path = value;
                NotifyOfPropertyChange(() => Path);
            }
        }

        // Collect all the duplicates found and split them in groups
        public string SampleDuplicates(IEnumerable<IDuplicate> duplicates)
        {
            string sampledDuplicates = "";
            var i = 1;
            foreach (var duplicate in duplicates)
            {
                sampledDuplicates += $"Group{i++}: \n";
                sampledDuplicates += SampleDuplicateGroup(duplicate);
            }
            return sampledDuplicates;
        }

        // Get every path of the duplicates
        public string SampleDuplicateGroup(IDuplicate duplicate)
        {
            string sampledDuplicateGroup = "";
            foreach (var filePath in duplicate.FilePaths)
            {
                sampledDuplicateGroup += filePath + "\n";
            }
            return sampledDuplicateGroup;
        }

        // Check if there were duplicates found, else, inform of result.
        public string CheckEmptyString(string sampledDuplicates)
        {
            if (!string.IsNullOrEmpty(sampledDuplicates))
            {
                return sampledDuplicates;
            }
            else
            {
                return "There are no duplicates of this kind";
            }
        }

        // Find duplicates and update variables with the sorted and filtered results
        public void Launch()
        {
            if (!Directory.Exists(Path))
            {
                Path = "Invalid input. Please try a different path";
            }
            else
            {
                var duplicateBySize = _finder.CollectCandidates(Path, DuplicateFinder.Logic.Model.CompareMode.Size);
                var duplicateBySizeAndName = _finder.CollectCandidates(Path, DuplicateFinder.Logic.Model.CompareMode.SizeAndName);
                var duplicateByMD5 = _finder.CheckCandidates(duplicateBySizeAndName);

                //Experimenting //these are the intializing
                //Console.WriteLine(duplicateBySize);
                //Console.WriteLine(duplicateBySizeAndName);
                //Console.WriteLine(duplicateByMD5);


                DuplicateBySize = CheckEmptyString(SampleDuplicates(duplicateBySize));
                DuplicateBySizeAndName = CheckEmptyString(SampleDuplicates(duplicateBySizeAndName));
                DuplicateByMD5 = CheckEmptyString(SampleDuplicates(duplicateByMD5));

                //Experimenting //this is the output
                //Console.WriteLine(DuplicateBySize);
                //Console.WriteLine(DuplicateBySizeAndName);
                //Console.WriteLine(DuplicateByMD5);
            }
        }
    }
}
