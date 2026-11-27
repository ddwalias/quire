using System.Globalization;
using System.Windows.Controls;

namespace Quire.Controls.CustomNumericUpDown;

public partial class NumericUpDownRangeRule : ValidationRule
{
    public NumericUpDownValidationRuleWrapper Wrapper { get; set; }

    public string Message { get; set; }

    public override ValidationResult Validate(object value, CultureInfo cultureInfo)
    {
        ValidationResult result = ValidationResult.ValidResult;
        string errorContent = "";
        if (!uint.TryParse(value?.ToString(), out var parsedValue))
        {
            result = new ValidationResult(false, errorContent);
        }
        else if (Wrapper.MinValue > parsedValue || parsedValue > Wrapper.MaxValue)
        {
            result = new ValidationResult(false, errorContent);
        }
        return result;
    }

    public NumericUpDownRangeRule()
    {
    }
}
