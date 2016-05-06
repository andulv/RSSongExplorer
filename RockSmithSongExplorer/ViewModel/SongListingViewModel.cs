using GalaSoft.MvvmLight;
using RockSmithSongExplorer.Services;
using GalaSoft.MvvmLight.Messaging;
using RockSmithSongExplorer.Models;
using RocksmithToolkitLib.Xml;
using System.Windows.Threading;
using System;
using System.Threading;
using System.Collections.Generic;
using RockSmithSongExplorer.Controls;
using System.Linq;
using System.Windows.Input;
using System.Threading.Tasks;
using System.Collections.ObjectModel;
using GalaSoft.MvvmLight.Command;

namespace RockSmithSongExplorer.ViewModel
{
    public class SongListingViewModel : ViewModelBase, IAsyncInitializtion
    {
        //TODO: Inject
        readonly SettingsService _settingsService = new SettingsService();

        public ICommand RefreshListingCommand { get; private set; }

        readonly ObservableCollection<RSSongInfo> _songs = new ObservableCollection<RSSongInfo>();
        public ObservableCollection<RSSongInfo> Songs { get { return _songs; } }

        RSSongInfo _selectedSong;
        public RSSongInfo SelectedSong
        {
            get { return _selectedSong; }
            set { Set(() => SelectedSong, ref _selectedSong, value); }
        }

        readonly BusyModel _busyModel;
        public BusyModel BusyModel
        {
            get { return _busyModel; }
        }

        public Task Initialization
        {
            get; protected set;
        }

        public SongListingViewModel()
        {
            //RefreshListingCommand = new RelayCommand(DoRefresh);
            _busyModel = new BusyModel();
            Initialization = InitAsync();
        }

        async Task InitAsync()
        {
            await DoRefresh();
        }

        async Task DoRefresh()
        {
            using (BusyModel.BeginBusyOperation("Reading files from directories..."))
            {
                Songs.Clear();
                foreach (var path in _settingsService.LibraryFolders)
                {
                    var files = System.IO.Directory.GetFiles(path, "*_p.psarc", System.IO.SearchOption.AllDirectories);
                    foreach (var file in files)
                    {
                        var arc = new ArcFileWrapper(file);
                        foreach (var song in await arc.GetAllSongInfos())
                        {
                            Songs.Add(song);
                        }
                    }
                }
            }
        }
    }
}
