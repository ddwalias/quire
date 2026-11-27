using System;
using System.ComponentModel;
using System.Globalization;
using System.Windows;
using System.Windows.Media;
using System.Windows.Shapes;
using Quire.Controls.Core.AttachedBehavior;
using Quire.Controls.Plots.Presenter;
using Quire.Controls.Plots.ViewModel;
using Quire.Core;

namespace Quire.Controls.Plots.Contour.View;

public partial class VisualData : DependencyObject, IOneTimePropertyOwner
{
    private static readonly Type TypeOfThis;

    private static readonly DependencyProperty GeometryDataProperty;

    public static readonly DependencyProperty StrokeProperty;

    public static readonly DependencyProperty StrokeThicknessProperty;

    public static readonly DependencyProperty DashStyleProperty;

    public static readonly DependencyProperty FillProperty;

    public static readonly DependencyProperty LabelProperty;

    private double _labelValue;

    public static readonly DependencyProperty LabelPositionProperty;

    public static readonly DependencyProperty LabelFormatterProperty;

    public static readonly DependencyProperty LabelVisibilityProperty;

    public PathGeometry GeometryData
    {
        get => (PathGeometry)GetValue(GeometryDataProperty);
        set => SetValue(GeometryDataProperty, value);
    }

    static VisualData()
    {
        TypeOfThis = typeof(VisualData);
        GeometryDataProperty = DependencyProperty.Register("GeometryData", typeof(PathGeometry), TypeOfThis);
        LabelProperty = DependencyProperty.Register("Label", typeof(string), TypeOfThis);
        LabelPositionProperty = DependencyProperty.Register("LabelPosition", typeof(Point), TypeOfThis);
        LabelFormatterProperty = DependencyProperty.Register("LabelFormatter", typeof(TickmarkFormatter), TypeOfThis, new PropertyMetadata(new PropertyChangedCallback(LabelFormatterChanged)));
        LabelVisibilityProperty = DependencyProperty.Register("LabelVisibility", typeof(Visibility), TypeOfThis);
        StrokeProperty = Shape.StrokeProperty.AddOwner(TypeOfThis);
        StrokeThicknessProperty = Shape.StrokeThicknessProperty.AddOwner(TypeOfThis);
        DashStyleProperty = Pen.DashStyleProperty.AddOwner(TypeOfThis);
        FillProperty = Shape.FillProperty.AddOwner(TypeOfThis);
    }

    public Brush Stroke
    {
        get => (Brush)GetValue(StrokeProperty);
        set => SetValue(StrokeProperty, value);
    }

    [TypeConverter(typeof(LengthConverter))]
    public double StrokeThickness
    {
        get => (double)GetValue(StrokeThicknessProperty);
        set => SetValue(StrokeThicknessProperty, value);
    }

    public DashStyle DashStyle
    {
        get => (DashStyle)GetValue(DashStyleProperty);
        set => SetValue(DashStyleProperty, value);
    }

    public Brush Fill
    {
        get => (Brush)GetValue(FillProperty);
        set => SetValue(FillProperty, value);
    }

    public string Label
    {
        get => (string)GetValue(LabelProperty);
        set => SetValue(LabelProperty, value);
    }

    private void UpdateLabel()
    {
        if (LabelFormatter != null)
        {
            Label = LabelFormatter.FormatReal(_labelValue);
        }
    }

    public Point LabelPosition
    {
        get => (Point)GetValue(LabelPositionProperty);
        set => SetValue(LabelPositionProperty, value);
    }

    public double LabelValue
    {
        get => _labelValue;
        set
        {
            _labelValue = value;
            UpdateLabel();
        }
    }

    private static void LabelFormatterChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
    {
        VisualData visualData = d as VisualData;
        DebugTools.DevAssert(visualData != null, null);
        visualData.UpdateLabel();
    }

    public TickmarkFormatter LabelFormatter
    {
        get => (TickmarkFormatter)GetValue(LabelFormatterProperty);
        set => SetValue(LabelFormatterProperty, value);
    }

    public Visibility LabelVisibility
    {
        get => (Visibility)GetValue(LabelVisibilityProperty);
        set => SetValue(LabelVisibilityProperty, value);
    }

    private IValueToScreen VerticalValueToScreen { get; set; }

    private IValueToScreen HorizontalValueToScreen { get; set; }

    public VisualData(Point[] points, IValueToScreen horizontalTransformer, IValueToScreen verticalTransformer)
    {
        DebugTools.DevAssert(points != null, null);
        PointsData = points;
        VerticalValueToScreen = verticalTransformer;
        HorizontalValueToScreen = horizontalTransformer;
        CreateGeometry();
    }

    public Point[] PointsData { get; private set; }

    internal void CreateGeometry()
    {
        GeometryData = new PathGeometry();
        if (PointsData.Length != 0)
        {
            PathSegmentCollection pathSegmentCollection = new PathSegmentCollection();
            Borders borders = ComputeBorderBelongingStatusForPoint(PointsData[0]);
            Borders border = borders;
            for (int i = 1; i < PointsData.Length; i++)
            {
                Borders nextPointBorders = ComputeBorderBelongingStatusForPoint(PointsData[i]);
                LineSegment value = new LineSegment
                {
                    Point = TransformPoint(PointsData[i]),
                    IsStroked = !AreOnSameBorder(border, nextPointBorders)
                };
                pathSegmentCollection.Add(value);
                border = nextPointBorders;
            }
            PathFigure pathFigure = new PathFigure(TransformPoint(PointsData[0]), pathSegmentCollection, !AreOnSameBorder(border, borders));
            PathFigureCollection pathFigureCollection = new PathFigureCollection(1);
            pathFigureCollection.Add(pathFigure);
            GeometryData.Figures = pathFigureCollection;
        }
    }

    private Borders ComputeBorderBelongingStatusForPoint(Point point)
    {
        Borders borders = Borders.None;
        EdgeBelongingStatus edgeBelongingStatus = HorizontalValueToScreen.GetEdgeBelongingStatusForCoordinate(point.X);
        if (edgeBelongingStatus == EdgeBelongingStatus.Start)
        {
            borders |= Borders.Left;
        }
        else if (edgeBelongingStatus == EdgeBelongingStatus.End)
        {
            borders |= Borders.Right;
        }
        edgeBelongingStatus = VerticalValueToScreen.GetEdgeBelongingStatusForCoordinate(point.Y);
        if (edgeBelongingStatus == EdgeBelongingStatus.Start)
        {
            borders |= Borders.Bottom;
        }
        else if (edgeBelongingStatus == EdgeBelongingStatus.End)
        {
            borders |= Borders.Top;
        }
        return borders;
    }

    private static bool AreOnSameBorder(Borders border1, Borders border2)
    {
        return (border1 & border2) != 0;
    }

    private Point TransformPoint(Point point)
    {
        if (HorizontalValueToScreen == null || VerticalValueToScreen == null)
        {
            DebugTools.DevAssert(false, null);
            return new Point(0.0, 0.0);
        }
        return new Point(HorizontalValueToScreen.ValueToScreen(point.X), VerticalValueToScreen.ValueToScreen(point.Y));
    }

    internal Size GetLabelSize()
    {
        Typeface typeface = new Typeface("Mathcad UniMath Prime");
        FormattedText formattedText = new FormattedText(Label ?? string.Empty, CultureInfo.CurrentCulture, FlowDirection.LeftToRight, typeface, 8.0, Brushes.Black);
        return new Size(formattedText.Width, formattedText.Height);
    }
}
