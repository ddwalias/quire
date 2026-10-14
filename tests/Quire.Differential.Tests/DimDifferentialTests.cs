using static Quire.Differential.Tests.Reflect;
using Q = Quire.Mcd;

namespace Quire.Differential.Tests;

/// <summary>
/// Compares mplrun's <c>Mcd.Dim</c> with Quire's over the markers, polymorphic null and interned dimensions: every
/// predicate, unary and binary operation, power, error and the private interning and hashing; cases are listed in
/// docs/mirror/evidence/Mcd.Dim.md.
/// </summary>
public class DimDifferentialTests
{
    private static System.Type M => Mathcad.Type("mplrun", "Mcd.Dim");

    private static readonly string[] Markers = ["identity", "mcd_zero", "string_dim", "function_dim", "mixed_matrix_dim"];

    private static readonly string[] Signatures =
    [
        "V{Length 60000}", "V{Time 60000}", "V{Mass 60000}", "V{Length 60000 Mass 60000 Time-120000}", "V{Length-60000 Mass 60000 Time-120000}",
        "V{Length 30000}", "V{Length 20000 Time-7}", "V{Money 60000 Current-60000}", "V{Length 120000}", "V{Length 180000}",
        "V{Temperature 60000}", "V{Substance 60000 Luminosity-60000}", "V{Length 1}", "V{Length 2147483647}", "V{Length-2147483647}",
    ];

    /// <summary>The operands: polymorphic null, the markers, then dimensions parsed from <see cref="Signatures"/>.</summary>
    private static List<(string Name, object? Dim)> Operands(System.Type dim)
    {
        var list = new List<(string, object?)> { ("null", null) };
        foreach (var m in Markers) list.Add((m, Get(dim, m)));
        foreach (var s in Signatures) list.Add((s, Call(dim, "Parse", s)));
        return list;
    }

    /// <summary>A dimension as text: a marker's name, or its coordinates and whether it is the interned one.</summary>
    private static Func<object?, string> Show(System.Type dim) => d =>
    {
        if (d == null) return "null";
        foreach (var m in Markers) if (ReferenceEquals(d, Get(dim, m))) return m;
        var coordinates = (int[])Get(d, "Coordinates")!;
        bool interned = ReferenceEquals(d, Call(dim, "intern", coordinates.Clone()));
        return $"[{string.Join(",", coordinates)}]{(interned ? "" : " uninterned")}";
    };

