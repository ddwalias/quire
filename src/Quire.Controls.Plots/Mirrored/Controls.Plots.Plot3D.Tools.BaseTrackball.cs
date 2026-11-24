using System;
using System.Windows;
using System.Windows.Input;
using System.Windows.Media.Media3D;

namespace Quire.Controls.Plots.Plot3D.Tools;

public abstract partial class BaseTrackball : IBaseTrackball
{
    protected Point GetPositionRelativeToViewport(MouseEventArgs args)
    {
        return args.GetPosition(Viewport);
    }

    private void FireMouseUp()
    {
        Action<Matrix3D> mouseUp = MouseUp;
        if (mouseUp != null)
        {
            mouseUp(_previousTransform);
        }
    }

    private void FireMouseWheel()
    {
        Action<Matrix3D> mouseWheel = MouseWheel;
        if (mouseWheel != null)
        {
            mouseWheel(_previousTransform);
        }
    }

    public void HandleMouseWheel(int ticks)
    {
        if (IsTracking)
        {
            return;
        }
        AddScale(ScaleFromTicks(ticks));
        FireMouseWheel();
    }

    protected double ScaleFromTicks(int ticks)
    {
        return Math.Pow(1.1, ticks);
    }

    protected void AddScale(double scale)
    {
        SetScale(CurrentScale * scale);
    }

    protected void SetScale(double scale)
    {
        scale = Math.Min(10.0, Math.Max(0.5, scale));
        Matrix3D matrix = ScaleTransform.Matrix;
        matrix.M44 = scale;
        ScaleTransform.Matrix = matrix;
    }

    private bool IsTrackingDifferentButton(MouseButton button)
    {
        if (IsTracking)
        {
            return CurrentTrackingInfo.TrackingButton != button;
        }
        return false;
    }

    protected virtual Point CoercePosition(Point rawPosition)
    {
        return rawPosition;
    }

    protected TrackingInfo CurrentTrackingInfo { get; private set; }

    protected partial class TrackingInfo
    {
        public MouseButton TrackingButton { get; set; }

        public bool HasSignificantMove { get; set; }

        public Point CaptureStartPosition { get; set; }

        public Point LastPosition { get; set; }

        public TrackingInfo()
        {
        }
    }
}
