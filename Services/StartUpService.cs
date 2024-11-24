using OneLastSong.Contracts;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OneLastSong.Services
{
    public class StartUpService
    {
        private List<INotifySubSystemChanged> _subSystemNotifiers = new List<INotifySubSystemChanged>();

        public StartUpService()
        {

        }

        public void NotifySystemInitialized()
        {
            foreach (var notifier in _subSystemNotifiers)
            {
                notifier.OnSystemInitialized();
            }
        }
    }
}
