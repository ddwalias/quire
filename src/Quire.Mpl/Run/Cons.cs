#nullable disable
using Quire.Mirror;

namespace Quire.Mpl.Run;

/// <summary>
/// A runtime list cell holding any objects, ended by null (docs/mirror/evidence/Mpl.Run.Cons.md). Prints as
/// <c>[a, b, c]</c>.
/// </summary>
public partial class Cons
{
    [MirrorVerified("MplRunHelpersDifferentialTests")]
    public object Field1;

    [MirrorVerified("MplRunHelpersDifferentialTests")]
    public object Field2;

    [MirrorVerified("MplRunHelpersDifferentialTests")]
    public Cons(object item, object list)
    {
        Field1 = item;
        Field2 = list;
    }

    [MirrorVerified("MplRunHelpersDifferentialTests")]
    public object head => Field1;

    [MirrorVerified("MplRunHelpersDifferentialTests")]
    public Cons tail => Field2 as Cons;

    /// <summary>The number of cells reachable through <see cref="tail"/>; 0 for null.</summary>
    [MirrorVerified("MplRunHelpersDifferentialTests")]
    public static int Length(Cons x)
    {
        int n = 0;
        for (; x != null; x = x.tail) n++;
        return n;
    }

    /// <summary>
    /// The items joined by ", ". Stops only at a null <see cref="Field2"/>, so a non-list tail or a null item throws
    /// <see cref="NullReferenceException"/>.
    /// </summary>
    [MirrorVerified("MplRunHelpersDifferentialTests")]
    private string FormatItems()
    {
        if (Field2 == null)
        {
            return Field1.ToString();
        }
        return Field1.ToString() + ", " + (Field2 as Cons).FormatItems();
    }

    [MirrorVerified("MplRunHelpersDifferentialTests")]
    public override string ToString() => "[" + FormatItems() + "]";

    /// <summary>
    /// Links <paramref name="ys"/> after the last cell of <paramref name="xs"/> and returns <paramref name="xs"/>, or
    /// <paramref name="ys"/> when <paramref name="xs"/> is null. The last cell is the one whose <see cref="Field2"/> is
    /// null; a non-list tail on the way throws.
    /// </summary>
    [MirrorVerified("MplRunHelpersDifferentialTests")]
    public static Cons destructive_append(Cons xs, Cons ys)
    {
        if (xs == null) return ys;
        Cons last = xs;
        while (last.Field2 != null) last = last.Field2 as Cons;
        last.Field2 = ys;
        return xs;
    }
}

/// <summary>Fills a fixed-size object array from the front (docs/mirror/evidence/Mpl.Run.VecBuilder.md).</summary>
public partial class VecBuilder
{
    /// <summary>The number of slots filled.</summary>
    [MirrorVerified("MplRunHelpersDifferentialTests")]
    public int Field1;

    [MirrorVerified("MplRunHelpersDifferentialTests")]
    public object[] Field2;

    [MirrorVerified("MplRunHelpersDifferentialTests")]
    public VecBuilder(int i, object[] v)
    {
        Field1 = i;
        Field2 = v;
    }

    [MirrorVerified("MplRunHelpersDifferentialTests")]
    public object vector => Field2;

    [MirrorVerified("MplRunHelpersDifferentialTests")]
    public int cnt => Field1;

    [MirrorVerified("MplRunHelpersDifferentialTests")]
    public static VecBuilder make(int n) => new(0, new object[n]);

    /// <summary>The first <see cref="cnt"/> slots as a <see cref="Cons"/> list; null when none are filled.</summary>
    [MirrorVerified("MplRunHelpersDifferentialTests")]
    public Cons AsList()
    {
        Cons list = null;
        for (int i = Field1; i > 0;) list = new Cons(Field2[--i], list);
        return list;
    }

    /// <summary>The whole array, unfilled slots included.</summary>
    [MirrorVerified("MplRunHelpersDifferentialTests")]
    public object[] AsVector() => Field2;

    /// <summary>Stores in the next slot. Past the end it throws, after the count has already moved on.</summary>
    [MirrorVerified("MplRunHelpersDifferentialTests")]
    public VecBuilder Push1(object a)
    {
        Field2[Field1++] = a;
        return this;
    }

    [MirrorVerified("MplRunHelpersDifferentialTests")]
    public VecBuilder Push2(object a, object b)
    {
        Field2[Field1++] = a;
        Field2[Field1++] = b;
        return this;
    }

    [MirrorVerified("MplRunHelpersDifferentialTests")]
    public VecBuilder Push3(object a, object b, object c)
    {
        Field2[Field1++] = a;
        Field2[Field1++] = b;
        Field2[Field1++] = c;
        return this;
    }

    [MirrorVerified("MplRunHelpersDifferentialTests")]
    public VecBuilder Push4(object a, object b, object c, object d)
    {
        Field2[Field1++] = a;
        Field2[Field1++] = b;
        Field2[Field1++] = c;
        Field2[Field1++] = d;
        return this;
    }
}
