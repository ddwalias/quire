#nullable disable
#pragma warning disable

using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Documents;
using System.Windows.Media;
using Quire.Controls.Plots.Model;
using Quire.Controls.Plots.Presenter;

namespace Quire.Controls.Plots;

internal partial class PlotGraphLinearAxis
{
    static PlotGraphLinearAxis()
    {
        _typeOfThis = typeof(PlotGraphLinearAxis);
        PositionProperty = DependencyProperty.Register("Position", typeof(Positions), _typeOfThis, new FrameworkPropertyMetadata(Positions.Origin, FrameworkPropertyMetadataOptions.AffectsArrange | FrameworkPropertyMetadataOptions.AffectsRender, PositionPropertyChanged));
    }

    private int ColOrRowIndex1 { get; set; }

    private int ColOrRowIndex2 { get; set; }

    private bool IsEffectAxis => TickmarksPattern != null;

    private PlotGraphLinearAxis OtherLinearAxis => OtherAxis as PlotGraphLinearAxis;

    private MouseMoveRTDataAdorner MouseMoveAdorner { get; set; }

    public override int PositionValue
    {
        get
        {
            int positionValue = base.PositionValue;
            DebugTools.DevAssert(positionValue == OtherLinearAxis.OffsetToTickNumber(GetGridSplitPosition(ColOrRowIndex1)), null);
            return positionValue;
        }
        set
        {
            base.PositionValue = value;
        }
    }

    internal bool SizeFrozen { get; private set; }

    public event Action<Positions> CalcPositionTickMarkNeeded;

    public PlotGraphLinearAxis()
        : this(null)
    {
    }
}
