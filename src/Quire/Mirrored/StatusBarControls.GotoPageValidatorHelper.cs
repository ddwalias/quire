#nullable disable

using System.Windows;

namespace Quire.StatusBarControls;

public partial class GotoPageValidatorHelper
{
    public static readonly DependencyProperty TotalPagesProperty = DependencyProperty.Register(
        "TotalPages",
        typeof(int),
        typeof(GotoPageValidatorHelper),
        new PropertyMetadata((PropertyChangedCallback)OnTotalPagesChanged));

    public int TotalPages
    {
        get => (int)GetValue(TotalPagesProperty);
        set => SetValue(TotalPagesProperty, value);
    }

    private GotoPageValidationRule Validator { get; set; }

    public void SetValidator(GotoPageValidationRule validator)
    {
        Validator = validator;
    }

    private static void OnTotalPagesChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
    {
        GotoPageValidatorHelper gotoPageValidatorHelper = (GotoPageValidatorHelper)d;
        if (gotoPageValidatorHelper.Validator != null)
        {
            gotoPageValidatorHelper.Validator.TotalPages = (int)e.NewValue;
        }
    }

    protected override Freezable CreateInstanceCore()
    {
        return new GotoPageValidatorHelper();
    }
}
