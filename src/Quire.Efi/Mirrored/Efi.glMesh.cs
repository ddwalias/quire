#nullable disable
#pragma warning disable

namespace Quire.Efi;

public partial class glMesh
{
    private LRect3 rect;

    public int m_rows;

    public int m_cols;

    private shpPoint3D[] m_vertices;

    public glMesh(LRect3 rc, int rows, int cols, shpPoint3D[] vertices)
    {
        rect = rc;
        m_rows = rows;
        m_cols = cols;
        m_vertices = vertices;
    }

    public double getZ(int row, int col)
    {
        return m_vertices[row * m_cols + col].Z();
    }

    public void setZ(int row, int col, double z)
    {
        m_vertices[row * m_cols + col].Z(z);
    }

    public LRect3 Rect()
    {
        return rect;
    }

    public shpPoint3D Vertex(int row, int col)
    {
        return m_vertices[row * m_cols + col];
    }

    public shpPoint3D NormalizedVertex(int col, int row)
    {
        LRect3 lRect = Rect();
        shpPoint3D shpPoint3D2 = new shpPoint3D(Vertex(row, col));
        shpPoint3D2.X(-1.0 + 2.0 * (shpPoint3D2.X() - lRect.xmin) / lRect.Dx);
        shpPoint3D2.Y(-1.0 + 2.0 * (shpPoint3D2.Y() - lRect.ymin) / lRect.Dy);
        shpPoint3D2.Z(-1.0 + 2.0 * (shpPoint3D2.Z() - lRect.zmin) / lRect.Dz);
        return shpPoint3D2;
    }
}
