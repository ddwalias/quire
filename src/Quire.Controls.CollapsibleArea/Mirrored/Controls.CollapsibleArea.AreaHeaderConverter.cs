#nullable disable
#pragma warning disable

using System;
using System.Collections;
using System.Globalization;
using System.Windows.Data;

namespace Quire.Controls.CollapsibleArea;

[ValueConversion(typeof(ProtectionData), typeof(bool))]
public partial class AreaHeaderConverter
{
    public static AreaHeaderConverter Instance = new AreaHeaderConverter();

    public virtual object Convert(object value, Type targetType, object parameter, CultureInfo culture)
    {
        string text = null;
        DateTime? dateTime = null;
        ProtectionData protectionData = value as ProtectionData;
        IDictionary dictionary = parameter as IDictionary;
        if (DebugTools.DevAssert(dictionary != null, null).Failed)
        {
            return null;
        }
        string key = "DefaultAreaHeaderKey";
        if (protectionData != null)
        {
            if (protectionData.ProtectionLevel().IsProtected())
            {
                key = protectionData.HasPassword ? "PasswordProtectedAreaHeaderKey" : "ProtectedAreaHeaderKey";
            }
            else
            {
                DebugTools.DevAssert(false, null);
            }
            dateTime = protectionData.ShowTimestamp && DebugTools.DevAssert(protectionData.Timestamp.HasValue, null).Passed
                ? protectionData.Timestamp
                : (DateTime?)null;
        }
        if (DebugTools.DevAssert(dictionary.Contains(key), null).Passed)
        {
            text = dictionary[key] as string;
        }
        if (!dateTime.HasValue)
        {
            return text;
        }
        return string.Format(CultureInfo.CurrentCulture, "{0}  {1:f}", text, dateTime.Value);
    }

    public virtual object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
    {
        throw new NotImplementedException();
    }

    public AreaHeaderConverter()
    {
    }
}
