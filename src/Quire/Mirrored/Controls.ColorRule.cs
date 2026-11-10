using System.Globalization;
using System.Text.RegularExpressions;
using System.Windows.Controls;

namespace Quire.Controls;

public partial class ColorRule : ValidationRule
{
    private const int ColorValueLength = 6;
    private const string PatternAlphaNumeric = "^[0-9a-fA-F]+$";

    public string Message { get; set; }

    public override ValidationResult Validate(object value, CultureInfo cultureInfo)
    {
        ValidationResult validResult = ValidationResult.ValidResult;
        Regex regex = new Regex("^[0-9a-fA-F]+$");
        if (value == null || ((string)value).Length != 6 || !regex.IsMatch((string)value))
        {
            return new ValidationResult(false, Message);
        }
        return validResult;
    }

    public ColorRule()
    {
    }
}
