using NAudio.Extras;
using OneLastSong.Cores.Equalizer;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OneLastSong.ViewModels
{
    public class EqualizerViewModel
    {
        public ObservableCollection<EqualizerBand> Bands { get; set; }

        public EqualizerViewModel()
        {
            Bands = new ObservableCollection<EqualizerBand>
            {
                new EqualizerBand { Frequency = 60 },
                new EqualizerBand { Frequency = 170 },
                new EqualizerBand { Frequency = 310 },
                new EqualizerBand { Frequency = 600 },
                new EqualizerBand { Frequency = 1000 },
                new EqualizerBand { Frequency = 3000 },
                new EqualizerBand { Frequency = 6000 },
                new EqualizerBand { Frequency = 12000 },
                new EqualizerBand { Frequency = 14000 },
                new EqualizerBand { Frequency = 16000 }
            };
        }
    }
}
