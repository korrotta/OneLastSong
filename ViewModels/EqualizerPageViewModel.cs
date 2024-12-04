using OneLastSong.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OneLastSong.ViewModels
{
    public class EqualizerPageViewModel
    {
        private EqualizerViewModel _equalizerViewModel;
        private ListeningService _listeningService;

        public EqualizerViewModel EqualizerViewModel
        {
            get => _equalizerViewModel;
        }

        public EqualizerPageViewModel()
        {
            _listeningService = ListeningService.Get();
            _equalizerViewModel = _listeningService.EqualizerViewModel;
        }

        public void UpdateEqualizer()
        {
            _listeningService.ApplyEQ();
        }
    }
}
