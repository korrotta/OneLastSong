using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OneLastSong.Contracts
{
    public interface INavigationStateSavable
    {
        public object GetCurrentParameterState();
        public void OnStateLoad(object parameter);
    }
}
