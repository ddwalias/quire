#nullable disable

using System;
using System.Globalization;
using System.Windows.Controls;

namespace Quire.StatusBarControls;

public partial class GotoPageValidationRule
{
    private GotoPageValidatorHelper _helper;

    public int TotalPages { get; set; }

    public GotoPageValidatorHelper Helper
    {
        get => _helper;
        set
        {
            _helper = value;
            value?.SetValidator(this);
        }
    }

    public override ValidationResult Validate(object value, CultureInfo cultureInfo)
    {
        ValidationResult validResult = ValidationResult.ValidResult;
        int num = 0;
        try
        {
            if (((string)value).Length > 0)
            {
                num = int.Parse((string)value);
            }

            if (num < 1 || num > TotalPages)
            {
                return new ValidationResult(isValid: false, "goto page needs to be between 1 to TotalPages");
            }
        }
        catch (Exception ex)
        {
            return new ValidationResult(isValid: false, "Illegal characters or " + ex.Message);
        }

        return validResult;
    }
}
