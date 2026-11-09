using System.Windows.Media.Media3D;

namespace Quire.Controls.Plots.Plot3D.Model;

internal partial class CurveVisualData3D
{
    internal CurveVisualData3D(Transform3D transform3D, int verticesCount)
        : base(TraceType3D.Curve, transform3D, verticesCount)
    {
    }

    public override bool HasNoData => Vertices.Count == 0;
}
