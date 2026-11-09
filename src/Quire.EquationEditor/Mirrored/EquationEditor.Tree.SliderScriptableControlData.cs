using System.Windows.Controls;
using System.Windows.Controls.Primitives;

namespace Quire.EquationEditor.Tree;

public partial class SliderScriptableControlData
{
    private double c;
    private double d;
    private bool n;
    private bool o;
    private double p;
    private double q;
    private double r;
    private bool s;
    private int t;
    private bool u;
    private TickPlacement v;
    private Orientation w;

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
}
