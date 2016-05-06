using System.Windows;
using RockSmithSongExplorer.ViewModel;
using System.Reflection;
using RockSmithSongExplorer.Controls;
using System.Windows.Controls;
using RockSmithSongExplorer.Models;

namespace RockSmithSongExplorer
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
            this.Title = this.Title + " - v" + Assembly.GetEntryAssembly().GetName().Version.ToString();
        }

        private void DataGrid_MouseDoubleClick(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            var vm = this.DataContext as MainViewModel;
            var dg = sender as DataGrid;
            vm.OpenSong(dg.SelectedItem as RSSongInfo);

        }
    }
}
