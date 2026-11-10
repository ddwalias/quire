using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using Quire.EquationEditor.Tree;

namespace Quire.Controls.InputControls;

internal partial class SliderModel
{
    private SliderScriptableControlData Data => (SliderScriptableControlData)_data;

    public Orientation Orientation
    {
        get => Data.Orientation;
        set
        {
            if (Data.Orientation != value)
            {
                Data.Orientation = value;
                OnPropertyChanged(nameof(Orientation));
            }
        }
    }

    public TickPlacement TickPlacement
    {
        get => Data.TickPlacement;
        set
        {
            Data.TickPlacement = value;
            OnPropertyChanged(nameof(TickPlacement));
        }
    }

    public double TickFrequency
    {
        get => Data.TickFrequency;
        set
        {
            Data.TickFrequency = value;
            OnPropertyChanged(nameof(TickFrequency));
        }
    }

    public double SmallChange
    {
        get => Data.SmallChange;
        set
        {
            Data.SmallChange = value;
            OnPropertyChanged(nameof(SmallChange));
        }
    }

    public double LargeChange
    {
        get => Data.LargeChange;
        set
        {
            Data.LargeChange = value;
            OnPropertyChanged(nameof(LargeChange));
        }
    }

    public double Minimum
    {
        get => Data.Minimum;
        set
        {
            Data.Minimum = value;
            OnPropertyChanged(nameof(Minimum));
        }
    }

    public double Maximum
    {
        get => Data.Maximum;
        set
        {
            Data.Maximum = value;
            OnPropertyChanged(nameof(Maximum));
        }
    }

    public bool ShowMinMax
    {
        get => Data.ShowMinMax;
        set
        {
            Data.ShowMinMax = value;
            OnPropertyChanged(nameof(ShowMinMax));
        }
    }

    public bool Invert
    {
        get => Data.Invert;
        set
        {
            Data.Invert = value;
            OnPropertyChanged(nameof(Invert));
        }
    }

    public bool IsSnapToTickEnabled
    {
        get => Data.IsSnapToTickEnabled;
        set
        {
            Data.IsSnapToTickEnabled = value;
            OnPropertyChanged(nameof(IsSnapToTickEnabled));
        }
    }

    public bool Border
    {
        get => Data.Border;
        set
        {
            Data.Border = value;
            OnPropertyChanged(nameof(Border));
        }
    }

    public int Interval
    {
        get => Data.Interval;
        set
        {
            Data.Interval = value;
            OnPropertyChanged(nameof(Interval));
        }
    }
}
