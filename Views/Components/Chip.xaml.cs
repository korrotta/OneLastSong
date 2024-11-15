using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Controls.Primitives;
using Microsoft.UI.Xaml.Data;
using Microsoft.UI.Xaml.Input;
using Microsoft.UI.Xaml.Media;
using Microsoft.UI.Xaml.Navigation;
using OneLastSong.Utils;

// To learn more about WinUI, the WinUI project structure,
// and more about our project templates, see: http://aka.ms/winui-project-info.

namespace OneLastSong.Views.Components
{
    public sealed partial class Chip : UserControl
    {
        private bool _isSelected = false;

        private SolidColorBrush SelectedBackgroundColorBrush = ThemeUtils.GetBrush(ThemeUtils.BG_CONTRAST1);
        private SolidColorBrush SelectedTextColorBrush = ThemeUtils.GetBrush(ThemeUtils.TEXT_CONTRAST1);

        private SolidColorBrush DefaultBackgroundColorBrush = ThemeUtils.GetBrush(ThemeUtils.BG_TERTIARY);
        private SolidColorBrush DefaultTextColorBrush = ThemeUtils.GetBrush(ThemeUtils.TEXT_PRIMARY);

        public Chip()
        {
            this.InitializeComponent();
            UpdateColorsState();
        }

        public string Text
        {
            get => ChipText.Text;
            set => ChipText.Text = value;
        }

        public bool IsSelected 
        { 
            get => _isSelected;
            set { _isSelected = value; UpdateColorsState(); }
        }

        public event RoutedEventHandler OnSelected;
        public event RoutedEventHandler OnDeselected;

        private void OnChip_Tapped(object sender, RoutedEventArgs e)
        {
            OnSelected?.Invoke(this, e);
        }

        private void UpdateColorsState()
        {
            if (IsSelected)
            {
                ChipText.Foreground = SelectedTextColorBrush;
                ChipBackground.Background = SelectedBackgroundColorBrush;
            }
            else
            {
                ChipText.Foreground = DefaultTextColorBrush;
                ChipBackground.Background = DefaultBackgroundColorBrush;
            }
        }
    }
}
