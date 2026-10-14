#nullable disable
using Quire.Mirror;

namespace Quire.Mpl.Run;

public partial class Cons
{
    [MirrorAnalyzed("Mpl.Run.Cons")] public object Field1;
    [MirrorAnalyzed("Mpl.Run.Cons")] public object Field2;
    [MirrorAnalyzed("Mpl.Run.Cons")] public object head => Field1;
    [MirrorAnalyzed("Mpl.Run.Cons")] public Cons tail => Field2 as Cons;
    [MirrorAnalyzed("Mpl.Run.Cons")]
    public Cons(object item, object list) { Field1 = item; Field2 = list; }
    [MirrorAnalyzed("Mpl.Run.Cons")]
    public static int Length(Cons x)
    {
        int result = 0;
        for (; x != null; x = x.tail) ++result;
        return result;
    }
    [MirrorAnalyzed("Mpl.Run.Cons")]
    private string FormatItems() => Field2 == null ? Field1.ToString() : Field1.ToString() + ", " + (Field2 as Cons).FormatItems();
    [MirrorAnalyzed("Mpl.Run.Cons")]
    public override string ToString() => "[" + FormatItems() + "]";
    [MirrorAnalyzed("Mpl.Run.Cons")]
    public static Cons destructive_append(Cons xs, Cons ys)
    {
        if (xs == null) return ys;
        var first = xs;
        while (xs.Field2 != null) xs = xs.Field2 as Cons;
        xs.Field2 = ys;
        return first;
    }
}
