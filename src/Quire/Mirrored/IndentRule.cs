#nullable disable

using System.Globalization;
using System.Windows.Controls;

namespace Quire;

public partial class IndentRule
{
    public override ValidationResult Validate(object value, CultureInfo cultureInfo)
    {
        if (value.ToString() == string.Empty)
        {
            return ValidationResult.ValidResult;
        }

        if (uint.TryParse(value?.ToString(), out var _))
        {
            return ValidationResult.ValidResult;
        }

        return new ValidationResult(isValid: false, "");
    }
}
