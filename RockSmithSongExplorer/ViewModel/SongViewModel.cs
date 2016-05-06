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
    public class SongViewModel : ViewModelBase, IAsyncInitializtion
    {
        Song2014 _selectedArrangement;
        public Song2014 SelectedArrangement
        {
            get { return _selectedArrangement; }
            set 
            { 
                Set(() => SelectedArrangement, ref _selectedArrangement, value);
                if (value == null)
                    MultiTrackData = null;
                else
                {
                    var simpleArrangement = ArrangementSimplifier.GetSimplifiedTrack(_selectedArrangement);
                    var otherTracks = InstrumentArrangements.
                                        Where(x => x != _selectedArrangement).
                                        Select(x => ArrangementSimplifier.GetSimplifiedTrack(x)).ToList();
                    var sections = _selectedArrangement.Sections.OrderBy(x => x.StartTime).ToList();
                    MultiTrackData = new SimplifiedSong(simpleArrangement, VocalArrangement, otherTracks, sections);
                }
            }
        }

        IList<Song2014> _instrumentArrangements;
        public IList<Song2014> InstrumentArrangements { get { return _instrumentArrangements; } }

        Vocals _vocalArrangement;
        public Vocals VocalArrangement { get { return _vocalArrangement; } }

        SimplifiedSong _multiTrackData;
        public SimplifiedSong MultiTrackData
        {
            get { return _multiTrackData; }
            private set { Set(() => MultiTrackData, ref _multiTrackData, value); }
        }

        public Task Initialization { get; private set; }

        public SongViewModel(RSSongInfo song)
        {
            Initialization = InitASync(song);
        }

        public string SongName {  get { return "SongName"; } }

        async Task InitASync(RSSongInfo song)
        {
            if (song == null)
                return;
            var fileReader = new ArcFileWrapper(song.ContainerFileName);
            var instArrangements = new List<Song2014>();
            foreach (var track in song.TrackInfos.Where(x => !x.Name.ToLower().StartsWith("vocal")))
            {
                var instarr = await fileReader.GetInstrumentTrack(song.Key, track.Name);
                instArrangements.Add(instarr);
            }
            _vocalArrangement = fileReader.GetVocalTrack(song.Key);
            _instrumentArrangements = instArrangements;
            RaisePropertyChanged(() => VocalArrangement);
            RaisePropertyChanged(() => InstrumentArrangements);
            SelectedArrangement = _instrumentArrangements.FirstOrDefault();
        }


    }
}
