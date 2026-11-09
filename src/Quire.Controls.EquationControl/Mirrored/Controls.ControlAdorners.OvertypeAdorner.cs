using System.Windows;
using System.Windows.Documents;
using System.Windows.Media;

namespace Quire.Controls.ControlAdorners;

internal partial class OvertypeAdorner
{
    private readonly SolidColorBrush _renderBrush;

    public static readonly DependencyProperty PointAProperty = DependencyProperty.Register(
        nameof(PointA),
        typeof(Point),
        typeof(OvertypeAdorner),
        new FrameworkPropertyMetadata(default(Point), FrameworkPropertyMetadataOptions.AffectsRender));

    public static readonly DependencyProperty PointBProperty = DependencyProperty.Register(
        nameof(PointB),
        typeof(Point),
        typeof(OvertypeAdorner),
        new FrameworkPropertyMetadata(default(Point), FrameworkPropertyMetadataOptions.AffectsRender));

    public OvertypeAdorner(UIElement adornedElement)
        : base(adornedElement)
    {
        IsHitTestVisible = false;
        var typeoverColor = MathcadSettings.Instance.TypeoverColor;
        _renderBrush = new SolidColorBrush(Color.FromArgb(typeoverColor.A, typeoverColor.R, typeoverColor.G, typeoverColor.B));
        _renderBrush.Opacity = MathcadSettings.Instance.CaretHighlightOpacity;
    }

    public Point PointA
    {
        get => (Point)GetValue(PointAProperty);
        set => SetValue(PointAProperty, value);
    }

    public Point PointB
    {
        get => (Point)GetValue(PointBProperty);
        set => SetValue(PointBProperty, value);
    }

    protected override void OnRender(DrawingContext drawingContext)
    {
        drawingContext.DrawRectangle(_renderBrush, null, new Rect(PointA, PointB));
    }

    public void UpdateAdornerLocation(Point pointA, Point pointB)
    {
        PointA = pointA;
        PointB = pointB;
    }
}
