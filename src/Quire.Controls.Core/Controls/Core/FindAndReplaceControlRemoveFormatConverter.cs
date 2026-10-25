#nullable disable
using System;
using System.Globalization;
using System.Text.RegularExpressions;
using Quire.Mirror;

namespace Quire.Controls.Core;

public partial class FindAndReplaceControlRemoveFormatConverter
{
    private readonly Regex _variableRegex = new Regex(@"^\(@LABEL VARIABLE (?<varValue>.*)\)");

    public FindAndReplaceControlRemoveFormatConverter() { }

    [MirrorVerified("PtcControlsCoreFindAndReplaceControlRemoveFormatConverterMechanicalDifferentialTests")]
    public object Convert(object value, Type targetType, object parameter, CultureInfo culture) => value;

    public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
    {
        if (value is string text && _variableRegex.IsMatch(text))
        {
            Match match = _variableRegex.Match(text);
            value = match.Success ? match.Groups["varValue"].ToString() : value;
        }
        return value;
    }
}
