using RockSmithSongExplorer.Models;
using RocksmithToolkitLib.Xml;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace RockSmithSongExplorer.Services
{
    interface IRocksmithSongProvider
    {
        Task<IList<RSSongInfo>> GetAllSongInfos();
        Task<Song2014> GetInstrumentTrack(string songKey, string arrangmentName);
        Vocals GetVocalTrack(string songKey);
    }
}
