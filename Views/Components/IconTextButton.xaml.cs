using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;

namespace OneLastSong.Views.Components
{
    public sealed partial class IconTextButton : UserControl
    {
        public IconTextButton()
        {
            this.InitializeComponent();
        }

        public string IconGlyph
        {
            get { return (string)GetValue(IconGlyphProperty); }
            set { SetValue(IconGlyphProperty, value); }
        }

        public static readonly DependencyProperty IconGlyphProperty =
            DependencyProperty.Register("IconGlyph", typeof(string), typeof(IconTextButton), new PropertyMetadata("\uE8A5"));

    }
}
