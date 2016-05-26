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
        public AwaitableDelegateCommand<object> AddNewLibraryPathCommand { get; private set; }
        public AwaitableDelegateCommand<object> EditSelectedLibraryPathCommand { get; private set; }
        public RelayCommand DeleteSelectedLibraryPathCommand { get; private set; }

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

        LibraryPath _selectedLibraryPath;
        public LibraryPath SelectedLibraryPath
        {
            get { return _selectedLibraryPath; }
            set
            {
                Set(() => SelectedLibraryPath, ref _selectedLibraryPath, value);
                EditSelectedLibraryPathCommand.RaiseCanExecuteChanged();
                DeleteSelectedLibraryPathCommand.RaiseCanExecuteChanged();
            }
        }

        readonly BusyModel _busyModel;
        public BusyModel BusyModel { get { return _busyModel; } }

        public Task Initialization { get; protected set; }

        public ObservableCollection<LibraryPath> LibraryPaths {  get { return _settingsService.LibraryPaths; } }

        public SongListingViewModel(ISongOpener songOpener)
        {
            _songOpener = songOpener;
            ReloadSongListCommand = new AwaitableDelegateCommand(DoLoadSongList,p=>true,ErrorCallback);
            OpenSelectedSongCommand = new AwaitableDelegateCommand<string>(DoOpenSelectedSong, p => SelectedSong!=null, ErrorCallback);
            AddNewLibraryPathCommand = new AwaitableDelegateCommand<object>(DoAddNewLibraryPath, p => true, ErrorCallback);
            EditSelectedLibraryPathCommand = new AwaitableDelegateCommand<object>(DoEditSelectedLibraryPath, p => SelectedLibraryPath != null, ErrorCallback);
            DeleteSelectedLibraryPathCommand = new RelayCommand(DoDeleteSelectedLibraryPath, () => SelectedLibraryPath != null);
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

        async Task DoSaveSettingsReloadSongList(object parameter)
        {

        }

        async Task DoAddNewLibraryPath(object parameter)
        {
            var vm = new EditLibraryPathViewModel();
            var result = await _songOpener.ShowDialog(vm);
            if (result.IsCanceled)
                return;

            LibraryPaths.Add((LibraryPath)result.DataItem);
        }

        async Task DoEditSelectedLibraryPath(object parameter)
        {
            var vm = new EditLibraryPathViewModel(SelectedLibraryPath);
            var result = await _songOpener.ShowDialog(vm);
        }

        void DoDeleteSelectedLibraryPath()
        {

        }

        async Task DoLoadSongList(object parameter)
        {
            using (BusyModel.BeginBusyOperation("Reading files from directories..."))
            {
                Songs.Clear();
                foreach (var path in _settingsService.LibraryPaths)
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
