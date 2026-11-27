using System.Windows;
using System.Windows.Media;
using System.Windows.Media.Media3D;

namespace Quire.Media3D;

public partial class Cylinder : ModelVisualBase
{
    private AxisAngleRotation3D rotate;
    private RotateTransform3D xform;

    public Cylinder()
    {
        rotate = new AxisAngleRotation3D();
        xform = new RotateTransform3D(rotate);
        PropertyChanged(default(DependencyPropertyChangedEventArgs));
    }

    public Point3D Point1
    {
        set => SetValue(Point1Property, value);
        get => (Point3D)GetValue(Point1Property);
    }

    public Point3D Point2
    {
        get => (Point3D)GetValue(Point2Property);
        set => SetValue(Point2Property, value);
    }

    public double Radius1
    {
        set => SetValue(Radius1Property, value);
        get => (double)GetValue(Radius1Property);
    }

    public double Radius2
    {
        set => SetValue(Radius2Property, value);
        get => (double)GetValue(Radius2Property);
    }

    public double Fold1
    {
        get => (double)GetValue(Fold1Property);
        set => SetValue(Fold1Property, value);
    }

    public double Fold2
    {
        get => (double)GetValue(Fold2Property);
        set => SetValue(Fold2Property, value);
    }

    public int Slices
    {
        set => SetValue(SlicesProperty, value);
        get => (int)GetValue(SlicesProperty);
    }

    public int Stacks
    {
        set => SetValue(StacksProperty, value);
        get => (int)GetValue(StacksProperty);
    }

    public int EndStacks
    {
        set => SetValue(EndStacksProperty, value);
        get => (int)GetValue(EndStacksProperty);
    }

    protected override void Triangulate(DependencyPropertyChangedEventArgs args, Point3DCollection vertices, Vector3DCollection normals, Int32Collection indices, PointCollection textures)
    {
        vertices.Clear();
        normals.Clear();
        indices.Clear();
        textures.Clear();
        Vector3D vector3D = Point2 - Point1;
        Vector3D radialDirection;
        if (vector3D.X == 0.0 && vector3D.Y == 0.0)
        {
            radialDirection = new Vector3D(0.0, -1.0, 0.0);
        }
        else
        {
            rotate.Axis = Vector3D.CrossProduct(vector3D, new Vector3D(0.0, 0.0, 1.0));
            rotate.Angle = -90.0;
            radialDirection = vector3D * xform.Value;
            radialDirection.Normalize();
        }
        rotate.Axis = -vector3D;
        int count;
        for (int i = 0; i <= EndStacks; i++)
        {
            Vector3D radialOffset = (double)i * Radius1 / (double)EndStacks * radialDirection;
            int num = i * (Slices + 1);
            int nextRingIndex = (i + 1) * (Slices + 1);
            for (int j = 0; j <= Slices; j++)
            {
                rotate.Angle = (double)j * 360.0 / (double)Slices;
                vertices.Add(Point1 + radialOffset * xform.Value);
                normals.Add(-vector3D);
                textures.Add(new Point((double)j / (double)Slices, Fold1 * (double)i / (double)EndStacks));
                if (i < EndStacks && j < Slices)
                {
                    if (i != 0)
                    {
                        indices.Add(num + j);
                        indices.Add(nextRingIndex + j);
                        indices.Add(num + j + 1);
                    }
                    indices.Add(num + j + 1);
                    indices.Add(nextRingIndex + j);
                    indices.Add(nextRingIndex + j + 1);
                }
            }
        }
        count = vertices.Count;
        for (int k = 0; k <= Stacks; k++)
        {
            Vector3D radialOffset = ((double)(Stacks - k) * Radius1 + (double)k * Radius2) / (double)Stacks * radialDirection;
            Point3D point3D = Point1 + (double)k * vector3D / (double)Stacks;
            int currentRingIndex = count + k * (Slices + 1);
            int nextRingIndex = count + (k + 1) * (Slices + 1);
            for (int l = 0; l <= Slices; l++)
            {
                rotate.Angle = (double)l * 360.0 / (double)Slices;
                Vector3D transformedRadialOffset = radialOffset * xform.Value;
                normals.Add(transformedRadialOffset);
                vertices.Add(point3D + transformedRadialOffset);
                textures.Add(new Point((double)l / (double)Slices, Fold1 + (Fold2 - Fold1) * (double)k / (double)Stacks));
                if (k < Stacks && l < Slices)
                {
                    indices.Add(currentRingIndex + l);
                    indices.Add(nextRingIndex + l);
                    indices.Add(currentRingIndex + l + 1);
                    indices.Add(currentRingIndex + l + 1);
                    indices.Add(nextRingIndex + l);
                    indices.Add(nextRingIndex + l + 1);
                }
            }
        }
        count = vertices.Count;
        for (int m = 0; m <= EndStacks; m++)
        {
            Vector3D radialOffset = Radius2 * (1.0 - (double)m / (double)EndStacks) * radialDirection;
            int currentRingIndex = count + m * (Slices + 1);
            int nextRingIndex = count + (m + 1) * (Slices + 1);
            for (int n = 0; n <= Slices; n++)
            {
                rotate.Angle = (double)n * 360.0 / (double)Slices;
                vertices.Add(Point2 + radialOffset * xform.Value);
                normals.Add(vector3D);
                textures.Add(new Point((double)n / (double)Slices, Fold2 + (1.0 - Fold2) * (double)m / (double)EndStacks));
                if (m < EndStacks && n < Slices)
                {
                    indices.Add(currentRingIndex + n);
                    indices.Add(nextRingIndex + n);
                    indices.Add(currentRingIndex + n + 1);
                    if (m != EndStacks - 1)
                    {
                        indices.Add(currentRingIndex + n + 1);
                        indices.Add(nextRingIndex + n);
                        indices.Add(nextRingIndex + n + 1);
                    }
                }
            }
        }
    }

