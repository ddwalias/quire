#nullable disable
#pragma warning disable

namespace Quire.Controls.Plots;

public partial class XyEquation
{
    private readonly string _xSExpression;
    private readonly string _ySExpression;

    public string XsExpression => _xSExpression;

    public string YsExpression => _ySExpression;

    public XyEquation(string xSExpression, string ySExpression)
    {
        _xSExpression = xSExpression;
        _ySExpression = ySExpression;
    }

    public XyEquation()
        : this(string.Empty, string.Empty)
    {
    }
}
