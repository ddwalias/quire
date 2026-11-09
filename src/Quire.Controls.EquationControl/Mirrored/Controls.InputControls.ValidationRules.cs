using System.Globalization;
using System.Windows.Controls;

namespace Quire.Controls.InputControls;

public partial class DoubleRule
{
    public override ValidationResult Validate(object value, CultureInfo cultureInfo)
    {
        ValidationResult validResult = ValidationResult.ValidResult;
        double parsedValue;
        if (value != null && !double.TryParse(value.ToString(), (NumberStyles)167, CultureInfo.InvariantCulture, out parsedValue))
        {
            return new ValidationResult(false, "");
        }

        return validResult;
    }
}

public partial class UDoubleRule
{
    public override ValidationResult Validate(object value, CultureInfo cultureInfo)
    {
        ValidationResult validResult = ValidationResult.ValidResult;
        double parsedValue;
        string text = value != null ? value.ToString() : null;
        if (!double.TryParse(text, (NumberStyles)167, CultureInfo.InvariantCulture, out parsedValue) || parsedValue < 0.0)
        {
            return new ValidationResult(false, "");
        }

        return validResult;
    }
}
