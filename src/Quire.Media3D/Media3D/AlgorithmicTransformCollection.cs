using System.Windows;

namespace Quire.Media3D;

public partial class AlgorithmicTransformCollection : FreezableCollection<AlgorithmicTransform>
{
    protected override Freezable CreateInstanceCore()
    {
        return new AlgorithmicTransformCollection();
    }

    public AlgorithmicTransformCollection()
    {
    }
}
