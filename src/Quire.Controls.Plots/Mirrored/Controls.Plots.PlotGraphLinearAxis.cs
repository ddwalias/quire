#nullable disable
#pragma warning disable

using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Documents;
using System.Windows.Input;
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

    public override GridSplitter Splitter
    {
        get
        {
            return _splitter;
        }
        set
        {
            if (_splitter == value)
            {
                return;
            }
            if (_splitter != null)
            {
                _splitter.DragCompleted -= Splitter_DragCompleted;
                _splitter.DragDelta -= Splitter_DragDelta;
            }
            _splitter = value;
            if (value != null)
            {
                _splitter.DragCompleted += Splitter_DragCompleted;
                _splitter.DragDelta += Splitter_DragDelta;
                InitializeColOrRowIndexProperties();
                FixateSizeOfRowsInner();
            }
        }
    }

    protected override bool ShouldArrangeConvertedEquationsHorizontally()
    {
        return Orientation == Orientation.Horizontal;
    }

    protected override void HandleStartChanged()
    {
        base.HandleStartChanged();
        CalcOriginPositioning();
    }

    protected override void HandleEndChanged()
    {
        base.HandleEndChanged();
        CalcOriginPositioning();
    }

    protected override void HandleDiscreteChanged()
    {
        ScaleEdgeEvenly = IsDiscrete;
        CalcOriginPositioning();
    }

    protected override void TickMarksChanged()
    {
        if (OtherLinearAxis != null && OtherLinearAxis.PositionTickMark > TickMarks)
        {
            OtherLinearAxis.Position = OtherLinearAxis.Orientation == Orientation.Vertical ? Positions.Right : Positions.Top;
        }
        CalcOriginPositioning();
    }

    protected override void OnMouseEnter(MouseEventArgs e)
    {
        base.OnMouseEnter(e);
        if (OtherAxis != null)
        {
            e.Handled = true;
            AddAdorner();
        }
    }

    protected override void OnMouseLeave(MouseEventArgs e)
    {
        base.OnMouseLeave(e);
        RemoveAdorner();
    }

    public override double GetPerpendicularAxisPositionRatio()
    {
        if (OtherAxis == null)
        {
            return base.GetPerpendicularAxisPositionRatio();
        }
        return GetTickRelativePosition(OtherAxis.PositionTickMark);
    }
}
