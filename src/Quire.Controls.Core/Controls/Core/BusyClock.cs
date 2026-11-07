#nullable disable
using System;
using System.Windows;
using System.Windows.Media;
using System.Windows.Shapes;
using Quire.Mirror;

namespace Quire.Controls.Core;

public partial class BusyClock
{
    public static readonly DependencyProperty BusyClockAngleProperty = DependencyProperty.Register(
        "BusyClockAngle",
        typeof(double),
        typeof(BusyClock),
        new FrameworkPropertyMetadata(0.0, FrameworkPropertyMetadataOptions.AffectsRender));

    public double BusyClockAngle
    {
        get => (double)GetValue(BusyClockAngleProperty);
        set => SetValue(BusyClockAngleProperty, value);
    }

    protected override Geometry DefiningGeometry
    {
        get
        {
            if (Visibility != Visibility.Visible)
            {
                return Geometry.Empty;
            }

            bool flag = BusyClockAngle < 360.0;
            double num = (flag ? BusyClockAngle : (BusyClockAngle - 360.0));
            PathFigure pathFigure = new PathFigure();
            double num2 = ActualWidth / 2.0;
            double num3 = ActualHeight / 2.0;
            pathFigure.StartPoint = new Point(num2, num3);
            pathFigure.IsClosed = true;
            pathFigure.IsFilled = true;
            pathFigure.Segments.Add(new LineSegment(pathFigure.StartPoint - new Vector(0.0, num3), true));
            pathFigure.Segments.Add(new ArcSegment(
                pathFigure.StartPoint + new Vector(num2 * Math.Sin(Math.PI * 2.0 * num / 360.0), -num3 * Math.Cos(Math.PI * 2.0 * num / 360.0)),
                new Size(num2, num3),
                num,
                flag ? (num > 180.0) : (num < 180.0),
                flag ? SweepDirection.Clockwise : SweepDirection.Counterclockwise,
                true));
            pathFigure.Segments.Add(new LineSegment(pathFigure.StartPoint, true));
            PathGeometry pathGeometry = new PathGeometry
            {
                Figures = { pathFigure }
            };
            pathGeometry.Freeze();
            return pathGeometry;
        }
    }

    [MirrorVerified("PtcControlsCoreBusyClockMechanicalDifferentialTests")]
    public BusyClock() { }
}
