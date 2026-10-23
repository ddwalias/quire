#nullable disable
using System.Windows.Media;
using Quire.Mirror;

namespace Quire.Wpf;

public static partial class ColorUtilities
{
    public static List<Color> GetExtendedBaseColorTable()
    {
        List<Color> list = new List<Color>(baseColors.Length * 4);
        for (int i = 0; i < baseColors.Length; i++)
        {
            list.Add(baseColors[i]);
            for (float num = 0.25f; num < 1f; num += 0.25f)
            {
                Color color = baseColors[i];
                Color color2 = baseColors[(i + 1) % baseColors.Length];
                Color item = color * (1f - num) + color2 * num;
                list.Add(item);
            }
        }
        return list;
    }

    [MirrorVerified("Release IL")]
    public static List<Color> BuildDistributedDistinctColorsTable() => DistributeColors(GetColorsTable());
}
