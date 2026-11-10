#nullable disable

using System;
using System.Globalization;
using System.Windows;
using System.Windows.Media;

namespace Quire;

public partial class FlowDocumentFamilyConverter
{
    public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
    {
        if (value == null)
        {
            return null;
        }

        string source = ((FontFamily)value).Source;
        if (source == null)
        {
            DebugTools.FailAssertion("Unexpected: source of FontFamily is null");
            return null;
        }

        if (source.Equals("Mathcad UniMath Prime") && !Fonts.SystemFontFamilies.Contains(new FontFamily("Mathcad UniMath Prime")))
        {
            // Mathcad joins the font path at run time; a literal + literal would be folded by the compiler.
            return new FontFamily(new Uri("pack://application:,,,"), string.Concat("Ptc.Controls.Core;component/Fonts/#", "Mathcad UniMath Prime"));
        }

        if (source.Equals("Mathcad UniMath") && !Fonts.SystemFontFamilies.Contains(new FontFamily("Mathcad UniMath")))
        {
            if (source.Equals("Mathcad UniMath") && !LocalizedFontNameConverter.IsUniMathFontExist())
            {
                return new FontFamily(new Uri("pack://application:,,,"), string.Concat("Ptc.Controls.Core;component/Fonts/#", "Mathcad UniMath"));
            }

            return new FontFamily(new Uri("pack://application:,,,"), string.Concat("Ptc.Controls.Core;component/Fonts/#", "Mathcad UniMath Prime"));
        }

        return value;
    }

    public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
    {
        if (value == null || value.ToString() == string.Empty)
        {
            return null;
        }

        return value;
    }
}
