using Quire.Mpl;
using Quire.Mpl.Run;
using static Quire.Differential.Tests.MplRunSides;

namespace Quire.Differential.Tests;

/// <summary>
/// Compares Quire's <c>Cons</c>, <c>VecBuilder</c>, <c>True</c>, <c>Tuple</c> (and its generic classes) and
/// <c>Mpl.MplSignature</c> with mplrun's on the same inputs. The cases are listed in the evidence records
/// docs/mirror/evidence/Mpl.Run.Cons.md, Mpl.Run.VecBuilder.md, Mpl.Run.True.md, Mpl.Run.Tuple.md and
/// Mpl.MplSignature.md.
/// </summary>
public class MplRunHelpersDifferentialTests
{
    private static Cons? List(params object?[] xs)
    {
        Cons? c = null;
        for (int i = xs.Length - 1; i >= 0; i--) c = new Cons(xs[i], c);
        return c;
    }

    private static List<Cons?> Lists() =>
    [
        null, List(1), List(1, "two", 3.5), new Cons(1, "tail"), new Cons(null, null), List(List(1, 2), 3),
        new Cons(1, new Cons(2, 5)), List(true, 'c', null),
    ];

    [SkippableFact]
    public void Cons_members()
    {
        var diffs = new List<string>();
        var mt = M("Mpl.Run.Cons");
        foreach (var q in Lists())
        {
            var m = ToMathcad(q);
            string label = Outcome(() => q);
            Same(diffs, $"copy of {label}", () => q, () => m);
            Same(diffs, $"Length({label})", () => Cons.Length(q!), () => Static(mt, "Length", ["Cons"], m));
            if (q == null) continue;
            Same(diffs, $"{label}.ToString", () => q.ToString(), () => m!.ToString());
            Same(diffs, $"{label}.head", () => q.head, () => Get(m!, "head"));
            Same(diffs, $"{label}.tail", () => q.tail, () => Get(m!, "tail"));
            Same(diffs, $"new Cons({label})", () => new Cons(q, q.Field1), () => New(mt, ["Object", "Object"], m, Get(m!, "Field1")));
        }
        foreach (var xs in Lists())
            foreach (var ys in Lists())
            {
                // Fresh copies: the append mutates its first argument.
                var qx = Copy(xs);
                var qy = Copy(ys);
                var mx = ToMathcad(qx);
                var my = ToMathcad(qy);
                Same(diffs, $"destructive_append({Outcome(() => xs)}, {Outcome(() => ys)})",
                    () => Pair(Cons.destructive_append(qx!, qy!), qx),
                    () => Pair(Static(mt, "destructive_append", ["Cons", "Cons"], mx, my), mx));
            }
        NoDiffs(diffs);
    }

    private static Cons? Copy(Cons? c) =>
        c == null ? null : new Cons(c.Field1 is Cons h ? Copy(h) : c.Field1, c.Field2 is Cons t ? Copy(t) : c.Field2);

    private static object?[] Pair(object? a, object? b) => [a, b];

    [SkippableFact]
    public void VecBuilder_members()
    {
        var diffs = new List<string>();
        var mt = M("Mpl.Run.VecBuilder");
        foreach (int n in new[] { 0, 1, 3, 4, 10 })
        {
            for (int pushes = 0; pushes <= 4; pushes++)
            {
                var q = VecBuilder.make(n);
                var m = Static(mt, "make", ["Int32"], n)!;
                Same(diffs, $"make({n})", () => q, () => m);
                object?[] items = ["a", 2, null, 4.5];
                string push = "Push" + pushes;
                if (pushes > 0)
                {
                    string[] sig = Enumerable.Repeat("Object", pushes).ToArray();
                    var args = items.Take(pushes).ToArray();
                    Same(diffs, $"make({n}).{push}", () => SameOrNew(q, Call(q, push, sig, args)), () => SameOrNew(m, Call(m, push, sig, args)));
                    Same(diffs, $"make({n}).{push} then {push}", () => SameOrNew(q, Call(q, push, sig, args)), () => SameOrNew(m, Call(m, push, sig, args)));
                }
                Same(diffs, $"make({n}) after {push}: state", () => q, () => m);
                Same(diffs, $"make({n}) after {push}: cnt", () => q.cnt, () => Get(m, "cnt"));
                Same(diffs, $"make({n}) after {push}: vector", () => q.vector, () => Get(m, "vector"));
                Same(diffs, $"make({n}) after {push}: AsVector", () => q.AsVector(), () => Call(m, "AsVector", []));
                Same(diffs, $"make({n}) after {push}: AsList", () => q.AsList(), () => Call(m, "AsList", []));
            }
        }
        foreach (var (i, v) in new (int, object[]?)[] { (2, ["x", "y", "z"]), (5, ["x"]), (-1, ["x"]), (0, null), (1, null) })
        {
            var q = new VecBuilder(i, v!);
            var m = New(mt, ["Int32", "Object[]"], i, v)!;
            Same(diffs, $"new VecBuilder({i}, {Outcome(() => v)}).AsList", () => q.AsList(), () => Call(m, "AsList", []));
            Same(diffs, $"new VecBuilder({i}, {Outcome(() => v)}).Push1", () => SameOrNew(q, q.Push1("p")), () => SameOrNew(m, Call(m, "Push1", ["Object"], "p")));
            Same(diffs, $"new VecBuilder({i}, {Outcome(() => v)}) state", () => q, () => m);
        }
        NoDiffs(diffs);
    }

