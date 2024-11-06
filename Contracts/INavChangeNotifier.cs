using OneLastSong.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OneLastSong.Contracts
{
    public interface INavChangeNotifier
    {
        public void OnNavHistoryChanged(NavigationService navService);
    }
}
