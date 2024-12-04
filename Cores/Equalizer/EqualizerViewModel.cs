using OneLastSong.Cores.Equalizer;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace OneLastSong.ViewModels
{
    public class EqualizerViewModel : INotifyPropertyChanged
    {
        public ObservableCollection<EqualizerBand> _bands { get; set; }
        public event EventHandler<List<EqualizerBand>> EqualizerBandsChanged;

        public ObservableCollection<EqualizerBand> Bands
        {
            get => _bands;
            set
            {
                _bands = value;
                EqualizerBandsChanged?.Invoke(this, _bands.ToList());
                OnPropertyChanged(nameof(Bands));
            }
        }

        protected void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

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

            foreach (var band in Bands)
            {
                band.BandChanged += Band_BandChanged;
            }
        }

        private void Band_BandChanged(object sender, EqualizerBand e)
        {
            EqualizerBandsChanged?.Invoke(this, Bands.ToList());
        }

        public event PropertyChangedEventHandler PropertyChanged;

        internal void ResetEqualizer()
        {
            foreach (var band in Bands)
            {
                band.Gain = 0;
            }
        }
    }

    public class EqualizerBandChangedEventArgs : EventArgs
    {
        public EqualizerBand Band { get; set; }
    }
}
