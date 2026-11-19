using System;
using System.Collections.Generic;
using System.Globalization;
using System.Windows.Data;
using Quire.Controls.Worksheet;

namespace Quire;

public partial class SavedDateIsCheckedConverter
{
    private readonly global::System.Collections.Generic.Dictionary<string, string> DateFormatsDictionary;
    public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
    {
        if (value as ISavedDateControl != null)
        {
            return DateFormatsDictionary[(value as ISavedDateControl).TemplateFormat];
        }

        return null;
    }
    public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
    {
        return parameter;
    }
}
