#nullable disable
using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Media;
using System.Windows.Shapes;
using Quire.Mirror;

namespace Quire.Controls.Core;

internal partial class BorderShape : Shape
{
    private Rect _rect;

    public static readonly DependencyProperty RightEdgeVisibilityProperty = DependencyProperty.Register("RightEdgeVisibility", typeof(bool), typeof(BorderShape), new FrameworkPropertyMetadata(true, FrameworkPropertyMetadataOptions.AffectsRender));

    public static readonly DependencyProperty CornerRadiusProperty = DependencyProperty.Register("CornerRadius", typeof(CornerRadius), typeof(BorderShape), new FrameworkPropertyMetadata(new CornerRadius(0.0), FrameworkPropertyMetadataOptions.AffectsRender));

    [MirrorVerified("PtcControlsCoreBorderShapeMechanicalDifferentialTests")]
    public BorderShape() { }

    [MirrorVerified("PtcControlsCoreBorderShapeMechanicalDifferentialTests")]
    protected override Geometry DefiningGeometry => GetBorderGeometry();

    public bool RightEdgeVisibility
    {
        get => (bool)GetValue(RightEdgeVisibilityProperty);
        set => SetValue(RightEdgeVisibilityProperty, value);
    }

    public CornerRadius CornerRadius
    {
        get => (CornerRadius)GetValue(CornerRadiusProperty);
        set => SetValue(CornerRadiusProperty, value);
    }

    protected override Size ArrangeOverride(Size finalSize)
    {
        double num = StrokeThickness / 2.0;
        _rect = new Rect(num, num, Math.Max(0.0, finalSize.Width - StrokeThickness), Math.Max(0.0, finalSize.Height - StrokeThickness));
        return finalSize;
    }

    protected override Size MeasureOverride(Size constraint)
    {
        return new Size(StrokeThickness, StrokeThickness);
    }

    private Pen GetPen()
    {
        Pen pen = new Pen
        {
            Thickness = Math.Abs(StrokeThickness),
            Brush = Stroke,
            StartLineCap = StrokeStartLineCap,
            EndLineCap = StrokeEndLineCap,
            DashCap = StrokeDashCap,
            LineJoin = StrokeLineJoin,
            MiterLimit = StrokeMiterLimit
        };
        DoubleCollection strokeDashArray = StrokeDashArray;
        double strokeDashOffset = StrokeDashOffset;
        if (strokeDashArray != null || strokeDashOffset != 0.0)
        {
            pen.DashStyle = new DashStyle(strokeDashArray, strokeDashOffset);
        }
        pen.Freeze();
        return pen;
    }

    protected override void OnRender(DrawingContext drawingContext)
    {
        drawingContext.DrawGeometry(Fill, GetPen(), DefiningGeometry);
    }

    private PathGeometry GetBorderGeometry()
    {
        CornerRadius cornerRadius = CornerRadius;
        if (!RightEdgeVisibility)
        {
            cornerRadius.TopRight = cornerRadius.BottomRight = 0.0;
        }
        List<PathSegment> list = new List<PathSegment>();
        list.Add(new LineSegment(_rect.TopLeft + new Vector(cornerRadius.TopLeft, 0.0), isStroked: true));
        if (cornerRadius.TopLeft > 0.0)
        {
            list.Add(new ArcSegment(_rect.TopLeft + new Vector(0.0, cornerRadius.TopLeft), new Size(cornerRadius.TopLeft, cornerRadius.TopLeft), 0.0, isLargeArc: false, SweepDirection.Counterclockwise, isStroked: true));
        }
        list.Add(new LineSegment(_rect.BottomLeft - new Vector(0.0, cornerRadius.BottomLeft), isStroked: true));
        if (cornerRadius.BottomLeft > 0.0)
        {
            list.Add(new ArcSegment(_rect.BottomLeft + new Vector(cornerRadius.BottomLeft, 0.0), new Size(cornerRadius.BottomLeft, cornerRadius.BottomLeft), 0.0, isLargeArc: false, SweepDirection.Counterclockwise, isStroked: true));
        }
        list.Add(new LineSegment(_rect.BottomRight - new Vector(cornerRadius.BottomRight, 0.0), isStroked: true));
        if (cornerRadius.BottomRight > 0.0)
        {
            list.Add(new ArcSegment(_rect.BottomRight - new Vector(0.0, cornerRadius.BottomRight), new Size(cornerRadius.BottomRight, cornerRadius.BottomRight), 0.0, isLargeArc: false, SweepDirection.Counterclockwise, isStroked: true));
        }
        list.Add(new LineSegment(_rect.TopRight + new Vector(0.0, cornerRadius.TopRight), RightEdgeVisibility));
        if (cornerRadius.TopRight > 0.0)
        {
            list.Add(new ArcSegment(_rect.TopRight - new Vector(cornerRadius.TopRight, 0.0), new Size(cornerRadius.TopRight, cornerRadius.TopRight), 0.0, isLargeArc: false, SweepDirection.Counterclockwise, isStroked: true));
        }
        PathGeometry pathGeometry = new PathGeometry
        {
            Figures =
            {
                new PathFigure(_rect.TopRight - new Vector(cornerRadius.TopRight, 0.0), list, closed: true)
            }
        };
        pathGeometry.Freeze();
        return pathGeometry;
    }
}