    private static object? SameOrNew(object input, object? result) => ReferenceEquals(input, result) ? "same" : result;

    [SkippableFact]
    public void True_members()
    {
        var diffs = new List<string>();
        var mt = M("Mpl.Run.True");
        Same(diffs, "singleton", () => True.singleton, () => Get(mt, "singleton"));
        Same(diffs, "singleton.ToString", () => True.singleton.ToString(), () => Get(mt, "singleton")!.ToString());
        Same(diffs, "new True().ToString", () => new True().ToString(), () => New(mt, [])!.ToString());
        NoDiffs(diffs);
    }

    private static readonly System.Type[][] TypeLists =
    [
        [], [typeof(int)], [typeof(string)], [typeof(string), typeof(object)], [typeof(int), typeof(string)],
        [typeof(double), typeof(int), typeof(string)], [typeof(int), typeof(int), typeof(int), typeof(int)],
        [typeof(int), typeof(int), typeof(int), typeof(int), typeof(int)],
        [typeof(int), typeof(int), typeof(int), typeof(int), typeof(int), typeof(int)],
        [typeof(string), typeof(string), typeof(string), typeof(string), typeof(string), typeof(string), typeof(string)],
        [typeof(DateTime), typeof(string)], [typeof(int?), typeof(object)],
    ];

    [SkippableFact]
    public void Tuple_members()
    {
        var diffs = new List<string>();
        var qt = typeof(Quire.Mpl.Run.Tuple);
        var mt = M("Mpl.Run.Tuple");
        foreach (object? o in new object?[] { null, new object[3], new object[0], 5, "s", new int[2], new string[4] })
            Same(diffs, $"tuple_length({Outcome(() => o)})", () => Quire.Mpl.Run.Tuple.tuple_length(o!), () => Static(mt, "tuple_length", ["Object"], o));
        foreach (var tys in TypeLists)
        {
            string label = string.Join(",", tys.Select(t => t.Name));
            Same(diffs, $"AllObjects({label})", () => Quire.Mpl.Run.Tuple.AllObjects(tys), () => Static(mt, "AllObjects", ["Type[]"], (object)tys));
            Same(diffs, $"TupleType({label})", () => Quire.Mpl.Run.Tuple.TupleType(tys), () => Static(mt, "TupleType", ["Type[]"], (object)tys));
            Same(diffs, $"IType({label})", () => Static(qt, "IType", ["Type[]"], (object)tys), () => Static(mt, "IType", ["Type[]"], (object)tys));
            object?[] qa = [null, tys], ma = [null, tys];
            Same(diffs, $"TupleRep({label})", () => Pair(Static(qt, "TupleRep", ["Rep", "Type[]"], qa), qa[0]),
                () => Pair(Static(mt, "TupleRep", ["Rep", "Type[]"], ma), ma[0]));
        }
        var qPair = Quire.Mpl.Run.Tuple.TupleType([typeof(int), typeof(string)]);
        var mPair = (System.Type)Static(mt, "TupleType", ["Type[]"], (object)new[] { typeof(int), typeof(string) })!;
        foreach (int n in new[] { 0, 1, 2, 3 })
        {
            Same(diffs, $"GetFieldInfo(pair, {n})", () => Quire.Mpl.Run.Tuple.GetFieldInfo(qPair, n), () => Static(mt, "GetFieldInfo", ["Type", "Int32"], mPair, n));
            Same(diffs, $"GetFieldInfo(null, {n})", () => Quire.Mpl.Run.Tuple.GetFieldInfo(null!, n), () => Static(mt, "GetFieldInfo", ["Type", "Int32"], null, n));
            Same(diffs, $"GetFieldInfo(string, {n})", () => Quire.Mpl.Run.Tuple.GetFieldInfo(typeof(string), n), () => Static(mt, "GetFieldInfo", ["Type", "Int32"], typeof(string), n));
        }
        foreach (var ty in new[] { typeof(int), typeof(string), typeof(object) })
        {
            Same(diffs, $"Getter({ty.Name})", () => Quire.Mpl.Run.Tuple.Getter(ty), () => Static(mt, "Getter", ["Type"], ty));
            Same(diffs, $"Setter({ty.Name})", () => Quire.Mpl.Run.Tuple.Setter(ty), () => Static(mt, "Setter", ["Type"], ty));
            foreach (object?[] tup in new object?[][] { [1, "s", null], [] })
                foreach (int i in new[] { 0, 1, 2 })
                {
                    var qs = (object?[])tup.Clone();
                    var ms = (object?[])tup.Clone();
                    Same(diffs, $"Getter({ty.Name}) on {Outcome(() => tup)}[{i}]", () => Quire.Mpl.Run.Tuple.Getter(ty).Invoke(null, [qs, i]),
                        () => ((System.Reflection.MethodInfo)Static(mt, "Getter", ["Type"], ty)!).Invoke(null, [ms, i]));
                    object? v = ty == typeof(int) ? 7 : "v";
                    Same(diffs, $"Setter({ty.Name}) on {Outcome(() => tup)}[{i}]", () => Pair(Quire.Mpl.Run.Tuple.Setter(ty).Invoke(null, [qs, i, v]), qs),
                        () => Pair(((System.Reflection.MethodInfo)Static(mt, "Setter", ["Type"], ty)!).Invoke(null, [ms, i, v]), ms));
                }
        }
        Same(diffs, "new Tuple()", () => new Quire.Mpl.Run.Tuple(), () => New(mt, []));
        NoDiffs(diffs);
    }

