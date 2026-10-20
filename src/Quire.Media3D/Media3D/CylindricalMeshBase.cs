#nullable disable
using System.Windows;

namespace Quire.Media3D;

public abstract partial class CylindricalMeshBase : MeshGeneratorBase
{
    public static readonly DependencyProperty LengthProperty = DependencyProperty.Register("Length", typeof(double), typeof(CylindricalMeshBase), new PropertyMetadata(1.0, PropertyChanged));

    public static readonly DependencyProperty RadiusProperty = DependencyProperty.Register("Radius", typeof(double), typeof(CylindricalMeshBase), new PropertyMetadata(1.0, PropertyChanged));

    public static readonly DependencyProperty SlicesProperty = DependencyProperty.Register("Slices", typeof(int), typeof(CylindricalMeshBase), new PropertyMetadata(36, PropertyChanged), ValidateSlices);

    public static readonly DependencyProperty StacksProperty = DependencyProperty.Register("Stacks", typeof(int), typeof(CylindricalMeshBase), new PropertyMetadata(1, PropertyChanged), ValidateStacks);

    public double Length
    {
        get => (double)GetValue(LengthProperty);
        set => SetValue(LengthProperty, value);
    }

    public double Radius
    {
        get => (double)GetValue(RadiusProperty);
        set => SetValue(RadiusProperty, value);
    }

    public int Slices
    {
        get => (int)GetValue(SlicesProperty);
        set => SetValue(SlicesProperty, value);
    }

    public int Stacks
    {
        get => (int)GetValue(StacksProperty);
        set => SetValue(StacksProperty, value);
    }

    private static bool ValidateSlices(object obj)
    {
        return (int)obj > 2;
    }

    private static bool ValidateStacks(object obj)
    {
        return (int)obj > 0;
    }

    protected CylindricalMeshBase()
    {
    }
}
