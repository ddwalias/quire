using System.Collections;
using System.Windows.Media;
using System.Windows.Media.Media3D;
using Quire;

namespace Quire.Controls.Plots.Plot3D.Model;

internal partial class SurfaceVisualData3D : PlotTraceVisualData3D
{
    private Point3DCollection _filteredVertices;

    public BitArray FilteredVerticesMap { get; private set; }

    public SurfaceVisualData3D(Transform3D transform3D, int verticesCount)
        : base(TraceType3D.Surface, transform3D, verticesCount)
    {
        Indices = new Int32Collection(verticesCount * 4);
        FilteredVerticesMap = new BitArray(verticesCount);
    }

    public override bool HasNoData => Indices.Count == 0;

    public override Point3DCollection FilteredVertices
    {
        get
        {
            if (_filteredVertices == null)
            {
                _filteredVertices = GetFilteredVerticesPoints();
            }
            return _filteredVertices;
        }
    }

    private Point3DCollection GetFilteredVerticesPoints()
    {
        bool flag = true;
        for (int i = 0; i < FilteredVerticesMap.Length; i++)
        {
            flag &= FilteredVerticesMap[i];
        }
        Point3DCollection points;
        if (flag)
        {
            points = Vertices;
        }
        else
        {
            points = new Point3DCollection(Vertices.Count);
            Vertices.EachWithIndex((int index, Point3D pt) =>
            {
                if (FilteredVerticesMap[index])
                {
                    points.Add(Vertices[index]);
                }
            });
        }
        return points;
    }
}
