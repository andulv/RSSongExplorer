using RockSmithSongExplorer.ViewModel;
using RocksmithToolkitLib.Xml;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Linq;
using System.Threading;
using System.Windows;

namespace RockSmithSongExplorer
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        protected override void OnStartup(StartupEventArgs e)
        {
            Thread.CurrentThread.CurrentUICulture = Thread.CurrentThread.CurrentCulture;


            //string fileName = @"c:\Program Files (x86)\Steam\SteamApps\common\Rocksmith2014\dlc\New\Metallica-SeekandDestroy_DD_p.psarc";
            ////string songName = "";

            //
            //var song = fileReader.GetAllSongInfos().FirstOrDefault();// x => x.Key == "mansoldworld");

            //var instrumentArrangements = new List<Song2014>();
            //foreach (var track in song.TrackInfos.Where(x=>!x.Name.ToLower().StartsWith("vocal")))
            //{
            //    var instarr = fileReader.GetInstrumentTrack(song.Key, track.Name);
            //    instrumentArrangements.Add(instarr);
            //}
            //var vocalArrangement = fileReader.GetVocalTrack(song.Key);
            //var vm = new SongViewModel(instrumentArrangements, vocalArrangement);

            var vm = new MainViewModel();
            var window = new MainWindow() { DataContext = vm };

            this.MainWindow = window;
            window.Show();
            base.OnStartup(e);
        }
    }
}
