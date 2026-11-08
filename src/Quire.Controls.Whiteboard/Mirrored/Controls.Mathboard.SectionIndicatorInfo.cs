#nullable disable
#pragma warning disable

using System.ComponentModel;
using Quire;

namespace Quire.Controls.Mathboard;

public partial class SectionIndicatorInfo
{
    // Declared in this order so the field-initializer delegate below lands in constructors 3 and 4 (Mathcad ordinals).
    private string _text;
    private double _top;
    private double _height;

    public SectionIndicatorInfo()
    {
    }

    public SectionIndicatorInfo(SectionIndicatorInfo originalSection, bool isReadonly)
    {
        Top = originalSection.Top;
        Height = originalSection.Height;
        Text = originalSection.Text;
        MinHeightRatio = originalSection.MinHeightRatio;
        Readonly = isReadonly;
    }

    public bool Readonly { get; private set; }

    public string Text
    {
        get
        {
            return _text;
        }
        set
        {
            if (_text == value)
            {
                return;
            }

            _text = value;
            FirePropertyChanged("Text");
        }
    }

    public double Top
    {
        get
        {
            return _top;
        }
        set
        {
            if (_top.IsEqualTo(value))
            {
                return;
            }

            _top = value;
            FirePropertyChanged("Top");
        }
    }

    public double Height
    {
        get
        {
            return _height;
        }
        set
        {
            if (_height.IsEqualTo(value))
            {
                return;
            }

            _height = value;
            FirePropertyChanged("Height");
        }
    }

    private double _minHeightRatio;

    public double MinHeightRatio
    {
        get
        {
            return _minHeightRatio;
        }
        set
        {
            if (_minHeightRatio.IsEqualTo(value))
            {
                return;
            }

            _minHeightRatio = value;
        }
    }

    public event PropertyChangedEventHandler PropertyChanged = delegate { };

    private void FirePropertyChanged(string propertyName)
    {
        if (PropertyChanged != null)
        {
            PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
