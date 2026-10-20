#nullable disable
using System.Windows.Media;
using Quire.Mirror;

namespace Quire.Wpf;

public static partial class ColorUtilities
{
    [MirrorVerified("Release IL")]
    public static List<Color> BuildDistributedDistinctColorsTable() => DistributeColors(GetColorsTable());
}
