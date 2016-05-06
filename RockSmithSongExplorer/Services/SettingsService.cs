using RockSmithSongExplorer.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RockSmithSongExplorer.Services
{
    class SettingsService
    {
        private IList<LibraryPath> _libraryFolders;

        public IList<LibraryPath> LibraryFolders
        {
            get { return _libraryFolders; }
//            set { _libraryFolders = value; }
        }


        public SettingsService()
        {
            _libraryFolders = new List<LibraryPath>();
            _libraryFolders.Add(new LibraryPath(@"c:\Program Files (x86)\Steam\SteamApps\common\Rocksmith2014\dlc","*_p.psarc",true));
        }
    }
}
