using System.Windows.Media;
using Quire.Controls.Core;

namespace Quire.Controls;

public partial class CustomColorPickerViewModel : ViewModelBase
{
    private ColorInfo _newColorInfo;
    private Color _currentColor;
    private bool _okIsEnabled = true;

    public Color CurrentColor
    {
        get => _currentColor;
        set
        {
            _currentColor = value;
            NewColor = new ColorInfo((double)value.R, (double)value.G, (double)value.B);
            OnPropertyChanged("CurrentColor");
        }
    }

    public Color SelectedColor => Color.FromRgb((byte)_newColorInfo.Red, (byte)_newColorInfo.Green, (byte)_newColorInfo.Blue);

    public ColorInfo NewColor
    {
        get => _newColorInfo;
        set
        {
            _newColorInfo = value;
            OnPropertyChanged("NewColor");
        }
    }

    public bool OkIsEnabled
    {
        get => _okIsEnabled;
        set
        {
            _okIsEnabled = value;
            OnPropertyChanged("OkIsEnabled");
        }
    }

    public void Init(object context)
    {
        CurrentColor = (context is SolidColorBrush solidColorBrush)
            ? Color.FromArgb(solidColorBrush.Color.A, solidColorBrush.Color.R, solidColorBrush.Color.G, solidColorBrush.Color.B)
            : Colors.Transparent;
    }

    public void Refresh()
    {
        OnPropertyChanged("NewColor");
    }

    public void Reset()
    {
        NewColor = new ColorInfo((double)_currentColor.R, (double)_currentColor.G, (double)_currentColor.B);
    }

    public CustomColorPickerViewModel()
    {
    }
}