    [SkippableFact]
    public void Unary_members()
    {
        Runtime.Sync();
        var diffs = new List<string>();
        var q = Operands(typeof(Q.Dim));
        var m = Operands(M);
        var qs = Show(typeof(Q.Dim));
        var ms = Show(M);
        Same(diffs, "operands", string.Join(" ", q.Select(o => qs(o.Dim))), string.Join(" ", m.Select(o => ms(o.Dim))));
        for (int i = 0; i < q.Count; i++)
        {
            string n = q[i].Name;
            object? a = q[i].Dim, b = m[i].Dim;
            foreach (var f in new[] { "IsDimensioned", "Id_b", "NonScalar_b", "IsSpecial" })
                Same(diffs, $"{f}({n})", () => Call(typeof(Q.Dim), f, a), () => Call(M, f, b));
            foreach (var f in new[] { "ValDim", "FixDimension", "intern" })
                Same(diffs, $"{f}({n})", Outcome(() => CallSig(typeof(Q.Dim), f, ["Dim"], a), qs), Outcome(() => CallSig(M, f, ["Dim"], b), ms));
            Same(diffs, $"Id({n})", Outcome(() => CallSig(typeof(Q.Dim), "Id", ["Dim"], a), qs), Outcome(() => CallSig(M, "Id", ["Dim"], b), ms));
            Same(diffs, $"Id({n}, 5)", Outcome(() => Call(typeof(Q.Dim), "Id", a, 5u), qs), Outcome(() => Call(M, "Id", b, 5u), ms));
            if (a == null) continue;
            Same(diffs, $"{n}.Format", () => Call(a, "Format"), () => Call(b!, "Format"));
            Same(diffs, $"{n}.ToString", () => a.ToString(), () => b!.ToString());
            Same(diffs, $"{n}.Product", () => Get(a, "Product"), () => Get(b!, "Product"));
            Same(diffs, $"{n}.Coordinates", () => Get(a, "Coordinates"), () => Get(b!, "Coordinates"));
            Same(diffs, $"{n}.Coordinates is live", () => ReferenceEquals(Get(a, "Coordinates"), Get(a, "Coordinates")),
                () => ReferenceEquals(Get(b!, "Coordinates"), Get(b!, "Coordinates")));
            Same(diffs, $"{n}.GetHashCode", () => a.GetHashCode(), () => b!.GetHashCode());
            Same(diffs, $"{n}.Equals(null)", () => a.Equals(null), () => b!.Equals(null));
            Same(diffs, $"{n}.Equals(\"x\")", () => a.Equals("x"), () => b!.Equals("x"));
            foreach (double e in new[] { 0, 1, 2, -1, 0.5, 1.0 / 3, -1.5, Math.PI, 1e10, -1e10, double.NaN, double.PositiveInfinity, -0.0, 2.5e-5 })
                Same(diffs, $"Pwr({n}, {e:R})", Outcome(() => Call(typeof(Q.Dim), "Pwr", a, e), qs), Outcome(() => Call(M, "Pwr", b, e), ms));
            foreach (double r in new[] { 1, 2, 3, -2, 0, 0.5, double.NaN, 7 })
                Same(diffs, $"Root({n}, {r:R})", Outcome(() => Call(typeof(Q.Dim), "Root", a, r), qs), Outcome(() => Call(M, "Root", b, r), ms));
        }
        Same(diffs, "Pwr(null, 2)", Outcome(() => Q.Dim.Pwr(null!, 2), qs), Outcome(() => Call(M, "Pwr", null, 2.0), ms));
        Same(diffs, "Root(null, 2)", Outcome(() => Q.Dim.Root(null!, 2), qs), Outcome(() => Call(M, "Root", null, 2.0), ms));
        NoDiffs(diffs);
    }

    [SkippableFact]
    public void Binary_members()
    {
        Runtime.Sync();
        var diffs = new List<string>();
        var q = Operands(typeof(Q.Dim));
        var m = Operands(M);
        var qs = Show(typeof(Q.Dim));
        var ms = Show(M);
        for (int i = 0; i < q.Count; i++)
            for (int j = 0; j < q.Count; j++)
            {
                string n = $"{q[i].Name}, {q[j].Name}";
                object? a = q[i].Dim, b = q[j].Dim, c = m[i].Dim, d = m[j].Dim;
                foreach (var f in new[] { "Eq_b", "Eq_b1" })
                    Same(diffs, $"{f}({n})", () => Call(typeof(Q.Dim), f, a, b), () => Call(M, f, c, d));
                foreach (var f in new[] { "Mult", "Div" })
                    Same(diffs, $"{f}({n})", Outcome(() => Call(typeof(Q.Dim), f, a, b), qs), Outcome(() => Call(M, f, c, d), ms));
                Same(diffs, $"Eq({n})", Outcome(() => CallSig(typeof(Q.Dim), "Eq", ["Dim", "Dim"], a, b), qs), Outcome(() => CallSig(M, "Eq", ["Dim", "Dim"], c, d), ms));
                Same(diffs, $"Eq({n}, 7)", Outcome(() => Call(typeof(Q.Dim), "Eq", a, b, 7u), qs), Outcome(() => Call(M, "Eq", c, d, 7u), ms));
                Same(diffs, $"OnError(m, {n}, 3)", () => Call(typeof(Q.Dim), "OnError", "msg", a, b, 3u), () => Call(M, "OnError", "msg", c, d, 3u));
                if (a != null) Same(diffs, $"Equals({n})", () => a.Equals(b), () => c!.Equals(d));
            }
        for (int i = 0; i < q.Count; i++)
        {
            Same(diffs, $"OnError(m, {q[i].Name}, 4)", () => CallSig(typeof(Q.Dim), "OnError", ["String", "Dim", "UInt32"], "m", q[i].Dim, 4u),
                () => CallSig(M, "OnError", ["String", "Dim", "UInt32"], "m", m[i].Dim, 4u));
            Same(diffs, $"OnError(tagged, {q[i].Name}, max)", () => CallSig(typeof(Q.Dim), "OnError", ["String", "Dim", "UInt32"], "12>m@3", q[i].Dim, uint.MaxValue),
                () => CallSig(M, "OnError", ["String", "Dim", "UInt32"], "12>m@3", m[i].Dim, uint.MaxValue));
        }
        NoDiffs(diffs);
    }

