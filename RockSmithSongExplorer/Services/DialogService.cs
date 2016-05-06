using System.Windows;
using Microsoft.Win32;

namespace RockSmithSongExplorer.Services
{
    /// <summary>
    /// This DialogService implementation opens uses WPF dialogs
    /// </summary>
    public class DialogService :IDialogService
    {
        public string OpenFile()
        {
            OpenFileDialog dialog = new OpenFileDialog
            {
                Filter = "RockSmith 2014 Song Archive (*.psarc)|*.psarc"
            };
            if (dialog.ShowDialog(Application.Current.MainWindow).GetValueOrDefault())
            {
                return dialog.FileName;
            }
            return null;
        }


    }
}
