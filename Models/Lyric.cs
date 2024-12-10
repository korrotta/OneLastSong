using System.ComponentModel;

public class Lyric : INotifyPropertyChanged
{
    private bool _isFocused;

    public int Id { get; set; }
    public int AudioId { get; set; }
    public float Timestamp { get; set; }
    public string LyricText { get; set; }

    public bool IsFocused
    {
        get => _isFocused;
        set
        {
            if (_isFocused != value)
            {
                _isFocused = value;
                OnPropertyChanged(nameof(IsFocused));
            }
        }
    }

    public event PropertyChangedEventHandler PropertyChanged;

    protected void OnPropertyChanged(string propertyName)
    {
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }
}