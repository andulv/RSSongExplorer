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
using System.Windows;

namespace RockSmithSongExplorer.ViewModel
{
    public class SongListingViewModel : ViewModelBase, IAsyncInitializtion
    {
        //TODO: Inject
        readonly SettingsService _settingsService = new SettingsService();
        readonly ISongOpener _songOpener;

        public ICommand ReloadSongListCommand { get; private set; }
        public AwaitableDelegateCommand<string> OpenSelectedSongCommand { get; private set; }

        readonly ObservableCollection<RSSongInfo> _songs = new ObservableCollection<RSSongInfo>();
        public ObservableCollection<RSSongInfo> Songs { get { return _songs; } }

        RSSongInfo _selectedSong;
        public RSSongInfo SelectedSong
        {
            get { return _selectedSong; }
            set
            {
                Set(() => SelectedSong, ref _selectedSong, value);
                OpenSelectedSongCommand.RaiseCanExecuteChanged();
            }
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

        public SongListingViewModel(ISongOpener songOpener)
        {
            _songOpener = songOpener;
            ReloadSongListCommand = new AwaitableDelegateCommand(DoLoadSongList,p=>true,ErrorCallback);
            OpenSelectedSongCommand = new AwaitableDelegateCommand<string>(DoOpenSelectedSong, p => SelectedSong!=null, ErrorCallback);           
            _busyModel = new BusyModel();
            Initialization = InitAsync();
        }

        void ErrorCallback(Exception ex)
        {
            MessageBox.Show("An error has occured:\n" + ex.Message);
        }

        async Task InitAsync()
        {
            await DoLoadSongList(null);
        }

        async Task DoOpenSelectedSong(string parameter)
        {

            if(string.IsNullOrEmpty(parameter))
                await _songOpener.OpenSongInCurrentTab(SelectedSong);
            else if (parameter.ToLower()=="tab")
                await _songOpener.OpenSongInNewTab(SelectedSong);
            else if (parameter.ToLower() == "window")
                await _songOpener.OpenSongInNewWindow(SelectedSong);
            else
                await _songOpener.OpenSongInCurrentTab(SelectedSong);
        }

        async Task DoLoadSongList(object parameter)
        {
            using (BusyModel.BeginBusyOperation("Reading files from directories..."))
            {
                Songs.Clear();
                foreach (var path in _settingsService.LibraryFolders)
                {
                    var filter = String.IsNullOrEmpty(path.IncludeFilter) ? "*" : path.IncludeFilter;
                    var files = System.IO.Directory.GetFiles(path.Folder, filter, System.IO.SearchOption.AllDirectories);
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
