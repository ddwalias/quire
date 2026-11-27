using System;
using System.Windows;
using System.Windows.Media;
using Quire.Controls.Core;

namespace Quire.Controls.Worksheet;

public partial class DirectRenderingAlignmentGridBorder
{
    private static readonly Type TypeOfThis;

    public static readonly DependencyProperty MajorLinesColorProperty;

    public static readonly DependencyProperty MinorLinesColorProperty;

    public static readonly DependencyProperty MajorLinesThicknessProperty;

    public static readonly DependencyProperty MinorLinesThicknessProperty;

    public static readonly DependencyProperty GridCellSizeProperty;

    public static readonly DependencyProperty MajorGridLinesProperty;

    public static readonly DependencyProperty DisplayGridProperty;

    public static readonly DependencyProperty UseMajorGridLinesProperty;

    public static readonly DependencyProperty UseDirectRenderingProperty;

    public static readonly DependencyProperty UseDashesProperty;

    static DirectRenderingAlignmentGridBorder()
    {
        TypeOfThis = typeof(DirectRenderingAlignmentGridBorder);
        MajorLinesColorProperty = DependencyProperty.Register("MajorLinesColor", typeof(Color), TypeOfThis, new FrameworkPropertyMetadata(PrintGridSettings.MajorLinesColor, RecreateMajorPen));
        MinorLinesColorProperty = DependencyProperty.Register("MinorLinesColor", typeof(Color), TypeOfThis, new FrameworkPropertyMetadata(PrintGridSettings.MinorLinesColor, RecreateMinorPen));
        MajorLinesThicknessProperty = DependencyProperty.Register("MajorLinesThickness", typeof(double), TypeOfThis, new FrameworkPropertyMetadata(PrintGridSettings.MajorLinesThickness, RecreateMajorPen));
        MinorLinesThicknessProperty = DependencyProperty.Register("MinorLinesThickness", typeof(double), TypeOfThis, new FrameworkPropertyMetadata(PrintGridSettings.MinorLinesThickness, RecreateMinorPen));
        MajorGridLinesProperty = DependencyProperty.Register("MajorGridLines", typeof(double), TypeOfThis, new FrameworkPropertyMetadata(double.NaN, HandleGridPropertyChanged));
        DisplayGridProperty = DependencyProperty.Register("DisplayGrid", typeof(bool), TypeOfThis, new FrameworkPropertyMetadata(false, HandleGridPropertyChanged));
        UseMajorGridLinesProperty = DependencyProperty.Register("UseMajorGridLines", typeof(bool), TypeOfThis, new FrameworkPropertyMetadata(false, HandleGridPropertyChanged));
        UseDirectRenderingProperty = DependencyProperty.Register("UseDirectRendering", typeof(bool), TypeOfThis, new FrameworkPropertyMetadata(false, FrameworkPropertyMetadataOptions.AffectsMeasure | FrameworkPropertyMetadataOptions.AffectsRender, HandleUseDirectRenderingChanged));
        UseDashesProperty = DependencyProperty.Register("UseDashes", typeof(bool), TypeOfThis, new FrameworkPropertyMetadata(false, HandleUseDashesChanged));
        GridCellSizeProperty = AlignmentGrid.GridCellSizeProperty.AddOwner(TypeOfThis, new FrameworkPropertyMetadata(AlignmentGrid.GridCellSizeProperty.DefaultMetadata.DefaultValue, HandleGridPropertyChanged));
    }

    public DirectRenderingAlignmentGridBorder()
    {
        CreateMinorPen();
        CreateMajorPen();
    }

    public Color MajorLinesColor
    {
        get
        {
            return (Color)((DependencyObject)this).GetValue(MajorLinesColorProperty);
        }
        set
        {
            ((DependencyObject)this).SetValue(MajorLinesColorProperty, value);
        }
    }