    [SkippableFact]
    public void Construction_parsing_and_interning()
    {
        Runtime.Sync();
        var diffs = new List<string>();
        var qs = Show(typeof(Q.Dim));
        var ms = Show(M);
        foreach (var s in TypeDifferentialTests.Signatures)
        {
            Same(diffs, $"Parse({Text(s)})", Outcome(() => Q.Dim.Parse(s), qs), Outcome(() => Call(M, "Parse", s), ms));
            Same(diffs, $"New(product of {Text(s)})", Outcome(() => Q.Dim.New(Q.Type.Parse(s).GetDimensionTypeProduct()), qs),
                Outcome(() => Call(M, "New", Call(Call(Mathcad.Type("mplrun", "Mcd.Type"), "Parse", s)!, "GetDimensionTypeProduct")), ms));
        }
        Same(diffs, "New(null)", Outcome(() => Q.Dim.New(null!), qs), Outcome(() => Call(M, "New", [null]), ms));
        Same(diffs, "polymorphic", () => Get(typeof(Q.Dim), "polymorphic") == null, () => Get(M, "polymorphic") == null);
        // the private interning: a new vector is kept, an equal one finds it, and the scratch vector is replaced
        Same(diffs, "intern(new)", () => { var v = new[] { 0, 0, 0, 0, 0, 0, 0, 77 }; var d = Call(typeof(Q.Dim), "intern", v); return ReferenceEquals(Get(d!, "Coordinates"), v); },
            () => { var v = new[] { 0, 0, 0, 0, 0, 0, 0, 77 }; var d = Call(M, "intern", v); return ReferenceEquals(Get(d!, "Coordinates"), v); });
        Same(diffs, "intern(equal)", () => { var v = new[] { 0, 0, 0, 0, 0, 0, 0, 77 }; return ReferenceEquals(Get(Call(typeof(Q.Dim), "intern", v)!, "Coordinates"), v); },
            () => { var v = new[] { 0, 0, 0, 0, 0, 0, 0, 77 }; return ReferenceEquals(Get(Call(M, "intern", v)!, "Coordinates"), v); });
        Same(diffs, "intern(scratch)", () => ScratchInterned(typeof(Q.Dim), 91), () => ScratchInterned(M, 91));
        Same(diffs, "intern(longer)", Outcome(() => Call(typeof(Q.Dim), "intern", new[] { 0, 0, 0, 0, 0, 0, 0, 0, 5 }), qs),
            Outcome(() => Call(M, "intern", new[] { 0, 0, 0, 0, 0, 0, 0, 0, 5 }), ms));
        Same(diffs, "intern(shorter)", Outcome(() => Call(typeof(Q.Dim), "intern", new[] { 1, 2 }), qs), Outcome(() => Call(M, "intern", new[] { 1, 2 }), ms));
        NoDiffs(diffs);
    }

    /// <summary>Interns the thread's scratch vector; reports whether the dimension kept it and the thread got a new one.</summary>
    private static string ScratchInterned(System.Type dim, int value)
    {
        var scratch = (int[])Get(dim, "s_vec")!;
        Array.Clear(scratch);
        scratch[7] = value;
        var d = CallSig(dim, "intern", []);
        var after = (int[])Get(dim, "s_vec")!;
        return $"kept {ReferenceEquals(Get(d!, "Coordinates"), scratch)}, replaced {!ReferenceEquals(after, scratch)}, fresh {after.All(x => x == 0)}";
    }

