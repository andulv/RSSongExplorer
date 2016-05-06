using Newtonsoft.Json;
using RockSmithSongExplorer.Utils;
using RocksmithToolkitLib;
using RocksmithToolkitLib.DLCPackage;
using RocksmithToolkitLib.DLCPackage.Manifest;
using RocksmithToolkitLib.PSARC;
using RocksmithToolkitLib.Sng2014HSL;
using RocksmithToolkitLib.Xml;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using System.Xml.Linq;
using Newtonsoft.Json.Linq;
using RockSmithSongExplorer.Models;

namespace RockSmithSongExplorer.Services
{
    public class ArcFileWrapper : IRocksmithSongProvider
    {
        readonly PSARC _psarc;
        readonly string _archiveFile;

        public ArcFileWrapper(string archiveFile)
        {
            _psarc = new PSARC();
            _archiveFile = archiveFile;
            using (var inputStream = System.IO.File.OpenRead(archiveFile))
            {
                _psarc.Read(inputStream);
            }
        }


        /// <summary>
        /// Reads .json manifest file for all arrangements in PSARC file. Creates and populates one RSSongInfo for each unique song. 
        /// </summary>
        /// <returns>List of all songs (with TrackInfo entry for each arrangement) in current PSARC file.</returns>
        public async Task<IList<RSSongInfo>> GetAllSongInfos()
        {
            var src = _psarc.Entries.Where(x => x.Name.StartsWith(@"manifests/songs") && x.Name.EndsWith(".json"))
                                    .OrderBy(x => x.Name);

            var retValue = new List<RSSongInfo>();
            RSSongInfo currentSongInfo = null;

            foreach (var entry in src)
            {
                var entryName = System.IO.Path.GetFileNameWithoutExtension(entry.Name);
                var splitPoint = entryName.LastIndexOf('_');
                var entrySongKey = entryName.Substring(0, splitPoint);
                var entryArrangmentName = entryName.Substring(splitPoint+1);
                if (currentSongInfo == null || entrySongKey != currentSongInfo.Key)
                {
                    string song_name, album_name, artist_name, song_year;
                    using (var wrappedStream = new NonClosingStreamWrapper(entry.Data))
                    {
                        using (var reader = new StreamReader(wrappedStream))
                        {
                            string json = await reader.ReadToEndAsync();
                            JObject o = JObject.Parse(json);
                            var attributes = o["Entries"].First.Last["Attributes"];

                            song_name = attributes["SongName"].ToString();
                            album_name = attributes["AlbumName"].ToString();
                            artist_name = attributes["ArtistName"].ToString();
                            song_year = attributes["SongYear"].ToString();
                        }
                    }

                    currentSongInfo = new RSSongInfo()
                    {
                        Key = entrySongKey,
                        ContainerFileName=_archiveFile,
                        TrackInfos = new List<RSTrackInfo>(),
                        SongName = song_name,
                        AlbumName = album_name,
                        ArtistName = artist_name,
                        SongYear = song_year
                    };
                    retValue.Add(currentSongInfo);
                }
                var arrangmentInfo = new RSTrackInfo() { Name = entryArrangmentName };
                currentSongInfo.TrackInfos.Add(arrangmentInfo);
            }

            return retValue;
        }



        public async Task<Song2014> GetInstrumentTrack(string songKey, string arrangmentName)
        {
            var sngEntry=_psarc.Entries.FirstOrDefault(x =>x.Name == @"songs/bin/generic/" + songKey + "_" + arrangmentName + ".sng");
            var jsonEntry = _psarc.Entries.FirstOrDefault(x => x.Name.StartsWith(@"manifests/songs") && x.Name.EndsWith("/" + songKey + "_" + arrangmentName + ".json"));
            if (sngEntry == null || jsonEntry == null)
            {
                return null;
            }

            Attributes2014 att;
            using (var wrappedStream = new NonClosingStreamWrapper(jsonEntry.Data))
            {
                using (var reader = new StreamReader(wrappedStream))
                {
                    var readResult = await reader.ReadToEndAsync();
                    var manifest = JsonConvert.DeserializeObject<Manifest2014<Attributes2014>>(readResult);
                    att = manifest.Entries.ToArray()[0].Value.ToArray()[0].Value;
                }               
            }

            Sng2014File sngFile;
            using (var wrappedStream = new NonClosingStreamWrapper(sngEntry.Data))
            {
                var platform = _archiveFile.GetPlatform();
                sngFile = Sng2014File.ReadSng(wrappedStream, platform);
            }
            var sngObject = new Song2014(sngFile,att);
            return sngObject;
        }

        public Vocals GetVocalTrack(string songKey)
        {
            var sngEntry = _psarc.Entries.FirstOrDefault(x => x.Name == @"songs/bin/generic/" + songKey + "_vocals.sng");
            if (sngEntry == null)
                return null;
            Sng2014File sngFile;
            using (var wrappedStream = new NonClosingStreamWrapper(sngEntry.Data))
            {
                var platform = _archiveFile.GetPlatform();
                sngFile = Sng2014File.ReadSng(wrappedStream, platform);
            }

            return new Vocals(sngFile);

        }
    }




}
