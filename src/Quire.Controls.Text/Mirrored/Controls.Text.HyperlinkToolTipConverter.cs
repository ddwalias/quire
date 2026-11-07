#nullable disable
#pragma warning disable

using System;
using System.Globalization;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;

namespace Quire.Controls.Text;

public partial class HyperlinkToolTipConverter : IValueConverter
{
    public static HyperlinkToolTipConverter Instance = new HyperlinkToolTipConverter();

    public const string HyperlinkTooltipKey = "HyperlinkTooltipKey";

    public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
    {
        Hyperlink hyperlink = value as Hyperlink;
        ToolTip toolTip = null;
        if (hyperlink != null)
        {
            toolTip = hyperlink.TryFindResource(HyperlinkTooltipKey) as ToolTip;
        }
        if (DebugTools.AssertNotNull(toolTip).Failed)
        {
            return null;
        }
        toolTip.DataContext = hyperlink;
        return toolTip;
    }

    public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture) =>
        throw new NotImplementedException();

    public HyperlinkToolTipConverter()
    {
    }
}
