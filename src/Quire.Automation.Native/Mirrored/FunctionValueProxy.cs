namespace Quire.Automation;

public partial class FunctionValueProxy
{
    protected global::Quire.Mpl.Run.Function m_value;

    public override object Core => m_value;

    public override IFunctionValueProxy AsFunction => this;

    public string FunctionInfo => Core.ToString();

    public FunctionValueProxy(global::Quire.Mpl.Run.Function v, bool bSimplify, global::Quire.Mcd.RuntimeOptions o)
        : base(bSimplify, o)
    {
        m_value = v;
    }
}
