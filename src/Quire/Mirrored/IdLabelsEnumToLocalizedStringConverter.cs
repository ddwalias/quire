#nullable disable

using System;
using System.Globalization;
using System.Linq;
using System.Windows;
using Quire.Controls.Core;

namespace Quire;

public partial class IdLabelsEnumToLocalizedStringConverter
{
    public object Convert(object[] values, Type targetType, object parameter, CultureInfo culture)
    {
        if (values.Any((object value) => value == DependencyProperty.UnsetValue))
        {
            return null;
        }

        StylesAndLabelsFormatRibbonBar stylesAndLabelsFormatRibbonBar = values[0] as StylesAndLabelsFormatRibbonBar;
        if (DebugTools.AssertNotNull(stylesAndLabelsFormatRibbonBar, null).Failed)
        {
            return null;
        }

        object obj = values[1];
        if (DebugTools.DevAssert(obj is IdLabels, null).Failed)
        {
            return null;
        }

        switch ((IdLabels)obj)
        {
            case IdLabels.Constant:
                return stylesAndLabelsFormatRibbonBar.TryFindResource("IdLabelsConstantString");
            case IdLabels.Function:
                return stylesAndLabelsFormatRibbonBar.TryFindResource("IdLabelsFunctionString");
            case IdLabels.Keyword:
                return stylesAndLabelsFormatRibbonBar.TryFindResource("IdLabelsKeywordString");
            case IdLabels.System:
                return stylesAndLabelsFormatRibbonBar.TryFindResource("IdLabelsSystemString");
            case IdLabels.Unit:
                return stylesAndLabelsFormatRibbonBar.TryFindResource("IdLabelsUnitString");
            case IdLabels.Variable:
                return stylesAndLabelsFormatRibbonBar.TryFindResource("IdLabelsVariableString");
            default:
                DebugTools.FailAssertion(null, Array.Empty<object>());
                return string.Empty;
        }
    }

    public object[] ConvertBack(object value, Type[] targetTypes, object parameter, CultureInfo culture)
    {
        throw new NotImplementedException();
    }

    public IdLabelsEnumToLocalizedStringConverter()
    {
    }
}
