using Quire.Controls.Core;

namespace Quire.Controls.Plots.ViewModel;

public partial class SExpressionFormatterHelper : IFormatterHelper
{
    public string Mult(string left, string right)
    {
        return SExpressionUtils.Mult(left, right);
    }

    public string Exponent(string left, string right)
    {
        return SExpressionUtils.Exponent(left, right);
    }

    public string Neg(string arg)
    {
        return SExpressionUtils.Neg(arg);
    }

    public string Percent(string arg)
    {
        return SExpressionUtils.Percent(arg);
    }
}