    [SkippableFact]
    public void Generic_tuples()
    {
        var diffs = new List<string>();
        System.Type[] args = [typeof(int), typeof(string), typeof(double), typeof(object), typeof(char)];
        object[] values = [3, "s", 1.5, new object(), 'c'];
        for (int n = 2; n <= 5; n++)
        {
            var qt = Q("Mpl.Run.Tuple`" + n).MakeGenericType(args[..n]);
            var mtt = M("Mpl.Run.Tuple`" + n).MakeGenericType(args[..n]);
            var q = Activator.CreateInstance(qt)!;
            var m = Activator.CreateInstance(mtt)!;
            Same(diffs, $"new Tuple`{n}", () => q, () => m);
            for (int i = 1; i <= n; i++)
            {
                Set(q, "m_" + i, values[i - 1]);
                Set(m, "m_" + i, values[i - 1]);
            }
            Same(diffs, $"Tuple`{n} after setting fields", () => q, () => m);
        }
        NoDiffs(diffs);
    }

    [SkippableFact]
    public void MplSignature_members()
    {
        var diffs = new List<string>();
        var mt = M("Mpl.MplSignature");
        (System.Type? R, System.Type[]? P)[] sigs =
        [
            (typeof(int), []), (typeof(int), [typeof(int)]), (typeof(int), [typeof(int), typeof(string)]),
            (typeof(string), [typeof(int), typeof(string)]), (typeof(int), [typeof(string), typeof(int)]),
            (typeof(int), [typeof(int), typeof(string), typeof(double)]), (typeof(void), [typeof(object)]),
            (null, [typeof(int)]), (typeof(int), null), (typeof(int), [null!]),
        ];
        var qs = sigs.Select(s => new MplSignature(s.R!, s.P!)).ToList();
        var ms = sigs.Select(s => New(mt, ["Type", "Type[]"], s.R, s.P)!).ToList();
        for (int a = 0; a < sigs.Length; a++)
        {
            string label = $"sig{a}";
            Same(diffs, $"{label}.RetType", () => qs[a].RetType, () => Get(ms[a], "RetType"));
            Same(diffs, $"{label}.ArgTypes", () => qs[a].ArgTypes, () => Get(ms[a], "ArgTypes"));
            Same(diffs, $"{label}.GetHashCode", () => qs[a].GetHashCode(), () => ms[a].GetHashCode());
            Same(diffs, $"{label}.Equals(null)", () => qs[a].Equals(null), () => ms[a].Equals(null));
            Same(diffs, $"{label}.Equals(string)", () => qs[a].Equals("x"), () => ms[a].Equals("x"));
            Same(diffs, $"{label}.Equals(self)", () => qs[a].Equals(qs[a]), () => ms[a].Equals(ms[a]));
            Same(diffs, $"{label}.ArgTypes is the array given", () => ReferenceEquals(qs[a].ArgTypes, sigs[a].P), () => ReferenceEquals(Get(ms[a], "ArgTypes"), sigs[a].P));
            for (int b = 0; b < sigs.Length; b++)
            {
                Same(diffs, $"{label}.Equals(sig{b})", () => qs[a].Equals(qs[b]), () => ms[a].Equals(ms[b]));
                Same(diffs, $"{label}.Equals(copy of sig{b})", () => qs[a].Equals(new MplSignature(sigs[b].R!, sigs[b].P?.ToArray()!)),
                    () => ms[a].Equals(New(mt, ["Type", "Type[]"], sigs[b].R, sigs[b].P?.ToArray())));
            }
        }
        NoDiffs(diffs);
    }
}
