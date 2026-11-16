using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Windows;
using Quire.Core;

namespace Quire.Controls.Worksheet;

public partial class NameBuilderHelper
{
    private const string PosPrefix = "pos";

    private const string SizePrefix = "sz";

    private const string DoublePairDelimiter = "x";

    private const string DoubleEmpty = "empty";

    private const string DoubleNegativeSign = "N";

    private const char RegionNameSeparatorChar = '_';

    private static readonly string _regionNameSeparator;

    private static readonly NumberFormatInfo _numberFormatForRegionName;

    internal static string DoubleToString(double value)
    {
        if (!DebugTools.DevAssert(value.IsValid(), null).Failed)
        {
            return value.ToString("F2", _numberFormatForRegionName);
        }

        return "empty";
    }

    private static string PositionToString(Point location)
    {
        return DoublePairToString("pos", location.X, location.Y);
    }

    private static string RectToString(Rect rect)
    {
        return Combine(PositionToString(rect.Location), SizeToString(rect.Size));
    }

    private static string SizeToString(Size size)
    {
        return DoublePairToString("sz", size.Width, size.Height);
    }

    private static string DoublePairToString(string prefix, double x, double y)
    {
        return Combine(prefix, DoubleToString(x), "x", DoubleToString(y));
    }

    public static string StripDimensionsFromName(string name)
    {
        int length = name.IndexOf("pos");
        name = name.Substring(0, length);
        name = name.TrimEnd('_');
        return name;
    }

    public static string Combine(params object[] values)
    {
        IEnumerable<object> values2 = values.Select((object obj) =>
        {
            if (obj is Rect)
            {
                return RectToString((Rect)obj);
            }

            if (obj is Size)
            {
                return SizeToString((Size)obj);
            }

            if (obj is Point)
            {
                return PositionToString((Point)obj);
            }

            if (obj is double)
            {
                return DoubleToString((double)obj);
            }

            return obj;
        });
        return string.Join(_regionNameSeparator, values2);
    }

    public NameBuilderHelper()
    {
    }

    static NameBuilderHelper()
    {
        _regionNameSeparator = '_'.ToString();
        _numberFormatForRegionName = new NumberFormatInfo
        {
            NumberDecimalDigits = 2,
            NumberDecimalSeparator = _regionNameSeparator,
            NegativeSign = "N"
        };
    }
}
