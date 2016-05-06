using RocksmithToolkitLib.Xml;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace RockSmithSongExplorer.Models
{
    public class SimplifiedBar
    {
        public short MesureId;
        public List<SongEbeat> EBeats = new List<SongEbeat>();
        public List<SongChord2014> Chords;// = new List<SongChord2014>();
        public List<SongNote2014> Notes;// = new List<SongNote2014>();

        public float StartTime;     //Time of first beat in this bar (redundant, could be get'er only?)
        public float EndTime;       //Time of first beat in next bar

        public override string ToString()
        {
            return "SimpliefiedBar " + MesureId + " : " + StartTime + " - " + EndTime;
        }
    }
}