    public Color MinorLinesColor
    {
        get
        {
            return (Color)((DependencyObject)this).GetValue(MinorLinesColorProperty);
        }
        set
        {
            ((DependencyObject)this).SetValue(MinorLinesColorProperty, value);
        }
    }

    public double MajorLinesThickness
    {
        get
        {
            return (double)((DependencyObject)this).GetValue(MajorLinesThicknessProperty);
        }
        set
        {
            ((DependencyObject)this).SetValue(MajorLinesThicknessProperty, value);
        }
    }

    public double MinorLinesThickness
    {
        get
        {
            return (double)((DependencyObject)this).GetValue(MinorLinesThicknessProperty);
        }
        set
        {
            ((DependencyObject)this).SetValue(MinorLinesThicknessProperty, value);
        }
    }

    public Size GridCellSize
    {
        get
        {
            return (Size)((DependencyObject)this).GetValue(GridCellSizeProperty);
        }
        set
        {
            ((DependencyObject)this).SetValue(GridCellSizeProperty, value);
        }
    }

    public double MajorGridLines
    {
        get
        {
            return (double)((DependencyObject)this).GetValue(MajorGridLinesProperty);
        }
        set
        {
            ((DependencyObject)this).SetValue(MajorGridLinesProperty, value);
        }
    }

    public bool DisplayGrid
    {
        get
        {
            return (bool)((DependencyObject)this).GetValue(DisplayGridProperty);
        }
        set
        {
            ((DependencyObject)this).SetValue(DisplayGridProperty, value);
        }
    }

    public bool UseMajorGridLines
    {
        get
        {
            return (bool)((DependencyObject)this).GetValue(UseMajorGridLinesProperty);
        }
        set
        {
            ((DependencyObject)this).SetValue(UseMajorGridLinesProperty, value);
        }
    }

    public bool UseDirectRendering
    {
        get
        {
            return (bool)((DependencyObject)this).GetValue(UseDirectRenderingProperty);
        }
        set
        {
            ((DependencyObject)this).SetValue(UseDirectRenderingProperty, value);
        }
    }

    public bool UseDashes
    {
        get
        {
            return (bool)((DependencyObject)this).GetValue(UseDashesProperty);
        }
        set
        {
            ((DependencyObject)this).SetValue(UseDashesProperty, value);
        }
    }

    private Pen MinorLinePen
    {
        get
        {
            return _minorLinePen;
        }
        set
        {
            if (object.Equals(_minorLinePen, value))
            {
                return;
            }
            _minorLinePen = value;
            InvalidateGrid();
        }
    }

    private Pen MajorLinePen
    {
        get
        {
            return _majorLinePen;
        }
        set
        {
            if (object.Equals(_majorLinePen, value))
            {
                return;
            }
            _majorLinePen = value;
            InvalidateGrid();
        }
    }

    private Pen ActualMajorLinePen
    {
        get
        {
            if (!UseMajorGridLines)
            {
                return MinorLinePen;
            }
            return MajorLinePen;
        }
    }

    private DrawingBrush DrawingBrush { get; set; }

    private static void RecreateMajorPen(DependencyObject d, DependencyPropertyChangedEventArgs e)
    {
        ((DirectRenderingAlignmentGridBorder)d).CreateMajorPen();
    }

    private static void RecreateMinorPen(DependencyObject d, DependencyPropertyChangedEventArgs e)
    {
        ((DirectRenderingAlignmentGridBorder)d).CreateMinorPen();
    }

    private static void HandleGridPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
    {
        ((DirectRenderingAlignmentGridBorder)d).InvalidateGrid();
    }

    private static void HandleUseDirectRenderingChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
    {
        ((DirectRenderingAlignmentGridBorder)d).DrawingBrush = null;
    }

    private static void HandleUseDashesChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
    {
        RecreateMinorPen(d, e);
        RecreateMajorPen(d, e);
    }

    private void CreateMinorPen()
    {
        MinorLinePen = CreatePen(MinorLinesColor, UseDashes, MinorLinesThickness);
    }

