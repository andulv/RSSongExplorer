using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RockSmithSongExplorer.Models
{
    public class LibraryPath
    {
        public string Folder { get; set; }
        public string IncludeFilter { get; set; }
        public bool RecurseSubFolders { get; set; }


        public LibraryPath()
        {

        }

        public LibraryPath(String folder, bool recurseSubFolders=false)
        {
            Folder = folder;
            RecurseSubFolders = recurseSubFolders;
        }

        public LibraryPath(String folder, string includeFilter=null,bool recurseSubFolders=false)
        {
            Folder = folder;
            IncludeFilter = includeFilter;
            RecurseSubFolders = recurseSubFolders;
        }
    }
}
