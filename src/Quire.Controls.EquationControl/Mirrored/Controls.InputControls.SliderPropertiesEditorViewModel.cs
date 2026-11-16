using System.ComponentModel;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;

namespace Quire.Controls.InputControls;

internal partial class SliderPropertiesEditorViewModel : InputControlEditorViewModel
{
    private bool _ticks;

    private ISliderPropertiesEditorModel Model => (ISliderPropertiesEditorModel)_model;

    public SliderPropertiesEditorViewModel(IInputControlEditorModel model)
        : base(model)
    {
        if (Model.TickPlacement != TickPlacement.None)
        {
            _ticks = true;
        }
    }

    protected override void HandleModelPropertyChanged(object sender, PropertyChangedEventArgs e)
    {
        switch (e.PropertyName)
        {
            case "Minimum":
            case "Maximum":
            case "ShowMinMax":
            case "Invert":
            case "Border":
            case "SmallChange":
            case "LargeChange":
            case "Orientation":
            case "TickPlacement":
            case "TickFrequency":
            case "Ticks":
                OnPropertyChanged(e.PropertyName);
                break;
        }
    }

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

    public TickPlacement TickPlacement
    {
        get
        {
            if (Model.TickPlacement != TickPlacement.None && !_ticks)
            {
                _ticks = true;
                OnPropertyChanged("Ticks");
            }
            return Model.TickPlacement;
        }
        set
        {
            Model.TickPlacement = value;
            OnPropertyChanged("TickPlacement");
            if (value == TickPlacement.None)
            {
                _ticks = false;
                OnPropertyChanged("Ticks");
            }
        }
    }

    public bool Ticks
    {
        get => _ticks;
        set
        {
            _ticks = value;
            if (_ticks)
            {
                TickPlacement = TickPlacement.Both;
            }
            else
            {
                TickPlacement = TickPlacement.None;
            }
            OnPropertyChanged("Ticks");
        }
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
