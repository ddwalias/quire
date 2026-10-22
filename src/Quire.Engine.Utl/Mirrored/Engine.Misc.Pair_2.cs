#nullable disable
#pragma warning disable

namespace Quire.Engine.Misc;

partial class Pair<T1, T2>
{
    private T1 m_Datum1;
    private T2 m_Datum2;

    public T1 First
    {
        get => m_Datum1;
        set => m_Datum1 = value;
    }

    public T2 Second
    {
        get => m_Datum2;
        set => m_Datum2 = value;
    }

    public Pair(T1 f, T2 s)
    {
        m_Datum1 = f;
        m_Datum2 = s;
    }
}
