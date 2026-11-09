#nullable disable
#pragma warning disable

namespace Quire.Controls.Plots;

public partial class XyzEquation
{
    private readonly string _zSExpression;

    public string ZsExpression => _zSExpression;

    public XyzEquation(string xSExpression, string ySExpression, string zSExpression)
        : base(xSExpression, ySExpression)
    {
        _zSExpression = zSExpression;
    }

    public XyzEquation()
        : this(string.Empty, string.Empty, "(@TRACE_EXPR @PLACEHOLDER (@TRACE_UNIT @PLACEHOLDER))")
    {
    }
}
