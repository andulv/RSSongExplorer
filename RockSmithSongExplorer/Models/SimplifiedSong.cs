using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using RocksmithToolkitLib.Xml;

namespace RockSmithSongExplorer.Models
{
    public class SimplifiedSong
    {
        public SimplifiedTrack MainTrack { get; private set; }
        public Vocals VocalTrack { get; private set; }
        public IList<SimplifiedTrack> AdditionalTracks { get; private set; }
        public IList<SongSection> Sections { get; private set; }

        public SimplifiedSong(SimplifiedTrack mainTrack, Vocals vocalTrack, IList<SimplifiedTrack> additionalTracks, IList<SongSection> sections)
        {
            MainTrack = mainTrack;
            VocalTrack = vocalTrack;
            AdditionalTracks = additionalTracks;
            Sections = sections;
        }
    }
}
