namespace Quire.utl_mgd;

public partial class UnaryVoidWrapper<T>
{
    private Function m_f;

    public UnaryVoidWrapper(Function f) => m_f = f;

    public void Invoke(T a1)
    {
        m_f.Invoke1(a1);
        GC.KeepAlive(this);
    }
}
