#nullable disable
#pragma warning disable

namespace Quire.Efi;

internal partial struct Pair<T>
{
    public T x;

    public T y;

    public Pair(T xx, T yy)
    {
        x = xx;
        y = yy;
    }
}
