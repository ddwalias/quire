#nullable disable
using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using Quire.Mirror;

namespace Quire.Controls.Core;

public partial class VisualTableContents
{
    [MirrorVerified("PtcControlsCoreVisualTableContentsMechanicalDifferentialTests")]
    protected readonly TableLayoutManager _layoutManager;

    [MirrorVerified("PtcControlsCoreVisualTableContentsMechanicalDifferentialTests")]
    protected VisualTableContents(TableLayoutManager layoutManager)
    {
        _layoutManager = layoutManager;
        foreach (IBaselinedVisual child in Children)
        {
            child.MeasureOverideCompleted += InvalidateMeasure;
        }
    }

    [MirrorVerified("PtcControlsCoreVisualTableContentsMechanicalDifferentialTests")]
    protected override void OnVisualChildrenChanged(DependencyObject visualAdded, DependencyObject visualRemoved)
    {
        base.OnVisualChildrenChanged(visualAdded, visualRemoved);
        if (visualAdded is IBaselinedVisual baselinedVisual)
        {
            baselinedVisual.MeasureOverideCompleted += InvalidateMeasure;
        }
        if (visualRemoved is IBaselinedVisual baselinedVisual2)
        {
            baselinedVisual2.MeasureOverideCompleted -= InvalidateMeasure;
        }
    }

    [MirrorVerified("PtcControlsCoreVisualTableContentsMechanicalDifferentialTests")]
    public virtual int Rows
    {
        get
        {
            if (Columns != 0)
            {
                return CellCount / Columns;
            }
            return 0;
        }
    }

    [MirrorVerified("PtcControlsCoreVisualTableContentsMechanicalDifferentialTests")]
    public int Columns { get => _layoutManager.Columns; set => _layoutManager.Columns = value; }

    [MirrorVerified("PtcControlsCoreVisualTableContentsMechanicalDifferentialTests")]
    public ColumnAlignments ColumnAlignment { get; set; }

    [MirrorVerified("PtcControlsCoreVisualTableContentsMechanicalDifferentialTests")]
    public double ColumnSpacing { get => _layoutManager.PaddingX; set => _layoutManager.PaddingX = value; }

    [MirrorVerified("PtcControlsCoreVisualTableContentsMechanicalDifferentialTests")]
    public double RowSpacing { get => _layoutManager.PaddingY; set => _layoutManager.PaddingY = value; }

    [MirrorVerified("PtcControlsCoreVisualTableContentsMechanicalDifferentialTests")]
    public Thickness Padding { get => _layoutManager.Padding; set => _layoutManager.Padding = value; }

    [MirrorVerified("PtcControlsCoreVisualTableContentsMechanicalDifferentialTests")]
    public double BaselineToMidPointAdjustment { get => _layoutManager.BaselineToMidPointAdjustment; set => _layoutManager.BaselineToMidPointAdjustment = value; }

    [MirrorVerified("PtcControlsCoreVisualTableContentsMechanicalDifferentialTests")]
    public double MatrixBaseline => _layoutManager.MatrixBaseline;

    [MirrorVerified("PtcControlsCoreVisualTableContentsMechanicalDifferentialTests")]
    public double GetRowHeight(int row) => _layoutManager.GetRowHeight(row);

    [MirrorVerified("PtcControlsCoreVisualTableContentsMechanicalDifferentialTests")]
    public double GetColumnWidth(int column) => _layoutManager.GetColumnWidth(column);

    [MirrorVerified("PtcControlsCoreVisualTableContentsMechanicalDifferentialTests")]
    protected override Size MeasureOverride(Size availableSize)
    {
        List<LayoutMetrics> list = new List<LayoutMetrics>(Children.Count);
        foreach (UIElement item in GetChildrenInRowMajorOrder())
        {
            item.Measure(new Size(double.PositiveInfinity, double.PositiveInfinity));
            IBaselinedVisual baselinedVisual = item as IBaselinedVisual;
            double baseline = baselinedVisual != null ? baselinedVisual.TextBaselineY : 0.0;
            list.Add(new LayoutMetrics(item.DesiredSize, baseline));
        }
        _layoutManager.Measure(list, Children, ColumnAlignment);
        return _layoutManager.MatrixSize;
    }

    [MirrorVerified("PtcControlsCoreVisualTableContentsMechanicalDifferentialTests")]
    public virtual IEnumerable<UIElement> GetChildrenInRowMajorOrder()
    {
        foreach (UIElement child in Children)
        {
            yield return child;
        }
    }

    [MirrorVerified("PtcControlsCoreVisualTableContentsMechanicalDifferentialTests")]
    public virtual int CellCount => Children.Count;

    [MirrorVerified("PtcControlsCoreVisualTableContentsMechanicalDifferentialTests")]
    protected override Size ArrangeOverride(Size finalSize)
    {
        List<UIElement> list = new List<UIElement>(GetChildrenInRowMajorOrder());
        for (int i = 0; i < list.Count; i++)
        {
            Point location = _layoutManager.CellArrangements[i].Location;
            list[i].Arrange(new Rect(location, _layoutManager.CellArrangements[i].Size));
        }
        return _layoutManager.MatrixSize;
    }
}
