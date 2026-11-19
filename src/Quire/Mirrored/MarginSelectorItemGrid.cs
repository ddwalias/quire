namespace Quire;

public partial class MarginSelectorItemGrid
{
    public string Header
    {
        get => _header;
        set
        {
            if (value == _header)
            {
                return;
            }

            _header = value;
            OnPropertyChanged("Header");
        }
    }

    public global::System.Windows.Media.ImageSource ImageSource
    {
        get => _imageSource;
        set
        {
            if (value == _imageSource)
            {
                return;
            }

            _imageSource = value;
            OnPropertyChanged("ImageSource");
        }
    }

    protected virtual void OnPropertyChanged(string propertyName = default)
    {
        PropertyChanged?.Invoke(this, new global::System.ComponentModel.PropertyChangedEventArgs(propertyName));
    }
}
