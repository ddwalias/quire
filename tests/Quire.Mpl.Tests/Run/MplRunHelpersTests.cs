using Quire.Mpl.Run;
using MTuple = Quire.Mpl.Run.Tuple;

namespace Quire.Mpl.Tests.Run;

/// <summary>
/// Behaviour of <see cref="Cons"/>, <see cref="VecBuilder"/>, <see cref="True"/>, <see cref="MTuple"/> and
/// <see cref="MplSignature"/> against their records in docs/mirror/evidence; MplRunHelpersDifferentialTests compares
/// the same members with Mathcad.
/// </summary>
public class MplRunHelpersTests
{
    [Fact]
    public void Cons_lists_print_count_and_append()
    {
        var xs = new Cons(1, new Cons("b", null));
        Assert.Equal("[1, b]", xs.ToString());
        Assert.Equal("[[1, b]]", new Cons(xs, null).ToString());
        Assert.Equal(2, Cons.Length(xs));
        Assert.Equal(0, Cons.Length(null!));
        Assert.Equal(1, Cons.Length(new Cons(1, "x")));
        Assert.Throws<NullReferenceException>(() => new Cons(1, "x").ToString());
        Assert.Throws<NullReferenceException>(() => new Cons(null, null).ToString());
        var ys = new Cons(3, null);
        Assert.Same(ys, Cons.destructive_append(null!, ys));
        Assert.Same(xs, Cons.destructive_append(xs, ys));
        Assert.Equal("[1, b, 3]", xs.ToString());
        Assert.Throws<NullReferenceException>(() => Cons.destructive_append(new Cons(1, "x"), ys));
    }

    [Fact]
    public void VecBuilder_fills_from_the_front()
    {
        var v = VecBuilder.make(4);
        Assert.Same(v, v.Push1("a").Push2("b", "c"));
        Assert.Equal(3, v.cnt);
        Assert.Equal("[a, b, c]", v.AsList().ToString());
        Assert.Equal(new object?[] { "a", "b", "c", null }, v.AsVector());
        Assert.Same(v.AsVector(), v.vector);
        Assert.Throws<IndexOutOfRangeException>(() => v.Push2("d", "e"));
        Assert.Equal(5, v.cnt);
        Assert.Null(VecBuilder.make(0).AsList());
        Assert.Throws<IndexOutOfRangeException>(() => new VecBuilder(3, ["x"]).AsList());
        Assert.Equal(4, VecBuilder.make(4).Push4(1, 2, 3, 4).cnt);
        Assert.Equal(3, VecBuilder.make(3).Push3(1, 2, 3).cnt);
    }

    [Fact]
    public void True_prints_its_name()
    {
        Assert.Equal("True", True.singleton.ToString());
        Assert.NotSame(True.singleton, new True());
    }

    [Fact]
    public void Tuple_representations()
    {
        Assert.Equal(0, MTuple.tuple_length(null!));
        Assert.Equal(3, MTuple.tuple_length(new object[3]));
        Assert.Equal(1, MTuple.tuple_length(new int[3]));
        Assert.True(MTuple.AllObjects([typeof(string), typeof(object)]));
        Assert.False(MTuple.AllObjects([typeof(string), typeof(int)]));
        Assert.True(MTuple.AllObjects([]));
        Assert.Equal(typeof(Quire.Mpl.Run.Tuple<int, string>), MTuple.TupleType([typeof(int), typeof(string)]));
        Assert.Null(MTuple.TupleType(new System.Type[6]));
        Assert.Throws<NullReferenceException>(() => MTuple.TupleType([typeof(int)]));

        Assert.Equal(typeof(int), MTuple.TupleRep(out var rep, [typeof(int)]));
        Assert.Equal(MTuple.Rep.Singleton, rep);
        Assert.Equal(typeof(object[]), MTuple.TupleRep(out rep, [typeof(string), typeof(object)]));
        Assert.Equal(MTuple.Rep.ObjVector, rep);
        Assert.Equal(typeof(Quire.Mpl.Run.Tuple<int, int, int>), MTuple.TupleRep(out rep, [typeof(int), typeof(int), typeof(int)]));
        Assert.Equal(MTuple.Rep.TupleType, rep);
        Assert.Equal(typeof(object[]), MTuple.TupleRep(out rep, Enumerable.Repeat(typeof(int), 6).ToArray()));
        Assert.Equal(MTuple.Rep.HeteroVector, rep);
        // No types at all are "all objects".
        Assert.Equal(typeof(object[]), MTuple.TupleRep(out rep, []));
        Assert.Equal(MTuple.Rep.ObjVector, rep);
    }

    [Fact]
    public void Tuple_fields_and_accessors()
    {
        var pair = typeof(Quire.Mpl.Run.Tuple<int, string>);
        Assert.Equal("m_2", MTuple.GetFieldInfo(pair, 2).Name);
        Assert.Null(MTuple.GetFieldInfo(pair, 3));
        Assert.Null(MTuple.GetFieldInfo(null!, 1));
        object?[] tup = [1, "s"];
        Assert.Equal("s", MTuple.Getter(typeof(string)).Invoke(null, [tup, 1]));
        MTuple.Setter(typeof(int)).Invoke(null, [tup, 1, 5]);
        Assert.Equal(5, tup[1]);
        Assert.Throws<InvalidCastException>(() => MTuple.GetField<int>(["s"], 0));
        MTuple.SetField<string>(tup, 0, "x");
        Assert.Equal("x", tup[0]);
        var t = new Quire.Mpl.Run.Tuple<int, string, double, char, bool> { m_1 = 1, m_5 = true };
        Assert.Equal((1, true), (t.m_1, t.m_5));
    }

    [Fact]
    public void Signatures_compare_position_by_position()
    {
        var a = new MplSignature(typeof(int), [typeof(int), typeof(string)]);
        var same = new MplSignature(typeof(int), [typeof(int), typeof(string)]);
        var longer = new MplSignature(typeof(int), [typeof(int), typeof(string), typeof(double)]);
        Assert.True(a.Equals(same));
        Assert.Equal(a.GetHashCode(), same.GetHashCode());
        Assert.True(a.Equals(longer));
        Assert.Throws<IndexOutOfRangeException>(() => longer.Equals(a));
        Assert.False(a.Equals(new MplSignature(typeof(string), [typeof(int), typeof(string)])));
        Assert.False(a.Equals(null));
        Assert.False(a.Equals("x"));
        int expected = unchecked((typeof(int).GetHashCode() + typeof(int).GetHashCode() + typeof(string).GetHashCode()) * 64 + 2);
        Assert.Equal(expected, a.GetHashCode());
        Assert.Throws<NullReferenceException>(() => new MplSignature(null!, []).GetHashCode());
        System.Type[] p = [typeof(int)];
        Assert.Same(p, new MplSignature(typeof(void), p).ArgTypes);
        Assert.Equal(typeof(void), new MplSignature(typeof(void), p).RetType);
    }
}
