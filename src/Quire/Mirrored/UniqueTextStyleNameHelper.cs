#nullable disable

using System.Windows;

namespace Quire;

public partial class UniqueTextStyleNameHelper
{
    public string TextStyleNames
    {
        get
        {
            return (string)GetValue(TextStyleNamesProperty);
        }
        set
        {
            SetValue(TextStyleNamesProperty, value);
        }
    }

    private UniqueTextStyleNameRule Validator { get; set; }

    public void SetValidator(UniqueTextStyleNameRule validator)
    {
        Validator = validator;
    }

    protected override Freezable CreateInstanceCore()
    {
        return new UniqueTextStyleNameHelper();
    }

    public UniqueTextStyleNameHelper() { }
}
