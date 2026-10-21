using System.Windows;
using System.Windows.Media;
using System.Windows.Media.Media3D;

namespace Quire.Media3D.Wires;

public partial class WireRectMesh : WireFrame
{
    public WireRectMesh()
    {
    }

    protected override void Generate(DependencyPropertyChangedEventArgs args, Point3DCollection lines)
    {
        Point3DCollection positions = Positions;
        Int32Collection triangleIndices = TriangleIndices;
        lines.Clear();
        if (positions != null && positions.Count > 0 && triangleIndices != null && triangleIndices.Count > 0)
        {
            if (triangleIndices.Count % 4 != 0)
            {
                return;
            }
            for (int i = 0; i < triangleIndices.Count; i += 4)
            {
                lines.Add(positions[triangleIndices[i]]);
                lines.Add(positions[triangleIndices[i + 1]]);
                lines.Add(positions[triangleIndices[i + 1]]);
                lines.Add(positions[triangleIndices[i + 2]]);
                lines.Add(positions[triangleIndices[i + 2]]);
                lines.Add(positions[triangleIndices[i + 3]]);
                lines.Add(positions[triangleIndices[i + 3]]);
                lines.Add(positions[triangleIndices[i]]);
            }
        }
    }
}
