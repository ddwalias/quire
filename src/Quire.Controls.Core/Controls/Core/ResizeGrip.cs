#nullable disable
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Shapes;
using Quire;
using Quire.Mirror;

namespace Quire.Controls.Core;

public partial class ResizeGrip : Shape
{
    public static readonly DependencyProperty DockSideProperty = DependencyProperty.Register("DockSide", typeof(Dock), typeof(ResizeGrip), new FrameworkPropertyMetadata(DockSideChanged));

    [MirrorVerified("PtcControlsCoreResizeGripMechanicalDifferentialTests")]
    public ResizeGrip() { }

    public Dock DockSide
    {
        get => (Dock)GetValue(DockSideProperty);
        set => SetValue(DockSideProperty, value);
    }

    protected override Geometry DefiningGeometry
    {
        get
        {
            RectangleGeometry geometry = new RectangleGeometry(new Rect(-1.0, -8.0, 2.0, 2.0));
            RectangleGeometry geometry2 = new RectangleGeometry(new Rect(-1.0, -1.0, 2.0, 2.0));
            RectangleGeometry geometry3 = new RectangleGeometry(new Rect(-1.0, 6.0, 2.0, 2.0));
            CombinedGeometry combinedGeometry = new CombinedGeometry(GeometryCombineMode.Union, geometry, new CombinedGeometry(GeometryCombineMode.Union, geometry2, geometry3));
            if (DockSide == Dock.Top || DockSide == Dock.Bottom)
            {
                combinedGeometry.Transform = new RotateTransform(90.0);
            }
            return combinedGeometry;
        }
    }

    private static void DockSideChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
    {
        if (d is ResizeGrip)
        {
            (d as ResizeGrip).DockSideChanged(e);
        }
    }

    protected virtual void DockSideChanged(DependencyPropertyChangedEventArgs args)
    {
        DebugTools.DevAssert(args.NewValue is Dock, null);
        Dock dock = (Dock)args.NewValue;
        Cursor = ((dock == Dock.Top || dock == Dock.Bottom) ? Cursors.SizeNS : Cursors.SizeWE);
        switch (dock)
        {
            case Dock.Left:
                Name = "PART_LeftResizeGrip";
                break;
            case Dock.Right:
                Name = "PART_RightResizeGrip";
                break;
            case Dock.Bottom:
                Name = "PART_BottomResizeGrip";
                break;
            case Dock.Top:
                break;
        }
    }

    protected override Size MeasureOverride(Size constraint)
    {
        Rect bounds = DefiningGeometry.Bounds;
        return bounds.Size;
    }

    protected override void OnRender(DrawingContext drawingContext)
    {
        Geometry geometry = DefiningGeometry.CloneCurrentValue();
        TransformGroup transformGroup = new TransformGroup();
        transformGroup.Children.Add(geometry.Transform);
        transformGroup.Children.Add(new TranslateTransform(0.5, 0.5));
        geometry.Transform = transformGroup;
        drawingContext.DrawGeometry(Brushes.White, null, geometry);
        drawingContext.DrawGeometry(Fill, new Pen(Stroke, StrokeThickness), DefiningGeometry);
        drawingContext.DrawRectangle(Brushes.Transparent, null, GetBoundingRect());
    }

    protected virtual Rect GetBoundingRect()
    {
        return DefiningGeometry.Transform.TransformBounds(new Rect(-4.0, -10.0, 8.0, 20.0));
    }

    public virtual Point GetLocation(Rect regionBorderBoundingRect, double gripThickness)
    {
        double num;
        double num2;
        switch (DockSide)
        {
            case Dock.Left:
                num = regionBorderBoundingRect.Left + gripThickness / 2.0;
                num2 = regionBorderBoundingRect.Top + regionBorderBoundingRect.Height / 2.0;
                break;
            case Dock.Right:
                num = regionBorderBoundingRect.Right - gripThickness / 2.0;
                num2 = regionBorderBoundingRect.Top + regionBorderBoundingRect.Height / 2.0;
                break;
            case Dock.Bottom:
                num = regionBorderBoundingRect.Left + regionBorderBoundingRect.Width / 2.0;
                num2 = regionBorderBoundingRect.Bottom - gripThickness / 2.0;
                break;
            default:
                num = regionBorderBoundingRect.Left + regionBorderBoundingRect.Width / 2.0;
                num2 = regionBorderBoundingRect.Top + gripThickness / 2.0;
                break;
        }
        return new Point(num, num2);
    }
}
