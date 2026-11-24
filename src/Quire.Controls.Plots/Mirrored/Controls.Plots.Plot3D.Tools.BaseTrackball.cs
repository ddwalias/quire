using System;
using System.Windows;
using System.Windows.Input;
using System.Windows.Media.Media3D;
using Quire.Controls.Core;
using Quire.Wpf;

namespace Quire.Controls.Plots.Plot3D.Tools;

public abstract partial class BaseTrackball : IBaseTrackball
{
    protected virtual void UpdateCurrentTrackingInfo(Point current)
    {
        CurrentTrackingInfo.LastPosition = current;
        if (CurrentTrackingInfo.HasSignificantMove)
        {
            return;
        }
        double zoom = DocumentAttachedProperties.GetZoom(Viewport);
        double threshold = 5.0 / zoom;
        CurrentTrackingInfo.HasSignificantMove = 1 == DoubleComparer.DefaultDoubleComparer.Compare((current - CurrentTrackingInfo.CaptureStartPosition).Length, threshold);
    }

    private void OnMouseDown(object sender, MouseButtonEventArgs args)
    {
        HandleMouseDown(GetPositionRelativeToViewport(args), args.ChangedButton, useTimer: true);
        if (IsTracking)
        {
            Mouse.Capture(MouseEventsSource, CaptureMode.Element);
        }
    }

    public void HandleMouseDown(Point rawLocation, MouseButton mouseButton, bool useTimer)
    {
        if (IsTrackingDifferentButton(mouseButton))
        {
            return;
        }
        Point position = CoercePosition(rawLocation);
        if (TrackStart(position, useTimer))
        {
            CurrentTrackingInfo = new TrackingInfo
            {
                TrackingButton = mouseButton,
                CaptureStartPosition = position,
                LastPosition = position
            };
            _previousTransform = CameraTransform.Value;
        }
    }

    private void OnMouseUp(object sender, MouseButtonEventArgs args)
    {
        bool wasTracking = IsTracking;
        HandleMouseUp(GetPositionRelativeToViewport(args), args.ChangedButton);
        if (wasTracking && !IsTracking)
        {
            Mouse.Capture(MouseEventsSource, CaptureMode.None);
        }
        args.Handled = false;
    }

    public void HandleMouseUp(Point rawLocation, MouseButton mouseButton)
    {
        if (!IsTracking || IsTrackingDifferentButton(mouseButton))
        {
            return;
        }
        Point current = CoercePosition(rawLocation);
        UpdateCurrentTrackingInfo(current);
        TrackEnd(current);
        FireMouseUp();
        CurrentTrackingInfo = null;
    }

    private void OnMouseMove(object sender, MouseEventArgs args)
    {
        HandleMouseMove(GetPositionRelativeToViewport(args));
    }

    public void HandleMouseMove(Point rawLocation)
    {
        if (!IsTracking)
        {
            return;
        }
        Point current = CoercePosition(rawLocation);
        UpdateCurrentTrackingInfo(current);
        if (CurrentTrackingInfo.HasSignificantMove)
        {
            Track(current);
        }
    }

    private void OnMouseWheel(object sender, MouseWheelEventArgs args)
    {
        _previousTransform = CameraTransform.Value;
        HandleMouseWheel(args.Delta / WpfUtils.MouseWheelScrollDelta);
        args.Handled = true;
    }

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
