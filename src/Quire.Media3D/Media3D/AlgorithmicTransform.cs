using System.Windows.Media.Animation;
using System.Windows.Media.Media3D;

namespace Quire.Media3D;

public abstract partial class AlgorithmicTransform : Animatable
{
    public abstract void Transform(Point3DCollection points);

    protected AlgorithmicTransform()
    {
    }
}
