using RockSmithSongExplorer.Models;
using System.Threading.Tasks;

namespace RockSmithSongExplorer.ViewModel
{
    public interface ISongOpener
    {
        Task OpenSongInCurrentTab(RSSongInfo song);
        Task OpenSongInNewTab(RSSongInfo song);
        Task OpenSongInNewWindow(RSSongInfo song);
    }
}