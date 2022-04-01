using System;
using Xunit;

namespace UnitTDD
{
    public class NewFeatures
    {
        [Fact]
        public void WeCanReadBlackList() //we can read the Blacklist
        {

        }

        [Fact]
        public void InputEqualBlacklist() //we can compare user input with the Blacklist
        {

        }

        [Fact]
        public void SelectFromSize() //we can elect a file path from Size comparison
        {

        }

        public void SelectFromNameandSize() //we can elect a file path from Size and Name comparison
        {

        }

        public void SelectFromMD5() //we can elect a file path from MD5 comparison
        {

        }

        [Fact]
        public void WeCanRenameFolderWithNewName() //We can rename a folder with a new name, not in blacklist
        {

        }

        public void StopRenamingInBlacklist() //We can rename a folder with a new name, not in blacklist
        {

        }


        [Fact]
        public void WeCannotRenameFolderWithExistingName() //We cannot rename a folder with the same name, else we would risk overwriting important files.
        {

        }
    }
}
