using System.Globalization;
using System.Windows.Controls;

namespace Quire.Controls;

public partial class RangeRule : ValidationRule
{
    public int MaxValue { get; set; }

    public string Message { get; set; }

    public override ValidationResult Validate(object value, CultureInfo cultureInfo)
    {
        ValidationResult validResult = ValidationResult.ValidResult;
        if (!uint.TryParse(value?.ToString(), out var result))
        {
            return new ValidationResult(false, Message.Replace("{0}", MaxValue.ToString()));
        }
        if (result > MaxValue)
        {
            return new ValidationResult(false, Message.Replace("{0}", MaxValue.ToString()));
        }
        return validResult;
    }

    public RangeRule()
    {
    }
}
