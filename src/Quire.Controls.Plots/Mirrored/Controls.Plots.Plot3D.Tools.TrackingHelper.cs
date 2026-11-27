using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Media.Media3D;
using Quire.Media3D;
using Quire.Media3D.Wires;

namespace Quire.Controls.Plots.Plot3D.Tools;

internal partial class TrackingHelper : ITrackingHelper
{
    private readonly IBaseTrackball _trackball;
    private Matrix3D? _wTransform;
    private ModelVisual3D _hitTestTarget;

    private WirePoints[] _trackingPointsArray = new WirePoints[2]
    {
        new WirePoints
        {
            Positions = new Point3DCollection(),
            Color = Brushes.Green.Color
        },
        new WirePoints
        {
            Positions = new Point3DCollection(),
            Color = Brushes.Violet.Color,
            Thickness = 2.0
        }
    };

    private WireLines[] _trackingLinesArray = new WireLines[1]
    {
        new WireLines
        {
            Lines = new Point3DCollection(),
            Color = Brushes.Violet.Color
        }
    };

    public TrackingHelper(IBaseTrackball trackball)
    {
        _trackball = trackball;
    }

    public void Attach()
    {
        UpdateHitTestTarget();
        _trackingPointsArray.Concat<WireBase>(_trackingLinesArray).Each((WireBase wire) =>
        {
            if (!Viewport.Children.Contains(wire))
            {
                Viewport.Children.Insert(0, wire);
            }
        });
    }

    public void Detach()
    {
        _wTransform = null;
        ClearTrackingVisuals();
    }

    public void Reset()
    {
        ClearTrackingVisuals();
    }

    private void ClearTrackingVisuals()
    {
        _trackingPointsArray.Each((WirePoints points) =>
        {
            points.Positions.Clear();
        });
        _trackingLinesArray.Each((WireLines lines) =>
        {
            lines.Lines.Clear();
        });
    }

    public void TrackStart()
    {
        ClearTrackingVisuals();
        UpdateHitTestTarget();
        Viewport.Camera.Changed += UpdateHitTestTarget;
    }

    public void TrackEnd()
    {
        Viewport.Camera.Changed -= UpdateHitTestTarget;
    }

    public ITrackingHelper SetTrackingPoint(Point3D point, int colorId)
    {
        _trackingPointsArray[colorId].Positions.Clear();
        return AddTrackingPoint(point, colorId);
    }

    public ITrackingHelper AddTrackingPoint(Point3D point, int colorId = 0)
    {
        _trackingPointsArray[colorId].Positions.Add(point);
        return this;
    }

    public ITrackingHelper SetTrackingLine(Point3D from, Point3D to, int colorId)
    {
        _trackingLinesArray[colorId].Lines.Clear();
        return AddTrackingLine(from, to, colorId);
    }

    public ITrackingHelper AddTrackingLine(Point3D from, Point3D to, int colorId = 0)
    {
        _trackingLinesArray[colorId].Lines.Add(from);
        _trackingLinesArray[colorId].Lines.Add(to);
        return this;
    }

    public ITrackingHelper SetTrackingPoint2D(Point point, int colorId)
    {
        Point3D? initialHit = null;
        PerformHitTest(point, ref initialHit);
        return initialHit.HasValue ? SetTrackingPoint(initialHit.Value, colorId) : this;
    }

    public ITrackingHelper AddTrackingPoint2D(Point point, int colorId)
    {
        Point3D? initialHit = null;
        PerformHitTest(point, ref initialHit);
        return initialHit.HasValue ? AddTrackingPoint(initialHit.Value, colorId) : this;
    }

    public ITrackingHelper SetTrackingLine2D(Point from, Point to, int colorId)
    {
        Point3D? initialHit = null;
        Point3D? secondaryHit = null;
        PerformHitTest(from, ref initialHit);
        PerformHitTest(to, ref secondaryHit);
        return (initialHit.HasValue && secondaryHit.HasValue) ? SetTrackingLine(initialHit.Value, secondaryHit.Value, colorId) : this;
    }

