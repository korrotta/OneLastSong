using OneLastSong.Cores.Equalizer;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OneLastSong.ViewModels
{
    public class EqualizerPageViewModel
    {
        public ObservableCollection<EQBand> Bands { get; set; }

        public EqualizerPageViewModel()
        {
            Bands = new ObservableCollection<EQBand>
            {
                new EQBand { Frequency = 60 },
                new EQBand { Frequency = 170 },
                new EQBand { Frequency = 310 },
                new EQBand { Frequency = 600 },
                new EQBand { Frequency = 1000 },
                new EQBand { Frequency = 3000 },
                new EQBand { Frequency = 6000 },
                new EQBand { Frequency = 12000 },
                new EQBand { Frequency = 14000 },
                new EQBand { Frequency = 16000 }
            };
        }
    }
}
