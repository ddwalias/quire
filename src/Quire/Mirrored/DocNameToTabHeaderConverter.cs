#nullable disable

using System;
using System.Globalization;
using System.IO;
using System.Windows;
using Quire.Controls.Core;

namespace Quire;

internal partial class DocNameToTabHeaderConverter
{
    public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
    {
        string text = value as string;
        IEngineeringDocument engineeringDocument = (IEngineeringDocument)parameter;
        string text2 = null;
        string text3 = (string.IsNullOrEmpty(engineeringDocument.FilePath) ? null : Path.GetExtension(engineeringDocument.FilePath));
        if (engineeringDocument.IsDocumentATemplate || (text3 != null && text3.Equals(".mctx", StringComparison.OrdinalIgnoreCase)))
        {
            text2 = Application.Current.TryFindResource("Application.TabPrefixForTemplateFile") as string;
        }

        if (!string.IsNullOrEmpty(text2))
        {
            return string.Join(" | ", [text2, text]);
        }

        return text;
    }

    public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
    {
        throw new NotImplementedException();
    }
}
