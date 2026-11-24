#nullable disable
#pragma warning disable

namespace Quire.Engine.Misc;

partial class Triple<T1, T2, T3>
{
    private T1 m_Datum1;
    private T2 m_Datum2;
    private T3 m_Datum3;

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

    public T3 Third
    {
        get => m_Datum3;
        set => m_Datum3 = value;
    }

    public Triple(T1 first, T2 second, T3 third)
    {
        m_Datum1 = first;
        m_Datum2 = second;
        m_Datum3 = third;
    }
}
