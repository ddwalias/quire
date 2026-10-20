#nullable disable
using System.Windows;

namespace Quire.Media3D;

public partial class PathSegment3DCollection : FreezableCollection<PathSegment3D>
{
    public PathSegment3DCollection()
    {
    }

    public override string ToString()
    {
        string text = "";
        foreach (PathSegment3D current in this)
        {
            text += current.ToString();
        }
        return text;
    }

    protected override Freezable CreateInstanceCore()
    {
        return new PathSegment3DCollection();
    }
}
