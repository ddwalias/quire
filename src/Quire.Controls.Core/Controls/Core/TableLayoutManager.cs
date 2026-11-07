#nullable disable
using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using Quire.Mirror;

namespace Quire.Controls.Core;

public abstract partial class TableLayoutManager : ICloneable
{
    [MirrorVerified("PtcControlsCoreTableLayoutManagerMechanicalDifferentialTests")]
    private int _columns;

    private double _paddingX;

    private double _paddingY;

    private double _matrixBaseline;

    private List<Rect> _cellArrangements = new List<Rect>();

    private double _baselineToMidPointAdjustment;

    private Size _matrixSize;

    private RowMetric[] _rowMetrics;

    private double[] _columnWidths;

    protected IList<LayoutMetrics> _cellsInfo;

    private Thickness _padding;

    [MirrorVerified("PtcControlsCoreTableLayoutManagerMechanicalDifferentialTests")]
    public int Columns { get => _columns; set => _columns = value; }

    [MirrorVerified("PtcControlsCoreTableLayoutManagerMechanicalDifferentialTests")]
    public double PaddingX { get => _paddingX; set => _paddingX = value; }

    [MirrorVerified("PtcControlsCoreTableLayoutManagerMechanicalDifferentialTests")]
    public double PaddingY { get => _paddingY; set => _paddingY = value; }

    [MirrorVerified("PtcControlsCoreTableLayoutManagerMechanicalDifferentialTests")]
    public Thickness Padding { get => _padding; set => _padding = value; }

    [MirrorVerified("PtcControlsCoreTableLayoutManagerMechanicalDifferentialTests")]
    public double MatrixBaseline => _matrixBaseline;

    [MirrorVerified("PtcControlsCoreTableLayoutManagerMechanicalDifferentialTests")]
    public IList<Rect> CellArrangements => _cellArrangements;

    [MirrorVerified("PtcControlsCoreTableLayoutManagerMechanicalDifferentialTests")]
    public double BaselineToMidPointAdjustment { get => _baselineToMidPointAdjustment; set => _baselineToMidPointAdjustment = value; }

    [MirrorVerified("PtcControlsCoreTableLayoutManagerMechanicalDifferentialTests")]
    public Size MatrixSize => _matrixSize;

    protected TableLayoutManager()
    {
    }

    protected TableLayoutManager(TableLayoutManager other)
    {
        _columns = other._columns;
        _paddingX = other._paddingX;
        _paddingY = other._paddingY;
        _matrixBaseline = other._matrixBaseline;
        _baselineToMidPointAdjustment = other._baselineToMidPointAdjustment;
        _matrixSize = other._matrixSize;
        _rowMetrics = other._rowMetrics;
        _columnWidths = other._columnWidths;
        _cellsInfo = other._cellsInfo;
        _padding = other._padding;
    }

    public Point TopLeftCellPointFromIndex(int cellIndex)
    {
        return CellArrangements[cellIndex].TopLeft;
    }

    public Point BottomRightCellPointFromIndex(int cellIndex)
    {
        Point result = TopLeftCellPointFromIndex(cellIndex);
        result.X += _cellsInfo[cellIndex].OverallSize.Width;
        result.Y += _cellsInfo[cellIndex].OverallSize.Height;
        return result;
    }

    public Point TopRightCellPointFromIndex(int cellIndex)
    {
        Point result = TopLeftCellPointFromIndex(cellIndex);
        result.X += _cellsInfo[cellIndex].OverallSize.Width;
        return result;
    }

    public Point BottomLeftCellPointFromIndex(int cellIndex)
    {
        Point result = TopLeftCellPointFromIndex(cellIndex);
        result.Y += _cellsInfo[cellIndex].OverallSize.Height;
        return result;
    }

    public double TopFromCellIndex(int cellIndex)
    {
        return TopLeftCellPointFromIndex(cellIndex).Y;
    }

    public double LeftFromCellIndex(int cellIndex)
    {
        return TopLeftCellPointFromIndex(cellIndex).X;
    }

    public double RightFromCellIndex(int cellIndex)
    {
        return TopRightCellPointFromIndex(cellIndex).X;
    }

    public double BottomFromCellIndex(int cellIndex)
    {
        return BottomRightCellPointFromIndex(cellIndex).Y;
    }

    public double GetMinLeftFromColumn(int columnIndex, int cellIndexA, int cellIndexB)
    {
        return SearchOnColumn(columnIndex, cellIndexA, cellIndexB, double.MaxValue, MinLeft);
    }

    public double GetMaxRightFromColumn(int columnIndex, int cellIndexA, int cellIndexB)
    {
        return SearchOnColumn(columnIndex, cellIndexA, cellIndexB, 0.0, MaxRight);
    }

