using System.Reflection;
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
    private const string Contract = "Mcd.Dim";
    private const BindingFlags DeclaredPublic = BindingFlags.Public | BindingFlags.DeclaredOnly;
    private const BindingFlags DeclaredStaticPublic = BindingFlags.Public | BindingFlags.Static | BindingFlags.DeclaredOnly;
    private const BindingFlags DeclaredInstancePublic = BindingFlags.Public | BindingFlags.Instance | BindingFlags.DeclaredOnly;
    private const BindingFlags DeclaredStaticPrivate = BindingFlags.NonPublic | BindingFlags.Static | BindingFlags.DeclaredOnly;
    private const BindingFlags DeclaredInstancePrivate = BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.DeclaredOnly;

    private static System.Type M => Mathcad.Type("mplrun", "Mcd.Dim");
    private static System.Type MHash => Mathcad.Type("mplrun", "Mcd.Dim+Hash");
    private static System.Type QHash => typeof(Q.Dim).GetNestedType("Hash", BindingFlags.NonPublic | BindingFlags.DeclaredOnly)!;

    private static readonly string[] Markers = ["identity", "mcd_zero", "string_dim", "function_dim", "mixed_matrix_dim"];

    private static readonly string[] Signatures =
    [
        "V{Length 60000}", "V{Time 60000}", "V{Mass 60000}", "V{Length 60000 Mass 60000 Time-120000}", "V{Length-60000 Mass 60000 Time-120000}",
        "V{Length 30000}", "V{Length 20000 Time-7}", "V{Money 60000 Current-60000}", "V{Length 120000}", "V{Length 180000}",
        "V{Temperature 60000}", "V{Substance 60000 Luminosity-60000}", "V{Length 1}", "V{Length 2147483647}", "V{Length-2147483647}",
    ];

    private static List<(string Name, object? Dim)> Operands(System.Type dim)
    {
        var list = new List<(string, object?)> { ("null", null) };
        foreach (var m in Markers) list.Add((m, Get(dim, m)));
        foreach (var s in Signatures) list.Add((s, Call(dim, "Parse", s)));
        return list;
    }

    private static Func<object?, string> Show(System.Type dim) => d =>
    {
        if (d == null) return "null";
        foreach (var m in Markers) if (ReferenceEquals(d, Get(dim, m))) return m;
        if (d is string or double or int or uint or bool or long or int[]) return Text(d);
        if (d.GetType() != dim && !d.GetType().IsSubclassOf(dim)) return Text(d);
        var coordinates = (int[])Get(d, "Coordinates")!;
        bool interned = ReferenceEquals(d, Call(dim, "intern", coordinates.Clone()));
        return $"[{string.Join(",", coordinates)}]{(interned ? "" : " uninterned")}";
    };

    [SkippableFact]
    public void Unary_members()
    {
        Runtime.Sync();
        var q = Operands(typeof(Q.Dim));
        var m = Operands(M);
        var qs = Show(typeof(Q.Dim));
        var ms = Show(M);

        var qIsDimensioned = typeof(Q.Dim).GetMethod("IsDimensioned", DeclaredStaticPublic, [typeof(Q.Dim)])!;
        var mIsDimensioned = M.GetMethod("IsDimensioned", DeclaredStaticPublic, [M])!;
        var qIdB = typeof(Q.Dim).GetMethod("Id_b", DeclaredStaticPublic, [typeof(Q.Dim)])!;
        var mIdB = M.GetMethod("Id_b", DeclaredStaticPublic, [M])!;
        var qNonScalarB = typeof(Q.Dim).GetMethod("NonScalar_b", DeclaredStaticPublic, [typeof(Q.Dim)])!;
        var mNonScalarB = M.GetMethod("NonScalar_b", DeclaredStaticPublic, [M])!;
        var qIsSpecial = typeof(Q.Dim).GetMethod("IsSpecial", DeclaredStaticPublic, [typeof(Q.Dim)])!;
        var mIsSpecial = M.GetMethod("IsSpecial", DeclaredStaticPublic, [M])!;

        var qValDim = typeof(Q.Dim).GetMethod("ValDim", DeclaredStaticPublic, [typeof(Q.Dim)])!;
        var mValDim = M.GetMethod("ValDim", DeclaredStaticPublic, [M])!;
        var qFixDim = typeof(Q.Dim).GetMethod("FixDimension", DeclaredStaticPublic, [typeof(Q.Dim)])!;
        var mFixDim = M.GetMethod("FixDimension", DeclaredStaticPublic, [M])!;
        var qInternDim = typeof(Q.Dim).GetMethod("intern", DeclaredStaticPublic, [typeof(Q.Dim)])!;
        var mInternDim = M.GetMethod("intern", DeclaredStaticPublic, [M])!;

        var qId1 = typeof(Q.Dim).GetMethod("Id", DeclaredStaticPublic, [typeof(Q.Dim)])!;
        var mId1 = M.GetMethod("Id", DeclaredStaticPublic, [M])!;
        var qId2 = typeof(Q.Dim).GetMethod("Id", DeclaredStaticPublic, [typeof(Q.Dim), typeof(uint)])!;
        var mId2 = M.GetMethod("Id", DeclaredStaticPublic, [M, typeof(uint)])!;

        var qFormat = typeof(Q.Dim).GetMethod("Format", DeclaredInstancePublic, [])!;
        var mFormat = M.GetMethod("Format", DeclaredInstancePublic, [])!;
        var qToString = typeof(Q.Dim).GetMethod("ToString", DeclaredInstancePublic, [])!;
        var mToString = M.GetMethod("ToString", DeclaredInstancePublic, [])!;
        var qProduct = typeof(Q.Dim).GetMethod("get_Product", DeclaredInstancePublic, [])!;
        var mProduct = M.GetMethod("get_Product", DeclaredInstancePublic, [])!;
        var qCoordinates = typeof(Q.Dim).GetMethod("get_Coordinates", DeclaredInstancePublic, [])!;
        var mCoordinates = M.GetMethod("get_Coordinates", DeclaredInstancePublic, [])!;
        var qGetHashCode = typeof(Q.Dim).GetMethod("GetHashCode", DeclaredInstancePublic, [])!;
        var mGetHashCode = M.GetMethod("GetHashCode", DeclaredInstancePublic, [])!;
        var qEquals = typeof(Q.Dim).GetMethod("Equals", DeclaredInstancePublic, [typeof(object)])!;
        var mEquals = M.GetMethod("Equals", DeclaredInstancePublic, [typeof(object)])!;

        var qPwr = typeof(Q.Dim).GetMethod("Pwr", DeclaredStaticPublic, [typeof(Q.Dim), typeof(double)])!;
        var mPwr = M.GetMethod("Pwr", DeclaredStaticPublic, [M, typeof(double)])!;
        var qRoot = typeof(Q.Dim).GetMethod("Root", DeclaredStaticPublic, [typeof(Q.Dim), typeof(double)])!;
        var mRoot = M.GetMethod("Root", DeclaredStaticPublic, [M, typeof(double)])!;

        for (int i = 0; i < q.Count; i++)
        {
            string n = q[i].Name;
            object? a = q[i].Dim, b = m[i].Dim;

            MethodComparison.Compare($"IsDimensioned({n})", Contract, qIsDimensioned, null, [a], mIsDimensioned, null, [b], qs, ms);
            MethodComparison.Compare($"Id_b({n})", Contract, qIdB, null, [a], mIdB, null, [b], qs, ms);
            MethodComparison.Compare($"NonScalar_b({n})", Contract, qNonScalarB, null, [a], mNonScalarB, null, [b], qs, ms);
            MethodComparison.Compare($"IsSpecial({n})", Contract, qIsSpecial, null, [a], mIsSpecial, null, [b], qs, ms);

            MethodComparison.Compare($"ValDim({n})", Contract, qValDim, null, [a], mValDim, null, [b], qs, ms);
            MethodComparison.Compare($"FixDimension({n})", Contract, qFixDim, null, [a], mFixDim, null, [b], qs, ms);
            MethodComparison.Compare($"intern({n})", Contract, qInternDim, null, [a], mInternDim, null, [b], qs, ms);

            MethodComparison.Compare($"Id({n})", Contract, qId1, null, [a], mId1, null, [b], qs, ms);
            MethodComparison.Compare($"Id({n}, 5)", Contract, qId2, null, [a, 5u], mId2, null, [b, 5u], qs, ms);

            if (a != null)
            {
                MethodComparison.Compare($"{n}.Format", Contract, qFormat, a, [], mFormat, b, [], qs, ms, d => qs(d), d => ms(d));
                MethodComparison.Compare($"{n}.ToString", Contract, qToString, a, [], mToString, b, [], qs, ms, d => qs(d), d => ms(d));
                MethodComparison.Compare($"{n}.Product", Contract, qProduct, a, [], mProduct, b, [], qs, ms, d => qs(d), d => ms(d));
                MethodComparison.Compare($"{n}.Coordinates", Contract, qCoordinates, a, [], mCoordinates, b, [], qs, ms, d => qs(d), d => ms(d));
                Assert.Same(Get(a, "Coordinates"), Get(a, "Coordinates"));
                Assert.Same(Get(b!, "Coordinates"), Get(b!, "Coordinates"));
                MethodComparison.Compare($"{n}.GetHashCode", Contract, qGetHashCode, a, [], mGetHashCode, b, [], qs, ms, d => qs(d), d => ms(d));
                MethodComparison.Compare($"{n}.Equals(null)", Contract, qEquals, a, [null], mEquals, b, [null], qs, ms, d => qs(d), d => ms(d));
                MethodComparison.Compare($"{n}.Equals(\"x\")", Contract, qEquals, a, ["x"], mEquals, b, ["x"], qs, ms, d => qs(d), d => ms(d));

                foreach (double e in new[] { 0, 1, 2, -1, 0.5, 1.0 / 3, -1.5, Math.PI, 1e10, -1e10, double.NaN, double.PositiveInfinity, -0.0, 2.5e-5 })
                    MethodComparison.Compare($"Pwr({n}, {e:R})", Contract, qPwr, null, [a, e], mPwr, null, [b, e], qs, ms);
                foreach (double r in new[] { 1, 2, 3, -2, 0, 0.5, double.NaN, 7 })
                    MethodComparison.Compare($"Root({n}, {r:R})", Contract, qRoot, null, [a, r], mRoot, null, [b, r], qs, ms);
            }
        }
        MethodComparison.Compare("Pwr(null, 2)", Contract, qPwr, null, [null, 2.0], mPwr, null, [null, 2.0], qs, ms);
        MethodComparison.Compare("Root(null, 2)", Contract, qRoot, null, [null, 2.0], mRoot, null, [null, 2.0], qs, ms);
    }

    [SkippableFact]
    public void Binary_members()
    {
        Runtime.Sync();
        var q = Operands(typeof(Q.Dim));
        var m = Operands(M);
        var qs = Show(typeof(Q.Dim));
        var ms = Show(M);

        var qEqB = typeof(Q.Dim).GetMethod("Eq_b", DeclaredStaticPublic, [typeof(Q.Dim), typeof(Q.Dim)])!;
        var mEqB = M.GetMethod("Eq_b", DeclaredStaticPublic, [M, M])!;
        var qEqB1 = typeof(Q.Dim).GetMethod("Eq_b1", DeclaredStaticPublic, [typeof(Q.Dim), typeof(Q.Dim)])!;
        var mEqB1 = M.GetMethod("Eq_b1", DeclaredStaticPublic, [M, M])!;

        var qMult = typeof(Q.Dim).GetMethod("Mult", DeclaredStaticPublic, [typeof(Q.Dim), typeof(Q.Dim)])!;
        var mMult = M.GetMethod("Mult", DeclaredStaticPublic, [M, M])!;
        var qDiv = typeof(Q.Dim).GetMethod("Div", DeclaredStaticPublic, [typeof(Q.Dim), typeof(Q.Dim)])!;
        var mDiv = M.GetMethod("Div", DeclaredStaticPublic, [M, M])!;

        var qEq = typeof(Q.Dim).GetMethod("Eq", DeclaredStaticPublic, [typeof(Q.Dim), typeof(Q.Dim)])!;
        var mEq = M.GetMethod("Eq", DeclaredStaticPublic, [M, M])!;
        var qEqA = typeof(Q.Dim).GetMethod("Eq", DeclaredStaticPublic, [typeof(Q.Dim), typeof(Q.Dim), typeof(uint)])!;
        var mEqA = M.GetMethod("Eq", DeclaredStaticPublic, [M, M, typeof(uint)])!;

        var qOnError4 = typeof(Q.Dim).GetMethod("OnError", DeclaredStaticPublic, [typeof(string), typeof(Q.Dim), typeof(Q.Dim), typeof(uint)])!;
        var mOnError4 = M.GetMethod("OnError", DeclaredStaticPublic, [typeof(string), M, M, typeof(uint)])!;
        var qOnError3 = typeof(Q.Dim).GetMethod("OnError", DeclaredStaticPublic, [typeof(string), typeof(Q.Dim), typeof(uint)])!;
        var mOnError3 = M.GetMethod("OnError", DeclaredStaticPublic, [typeof(string), M, typeof(uint)])!;

        var qEquals = typeof(Q.Dim).GetMethod("Equals", DeclaredInstancePublic, [typeof(object)])!;
        var mEquals = M.GetMethod("Equals", DeclaredInstancePublic, [typeof(object)])!;

        for (int i = 0; i < q.Count; i++)
            for (int j = 0; j < q.Count; j++)
            {
                string n = $"{q[i].Name}, {q[j].Name}";
                object? a = q[i].Dim, b = q[j].Dim, c = m[i].Dim, d = m[j].Dim;

                MethodComparison.Compare($"Eq_b({n})", Contract, qEqB, null, [a, b], mEqB, null, [c, d], qs, ms);
                MethodComparison.Compare($"Eq_b1({n})", Contract, qEqB1, null, [a, b], mEqB1, null, [c, d], qs, ms);
                MethodComparison.Compare($"Mult({n})", Contract, qMult, null, [a, b], mMult, null, [c, d], qs, ms);
                MethodComparison.Compare($"Div({n})", Contract, qDiv, null, [a, b], mDiv, null, [c, d], qs, ms);
                MethodComparison.Compare($"Eq({n})", Contract, qEq, null, [a, b], mEq, null, [c, d], qs, ms);
                MethodComparison.Compare($"Eq({n}, 7)", Contract, qEqA, null, [a, b, 7u], mEqA, null, [c, d, 7u], qs, ms);
                MethodComparison.Compare($"OnError(m, {n}, 3)", Contract, qOnError4, null, ["msg", a, b, 3u], mOnError4, null, ["msg", c, d, 3u], qs, ms);
                if (a != null)
                    MethodComparison.Compare($"Equals({n})", Contract, qEquals, a, [b], mEquals, c, [d], qs, ms, obj => qs(obj), obj => ms(obj));
            }

        for (int i = 0; i < q.Count; i++)
        {
            MethodComparison.Compare($"OnError(m, {q[i].Name}, 4)", Contract, qOnError3, null, ["m", q[i].Dim, 4u], mOnError3, null, ["m", m[i].Dim, 4u], qs, ms);
            MethodComparison.Compare($"OnError(tagged, {q[i].Name}, max)", Contract, qOnError3, null, ["12>m@3", q[i].Dim, uint.MaxValue], mOnError3, null, ["12>m@3", m[i].Dim, uint.MaxValue], qs, ms);
        }
    }

    [SkippableFact]
    public void Construction_parsing_and_interning()
    {
        Runtime.Sync();
        var qs = Show(typeof(Q.Dim));
        var ms = Show(M);

        var qParse = typeof(Q.Dim).GetMethod("Parse", DeclaredStaticPublic, [typeof(string)])!;
        var mParse = M.GetMethod("Parse", DeclaredStaticPublic, [typeof(string)])!;
        var qTypeProd = typeof(Q.Type).GetNestedType("Product", DeclaredPublic)!;
        var mTypeProd = Mathcad.Type("mplrun", "Mcd.Type+Product");
        var qNew = typeof(Q.Dim).GetMethod("New", DeclaredStaticPublic, [qTypeProd])!;
        var mNew = M.GetMethod("New", DeclaredStaticPublic, [mTypeProd])!;

        var qInternArr = typeof(Q.Dim).GetMethod("intern", DeclaredStaticPrivate, [typeof(int[])])!;
        var mInternArr = M.GetMethod("intern", DeclaredStaticPrivate, [typeof(int[])])!;
        var qIntern0 = typeof(Q.Dim).GetMethod("intern", DeclaredStaticPrivate, [])!;
        var mIntern0 = M.GetMethod("intern", DeclaredStaticPrivate, [])!;

        var qCtorArr = typeof(Q.Dim).GetConstructor(DeclaredInstancePrivate, [typeof(int[])])!;
        var mCtorArr = M.GetConstructor(DeclaredInstancePrivate, [typeof(int[])])!;
        var qCtor0 = typeof(Q.Dim).GetConstructor(DeclaredInstancePrivate, [])!;
        var mCtor0 = M.GetConstructor(DeclaredInstancePrivate, [])!;

        var qCc = typeof(Q.Dim).TypeInitializer!;
        var mCc = M.TypeInitializer!;

        foreach (var s in TypeDifferentialTests.Signatures)
        {
            MethodComparison.Compare($"Parse({Text(s)})", Contract, qParse, null, [s], mParse, null, [s], qs, ms);
            object? qProd = null, mProd = null;
            try
            {
                qProd = Q.Type.Parse(s).GetDimensionTypeProduct();
                mProd = Call(Call(Mathcad.Type("mplrun", "Mcd.Type"), "Parse", s)!, "GetDimensionTypeProduct");
            }
            catch
            {
                continue;
            }
            MethodComparison.Compare($"New(product of {Text(s)})", Contract, qNew, null, [qProd], mNew, null, [mProd], qs, ms);
        }
        MethodComparison.Compare("New(null)", Contract, qNew, null, [null], mNew, null, [null], qs, ms);

        // private constructors
        var vFresh = new[] { 999991, 1, 2, 3, 4, 5, 6, 7 };
        MethodComparison.Compare("ctor(int[]): fresh vector", Contract, qCtorArr, null, [vFresh.Clone()], mCtorArr, null, [vFresh.Clone()], qs, ms);
        var vDup = new int[8];
        MethodComparison.Compare("ctor(int[]): duplicate key throws", Contract, qCtorArr, null, [vDup.Clone()], mCtorArr, null, [vDup.Clone()], qs, ms);
        MethodComparison.Compare("ctor(): parameterless marker", Contract, qCtor0, null, [], mCtor0, null, [], qs, ms);

        // static constructor (.cctor)
        MethodComparison.Compare("cctor: type initializer", Contract, qCc, null, [], mCc, null, [], qs, ms);

        // private interning
        var vIntern = new[] { 0, 0, 0, 0, 0, 0, 0, 77 };
        MethodComparison.Compare("intern(new)", Contract, qInternArr, null, [vIntern.Clone()], mInternArr, null, [vIntern.Clone()], qs, ms);
        MethodComparison.Compare("intern(equal)", Contract, qInternArr, null, [vIntern.Clone()], mInternArr, null, [vIntern.Clone()], qs, ms);
        MethodComparison.Compare("intern(longer)", Contract, qInternArr, null, [new[] { 0, 0, 0, 0, 0, 0, 0, 0, 5 }], mInternArr, null, [new[] { 0, 0, 0, 0, 0, 0, 0, 0, 5 }], qs, ms);
        MethodComparison.Compare("intern(shorter)", Contract, qInternArr, null, [new[] { 1, 2 }], mInternArr, null, [new[] { 1, 2 }], qs, ms);

        // scratch vector intern
        MethodComparison.Compare("intern(): scratch vector", Contract, qIntern0, null, [], mIntern0, null, [], qs, ms);
    }

    [SkippableFact]
    public void Hash_compares_and_hashes_vectors()
    {
        Runtime.Sync();
        var qh = QHash;
        var mh = MHash;

        var qCtor = qh.GetConstructor(DeclaredInstancePublic, [])!;
        var mCtor = mh.GetConstructor(DeclaredInstancePublic, [])!;
        MethodComparison.Compare("Hash.ctor()", Contract, qCtor, null, [], mCtor, null, [], _ => "Hash", _ => "Hash");

        var qi = qCtor.Invoke([]);
        var mi = mCtor.Invoke([]);

        var qHashObj = qh.GetMethod("GetHashCode", DeclaredInstancePublic, [typeof(object)])!;
        var mHashObj = mh.GetMethod("GetHashCode", DeclaredInstancePublic, [typeof(object)])!;
        var qEqualsObj = qh.GetMethod("Equals", DeclaredInstancePublic, [typeof(object), typeof(object)])!;
        var mEqualsObj = mh.GetMethod("Equals", DeclaredInstancePublic, [typeof(object), typeof(object)])!;

        var qHashArr = qh.GetMethod("GetHashCode", DeclaredStaticPublic, [typeof(int[])])!;
        var mHashArr = mh.GetMethod("GetHashCode", DeclaredStaticPublic, [typeof(int[])])!;
        var qEqualsArr = qh.GetMethod("Equals", DeclaredStaticPublic, [typeof(int[]), typeof(int[])])!;
        var mEqualsArr = mh.GetMethod("Equals", DeclaredStaticPublic, [typeof(int[]), typeof(int[])])!;

        var rng = new Random(11);
        var vectors = new List<int[]> { new int[8], new int[9], new int[7], new int[0] };
        for (int k = 0; k < 400; k++)
            vectors.Add(Enumerable.Range(0, 8).Select(_ => rng.Next(4) switch
            {
                0 => 0, 1 => rng.Next(-3, 4) * 60000, 2 => rng.Next(-1_000_000, 1_000_000), _ => rng.Next(int.MinValue, int.MaxValue),
            }).ToArray());

        foreach (var v in vectors)
        {
            string desc = $"[{string.Join(",", v)}]";
            MethodComparison.Compare($"Hash.GetHashCode({desc})", Contract, qHashArr, null, [v], mHashArr, null, [v]);
            MethodComparison.Compare($"Hash.GetHashCode(object {desc})", Contract, qHashObj, qi, [v], mHashObj, mi, [v], null, null, _ => "Hash", _ => "Hash");

            var w = (int[])v.Clone();
            if (w.Length > 3) w[3]++;
            MethodComparison.Compare($"Hash.Equals({desc}, changed)", Contract, qEqualsArr, null, [v, w], mEqualsArr, null, [v, w]);
            MethodComparison.Compare($"Hash.Equals(object {desc}, same)", Contract, qEqualsObj, qi, [v, v.Clone()], mEqualsObj, mi, [v, v.Clone()], null, null, _ => "Hash", _ => "Hash");
        }

        MethodComparison.Compare("Hash.Equals(object, non-array)", Contract, qEqualsObj, qi, ["x", new int[8]], mEqualsObj, mi, ["x", new int[8]], null, null, _ => "Hash", _ => "Hash");
        MethodComparison.Compare("Hash.GetHashCode(null)", Contract, qHashArr, null, [null], mHashArr, null, [null]);
    }

    [SkippableFact]
    public void ThreadInit_gives_new_threads_scratch_space()
    {
        Runtime.Sync();
        var qThreadInit = typeof(Q.Dim).GetMethod("ThreadInit", DeclaredStaticPublic, [])!;
        var mThreadInit = M.GetMethod("ThreadInit", DeclaredStaticPublic, [])!;
        MethodComparison.Compare("ThreadInit: calling thread", Contract, qThreadInit, null, [], mThreadInit, null, []);

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
