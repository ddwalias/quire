using System.Windows.Media;

namespace Quire.Media3D;

internal partial class StrokeCharacter
{
    private int width;
    private Geometry geo;

    public int Width
    {
        get => width;
        set => width = value;
    }

    public Geometry Geometry
    {
        get => geo;
        set => geo = value;
    }

    public StrokeCharacter(int width, string strGeometry)
    {
        Width = width;
        Geometry = Geometry.Parse(strGeometry);
    }
}