    private double SearchOnColumn(int columnIndex, int cellIndexA, int cellIndexB, double currentCandidate, Func<int, int, double, double> nextCandidate)
    {
        int maxRowIndex;
        int minRowIndex;
        // Release IL requires callvirt on newobj without a local slot.
        (new RowColumnHelper(Columns) switch { var h => h }).GetRowIndices(cellIndexA, cellIndexB, out minRowIndex, out maxRowIndex);
        for (int i = minRowIndex; i <= maxRowIndex; i++)
        {
            currentCandidate = nextCandidate(i, columnIndex, currentCandidate);
        }
        return currentCandidate;
    }

    private double MaxRight(int rowIndex, int columnIndex, double maxRight)
    {
        RowColumnHelper rowColumnHelper = new RowColumnHelper(Columns);
        return Math.Max(maxRight, RightFromCellIndex(rowColumnHelper.IndexFromRowColumn(rowIndex, columnIndex)));
    }

    private double MaxBottom(int rowIndex, int columnIndex, double maxBottom)
    {
        RowColumnHelper rowColumnHelper = new RowColumnHelper(Columns);
        return Math.Max(maxBottom, BottomFromCellIndex(rowColumnHelper.IndexFromRowColumn(rowIndex, columnIndex)));
    }

    private double MinLeft(int rowIndex, int columnIndex, double minLeft)
    {
        RowColumnHelper rowColumnHelper = new RowColumnHelper(Columns);
        return Math.Min(minLeft, LeftFromCellIndex(rowColumnHelper.IndexFromRowColumn(rowIndex, columnIndex)));
    }

    private double MinTop(int rowIndex, int columnIndex, double minTop)
    {
        RowColumnHelper rowColumnHelper = new RowColumnHelper(Columns);
        return Math.Min(minTop, TopFromCellIndex(rowColumnHelper.IndexFromRowColumn(rowIndex, columnIndex)));
    }

    public double GetMinTopFromRow(int rowIndex, int cellIndexA, int cellIndexB)
    {
        return SearchOnRow(rowIndex, cellIndexA, cellIndexB, double.MaxValue, MinTop);
    }

    public double GetMaxBottomFromRow(int rowIndex, int cellIndexA, int cellIndexB)
    {
        return SearchOnRow(rowIndex, cellIndexA, cellIndexB, 0.0, MaxBottom);
    }

    private double SearchOnRow(int rowIndex, int cellIndexA, int cellIndexB, double currentCandidate, Func<int, int, double, double> nextCandidate)
    {
        int maxColumnIndex;
        int minColumnIndex;
        // Release IL requires callvirt on newobj without a local slot.
        (new RowColumnHelper(Columns) switch { var h => h }).GetColumnsIndices(cellIndexA, cellIndexB, out minColumnIndex, out maxColumnIndex);
        for (int i = minColumnIndex; i <= maxColumnIndex; i++)
        {
            currentCandidate = nextCandidate(rowIndex, i, currentCandidate);
        }
        return currentCandidate;
    }

    public int CellIndexFromPoint(Point point)
    {
        int num = _rowMetrics.Length - 1;
        int num2 = Columns - 1;
        double num3 = Padding.Left;
        for (int i = 0; i < Columns; i++)
        {
            num3 += _columnWidths[i] + PaddingX / 2.0;
            if (point.X < num3)
            {
                num2 = i;
                break;
            }
            num3 += PaddingX / 2.0;
        }
        double num4 = Padding.Top;
        for (int j = 0; j < _rowMetrics.Length; j++)
        {
            num4 += _rowMetrics[j].RowHeight + PaddingY / 2.0;
            if (point.Y < num4)
            {
                num = j;
                break;
            }
            num4 += PaddingY / 2.0;
        }
        return num * Columns + num2;
    }

    public void Measure(bool isResultMatrix, params LayoutMetrics[] cells)
    {
        Measure(cells, null, isResultMatrix ? ColumnAlignments.DecimalPointAlign : ColumnAlignments.CenterAlign);
    }

    public void Measure(IList<LayoutMetrics> cellsInfo, UIElementCollection cellsContent, ColumnAlignments alignment)
    {
        _cellsInfo = cellsInfo;
        Measure(cellsContent, alignment);
    }

