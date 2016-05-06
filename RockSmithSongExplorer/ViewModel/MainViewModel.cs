using GalaSoft.MvvmLight;
using RockSmithSongExplorer.Models;
using RocksmithToolkitLib.Xml;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace RockSmithSongExplorer.ViewModel
{
    public class MainViewModel : ViewModelBase, ISongOpener
    {

        readonly ObservableCollection<SongViewModel> _songs = new ObservableCollection<SongViewModel>();
        public ObservableCollection<SongViewModel> OpenSongs { get { return _songs; } }

        SongViewModel _selectedSong = null;
        public SongViewModel SelectedSong
        {
            get { return _selectedSong; }
            set { Set(() => SelectedSong, ref _selectedSong, value); }
        }

        readonly SongListingViewModel _songListing;
        public SongListingViewModel SongListing { get { return _songListing; } }

        public MainViewModel()
        {
            _songListing = new SongListingViewModel(this);
            _songs.Add(new SongViewModel(null));
            _selectedSong = _songs[0];
        }

        public async Task OpenSongInCurrentTab(RSSongInfo song)
        {
            var vm = new SongViewModel(song);
            var idx = OpenSongs.IndexOf(SelectedSong);
            if (idx == -1)
                idx = 0;
            _songs[idx] = vm;
            SelectedSong = vm;
            await vm.Initialization;
        }

        public async Task OpenSongInNewTab(RSSongInfo song)
        {
            var vm = new SongViewModel(song);
            _songs.Add(vm);
            SelectedSong = vm;
            await vm.Initialization;
        }

        public async Task OpenSongInNewWindow(RSSongInfo song)
        {
            var vm = new SongViewModel(song);
            _songs.Add(vm);
            SelectedSong = vm;
            await vm.Initialization;
        }


    }
}