    public ITrackingHelper AddTrackingLine2D(Point from, Point to, int colorId)
    {
        Point3D? initialHit = null;
        Point3D? secondaryHit = null;
        PerformHitTest(from, ref initialHit);
        PerformHitTest(to, ref secondaryHit);
        return (initialHit.HasValue && secondaryHit.HasValue) ? AddTrackingLine(initialHit.Value, secondaryHit.Value, colorId) : this;
    }

    public void PerformHitTest(Point position, ref Point3D? initialHit)
    {
        bool isHitTestVisible = Viewport.IsHitTestVisible;
        try
        {
            Viewport.IsHitTestVisible = true;
            PointHitTestParameters hitTestParameters = new PointHitTestParameters(position);
            Point3D? point3D = initialHit;
            Point3D? current2 = null;
            VisualTreeHelper.HitTest(Viewport, null, (HitTestResult result) => HitTestResultCallback(result, out current2), hitTestParameters);
            DebugTools.DevAssert(!point3D.HasValue || !current2.HasValue || (DoubleComparer.DefaultDoubleComparer.Compare(point3D.Value.X, current2.Value.X) == 0 && DoubleComparer.DefaultDoubleComparer.Compare(point3D.Value.Y, current2.Value.Y) == 0 && DoubleComparer.DefaultDoubleComparer.Compare(point3D.Value.Z, current2.Value.Z) == 0), null);
            if (!initialHit.HasValue)
            {
                initialHit = current2;
            }
        }
        finally
        {
            Viewport.IsHitTestVisible = isHitTestVisible;
        }
    }

    private Viewport3D Viewport => _trackball.Viewport;

    private HitTestResultBehavior HitTestResultCallback(HitTestResult result, out Point3D? current)
    {
        current = null;
        HitTestResultBehavior hitTestBehavior = HitTestResultBehavior.Continue;
        if (result is RayHitTestResult rayHitTestResult && rayHitTestResult.VisualHit == _hitTestTarget)
        {
            hitTestBehavior = HitTestResultBehavior.Stop;
            current = rayHitTestResult.PointHit;
        }
        return hitTestBehavior;
    }

    private void UpdateHitTestTarget(object sender, EventArgs e)
    {
        UpdateHitTestTarget();
    }

    private void UpdateHitTestTarget()
    {
        Matrix3D totalTransform = CameraInfo.GetTotalTransform(Viewport.Camera, 1.0);
        if (_wTransform.HasValue && _wTransform.Value == totalTransform)
        {
            return;
        }
        Matrix3D inverseTransform = CameraInfo.GetInverseTransform(Viewport.Camera, 1.0);
        double zValueForHitTesting = ViewportInfo.GetZValueForHitTesting(Viewport.Camera, totalTransform, _trackball.Model3DBoundingBox);
        Point3DCollection hitTestTargetPoints = new Point3DCollection
        {
            new Point3D(-1.0, -1.0, zValueForHitTesting),
            new Point3D(-1.0, 1.0, zValueForHitTesting),
            new Point3D(1.0, -1.0, zValueForHitTesting),
            new Point3D(1.0, 1.0, zValueForHitTesting)
        };
        Point3DCollection hitTestTargetTransformed = GetHitTestTargetTransformed(inverseTransform, hitTestTargetPoints);
        if (_hitTestTarget != null)
        {
            Viewport.Children.Remove(_hitTestTarget);
        }
        _hitTestTarget = new ModelVisual3D
        {
            Content = new GeometryModel3D
            {
                Geometry = new MeshGeometry3D
                {
                    Positions = hitTestTargetTransformed,
                    TriangleIndices = { 1, 0, 2, 3, 1, 2 }
                },
                Material = new DiffuseMaterial
                {
                    Brush = Brushes.Transparent
                }
            }
        };
        ((Freezable)_hitTestTarget.Content).Freeze();
        Viewport.Children.Add(_hitTestTarget);
    }

    private static Point3DCollection GetHitTestTargetTransformed(Matrix3D htTransform, Point3DCollection hitTestTargetPoints)
    {
        Point3DCollection hitTestTargetTransformed = new Point3DCollection(hitTestTargetPoints.Count);
        hitTestTargetPoints.Each((Point3D pt) =>
        {
            hitTestTargetTransformed.Add(htTransform.Transform(pt));
        });
        return hitTestTargetTransformed;
    }
}
