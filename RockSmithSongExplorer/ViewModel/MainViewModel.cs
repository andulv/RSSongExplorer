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
    public class MainViewModel : ViewModelBase
    {

        readonly ObservableCollection<SongViewModel> _songs = new ObservableCollection<SongViewModel>();
        public ObservableCollection<SongViewModel> OpenSongs { get { return _songs; } }

        SongViewModel _selectedSong = null;
        public SongViewModel SelectedSong
        {
            get { return _selectedSong; }
            set { Set(() => SelectedSong, ref _selectedSong, value); }
        }

        readonly SongListingViewModel _songListing = new SongListingViewModel();
        public SongListingViewModel SongListing { get { return _songListing; } }

        public MainViewModel()
        {
            _songs.Add(new SongViewModel(null));
            _selectedSong = _songs[0];
        }

        public void OpenSong(RSSongInfo song)
        {
            var vm = new SongViewModel(song);
            _songs[0] = vm;
            SelectedSong = vm;
            vm.Initialization.ContinueWith(t =>
            {
                if (t.Exception != null)
                    MessageBox.Show("Error openining song: \n" + t.Exception.Message);
            });
        }

    }
}
