using System;
using System.Windows;
using System.Windows.Media.Media3D;

namespace Quire.Media3D.Wires;

public partial class WireDashes : WireLines
{
    public static readonly DependencyProperty PitchProperty =
        DependencyProperty.Register("Pitch", typeof(double), typeof(WireDashes), new PropertyMetadata(0.02, WireBase.PropertyChanged));

    public double Pitch
    {
        get => (double)GetValue(PitchProperty);
        set => SetValue(PitchProperty, (double)value);
    }

    private void AddDashedLine(Point3D start, Point3D end, Point3DCollection resultVertices)
    {
        double num = Math.Abs((end - start).Length);
        double exactDashCount = num / Pitch;
        int dashCount = (int)Math.Floor(exactDashCount);
        Vector3D vector3D = (end - start) / num * Pitch;
        resultVertices.Add(start);
        for (int i = 0; i < dashCount; i++)
        {
            resultVertices.Add(start + vector3D * i);
        }
        resultVertices.Add(end);
        bool isExact = (double)dashCount - Math.Ceiling(exactDashCount) == 0.0;
        if (!isExact)
        {
            resultVertices.Add(end);
        }
    }

    protected override void Generate(DependencyPropertyChangedEventArgs args, Point3DCollection resultVertices)
    {
        resultVertices.Clear();
        if (Lines.Count % 2 != 0)
        {
            return;
        }

        for (int i = 0; i < Lines.Count; i += 2)
        {
            AddDashedLine(Lines[i], Lines[i + 1], resultVertices);
        }
    }

    public WireDashes()
    {
    }
}
