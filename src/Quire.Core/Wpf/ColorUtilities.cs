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

    public static List<List<Color>> GetColorsTable()
    {
        List<List<Color>> list = new List<List<Color>>();
        List<Color> extendedBaseColorTable = GetExtendedBaseColorTable();
        list.Add(extendedBaseColorTable);
        Color[] array = grayScaleColors;
        foreach (Color shadeColor in array)
        {
            List<Color> shadesList = GetShadesList(extendedBaseColorTable, shadeColor);
            list.Add(shadesList);
        }
        return list;
    }

    public static List<Color> GetShadesList(List<Color> hueColors, Color shadeColor, float weightG = 0.5f)
    {
        return hueColors.ConvertAll((Color color) => color * (1f - weightG) + shadeColor * weightG);
    }

    public static List<Color> DistributeColors(List<List<Color>> colorsTable)
    {
        int count = colorsTable.Count;
        int count2 = colorsTable[0].Count;
        List<Color> list = new List<Color>(count2 * count);
        for (int i = 0; i < 4; i++)
        {
            for (int j = i; j < 6; j++)
            {
                for (int k = 0; k < count; k++)
                {
                    int num = k * count2 / count;
                    list.Add(colorsTable[k][(i + num + j * 4) % count2]);
                }
            }
        }
        return list;
    }

    [MirrorVerified("Release IL")]
    public static List<Color> BuildDistributedDistinctColorsTable() => DistributeColors(GetColorsTable());
}
