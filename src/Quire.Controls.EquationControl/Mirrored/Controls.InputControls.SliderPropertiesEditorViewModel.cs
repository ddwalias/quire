using System.Windows.Controls;

namespace Quire.Controls.InputControls;

internal partial class SliderPropertiesEditorViewModel
{
    private ISliderPropertiesEditorModel Model => (ISliderPropertiesEditorModel)_model;

    public Orientation Orientation
    {
        get => Model.Orientation;
        set => Model.Orientation = value;
    }

    public bool Border
    {
        get => Model.Border;
        set => Model.Border = value;
    }

    public double SmallChange
    {
        get => Model.SmallChange;
        set => Model.SmallChange = value;
    }

    public double LargeChange
    {
        get => Model.LargeChange;
        set => Model.LargeChange = value;
    }

    public double TickFrequency
    {
        get => Model.TickFrequency;
        set => Model.TickFrequency = value;
    }

    public double Minimum
    {
        get => Model.Minimum;
        set => Model.Minimum = value;
    }

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
}
