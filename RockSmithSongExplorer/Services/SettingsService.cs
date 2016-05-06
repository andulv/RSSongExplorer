using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RockSmithSongExplorer.Services
{
    class SettingsService
    {
        private IList<string> _libraryFolders;

        public IList<string> LibraryFolders
        {
            get { return _libraryFolders; }
//            set { _libraryFolders = value; }
        }


        public SettingsService()
        {
            _libraryFolders = new List<string>();
            _libraryFolders.Add(@"c:\Program Files (x86)\Steam\SteamApps\common\Rocksmith2014\dlc");
        }
    }
}
