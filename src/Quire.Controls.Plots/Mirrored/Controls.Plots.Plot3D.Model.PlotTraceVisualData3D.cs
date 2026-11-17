using System.Windows.Media;
using System.Windows.Media.Media3D;

namespace Quire.Controls.Plots.Plot3D.Model;

public abstract partial class PlotTraceVisualData3D
{
    public static readonly PlotTraceVisualData3D Undefined = new UndefinedVisualData3D();

    private static readonly Int32Collection _emptyIndices = new Int32Collection();

    private readonly Transform3D _transformLogicalToVisual;

    public Int32Collection Indices { get; protected set; }

    public TraceType3D TraceType { get; private set; }

    public Point3DCollection Vertices { get; private set; }

    public Point3DCollection LogicalVertices { get; private set; }

    public virtual Point3DCollection FilteredVertices => Vertices;

    protected PlotTraceVisualData3D(TraceType3D type, Transform3D transformLogicalToVisual, int verticesCount)
    {
        TraceType = type;
        _transformLogicalToVisual = transformLogicalToVisual;
        Vertices = new Point3DCollection(verticesCount);
        LogicalVertices = new Point3DCollection(verticesCount);
        Indices = _emptyIndices;
    }

    public abstract bool HasNoData { get; }

    public void AddVertex(Point3D point3D)
    {
        LogicalVertices.Add(point3D);
        Vertices.Add(_transformLogicalToVisual.Transform(point3D));
    }
}
