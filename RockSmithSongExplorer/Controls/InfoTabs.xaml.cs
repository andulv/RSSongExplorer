using GalaSoft.MvvmLight.Messaging;
using RocksmithToolkitLib.Xml;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace RockSmithSongExplorer.Controls
{
    /// <summary>
    /// Interaction logic for InfoTabs.xaml
    /// </summary>
    public partial class InfoTabs : UserControl
    {
        public InfoTabs()
        {
            InitializeComponent();
        }

        void ChordTemplateFilter(object sender, System.Windows.Data.FilterEventArgs e)
        {
            var chordTemplate = e.Item as RocksmithToolkitLib.Xml.SongChordTemplate2014;
            e.Accepted = true;// chordTemplate.ChordId != null;
        }

        private void ListView_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            var lstview = sender as ListView;
            var selectedSection = lstview.SelectedItem as SongSection;
            if(selectedSection!=null)
            {
                var msg = new NavigateToTimeMessage() { Time = selectedSection.StartTime };
                Messenger.Default.Send(msg);
            }
        }
    }

    public class NavigateToTimeMessage
    {
        public float Time { get; set; }
    }
}
