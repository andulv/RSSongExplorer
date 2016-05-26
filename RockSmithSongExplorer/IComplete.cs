using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RockSmithSongExplorer
{
    public interface IComplete
    {
        Task Completed { get; }
    }

    public interface ICompleteWithResult<T> 
    {
        Task<T> Completed { get; }
    }
}
