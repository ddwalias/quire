#nullable disable
using System.Windows.Media;
using Quire.Mirror;

namespace Quire.Wpf;

public static partial class ColorUtilities
{
    public static Color[] baseColors = new Color[6]
    {
        new Color
        {
            A = byte.MaxValue,
            R = 125,
            G = 125,
            B = byte.MaxValue
        },
        new Color
        {
            A = byte.MaxValue,
            R = 25,
            G = byte.MaxValue,
            B = byte.MaxValue
        },
        new Color
        {
            A = byte.MaxValue,
            R = 50,
            G = 200,
            B = 50
        },
        new Color
        {
            A = byte.MaxValue,
            R = byte.MaxValue,
            G = 200,
            B = 0
        },
        new Color
        {
            A = byte.MaxValue,
            R = byte.MaxValue,
            G = 91,
            B = 0
        },
        new Color
        {
            A = byte.MaxValue,
            R = 208,
            G = 0,
            B = byte.MaxValue
        }
    };

    public static Color[] grayScaleColors = new Color[3]
    {
        new Color
        {
            A = byte.MaxValue,
            R = 130,
            G = 130,
            B = 130
        },
        new Color
        {
            A = byte.MaxValue,
            R = 160,
            G = 160,
            B = 160
        },
        new Color
        {
            A = byte.MaxValue,
            R = 190,
            G = 190,
            B = 190
        }
    };

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
