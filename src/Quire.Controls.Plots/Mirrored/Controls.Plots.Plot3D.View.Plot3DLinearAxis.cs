#nullable disable
#pragma warning disable

using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using Quire.Controls.Plots.Presenter;

namespace Quire.Controls.Plots.Plot3D.View;

public partial class Plot3DLinearAxis : PlotGraphLinearAxisBase
{
    private static readonly Type _typeOfThis;
    private const int DefaultTickMarks = 5;

    static Plot3DLinearAxis()
    {
        _typeOfThis = typeof(Plot3DLinearAxis);
        PlotGraphAxisBase.TickMarksProperty.OverrideMetadata(_typeOfThis, PlotGraphAxisBase.CreateTickmarksPropertyMetadata(5));
        PlotGraphAxis.OrientationProperty.OverrideMetadata(_typeOfThis, PlotGraphAxis.CreateOrientationPropertyMetadata(Orientation.Vertical));
    }

    private Plot3DLinearAxis(EquationList equationList, int defaultTickMarks = 5, double defaultTickMagnitudeThreshold = 2.0)
        : base(equationList, defaultTickMarks, defaultTickMagnitudeThreshold)
    {
    }

    public Plot3DLinearAxis()
        : this(null)
    {
    }

    protected override void PositionTickMarkChanged(int number)
    {
    }

    protected override void MouseMoveDraw(DrawingContext context)
    {
        throw new NotImplementedException();
    }

    public override Positions Position { get; set; }

    public override void UpdateAxisOriginPositions()
    {
        throw new NotImplementedException();
    }

    public override void MoveToValue(double value)
    {
        throw new NotImplementedException();
    }

    public override void MoveByValue(double value)
    {
        throw new NotImplementedException();
    }

    public override bool FindClosestTickValueToZero(ref int index)
    {
        throw new NotImplementedException();
    }

    protected override void ApplyTickNumber(int value, bool b)
    {
        throw new NotImplementedException();
    }

    protected override void TickMarksChanged()
    {
    }
}