    public void Measure(UIElementCollection cellsContent, ColumnAlignments alignment)
    {
        DebugTools.AssertNotNull(_cellsInfo, null);
        _cellArrangements = new List<Rect>();
        int num = (int)Math.Ceiling((double)_cellsInfo.Count / (double)Columns);
        MaxMatrixWidth[] array = CalculateRowAndColumnMetrics(num, _cellsInfo, cellsContent, alignment == ColumnAlignments.DecimalPointAlign);
        double num2 = Padding.Top;
        int num3 = num / 2;
        for (int i = 0; i < num; i++)
        {
            double num4 = Padding.Left;
            RowMetric rowMetric = _rowMetrics[i];
            double rowBottom = num2 + rowMetric.RowHeight;
            if (i == num3) { }
            rowMetric.RowBottom = rowBottom;
            _rowMetrics[i] = rowMetric;
            for (int j = 0; j < Columns; j++)
            {
                double num5 = _columnWidths[j];
                int num6 = i * Columns + j;
                if (num6 >= _cellsInfo.Count)
                {
                    break;
                }
                double num7 = rowMetric.RowBaseline - _cellsInfo[num6].Baseline;
                double num9;
                if (alignment == ColumnAlignments.DecimalPointAlign && _cellsInfo[num6].ItegralTextWidth != -1.0)
                {
                    double num8 = 0.0;
                    double maxFractional = array[j].MaxFractional;
                    double maxIntegral = array[j].MaxIntegral;
                    double maxVisualNode = array[j].MaxVisualNode;
                    if (maxVisualNode > maxIntegral + maxFractional)
                    {
                        num8 = (maxVisualNode - (maxIntegral + maxFractional)) / 2.0;
                    }
                    num9 = num8 + maxIntegral - _cellsInfo[num6].ItegralTextWidth;
                }
                else if (alignment == ColumnAlignments.LeftAlign || alignment == ColumnAlignments.Stretch)
                {
                    num9 = 0.0;
                }
                else
                {
                    num9 = (num5 - _cellsInfo[num6].OverallSize.Width) / 2.0;
                }
                Point val = new Point(num4 + num9, num2 + num7);
                Size val2 = (alignment == ColumnAlignments.Stretch)
                    ? new Size(num5, _cellsInfo[num6].OverallSize.Height)
                    : _cellsInfo[num6].OverallSize;
                _cellArrangements.Add(new Rect(val, val2));
                num4 += num5 + _paddingX;
            }
            num2 += rowMetric.RowHeight + _paddingY;
        }
        double num10 = ((_columnWidths.Length != 0) ? ((double)(_columnWidths.Length - 1) * PaddingX) : 0.0);
        double num11 = ((_rowMetrics.Length != 0) ? ((double)(_rowMetrics.Length - 1) * PaddingY) : 0.0);
        _matrixSize = new Size(_columnWidths.Sum() + num10 + Padding.Left + Padding.Right, _rowMetrics.Sum((RowMetric row) => row.RowHeight) + num11 + Padding.Top + Padding.Bottom);
        _matrixBaseline = _matrixSize.Height / 2.0 + _baselineToMidPointAdjustment;
    }

    protected abstract MaxMatrixWidth[] CalcMaxDecimalPerColumn(IList<LayoutMetrics> cellsInfo, UIElementCollection cellsContent, int rows);

    private MaxMatrixWidth[] CalculateRowAndColumnMetrics(int rows, IList<LayoutMetrics> cellsInfo, UIElementCollection cellsContent, bool isResultMatrix)
    {
        _rowMetrics = new RowMetric[rows];
        _columnWidths = new double[Columns];
        MaxMatrixWidth[] array = new MaxMatrixWidth[Columns];
        if (isResultMatrix)
        {
            array = CalcMaxDecimalPerColumn(cellsInfo, cellsContent, rows);
        }
        for (int i = 0; i < rows; i++)
        {
            double num = 0.0;
            double num2 = 0.0;
            for (int j = 0; j < Columns; j++)
            {
                int num3 = i * Columns + j;
                if (num3 >= cellsInfo.Count)
                {
                    break;
                }
                LayoutMetrics layoutMetrics = cellsInfo[num3];
                num = Math.Max(num, layoutMetrics.Baseline);
                double val = layoutMetrics.OverallSize.Height - layoutMetrics.Baseline;
                num2 = Math.Max(num2, val);
                if (isResultMatrix && layoutMetrics.ItegralTextWidth != -1.0)
                {
                    double num4 = array[j].MaxIntegral - cellsInfo[num3].ItegralTextWidth;
                    _columnWidths[j] = Math.Max(_columnWidths[j], layoutMetrics.OverallSize.Width + num4);
                }
                else
                {
                    _columnWidths[j] = Math.Max(_columnWidths[j], layoutMetrics.OverallSize.Width);
                }
            }
            double rowHeight = num + num2;
            _rowMetrics[i] = new RowMetric(rowHeight, num);
        }
        return array;
    }

    public double GetRowBottom(int rowIndex)
    {
        return _rowMetrics[rowIndex].RowBottom;
    }

    public double GetRowHeight(int rowIndex)
    {
        return _rowMetrics[rowIndex].RowHeight;
    }

    public double GetColumnWidth(int columnIndex)
    {
        return _columnWidths[columnIndex];
    }

    public abstract object Clone();

    // Declare RowMetric after TableLayoutManager members to preserve <Measure>b__55_0 ordinal.
    [global::Quire.Mirror.MirrorOf("Ptc.Controls.Core.TableLayoutManager+RowMetric, Ptc.Controls.Core")]
    private partial struct RowMetric
    {
        public double RowHeight;
        public double RowBaseline;
        public double RowBottom;

        public RowMetric(double rowHeight, double rowBaseline)
        {
            RowHeight = rowHeight;
            RowBaseline = rowBaseline;
            RowBottom = 0.0;
        }
    }
}
