namespace Quire.Automation;

public partial class SymbolicValueProxy
{
    protected object m_value;

    public override object Core => m_value;

    public override ISymbolicValueProxy AsSymbolic => this;

    public object Symbolic => m_value;

    public SymbolicValueProxy(object v, bool bSimplify, global::Quire.Mcd.RuntimeOptions o)
        : base(bSimplify, o)
    {
        m_value = v;
    }
}
