namespace Quire.Controls.Plots.ViewModel;

public partial class SExpressionFormatter
{
    public SExpressionFormatter(Core.NumberFormat numberFormat)
        : base(numberFormat, new SExpressionFormatterHelper())
    {
    }
}
