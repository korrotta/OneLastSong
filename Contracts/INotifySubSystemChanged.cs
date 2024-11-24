using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OneLastSong.Contracts
{
    public interface INotifySubSystemChanged
    {
        public void OnSystemInitialized();
    }
}
