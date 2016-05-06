using RockSmithSongExplorer.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
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
    /// Interaction logic for SongListingView.xaml
    /// </summary>
    public partial class SongListingView : UserControl
    {
        public SongListingView()
        {
            InitializeComponent();
        }

        private void csvSongs_Filter(object sender, FilterEventArgs e)
        {
            if(!string.IsNullOrWhiteSpace(txtSongFilter.Text))
            {
                var item = e.Item as RSSongInfo;
                var txt = txtSongFilter.Text.Trim().ToUpper();

                e.Accepted = item.AlbumName.ToUpper().Contains(txt) ||
                            item.ArtistName.ToUpper().Contains(txt) ||
                            item.SongName.ToUpper().Contains(txt);
            }
        }

        private void txtSongFilter_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter || e.Key == Key.Return)
            {
                (this.Resources["cvsSongs"] as CollectionViewSource).View.Refresh();
            }
        }

        private void txtSongFilter_LostFocus(object sender, RoutedEventArgs e)
        {
            (this.Resources["cvsSongs"] as CollectionViewSource).View.Refresh();
        }
    }
}