    [SkippableFact]
    public void Hash_compares_and_hashes_vectors()
    {
        Runtime.Sync();
        var diffs = new List<string>();
        var qh = Nested(typeof(Q.Dim), "Hash");
        var mh = Nested(M, "Hash");
        object qi = New(qh), mi = New(mh);
        var rng = new Random(11);
        var vectors = new List<int[]> { new int[8], new int[9], new int[7], new int[0] };
        for (int k = 0; k < 400; k++)
            vectors.Add(Enumerable.Range(0, 8).Select(_ => rng.Next(4) switch
            {
                0 => 0, 1 => rng.Next(-3, 4) * 60000, 2 => rng.Next(-1_000_000, 1_000_000), _ => rng.Next(int.MinValue, int.MaxValue),
            }).ToArray());
        foreach (var v in vectors)
        {
            Same(diffs, $"GetHashCode([{string.Join(",", v)}])", () => CallSig(qh, "GetHashCode", ["Int32[]"], v), () => CallSig(mh, "GetHashCode", ["Int32[]"], v));
            Same(diffs, $"GetHashCode(object [{string.Join(",", v)}])", () => CallSig(qi, "GetHashCode", ["Object"], v), () => CallSig(mi, "GetHashCode", ["Object"], v));
            var w = (int[])v.Clone();
            if (w.Length > 3) w[3]++;
            Same(diffs, $"Equals([{string.Join(",", v)}], changed)", () => CallSig(qh, "Equals", ["Int32[]", "Int32[]"], v, w), () => CallSig(mh, "Equals", ["Int32[]", "Int32[]"], v, w));
            Same(diffs, $"Equals(object [{string.Join(",", v)}], same)", () => CallSig(qi, "Equals", ["Object", "Object"], v, v.Clone()), () => CallSig(mi, "Equals", ["Object", "Object"], v, v.Clone()));
        }
        Same(diffs, "Equals(object, non-array)", () => CallSig(qi, "Equals", ["Object", "Object"], "x", new int[8]), () => CallSig(mi, "Equals", ["Object", "Object"], "x", new int[8]));
        Same(diffs, "GetHashCode(null)", () => CallSig(qh, "GetHashCode", ["Int32[]"], [null]), () => CallSig(mh, "GetHashCode", ["Int32[]"], [null]));
        NoDiffs(diffs);
    }

    [SkippableFact]
    public void ThreadInit_gives_new_threads_scratch_space()
    {
        Runtime.Sync();
        string OnNewThread(System.Type dim, System.Type options)
        {
            string result = "";
            var t = new Thread(() =>
            {
                var log = new List<string>
                {
                    "before " + Outcome(() => Get(dim, "s_vec"), v => v == null ? "null" : "set"),
                    "unit system " + Outcome(() => Get(options, "the_unit_system")),
                    "Mult " + Outcome(() => Call(dim, "Mult", Call(dim, "Parse", "V{Length 60000}"), Get(dim, "identity")), _ => "ok"),
                    "Mult dimensioned " + Outcome(() => Call(dim, "Mult", Get(dim, "mcd_zero"), Get(dim, "mcd_zero")), _ => "ok"),
                };
                Call(dim, "ThreadInit");
                log.Add("after " + Outcome(() => Get(dim, "s_vec"), v => $"length {((int[])v!).Length}"));
                var before = Get(dim, "s_vec");
                Call(dim, "ThreadInit");
                log.Add("again keeps " + ReferenceEquals(before, Get(dim, "s_vec")));
                log.Add("Mult after " + Outcome(() => Call(dim, "Mult", Get(dim, "mcd_zero"), Get(dim, "mcd_zero")), Show(dim)));
                log.Add("Parse after " + Outcome(() => Call(dim, "Parse", "V{Length 60000}"), _ => "ok"));
                result = string.Join("\n", log);
            });
            t.Start();
            t.Join();
            return result;
        }
        Assert.Equal(OnNewThread(M, Mathcad.Type("mplrun", "Mcd.RuntimeOptions")), OnNewThread(typeof(Q.Dim), typeof(Q.RuntimeOptions)));
    }
}