    public static readonly DependencyProperty Point1Property = DependencyProperty.Register("Point1", typeof(Point3D), typeof(Cylinder), new PropertyMetadata((object)new Point3D(0.0, 1.0, 0.0), (PropertyChangedCallback)ModelVisualBase.PropertyChanged));

    public static readonly DependencyProperty Point2Property = DependencyProperty.Register("Point2", typeof(Point3D), typeof(Cylinder), new PropertyMetadata((object)new Point3D(0.0, 0.0, 0.0), (PropertyChangedCallback)ModelVisualBase.PropertyChanged));

    public static readonly DependencyProperty Radius1Property = DependencyProperty.Register("Radius1", typeof(double), typeof(Cylinder), new PropertyMetadata((object)1.0, (PropertyChangedCallback)ModelVisualBase.PropertyChanged), (ValidateValueCallback)((object value) => (double)value >= 0.0));

    public static readonly DependencyProperty Radius2Property = DependencyProperty.Register("Radius2", typeof(double), typeof(Cylinder), new PropertyMetadata((object)1.0, (PropertyChangedCallback)ModelVisualBase.PropertyChanged), (ValidateValueCallback)((object value) => (double)value >= 0.0));

    public static readonly DependencyProperty Fold1Property = DependencyProperty.Register("Fold1", typeof(double), typeof(Cylinder), new PropertyMetadata((object)0.1, (PropertyChangedCallback)ModelVisualBase.PropertyChanged), (ValidateValueCallback)((object value) => (double)value >= 0.0 && (double)value <= 1.0));

    public static readonly DependencyProperty Fold2Property = DependencyProperty.Register("Fold2", typeof(double), typeof(Cylinder), new PropertyMetadata((object)0.9, (PropertyChangedCallback)ModelVisualBase.PropertyChanged), (ValidateValueCallback)((object value) => (double)value >= 0.0 && (double)value <= 1.0));

    public static readonly DependencyProperty SlicesProperty = DependencyProperty.Register("Slices", typeof(int), typeof(Cylinder), new PropertyMetadata((object)36, (PropertyChangedCallback)ModelVisualBase.PropertyChanged), (ValidateValueCallback)((object value) => (int)value > 2));

    public static readonly DependencyProperty StacksProperty = DependencyProperty.Register("Stacks", typeof(int), typeof(Cylinder), new PropertyMetadata((object)1, (PropertyChangedCallback)ModelVisualBase.PropertyChanged), (ValidateValueCallback)((object value) => (int)value > 0));

    public static readonly DependencyProperty EndStacksProperty = DependencyProperty.Register("EndStacks", typeof(int), typeof(Cylinder), new PropertyMetadata((object)1, (PropertyChangedCallback)ModelVisualBase.PropertyChanged), (ValidateValueCallback)((object value) => (int)value > 0));
}
