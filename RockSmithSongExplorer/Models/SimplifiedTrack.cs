using RocksmithToolkitLib.Sng;
using RocksmithToolkitLib.Xml;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace RockSmithSongExplorer.Models
{
    public class SimplifiedTrack
    {
        public string ArrangementName { get; set; }
        public int NumberOfStrings { get { return Tuning==null ? 0 : Tuning.Length; } }
        public List<SimplifiedBar> Bars { get; set; }

        public short[] Tuning { get; set; }
        public byte Capo { get; set; }

        public int GetBarIndex(float time)
        {
            var idx = Bars.FindIndex(x => x.StartTime <= time && x.EndTime >= time);
            return idx;
        }

        public SimplifiedBar GetBar(float time)
        {
            var bar = Bars.FirstOrDefault(x => x.StartTime <= time && x.EndTime >= time);
            return bar;
        }

        public List<SongChordTemplate2014> ChordTemplates { get; set; }
    }
}
