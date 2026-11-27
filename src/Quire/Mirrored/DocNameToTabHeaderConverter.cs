#nullable disable

using System;
using System.Globalization;
using System.IO;
using System.Windows;
using Quire.Controls.Core;

namespace Quire;

internal partial class DocNameToTabHeaderConverter
{
    public DocNameToTabHeaderConverter() { }

    public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
    {
        string text = value as string;
        IEngineeringDocument engineeringDocument = (IEngineeringDocument)parameter;
        string templatePrefix = null;
        string extension = (string.IsNullOrEmpty(engineeringDocument.FilePath) ? null : Path.GetExtension(engineeringDocument.FilePath));
        if (engineeringDocument.IsDocumentATemplate || (extension != null && extension.Equals(".mctx", StringComparison.OrdinalIgnoreCase)))
        {
            templatePrefix = Application.Current.TryFindResource("Application.TabPrefixForTemplateFile") as string;
        }

        if (!string.IsNullOrEmpty(templatePrefix))
        {
            return string.Join(" | ", [templatePrefix, text]);
        }

        return text;
    }

    public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
    {
        throw new NotImplementedException();
    }
}
