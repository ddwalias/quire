namespace Quire.Automation;

public partial class ScalarValueProxy
{
    protected global::Quire.Mcd.NumVal m_value;

    public override object Core => m_value;

    public override IScalarValueProxy AsScalar => this;

    public double RealPart
    {
        get
        {
            double magnitude = Unit.Magnitude;
            return m_value.Real / magnitude;
        }
    }

    public double ImagPart
    {
        get
        {
            double magnitude = Unit.Magnitude;
            return m_value.Imag / magnitude;
        }
    }

    public ScalarValueProxy(global::Quire.Mcd.NumVal v, bool bSimplify, global::Quire.Mcd.RuntimeOptions o)
        : base(bSimplify, o)
    {
        m_value = v;
    }
}
