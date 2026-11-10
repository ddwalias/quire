using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;

namespace Quire.Controls.InputControls;

internal partial class SliderViewModel
{
    private SliderModel Model => (SliderModel)_model;

    public double Position
    {
        get => Model.Position;
        set => Model.Position = value;
    }

    public VerticalAlignment VerticalAlignment
    {
        get
        {
            if (Orientation == Orientation.Horizontal)
            {
                return VerticalAlignment.Top;
            }

            return VerticalAlignment.Stretch;
        }
    }

    public HorizontalAlignment HorizontalAlignment
    {
        get
        {
            if (Orientation == Orientation.Horizontal)
            {
                return HorizontalAlignment.Stretch;
            }

            return HorizontalAlignment.Left;
        }
    }

    public Orientation Orientation
    {
        get => Model.Orientation;
        set => Model.Orientation = value;
    }

    public TickPlacement TickPlacement
    {
        get => Model.TickPlacement;
        set => Model.TickPlacement = value;
    }

    public double TickFrequency => Model.TickFrequency;

    public double SmallChange => Model.SmallChange;

    public double LargeChange => Model.LargeChange;

    public double Minimum
    {
        get => Model.Minimum;
        set => Model.Minimum = value;
    }

    public Thickness BorderThickness => Model.Border ? new Thickness(1) : new Thickness(0);

    public double Maximum
    {
        get => Model.Maximum;
        set => Model.Maximum = value;
    }

    public bool ShowMinMax
    {
        get => Model.ShowMinMax;
        set => Model.ShowMinMax = value;
    }

    public bool Invert
    {
        get => Model.Invert;
        set => Model.Invert = value;
    }

    public bool IsSnapToTickEnabled => Model.IsSnapToTickEnabled;

    public int Interval => Model.Interval;
}
