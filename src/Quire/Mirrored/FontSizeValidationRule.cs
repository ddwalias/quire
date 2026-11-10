#nullable disable

using System.Globalization;
using System.Windows.Controls;

namespace Quire;

public partial class FontSizeValidationRule
{
    private const double MinFontSizeForText = 1.0;
    private const double MaxFontSizeForText = 1638.0;
    private const int PrecisionForText = 1;

    protected virtual double MinFontSize => 1.0;

    protected virtual double MaxFontSize => 1638.0;

    protected virtual int Precision => 1;

    public static bool ValidateFontSizeStringInput(string input, CultureInfo cultureInfo, out double fontSize)
    {
        bool result = false;
        fontSize = 0.0;
        if (!string.IsNullOrEmpty(input))
        {
            result = double.TryParse(input, NumberStyles.AllowDecimalPoint, cultureInfo, out fontSize);
        }

        return result;
    }

    public override ValidationResult Validate(object value, CultureInfo cultureInfo)
    {
        string text = value as string;
        if (!ValidateFontSizeStringInput(text, cultureInfo, out var fontSize))
        {
            return new ValidationResult(isValid: false, null);
        }

        DebugTools.DevAssert(!string.IsNullOrEmpty(text), null);
        if (fontSize < MinFontSize || fontSize > MaxFontSize)
        {
            return new ValidationResult(isValid: false, null);
        }

        int num = text.IndexOf('.');
        if (num >= 0 && text.Substring(num + 1).Length > Precision)
        {
            return new ValidationResult(isValid: false, null);
        }

        return ValidationResult.ValidResult;
    }
}
