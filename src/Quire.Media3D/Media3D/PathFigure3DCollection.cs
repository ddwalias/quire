using System.Windows;

namespace Quire.Media3D;

public partial class PathFigure3DCollection : FreezableCollection<PathFigure3D>
{
    public override string ToString()
    {
        string text = "";
        using (Enumerator enumerator = GetEnumerator())
        {
            while (enumerator.MoveNext())
            {
                PathFigure3D current = enumerator.Current;
                text += ((object)current).ToString();
            }
        }
        return text;
    }

    protected override Freezable CreateInstanceCore()
    {
        return new PathFigure3DCollection();
    }

    public PathFigure3DCollection()
    {
    }
}
