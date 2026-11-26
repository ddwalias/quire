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
    private double _savedLength;

    public PlotGraphLinearAxis(global::Quire.Controls.Plots.EquationList equationList, global::Quire.Controls.Plots.PlotTitle axisTitle = default) : base(default(global::Quire.Controls.Plots.EquationList), default(int), default(double)) { }

    static PlotGraphLinearAxis()
    {
        _typeOfThis = typeof(PlotGraphLinearAxis);
        PositionProperty = DependencyProperty.Register("Position", typeof(Positions), _typeOfThis, new FrameworkPropertyMetadata(Positions.Origin, FrameworkPropertyMetadataOptions.AffectsArrange | FrameworkPropertyMetadataOptions.AffectsRender, PositionPropertyChanged));
    }

    private int ColOrRowIndex1 { get; set; }

    private int ColOrRowIndex2 { get; set; }

    private global::System.Windows.Controls.GridSplitter _splitter;

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

    private void InitializeColOrRowIndexProperties() => throw global::Quire.Mirror.MirrorStub.NotImplemented();

    private void Splitter_DragCompleted(object sender, global::System.Windows.Controls.Primitives.DragCompletedEventArgs e) => throw global::Quire.Mirror.MirrorStub.NotImplemented();

    internal void MoveRowOrCol(int tickNumber) => throw global::Quire.Mirror.MirrorStub.NotImplemented();

    private double GetGridSplitPosition(int index) => throw global::Quire.Mirror.MirrorStub.NotImplemented();

    private static void AdjustSnap(ref double snapOffset, ref double index2SnapOffset, double totalWidthOrHeight) => throw global::Quire.Mirror.MirrorStub.NotImplemented();

    private void Splitter_DragDelta(object sender, global::System.Windows.Controls.Primitives.DragDeltaEventArgs e) => throw global::Quire.Mirror.MirrorStub.NotImplemented();

    private void UpdateOffset() => throw global::Quire.Mirror.MirrorStub.NotImplemented();

    private void PlotGraphAxis_SizeChanged(object sender, global::System.Windows.SizeChangedEventArgs e) => throw global::Quire.Mirror.MirrorStub.NotImplemented();

    protected override bool ShouldArrangeConvertedEquationsHorizontally()
    {
        return Orientation == Orientation.Horizontal;
    }

    private bool IsEffectAxis => TickmarksPattern != null;

    private PlotGraphLinearAxis OtherLinearAxis => OtherAxis as PlotGraphLinearAxis;

    protected override void SetOtherAxis(PlotGraphAxis value)
    {
        if (OtherLinearAxis != null)
        {
            OtherLinearAxis.CalcPositionTickMarkNeeded -= CalcPositionTickMark;
            SizeChanged -= PlotGraphAxis_SizeChanged;
        }
        PlotGraphLinearAxis axis = value as PlotGraphLinearAxis;
        if (axis != null)
        {
            axis.CalcPositionTickMarkNeeded += CalcPositionTickMark;
            SizeChanged += PlotGraphAxis_SizeChanged;
        }
        base.SetOtherAxis(value);
    }

    public event Action<Positions> CalcPositionTickMarkNeeded;

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

    protected override void PositionTickMarkChanged(int tickNumber) => throw global::Quire.Mirror.MirrorStub.NotImplemented();

    private void CalcNewPosition(int tickNumber) => throw global::Quire.Mirror.MirrorStub.NotImplemented();

    private static void PositionPropertyChanged(global::System.Windows.DependencyObject d, global::System.Windows.DependencyPropertyChangedEventArgs e) => throw global::Quire.Mirror.MirrorStub.NotImplemented();

    public static readonly global::System.Windows.DependencyProperty PositionProperty;

    public override Positions Position
    {
        get => (Positions)GetValue(PositionProperty);
        set => Dispatch(() => SetValue(PositionProperty, value));
    }

    private void CalcPositionTickMark(global::Quire.Controls.Plots.Presenter.Positions newPosition) => throw global::Quire.Mirror.MirrorStub.NotImplemented();

    private void SetupAxisArrowHeadsDisplay() => throw global::Quire.Mirror.MirrorStub.NotImplemented();

    private void CalcOriginPositioning() => throw global::Quire.Mirror.MirrorStub.NotImplemented();

    private void FireCalcPositionTickMarkNeeded(global::Quire.Controls.Plots.Presenter.Positions newPosition) => throw global::Quire.Mirror.MirrorStub.NotImplemented();

    protected override void HandleDiscreteChanged()
    {
        ScaleEdgeEvenly = IsDiscrete;
        CalcOriginPositioning();
    }

    public override void UpdateAxisOriginPositions() => throw global::Quire.Mirror.MirrorStub.NotImplemented();

    protected override void TickMarksChanged()
    {
        if (OtherLinearAxis != null && OtherLinearAxis.PositionTickMark > TickMarks)
        {
            OtherLinearAxis.Position = OtherLinearAxis.Orientation == Orientation.Vertical ? Positions.Right : Positions.Top;
        }
        CalcOriginPositioning();
    }

    private MouseMoveRTDataAdorner MouseMoveAdorner { get; set; }

    private global::System.Windows.Media.LineGeometry GetTickLineByTickNumber(int totalTickNumber) => throw global::Quire.Mirror.MirrorStub.NotImplemented();

    private bool HasZeroInRange() => throw global::Quire.Mirror.MirrorStub.NotImplemented();

    private bool HasZeroInRange(ref int index) => throw global::Quire.Mirror.MirrorStub.NotImplemented();

    public override bool FindClosestTickValueToZero(ref int index) => throw global::Quire.Mirror.MirrorStub.NotImplemented();

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

    protected override void MouseMoveDraw(global::System.Windows.Media.DrawingContext drawingContext) => throw global::Quire.Mirror.MirrorStub.NotImplemented();

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

    protected override void OnRender(DrawingContext drawingContext)
    {
        base.OnRender(drawingContext);
        if (MouseMoveAdorner != null)
        {
            MouseMoveAdorner.InvalidateVisual();
        }
    }

    protected override global::System.Windows.Media.HitTestResult HitTestCore(global::System.Windows.Media.PointHitTestParameters hitTestParameters) => base.HitTestCore(hitTestParameters);

    public void DrawTickLineAndValue(int tickNumber, global::System.Windows.Media.DrawingContext drawingContext, global::System.Windows.Media.Brush fill, global::System.Windows.Media.Brush stroke, double strokeThickness) => throw global::Quire.Mirror.MirrorStub.NotImplemented();

    public override void MoveToValue(double value) => throw global::Quire.Mirror.MirrorStub.NotImplemented();

    public override void MoveByValue(double value) => throw global::Quire.Mirror.MirrorStub.NotImplemented();

    protected override void ApplyTickNumber(int tickNumber, bool hasOriginPositioning) => throw global::Quire.Mirror.MirrorStub.NotImplemented();

    private int OffsetToTickNumber(double offset) => throw global::Quire.Mirror.MirrorStub.NotImplemented();

    protected void AddAdorner() => throw global::Quire.Mirror.MirrorStub.NotImplemented();

    private void RemoveAdorner() => throw global::Quire.Mirror.MirrorStub.NotImplemented();

    internal void FixateSize(double savedLength) => throw global::Quire.Mirror.MirrorStub.NotImplemented();

    internal bool SizeFrozen { get; private set; }

    private void FixateSizeOfRowsInner() => throw global::Quire.Mirror.MirrorStub.NotImplemented();

    public override double GetPerpendicularAxisPositionRatio()
    {
        if (OtherAxis == null)
        {
            return base.GetPerpendicularAxisPositionRatio();
        }
        return GetTickRelativePosition(OtherAxis.PositionTickMark);
    }

    internal bool ReleaseAutoReSize() => throw global::Quire.Mirror.MirrorStub.NotImplemented();

    private static readonly global::System.Type _typeOfThis;

    public PlotGraphLinearAxis()
        : this(null)
    {
    }
}
