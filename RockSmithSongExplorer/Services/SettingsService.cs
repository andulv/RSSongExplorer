using Newtonsoft.Json;
using RockSmithSongExplorer.Models;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RockSmithSongExplorer.Services
{
    class SettingsService
    {
        readonly string _settingsFolder;

        private ObservableCollection<LibraryPath> _libraryFolders;

        public ObservableCollection<LibraryPath> LibraryPaths
        {
            get { return _libraryFolders; }
//            set { _libraryFolders = value; }
        }

        public void SaveSettings()
        {
            var fileName = System.IO.Path.Combine(_settingsFolder, "LibraryFolders.json");
            string json = JsonConvert.SerializeObject(_libraryFolders, Formatting.Indented);
            System.IO.File.WriteAllText(fileName, json);
        }

        public void LoadSettings()
        {

        }


        public SettingsService()
        {
            var appDataFolder = System.Environment.GetFolderPath(Environment.SpecialFolder.CommonApplicationData);
            _settingsFolder = System.IO.Path.Combine(appDataFolder, "RockSmithSongExplorer");
            _libraryFolders = new ObservableCollection<LibraryPath>();
            _libraryFolders.Add(new LibraryPath(@"c:\Program Files (x86)\Steam\SteamApps\common\Rocksmith2014\dlc","*_p.psarc",true));
        }
    }
}
