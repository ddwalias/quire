#nullable disable
#pragma warning disable

using System;
using System.Windows;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Threading;
using Quire.Controls.Core;

namespace Quire.Controls.Plots.Plot3D.Tools;

public partial class ZoomTrackball : BasePanZoomTrackball
{
    private readonly DispatcherTimer _timer;

    private const int CLICK_ZOOM_FACTOR = 3;

    private SelectionDraggerRubberBand _selectionDraggerRubberBand;

    public ZoomTrackball()
    {
        _timer = new DispatcherTimer((DispatcherPriority)7)
        {
            Interval = new TimeSpan(0, 0, 0, 0, 150)
        };
        _timer.Tick += delegate
        {
            _timer.Stop();
            if (_selectionDraggerRubberBand == null && CurrentTrackingInfo.HasSignificantMove)
            {
                UpdateSelectionRubberBand(CurrentTrackingInfo.LastPosition);
            }
        };
    }

    protected override bool TrackStart(Point current, bool useTimer)
    {
        if (DebugTools.DevAssert(_selectionDraggerRubberBand == null, null).Failed)
        {
            RemoveSelectionRubberBand();
        }
        if (useTimer)
        {
            _timer.Start();
        }
        return base.TrackStart(current, useTimer);
    }

    protected override int GetNumTicksForScaleClick()
    {
        return (CurrentTrackingInfo.TrackingButton == MouseButton.Left ? -1 : 1) * 3;
    }

    protected override bool Track(Point current)
    {
        if (!_timer.IsEnabled)
        {
            UpdateSelectionRubberBand(current);
        }
        return true;
    }

    public override void Detach()
    {
        Clean();
        base.Detach();
    }

    private void Clean()
    {
        _timer.Stop();
        RemoveSelectionRubberBand();
    }

    protected override bool TrackEnd(Point current)
    {
        bool flag = _selectionDraggerRubberBand != null;
        Clean();
        if (!flag)
        {
            return base.TrackEnd(current);
        }
        Point captureStartPosition = CurrentTrackingInfo.CaptureStartPosition;
        double num = Math.Min(current.X, captureStartPosition.X);
        double rightEdge = Math.Max(current.X, captureStartPosition.X);
        double topEdge = Math.Min(current.Y, captureStartPosition.Y);
        double bottomEdge = Math.Max(current.Y, captureStartPosition.Y);
        Point screenAreaCenter = new Point((num + rightEdge) / 2.0, (topEdge + bottomEdge) / 2.0);
        DoubleComparer defaultDoubleComparer = DoubleComparer.DefaultDoubleComparer;
        Vector val = current - captureStartPosition;
        double horizontalScale = ((defaultDoubleComparer.Compare(val.X, 0.0) == 0) ? 1.0 : Math.Abs(Viewport.ActualWidth / val.X));
        double verticalScale = ((defaultDoubleComparer.Compare(val.Y, 0.0) == 0) ? 1.0 : Math.Abs(Viewport.ActualHeight / val.Y));
        double requestedScale = Math.Min(horizontalScale, verticalScale);
        TrackingHelper.SetTrackingPoint2D(captureStartPosition).SetTrackingPoint2D(current, 1).SetTrackingLine2D(captureStartPosition, current);
        ApplyScaleWithCenter(requestedScale, screenAreaCenter);
        return true;
    }

    protected override Point CoercePosition(Point rawPosition)
    {
        Point result = default;
        result.X = Math.Max(0.0, Math.Min(Viewport.ActualWidth, rawPosition.X));
        result.Y = Math.Max(0.0, Math.Min(Viewport.ActualHeight, rawPosition.Y));
        return result;
    }

    private void UpdateSelectionRubberBand(Point current)
    {
        DebugTools.DevAssert(!_timer.IsEnabled, null);
        if (_selectionDraggerRubberBand == null)
        {
            _selectionDraggerRubberBand = new SelectionDraggerRubberBand(Viewport, Brushes.DodgerBlue);
        }
        _selectionDraggerRubberBand.Bounds = new Rect(CurrentTrackingInfo.CaptureStartPosition, current);
    }

    private void RemoveSelectionRubberBand()
    {
        if (_selectionDraggerRubberBand != null)
        {
            _selectionDraggerRubberBand.Remove();
            _selectionDraggerRubberBand = null;
        }
    }
}
