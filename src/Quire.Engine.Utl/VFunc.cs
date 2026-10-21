namespace Quire.utl_mgd;

internal partial class VFunc
{
    public Function m_function;

    public VFunc(Function f) => m_function = f;

    public void Invoke()
    {
        m_function.Invoke0();
        GC.KeepAlive(this);
    }
}
