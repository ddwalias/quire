namespace Quire.Automation;

public partial class StringValueProxy
{
    protected object m_value;

    public override object Core => m_value;

    public override IStringValueProxy AsString => this;

    public string String => m_value.ToString();

    public StringValueProxy(global::Quire.Mcd.StrVal v, bool bSimplify, global::Quire.Mcd.RuntimeOptions o)
        : base(bSimplify, o)
    {
        m_value = v;
    }

    public StringValueProxy(string v, bool bSimplify, global::Quire.Mcd.RuntimeOptions o)
        : base(bSimplify, o)
    {
        m_value = v;
    }
}
