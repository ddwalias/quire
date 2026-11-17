#nullable disable
#pragma warning disable

using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Media.Media3D;

namespace Quire.Controls.Plots.Plot3D.View;

internal partial class PlotLabelsContainer3D : ModelVisual3D
{
    private static readonly Type TypeOfThis = typeof(PlotLabelsContainer3D);

    public static readonly DependencyProperty CameraLookDirectionProperty = DependencyProperty.Register("CameraLookDirection", typeof(Vector3D), TypeOfThis);

    public Vector3D CameraLookDirection
    {
        get
        {
            return (Vector3D)((DependencyObject)this).GetValue(CameraLookDirectionProperty);
        }
        set
        {
            ((DependencyObject)this).SetValue(CameraLookDirectionProperty, (object)value);
        }
    }

    internal void Add(Viewport2DVisual3D label, bool shouldReorder)
    {
        Children.Add(label);
        if (shouldReorder)
        {
            ReorderChildren();
        }
    }

    internal void Add(IEnumerable<Viewport2DVisual3D> labels, bool shouldReorder)
    {
        labels.Each(Children.Add);
        if (shouldReorder)
        {
            ReorderChildren();
        }
    }

    internal bool Remove(Viewport2DVisual3D label)
    {
        return Children.Remove(label);
    }

    private static int SignEx(double d)
    {
        return Math.Sign(double.IsNaN(d) ? ((double)(double.IsNegativeInfinity(d) ? -1 : 1)) : d);
    }

    internal void ReorderChildren()
    {
        Visual3D[] array = new Visual3D[Children.Count];
        Children.CopyTo(array, 0);
        Array.Sort(array, (Visual3D first, Visual3D second) => SignEx(LabelDistanceFromCameraMagnitude(first as Viewport2DVisual3D) - LabelDistanceFromCameraMagnitude(second as Viewport2DVisual3D)));
        Children.Clear();
        array.Each((Visual3D visual) =>
        {
            Children.Add(visual);
        });
    }

    private double LabelDistanceFromCameraMagnitude(Viewport2DVisual3D label)
    {
        if (label == null || label.Geometry == null)
        {
            return double.PositiveInfinity;
        }
        return Vector3D.DotProduct((Vector3D)label.Geometry.Bounds.Location, -CameraLookDirection);
    }

    public PlotLabelsContainer3D()
    {
    }
}
