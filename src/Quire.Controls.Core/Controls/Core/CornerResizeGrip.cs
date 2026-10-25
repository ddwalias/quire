#nullable disable
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using Quire;
using Quire.Mirror;

namespace Quire.Controls.Core;

public partial class CornerResizeGrip : ResizeGrip
{
    [MirrorVerified("PtcControlsCoreCornerResizeGripMechanicalDifferentialTests")]
    public CornerResizeGrip() { }

    protected override Geometry DefiningGeometry
    {
        get
        {
            RectangleGeometry geometry = new RectangleGeometry(new Rect(-1.0, -4.0, 2.0, 2.0));
            RectangleGeometry geometry2 = new RectangleGeometry(new Rect(-1.0, -1.0, 2.0, 2.0));
            RectangleGeometry geometry3 = new RectangleGeometry(new Rect(-4.0, -1.0, 2.0, 2.0));
            CombinedGeometry combinedGeometry = new CombinedGeometry(GeometryCombineMode.Union, geometry3, new CombinedGeometry(GeometryCombineMode.Union, geometry, geometry2));
            if (DockSide == Dock.Left)
            {
                combinedGeometry.Transform = new ScaleTransform(-1.0, 1.0);
            }
            return combinedGeometry;
        }
    }

    protected override void DockSideChanged(DependencyPropertyChangedEventArgs args)
    {
        DebugTools.DevAssert(args.NewValue is Dock, null);
        Dock dock = (Dock)args.NewValue;
        if (dock == Dock.Left)
        {
            Cursor = Cursors.SizeNESW;
            Name = "PART_BottomLeftResizeGrip";
        }
        else
        {
            DebugTools.DevAssert(dock == Dock.Right, null);
            Cursor = Cursors.SizeNWSE;
            Name = "PART_BottomRightResizeGrip";
        }
    }

    protected override Rect GetBoundingRect()
    {
        return DefiningGeometry.Transform.TransformBounds(new Rect(-6.0, -6.0, 9.0, 9.0));
    }

    public override Point GetLocation(Rect regionBorderBoundingRect, double gripThickness)
    {
        double num = 0.0;
        double num2 = 0.0;
        switch (DockSide)
        {
            case Dock.Left:
                num = regionBorderBoundingRect.Left + gripThickness / 2.0;
                num2 = regionBorderBoundingRect.Bottom - gripThickness / 2.0;
                break;
            case Dock.Right:
                num = regionBorderBoundingRect.Right - gripThickness / 2.0;
                num2 = regionBorderBoundingRect.Bottom - gripThickness / 2.0;
                break;
            default:
                DebugTools.DevAssert(false, null);
                break;
        }
        return new Point(num, num2);
    }
}
