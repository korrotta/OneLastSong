using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OneLastSong.Cores.Equalizer
{
    public class EQBand : INotifyPropertyChanged
    {
        public int Frequency { get; set; }
        public double Gain { get; set; }

        public event PropertyChangedEventHandler PropertyChanged;
    }
}
