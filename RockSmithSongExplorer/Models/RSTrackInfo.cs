using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RockSmithSongExplorer.Models
{
    public class RSSongInfo
    {
        public string Key { get; set; }
        public string SongName { get; set; }
        public string AlbumName { get; set; }
        public string ArtistName { get; set; }
        public string SongYear { get; set; }

        public string ContainerFileName { get; set; }

        public IList<RSTrackInfo> TrackInfos { get; set; }
    }

    public class RSTrackInfo
    {
        public string Name { get; set; }

        public override string ToString()
        {
            return Name ?? base.ToString();
        }
    }
}
