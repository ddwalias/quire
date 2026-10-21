#nullable disable
using System.Windows;

namespace Quire.Media3D;

internal partial class WireBaseAndUltimateParent
{
    public WireBase wirebase;
    public Window window;

    public WireBaseAndUltimateParent(WireBase wirebase)
    {
        this.wirebase = wirebase;
    }
}
