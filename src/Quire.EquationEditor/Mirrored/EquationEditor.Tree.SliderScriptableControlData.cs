using System.Windows.Controls;
using System.Windows.Controls.Primitives;

namespace Quire.EquationEditor.Tree;

public partial class SliderScriptableControlData
{
    private double c;
    private double d = 100.0;
    private bool n;
    private bool o;
    private double p = 1.0;
    private double q = 1.0;
    private double r = 10.0;
    private bool s = true;
    private int t;
    private bool u = true;
    private TickPlacement v;
    private Orientation w;

    [global::System.Runtime.CompilerServices.SkipLocalsInit]
    public SliderScriptableControlData(string mathNodeName, Quire.Controls.Core.InputControlTypes inputControlTypes)
        : base(mathNodeName, inputControlTypes)
    {
        Height = a;
        Width = b;
    }

    public double Minimum
    {
        get => c;
        set => c = value;
    }

    public double Maximum
    {
        get => d;
        set => d = value;
    }

    public bool ShowMinMax
    {
        get => n;
        set => n = value;
    }

    public bool Invert
    {
        get => o;
        set => o = value;
    }

    public double TickFrequency
    {
        get => p;
        set => p = value;
    }

    public double SmallChange
    {
        get => q;
        set => q = value;
    }

    public double LargeChange
    {
        get => r;
        set => r = value;
    }

    public bool IsSnapToTickEnabled
    {
        get => s;
        set => s = value;
    }

    public int Interval
    {
        get => t;
        set => t = value;
    }

    public bool Border
    {
        get => u;
        set => u = value;
    }

    public TickPlacement TickPlacement
    {
        get => v;
        set => v = value;
    }

    public Orientation Orientation
    {
        get => w;
        set => w = value;
    }

    [global::System.Runtime.CompilerServices.SkipLocalsInit]
    public override object Clone()
    {
        return new SliderScriptableControlData(base.MathNodeName, base.InputControlType)
        {
            SelectedValueIndex = base.SelectedValueIndex,
            Script = base.Script,
            EngineType = base.EngineType,
            Width = base.Width,
            Height = base.Height,
            IsInputsExists = base.IsInputsExists,
            IsInputsAreaShown = base.IsInputsAreaShown,
            InputFieldSExpression = base.InputFieldSExpression,
            InputsResultValue = base.InputsResultValue,
            Orientation = Orientation,
            Values = base.Values,
            EngineError = base.EngineError,
            SelectedCalculatedValue = base.SelectedCalculatedValue,
            TickPlacement = TickPlacement,
            Border = Border,
            Interval = Interval,
            LargeChange = LargeChange,
            SmallChange = SmallChange,
            TickFrequency = TickFrequency,
            Invert = Invert,
            ShowMinMax = ShowMinMax,
            Maximum = Maximum,
            Minimum = Minimum
        };
    }
}
