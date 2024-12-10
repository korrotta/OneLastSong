using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OneLastSong.Cores.Equalizer
{
    public class EqualizerBand : INotifyPropertyChanged
    {
        private float _frequency;
        private float _bandwidth = 1f;
        private float _gain;

        public event EventHandler<EqualizerBand> BandChanged;

        public float Frequency
        {
            get => _frequency;
            set
            {
                if (_frequency != value)
                {
                    _frequency = value;
                    BandChanged?.Invoke(this, this);
                    OnPropertyChanged(nameof(Frequency));
                }
            }
        }

        public float Bandwidth
        {
            get => _bandwidth;
            set
            {
                if (_bandwidth != value)
                {
                    _bandwidth = value;
                    BandChanged?.Invoke(this, this);
                    OnPropertyChanged(nameof(Bandwidth));
                }
            }
        }

        public float Gain
        {
            get => _gain;
            set
            {
                if (_gain != value)
                {
                    _gain = value;
                    BandChanged?.Invoke(this, this);
                    OnPropertyChanged(nameof(Gain));
                }
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;

        protected void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