    private void CreateMajorPen()
    {
        MajorLinePen = CreatePen(MajorLinesColor, UseDashes, MajorLinesThickness);
    }

    private void InvalidateGrid()
    {
        DrawingBrush = null;
        if (UseDirectRendering)
        {
            InvalidateVisual();
        }
        else
        {
            InvalidateMeasure();
        }
    }

    private static Pen CreatePen(Color color, bool drawDashedLines, double thickness)
    {
        SolidColorBrush solidColorBrush = new SolidColorBrush(color);
        ((Freezable)solidColorBrush).Freeze();
        Pen pen = new Pen(solidColorBrush, thickness)
        {
            DashStyle = (drawDashedLines ? DashStyles.Dash : DashStyles.Solid)
        };
        ((Freezable)pen).Freeze();
        return pen;
    }

    private static void DirectDcDrawing(DrawingContext drawingContext, double majorGridLines, Size gridCellSize, Size borderSize, Pen majorLinePen, Pen minorLinePen)
    {
        for (double num = 0.0; num <= borderSize.Width; num += majorGridLines)
        {
            double value = Math.Min(num + majorGridLines, borderSize.Width);
            for (double minorLineX = num + gridCellSize.Width; minorLineX.IsLessThan(value); minorLineX += gridCellSize.Width)
            {
                drawingContext.DrawLine(minorLinePen, new Point(minorLineX, 0.0), new Point(minorLineX, borderSize.Height));
            }
        }
        for (double majorLineY = 0.0; majorLineY <= borderSize.Height; majorLineY += majorGridLines)
        {
            double nextMajorLineY = Math.Min(majorLineY + majorGridLines, borderSize.Height);
            for (double minorLineY = majorLineY + gridCellSize.Height; minorLineY.IsLessThan(nextMajorLineY); minorLineY += gridCellSize.Height)
            {
                drawingContext.DrawLine(minorLinePen, new Point(0.0, minorLineY), new Point(borderSize.Width, minorLineY));
            }
        }
        for (double majorLineX = majorGridLines; majorLineX.IsLessThan(borderSize.Width); majorLineX += majorGridLines)
        {
            drawingContext.DrawLine(majorLinePen, new Point(majorLineX, 0.0), new Point(majorLineX, borderSize.Height));
        }
        for (double majorLineY = majorGridLines; majorLineY.IsLessThan(borderSize.Height); majorLineY += majorGridLines)
        {
            drawingContext.DrawLine(majorLinePen, new Point(0.0, majorLineY), new Point(borderSize.Width, majorLineY));
        }
    }

    protected override Size MeasureOverride(Size constraint)
    {
        if (!UseDirectRendering && DisplayGrid)
        {
            if (DrawingBrush == null)
            {
                DrawingBrush = new DrawingBrush
                {
                    TileMode = TileMode.Tile,
                    ViewportUnits = BrushMappingMode.Absolute,
                    Viewport = new Rect(new Size(MajorGridLines, MajorGridLines)),
                    Stretch = Stretch.None,
                    Drawing = BuildTileDrawingGeometry(MinorLinePen, ActualMajorLinePen)
                };
            }
        }
        else if (DebugTools.DevAssert(DrawingBrush == null, null).Failed)
        {
            DrawingBrush = null;
        }
        Background = DrawingBrush;
        return base.MeasureOverride(constraint);
    }

    protected override void OnRender(DrawingContext drawingContext)
    {
        if (UseDirectRendering && DisplayGrid)
        {
            Size renderSize = RenderSize;
            if (!renderSize.IsEmpty && DebugTools.DevAssert(MajorGridLines.IsValid() && MajorGridLines > 0.0, null).Passed)
            {
                DirectDcDrawing(drawingContext, MajorGridLines, GridCellSize, RenderSize, ActualMajorLinePen, MinorLinePen);
            }
        }
        base.OnRender(drawingContext);
    }
}
