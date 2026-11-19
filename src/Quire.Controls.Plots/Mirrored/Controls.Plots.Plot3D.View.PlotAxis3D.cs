#nullable disable
#pragma warning disable

using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Media.Media3D;
using Quire.Controls.Plots.Base3D.ViewModel;

namespace Quire.Controls.Plots.Plot3D.View;

internal partial class PlotAxis3D
{
    public PlotLabelsContainer3D LabelsContainer { get; set; }

    public Color LabelsColor
    {
        get
        {
            return (Color)GetValue(LabelsColorProperty);
        }
        set
        {
            SetValue(LabelsColorProperty, value);
        }
    }

    public bool ShowTicks
    {
        get
        {
            return (bool)GetValue(ShowTicksProperty);
        }
        set
        {
            SetValue(ShowTicksProperty, value);
        }
    }

    public bool ShowNumbers
    {
        get
        {
            return (bool)GetValue(ShowNumbersProperty);
        }
        set
        {
            SetValue(ShowNumbersProperty, value);
        }
    }

    private GeometryModel3D BaseModel
    {
        get
        {
            GeometryModel3D obj = Content as GeometryModel3D;
            DebugTools.DevAssert(obj != null, null);
            return obj;
        }
    }
}
