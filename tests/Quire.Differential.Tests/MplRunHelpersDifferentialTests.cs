using System.Reflection;
using Quire.Mpl;
using Quire.Mpl.Run;
using Xunit;
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
        null,
        List(1),
        List(1, "two", 3.5),
        new Cons(1, "tail"),
        new Cons(null, null),
        List(List(1, 2), 3),
        new Cons(1, new Cons(2, 5)),
        List(true, 'c', null),
    ];

    private static Cons? Copy(Cons? c) =>
        c == null ? null : new Cons(c.Field1 is Cons h ? Copy(h) : c.Field1, c.Field2 is Cons t ? Copy(t) : c.Field2);

    [SkippableFact]
    public void Cons_members()
    {
        var mt = M("Mpl.Run.Cons");
        var qCtor = typeof(Cons).GetConstructor([typeof(object), typeof(object)])!;
        var mCtor = mt.GetConstructor([typeof(object), typeof(object)])!;
        var qHead = typeof(Cons).GetProperty("head", BindingFlags.Public | BindingFlags.Instance)!.GetGetMethod()!;
        var mHead = mt.GetProperty("head", BindingFlags.Public | BindingFlags.Instance)!.GetGetMethod()!;
        var qTail = typeof(Cons).GetProperty("tail", BindingFlags.Public | BindingFlags.Instance)!.GetGetMethod()!;
        var mTail = mt.GetProperty("tail", BindingFlags.Public | BindingFlags.Instance)!.GetGetMethod()!;
        var qLength = typeof(Cons).GetMethod("Length", BindingFlags.Public | BindingFlags.Static)!;
        var mLength = mt.GetMethod("Length", BindingFlags.Public | BindingFlags.Static)!;
        var qFormatItems = typeof(Cons).GetMethod("FormatItems", BindingFlags.NonPublic | BindingFlags.Instance)!;
        var mFormatItems = mt.GetMethod("FormatItems", BindingFlags.NonPublic | BindingFlags.Instance)!;
        var qToString = typeof(Cons).GetMethod("ToString", BindingFlags.Public | BindingFlags.Instance | BindingFlags.DeclaredOnly, [])!;
        var mToString = mt.GetMethod("ToString", BindingFlags.Public | BindingFlags.Instance | BindingFlags.DeclaredOnly, [])!;
        var qAppend = typeof(Cons).GetMethod("destructive_append", BindingFlags.Public | BindingFlags.Static)!;
        var mAppend = mt.GetMethod("destructive_append", BindingFlags.Public | BindingFlags.Static)!;

        Func<object?, string> show = o => Describe(o);
        Func<object, string> state = o => Describe(o);

        MethodComparison.Compare("new Cons(null, null)", "Mpl.Run.Cons",
            qCtor, null, [null, null], mCtor, null, [null, null], show, show);
        MethodComparison.Compare("new Cons(1, 'tail')", "Mpl.Run.Cons",
            qCtor, null, [1, "tail"], mCtor, null, [1, "tail"], show, show);

        foreach (var q in Lists())
        {
            var m = ToMathcad(q);
            string label = Describe(q);
            MethodComparison.Compare($"Length({label})", "Mpl.Run.Cons",
                qLength, null, [q], mLength, null, [m], show, show);

            if (q == null) continue;

            MethodComparison.Compare($"{label}.head", "Mpl.Run.Cons",
                qHead, q, [], mHead, m, [], show, show, state, state);
            MethodComparison.Compare($"{label}.tail", "Mpl.Run.Cons",
                qTail, q, [], mTail, m, [], show, show, state, state);
            MethodComparison.Compare($"{label}.FormatItems()", "Mpl.Run.Cons",
                qFormatItems, q, [], mFormatItems, m, [], show, show, state, state);
            MethodComparison.Compare($"{label}.ToString()", "Mpl.Run.Cons",
                qToString, q, [], mToString, m, [], show, show, state, state);

            var qHeadVal = q.Field1;
            var mHeadVal = Get(m!, "Field1");
            MethodComparison.Compare($"new Cons({label}, {Describe(qHeadVal)})", "Mpl.Run.Cons",
                qCtor, null, [q, qHeadVal], mCtor, null, [m, mHeadVal], show, show);
        }

        foreach (var xs in Lists())
            foreach (var ys in Lists())
            {
                var qx = Copy(xs);
                var qy = Copy(ys);
                var mx = ToMathcad(qx);
                var my = ToMathcad(qy);
                string descX = Describe(xs);
                string descY = Describe(ys);
                MethodComparison.Compare($"destructive_append({descX}, {descY})", "Mpl.Run.Cons",
                    qAppend, null, [qx, qy], mAppend, null, [mx, my], show, show);
            }
    }

    [SkippableFact]
    public void VecBuilder_members()
    {
        var mt = M("Mpl.Run.VecBuilder");
        var qMake = typeof(VecBuilder).GetMethod("make", BindingFlags.Public | BindingFlags.Static)!;
        var mMake = mt.GetMethod("make", BindingFlags.Public | BindingFlags.Static)!;
        var qCtor = typeof(VecBuilder).GetConstructor([typeof(int), typeof(object[])])!;
        var mCtor = mt.GetConstructor([typeof(int), typeof(object[])])!;
        var qCnt = typeof(VecBuilder).GetProperty("cnt", BindingFlags.Public | BindingFlags.Instance)!.GetGetMethod()!;
        var mCnt = mt.GetProperty("cnt", BindingFlags.Public | BindingFlags.Instance)!.GetGetMethod()!;
        var qVector = typeof(VecBuilder).GetProperty("vector", BindingFlags.Public | BindingFlags.Instance)!.GetGetMethod()!;
        var mVector = mt.GetProperty("vector", BindingFlags.Public | BindingFlags.Instance)!.GetGetMethod()!;
        var qAsVector = typeof(VecBuilder).GetMethod("AsVector", BindingFlags.Public | BindingFlags.Instance)!;
        var mAsVector = mt.GetMethod("AsVector", BindingFlags.Public | BindingFlags.Instance)!;
        var qAsList = typeof(VecBuilder).GetMethod("AsList", BindingFlags.Public | BindingFlags.Instance)!;
        var mAsList = mt.GetMethod("AsList", BindingFlags.Public | BindingFlags.Instance)!;
        var qPush1 = typeof(VecBuilder).GetMethod("Push1", BindingFlags.Public | BindingFlags.Instance)!;
        var mPush1 = mt.GetMethod("Push1", BindingFlags.Public | BindingFlags.Instance)!;
        var qPush2 = typeof(VecBuilder).GetMethod("Push2", BindingFlags.Public | BindingFlags.Instance)!;
        var mPush2 = mt.GetMethod("Push2", BindingFlags.Public | BindingFlags.Instance)!;
        var qPush3 = typeof(VecBuilder).GetMethod("Push3", BindingFlags.Public | BindingFlags.Instance)!;
        var mPush3 = mt.GetMethod("Push3", BindingFlags.Public | BindingFlags.Instance)!;
        var qPush4 = typeof(VecBuilder).GetMethod("Push4", BindingFlags.Public | BindingFlags.Instance)!;
        var mPush4 = mt.GetMethod("Push4", BindingFlags.Public | BindingFlags.Instance)!;

        Func<object?, string> show = o => Describe(o);
        Func<object, string> state = o => Describe(o);

        foreach (int n in new[] { 0, 1, 3, 4, 10, -1 })
        {
            MethodComparison.Compare($"make({n})", "Mpl.Run.VecBuilder",
                qMake, null, [n], mMake, null, [n], show, show);
        }

        foreach (int size in new[] { 0, 1, 3, 5, 10 })
        {
            var q = VecBuilder.make(size);
            var m = Static(mt, "make", ["Int32"], size)!;

            MethodComparison.Compare($"make({size}).cnt initial", "Mpl.Run.VecBuilder",
                qCnt, q, [], mCnt, m, [], show, show, state, state);
            MethodComparison.Compare($"make({size}).vector initial", "Mpl.Run.VecBuilder",
                qVector, q, [], mVector, m, [], show, show, state, state);
            MethodComparison.Compare($"make({size}).AsVector() initial", "Mpl.Run.VecBuilder",
                qAsVector, q, [], mAsVector, m, [], show, show, state, state);
            MethodComparison.Compare($"make({size}).AsList() initial", "Mpl.Run.VecBuilder",
                qAsList, q, [], mAsList, m, [], show, show, state, state);

            MethodComparison.Compare($"make({size}).Push1('a')", "Mpl.Run.VecBuilder",
                qPush1, q, ["a"], mPush1, m, ["a"], show, show, state, state);
            MethodComparison.Compare($"make({size}).Push1('b') again", "Mpl.Run.VecBuilder",
                qPush1, q, ["b"], mPush1, m, ["b"], show, show, state, state);

            MethodComparison.Compare($"make({size}).cnt after Push1", "Mpl.Run.VecBuilder",
                qCnt, q, [], mCnt, m, [], show, show, state, state);
            MethodComparison.Compare($"make({size}).vector after Push1", "Mpl.Run.VecBuilder",
                qVector, q, [], mVector, m, [], show, show, state, state);
            MethodComparison.Compare($"make({size}).AsVector() after Push1", "Mpl.Run.VecBuilder",
                qAsVector, q, [], mAsVector, m, [], show, show, state, state);
            MethodComparison.Compare($"make({size}).AsList() after Push1", "Mpl.Run.VecBuilder",
                qAsList, q, [], mAsList, m, [], show, show, state, state);
        }

        foreach (int size in new[] { 1, 2, 4, 8 })
        {
            var q = VecBuilder.make(size);
            var m = Static(mt, "make", ["Int32"], size)!;
            MethodComparison.Compare($"make({size}).Push2('a', 2)", "Mpl.Run.VecBuilder",
                qPush2, q, ["a", 2], mPush2, m, ["a", 2], show, show, state, state);
            MethodComparison.Compare($"make({size}).Push2(null, 4.5) again", "Mpl.Run.VecBuilder",
                qPush2, q, [null, 4.5], mPush2, m, [null, 4.5], show, show, state, state);
            MethodComparison.Compare($"make({size}).cnt after Push2", "Mpl.Run.VecBuilder",
                qCnt, q, [], mCnt, m, [], show, show, state, state);
            MethodComparison.Compare($"make({size}).AsList() after Push2", "Mpl.Run.VecBuilder",
                qAsList, q, [], mAsList, m, [], show, show, state, state);
        }

        foreach (int size in new[] { 2, 3, 6 })
        {
            var q = VecBuilder.make(size);
            var m = Static(mt, "make", ["Int32"], size)!;
            MethodComparison.Compare($"make({size}).Push3('a', 2, null)", "Mpl.Run.VecBuilder",
                qPush3, q, ["a", 2, null], mPush3, m, ["a", 2, null], show, show, state, state);
            MethodComparison.Compare($"make({size}).Push3(1, 2, 3) again", "Mpl.Run.VecBuilder",
                qPush3, q, [1, 2, 3], mPush3, m, [1, 2, 3], show, show, state, state);
            MethodComparison.Compare($"make({size}).cnt after Push3", "Mpl.Run.VecBuilder",
                qCnt, q, [], mCnt, m, [], show, show, state, state);
            MethodComparison.Compare($"make({size}).AsList() after Push3", "Mpl.Run.VecBuilder",
                qAsList, q, [], mAsList, m, [], show, show, state, state);
        }

        foreach (int size in new[] { 3, 4, 8 })
        {
            var q = VecBuilder.make(size);
            var m = Static(mt, "make", ["Int32"], size)!;
            MethodComparison.Compare($"make({size}).Push4('a', 2, null, 4.5)", "Mpl.Run.VecBuilder",
                qPush4, q, ["a", 2, null, 4.5], mPush4, m, ["a", 2, null, 4.5], show, show, state, state);
            MethodComparison.Compare($"make({size}).Push4(1, 2, 3, 4) again", "Mpl.Run.VecBuilder",
                qPush4, q, [1, 2, 3, 4], mPush4, m, [1, 2, 3, 4], show, show, state, state);
            MethodComparison.Compare($"make({size}).cnt after Push4", "Mpl.Run.VecBuilder",
                qCnt, q, [], mCnt, m, [], show, show, state, state);
            MethodComparison.Compare($"make({size}).AsList() after Push4", "Mpl.Run.VecBuilder",
                qAsList, q, [], mAsList, m, [], show, show, state, state);
        }

        var qChained = VecBuilder.make(10);
        var mChained = Static(mt, "make", ["Int32"], 10)!;
        MethodComparison.Compare("make(10).Push1('a')", "Mpl.Run.VecBuilder",
            qPush1, qChained, ["a"], mPush1, mChained, ["a"], show, show, state, state);
        MethodComparison.Compare("make(10).Push2('b', 'c')", "Mpl.Run.VecBuilder",
            qPush2, qChained, ["b", "c"], mPush2, mChained, ["b", "c"], show, show, state, state);
        MethodComparison.Compare("make(10).Push3('d', 'e', 'f')", "Mpl.Run.VecBuilder",
            qPush3, qChained, ["d", "e", "f"], mPush3, mChained, ["d", "e", "f"], show, show, state, state);
        MethodComparison.Compare("make(10).Push4('g', 'h', 'i', 'j')", "Mpl.Run.VecBuilder",
            qPush4, qChained, ["g", "h", "i", "j"], mPush4, mChained, ["g", "h", "i", "j"], show, show, state, state);
        MethodComparison.Compare("make(10).cnt full", "Mpl.Run.VecBuilder",
            qCnt, qChained, [], mCnt, mChained, [], show, show, state, state);
        MethodComparison.Compare("make(10).AsList() full", "Mpl.Run.VecBuilder",
            qAsList, qChained, [], mAsList, mChained, [], show, show, state, state);
        MethodComparison.Compare("make(10).Push1('overflow')", "Mpl.Run.VecBuilder",
            qPush1, qChained, ["overflow"], mPush1, mChained, ["overflow"], show, show, state, state);

        foreach (var (idx, items) in new (int, object[]?)[]
        {
            (2, new object[] { "x", "y", "z" }),
            (5, new object[] { "x" }),
            (-1, new object[] { "x" }),
            (0, null),
            (1, null),
            (0, new object[0])
        })
        {
            MethodComparison.Compare($"new VecBuilder({idx}, {Describe(items)})", "Mpl.Run.VecBuilder",
                qCtor, null, [idx, items], mCtor, null, [idx, items], show, show);

            var q = new VecBuilder(idx, items!);
            var m = New(mt, ["Int32", "Object[]"], idx, items)!;
            MethodComparison.Compare($"new VecBuilder({idx}, {Describe(items)}).AsList()", "Mpl.Run.VecBuilder",
                qAsList, q, [], mAsList, m, [], show, show, state, state);
            MethodComparison.Compare($"new VecBuilder({idx}, {Describe(items)}).Push1('p')", "Mpl.Run.VecBuilder",
                qPush1, q, ["p"], mPush1, m, ["p"], show, show, state, state);
            MethodComparison.Compare($"new VecBuilder({idx}, {Describe(items)}).cnt", "Mpl.Run.VecBuilder",
                qCnt, q, [], mCnt, m, [], show, show, state, state);
            MethodComparison.Compare($"new VecBuilder({idx}, {Describe(items)}).vector", "Mpl.Run.VecBuilder",
                qVector, q, [], mVector, m, [], show, show, state, state);
            MethodComparison.Compare($"new VecBuilder({idx}, {Describe(items)}).AsVector()", "Mpl.Run.VecBuilder",
                qAsVector, q, [], mAsVector, m, [], show, show, state, state);
        }
    }

    [SkippableFact]
    public void True_members()
    {
        var mt = M("Mpl.Run.True");
        var qCctor = typeof(True).TypeInitializer!;
        var mCctor = mt.TypeInitializer!;
        var qCtor = typeof(True).GetConstructor([])!;
        var mCtor = mt.GetConstructor([])!;
        var qToString = typeof(True).GetMethod("ToString", BindingFlags.Public | BindingFlags.Instance | BindingFlags.DeclaredOnly, [])!;
        var mToString = mt.GetMethod("ToString", BindingFlags.Public | BindingFlags.Instance | BindingFlags.DeclaredOnly, [])!;

        Func<object?, string> show = o => Describe(o);
        Func<object, string> state = o => Describe(o);

        MethodComparison.Compare(".cctor() initialization", "Mpl.Run.True",
            qCctor, null, [], mCctor, null, [], show, show);
        MethodComparison.Compare("new True()", "Mpl.Run.True",
            qCtor, null, [], mCtor, null, [], show, show);

        var qInstance = new True();
        var mInstance = Activator.CreateInstance(mt)!;
        MethodComparison.Compare("new True().ToString()", "Mpl.Run.True",
            qToString, qInstance, [], mToString, mInstance, [], show, show, state, state);

        var qSingleton = True.singleton;
        var mSingleton = Get(mt, "singleton")!;
        MethodComparison.Compare("True.singleton.ToString()", "Mpl.Run.True",
            qToString, qSingleton, [], mToString, mSingleton, [], show, show, state, state);
    }

    private static readonly System.Type[][] TypeLists =
    [
        [],
        [typeof(int)],
        [typeof(string)],
        [typeof(string), typeof(object)],
        [typeof(int), typeof(string)],
        [typeof(double), typeof(int), typeof(string)],
        [typeof(int), typeof(int), typeof(int), typeof(int)],
        [typeof(int), typeof(int), typeof(int), typeof(int), typeof(int)],
        [typeof(int), typeof(int), typeof(int), typeof(int), typeof(int), typeof(int)],
        [typeof(string), typeof(string), typeof(string), typeof(string), typeof(string), typeof(string), typeof(string)],
        [typeof(DateTime), typeof(string)],
        [typeof(int?), typeof(object)],
    ];

    [SkippableFact]
    public void Tuple_members()
    {
        var qt = typeof(Quire.Mpl.Run.Tuple);
        var mt = M("Mpl.Run.Tuple");
        var qCtor = qt.GetConstructor([])!;
        var mCtor = mt.GetConstructor([])!;
        var qTupleLength = qt.GetMethod("tuple_length", BindingFlags.Public | BindingFlags.Static)!;
        var mTupleLength = mt.GetMethod("tuple_length", BindingFlags.Public | BindingFlags.Static)!;
        var qIType = qt.GetMethod("IType", BindingFlags.NonPublic | BindingFlags.Static)!;
        var mIType = mt.GetMethod("IType", BindingFlags.NonPublic | BindingFlags.Static)!;
        var qAllObjects = qt.GetMethod("AllObjects", BindingFlags.Public | BindingFlags.Static)!;
        var mAllObjects = mt.GetMethod("AllObjects", BindingFlags.Public | BindingFlags.Static)!;
        var qTupleType = qt.GetMethod("TupleType", BindingFlags.Public | BindingFlags.Static)!;
        var mTupleType = mt.GetMethod("TupleType", BindingFlags.Public | BindingFlags.Static)!;
        var qTupleRep = qt.GetMethod("TupleRep", BindingFlags.Public | BindingFlags.Static)!;
        var mTupleRep = mt.GetMethod("TupleRep", BindingFlags.Public | BindingFlags.Static)!;
        var qGetFieldInfo = qt.GetMethod("GetFieldInfo", BindingFlags.Public | BindingFlags.Static)!;
        var mGetFieldInfo = mt.GetMethod("GetFieldInfo", BindingFlags.Public | BindingFlags.Static)!;
        var qGetter = qt.GetMethod("Getter", BindingFlags.Public | BindingFlags.Static)!;
        var mGetter = mt.GetMethod("Getter", BindingFlags.Public | BindingFlags.Static)!;
        var qSetter = qt.GetMethod("Setter", BindingFlags.Public | BindingFlags.Static)!;
        var mSetter = mt.GetMethod("Setter", BindingFlags.Public | BindingFlags.Static)!;
        var qGetFieldDef = qt.GetMethod("GetField", BindingFlags.Public | BindingFlags.Static)!;
        var mGetFieldDef = mt.GetMethod("GetField", BindingFlags.Public | BindingFlags.Static)!;
        var qSetFieldDef = qt.GetMethod("SetField", BindingFlags.Public | BindingFlags.Static)!;
        var mSetFieldDef = mt.GetMethod("SetField", BindingFlags.Public | BindingFlags.Static)!;

        Func<object?, string> show = o => Describe(o);

        MethodComparison.Compare("new Tuple()", "Mpl.Run.Tuple",
            qCtor, null, [], mCtor, null, [], show, show);

        // The type initializer fills the generic tuple classes by arity and the GetField/SetField definitions;
        // rerunning it is rendered as the static state it leaves behind.
        static string Statics(System.Type t)
        {
            var types = (System.Type?[])Get(t, "tup_types")!;
            string Full(System.Type ty) => ty.FullName!.StartsWith("Quire.", StringComparison.Ordinal) ? ty.FullName[6..] : ty.FullName;
            string Name(object? o) => o switch
            {
                null => "null",
                System.Type ty => Full(ty),
                MethodInfo mi => $"{Full(mi.DeclaringType!)}.{mi.Name}`{mi.GetGenericArguments().Length} own={mi.DeclaringType == t}",
                _ => o.GetType().Name,
            };
            return $"tup_types=[{string.Join(", ", types.Select(Name))}] gf={Name(Get(t, "gf"))} sf={Name(Get(t, "sf"))}";
        }
        MethodComparison.Compare(".cctor() statics", "Mpl.Run.Tuple",
            qt.TypeInitializer!, null, [], mt.TypeInitializer!, null, [],
            _ => Statics(qt), _ => Statics(mt));

        foreach (object? o in new object?[] { null, new object[3], new object[0], 5, "s", new int[2], new string[4] })
        {
            MethodComparison.Compare($"tuple_length({Describe(o)})", "Mpl.Run.Tuple",
                qTupleLength, null, [o], mTupleLength, null, [o], show, show);
        }

        foreach (var tys in TypeLists)
        {
            string label = string.Join(",", tys.Select(t => t.Name));
            MethodComparison.Compare($"AllObjects({label})", "Mpl.Run.Tuple",
                qAllObjects, null, [tys], mAllObjects, null, [tys], show, show);
            MethodComparison.Compare($"TupleType({label})", "Mpl.Run.Tuple",
                qTupleType, null, [tys], mTupleType, null, [tys], show, show);
            MethodComparison.Compare($"IType({label})", "Mpl.Run.Tuple",
                qIType, null, [tys], mIType, null, [tys], show, show);

            object?[] qa = [null, tys];
            object?[] ma = [null, tys];
            MethodComparison.Compare($"TupleRep({label})", "Mpl.Run.Tuple",
                qTupleRep, null, qa, mTupleRep, null, ma, show, show);
        }

        var qPair = (System.Type)qTupleType.Invoke(null, [new[] { typeof(int), typeof(string) }])!;
        var mPair = (System.Type)mTupleType.Invoke(null, [new[] { typeof(int), typeof(string) }])!;
        var qTriple = (System.Type)qTupleType.Invoke(null, [new[] { typeof(int), typeof(string), typeof(double) }])!;
        var mTriple = (System.Type)mTupleType.Invoke(null, [new[] { typeof(int), typeof(string), typeof(double) }])!;
        var qQuad = (System.Type)qTupleType.Invoke(null, [new[] { typeof(int), typeof(int), typeof(int), typeof(int) }])!;
        var mQuad = (System.Type)mTupleType.Invoke(null, [new[] { typeof(int), typeof(int), typeof(int), typeof(int) }])!;
        var qQuint = (System.Type)qTupleType.Invoke(null, [new[] { typeof(int), typeof(int), typeof(int), typeof(int), typeof(int) }])!;
        var mQuint = (System.Type)mTupleType.Invoke(null, [new[] { typeof(int), typeof(int), typeof(int), typeof(int), typeof(int) }])!;

        var typeCases = new (string Name, System.Type? QType, System.Type? MType)[]
        {
            ("null", null, null),
            ("string", typeof(string), typeof(string)),
            ("pair", qPair, mPair),
            ("triple", qTriple, mTriple),
            ("quad", qQuad, mQuad),
            ("quint", qQuint, mQuint)
        };

        foreach (var (typeName, qType, mType) in typeCases)
        {
            for (int fieldIndex = 0; fieldIndex <= 6; fieldIndex++)
            {
                MethodComparison.Compare($"GetFieldInfo({typeName}, {fieldIndex})", "Mpl.Run.Tuple",
                    qGetFieldInfo, null, [qType, fieldIndex], mGetFieldInfo, null, [mType, fieldIndex], show, show);
            }
        }

        foreach (var ty in new[] { typeof(int), typeof(string), typeof(object), typeof(double) })
        {
            MethodComparison.Compare($"Getter({ty.Name})", "Mpl.Run.Tuple",
                qGetter, null, [ty], mGetter, null, [ty], show, show);
            MethodComparison.Compare($"Setter({ty.Name})", "Mpl.Run.Tuple",
                qSetter, null, [ty], mSetter, null, [ty], show, show);

            var qGetField = qGetFieldDef.MakeGenericMethod(ty);
            var mGetField = mGetFieldDef.MakeGenericMethod(ty);
            var qSetField = qSetFieldDef.MakeGenericMethod(ty);
            var mSetField = mSetFieldDef.MakeGenericMethod(ty);

            foreach (object?[] tupleArray in new object?[][] { [1, "s", null], [] })
            {
                for (int slot = 0; slot <= 3; slot++)
                {
                    var qs = (object?[])tupleArray.Clone();
                    var ms = (object?[])tupleArray.Clone();
                    MethodComparison.Compare($"GetField<{ty.Name}>({Describe(tupleArray)}[{slot}])", "Mpl.Run.Tuple",
                        qGetField, null, [qs, slot], mGetField, null, [ms, slot], show, show);

                    object? testVal = ty == typeof(int) ? 7 : ty == typeof(double) ? 3.14 : "val";
                    var qSetTarget = (object?[])tupleArray.Clone();
                    var mSetTarget = (object?[])tupleArray.Clone();
                    MethodComparison.Compare($"SetField<{ty.Name}>({Describe(tupleArray)}[{slot}], {Describe(testVal)})", "Mpl.Run.Tuple",
                        qSetField, null, [qSetTarget, slot, testVal], mSetField, null, [mSetTarget, slot, testVal], show, show);
                }
            }
        }
    }

    [SkippableFact]
    public void Generic_tuples()
    {
        System.Type[] args = [typeof(int), typeof(string), typeof(double), typeof(object), typeof(char)];
        object[] values = [3, "s", 1.5, new object(), 'c'];
        Func<object?, string> show = o => Describe(o);

        for (int arity = 2; arity <= 5; arity++)
        {
            var qt = Q("Mpl.Run.Tuple`" + arity).MakeGenericType(args[..arity]);
            var mtt = M("Mpl.Run.Tuple`" + arity).MakeGenericType(args[..arity]);
            var qCtor = qt.GetConstructor([])!;
            var mCtor = mtt.GetConstructor([])!;

            MethodComparison.Compare($"Tuple`{arity}.ctor()", "Mpl.Run.Tuple",
                qCtor, null, [], mCtor, null, [], show, show);

            var q = Activator.CreateInstance(qt)!;
            var m = Activator.CreateInstance(mtt)!;
            for (int fieldNum = 1; fieldNum <= arity; fieldNum++)
            {
                Set(q, "m_" + fieldNum, values[fieldNum - 1]);
                Set(m, "m_" + fieldNum, values[fieldNum - 1]);
            }
            Assert.Equal(Describe(m), Describe(q));
        }
    }

    [SkippableFact]
    public void MplSignature_members()
    {
        var mt = M("Mpl.MplSignature");
        var qCtor = typeof(MplSignature).GetConstructor([typeof(System.Type), typeof(System.Type[])])!;
        var mCtor = mt.GetConstructor([typeof(System.Type), typeof(System.Type[])])!;
        var qRetType = typeof(MplSignature).GetProperty("RetType", BindingFlags.Public | BindingFlags.Instance)!.GetGetMethod()!;
        var mRetType = mt.GetProperty("RetType", BindingFlags.Public | BindingFlags.Instance)!.GetGetMethod()!;
        var qArgTypes = typeof(MplSignature).GetProperty("ArgTypes", BindingFlags.Public | BindingFlags.Instance)!.GetGetMethod()!;
        var mArgTypes = mt.GetProperty("ArgTypes", BindingFlags.Public | BindingFlags.Instance)!.GetGetMethod()!;
        var qGetHashCode = typeof(MplSignature).GetMethod("GetHashCode", BindingFlags.Public | BindingFlags.Instance | BindingFlags.DeclaredOnly, [])!;
        var mGetHashCode = mt.GetMethod("GetHashCode", BindingFlags.Public | BindingFlags.Instance | BindingFlags.DeclaredOnly, [])!;
        var qEquals = typeof(MplSignature).GetMethod("Equals", BindingFlags.Public | BindingFlags.Instance | BindingFlags.DeclaredOnly, [typeof(object)])!;
        var mEquals = mt.GetMethod("Equals", BindingFlags.Public | BindingFlags.Instance | BindingFlags.DeclaredOnly, [typeof(object)])!;

        Func<object?, string> show = o => Describe(o);
        Func<object, string> state = o => Describe(o);

        (System.Type? R, System.Type[]? P)[] sigs =
        [
            (typeof(int), []),
            (typeof(int), [typeof(int)]),
            (typeof(int), [typeof(int), typeof(string)]),
            (typeof(string), [typeof(int), typeof(string)]),
            (typeof(int), [typeof(string), typeof(int)]),
            (typeof(int), [typeof(int), typeof(string), typeof(double)]),
            (typeof(void), [typeof(object)]),
            (null, [typeof(int)]),
            (typeof(int), null),
            (typeof(int), [null!]),
        ];

        for (int i = 0; i < sigs.Length; i++)
        {
            var sig = sigs[i];
            MethodComparison.Compare($"MplSignature.ctor({Describe(sig.R)}, {Describe(sig.P)})", "Mpl.MplSignature",
                qCtor, null, [sig.R, sig.P], mCtor, null, [sig.R, sig.P], show, show);
        }

        var qs = sigs.Select(s => new MplSignature(s.R!, s.P!)).ToList();
        var ms = sigs.Select(s => New(mt, ["Type", "Type[]"], s.R, s.P)!).ToList();

        for (int a = 0; a < sigs.Length; a++)
        {
            string label = $"sig{a}";
            MethodComparison.Compare($"{label}.RetType", "Mpl.MplSignature",
                qRetType, qs[a], [], mRetType, ms[a], [], show, show, state, state);
            MethodComparison.Compare($"{label}.ArgTypes", "Mpl.MplSignature",
                qArgTypes, qs[a], [], mArgTypes, ms[a], [], show, show, state, state);
            MethodComparison.Compare($"{label}.GetHashCode()", "Mpl.MplSignature",
                qGetHashCode, qs[a], [], mGetHashCode, ms[a], [], show, show, state, state);

            MethodComparison.Compare($"{label}.Equals(null)", "Mpl.MplSignature",
                qEquals, qs[a], [null], mEquals, ms[a], [null], show, show, state, state);
            MethodComparison.Compare($"{label}.Equals('x')", "Mpl.MplSignature",
                qEquals, qs[a], ["x"], mEquals, ms[a], ["x"], show, show, state, state);
            MethodComparison.Compare($"{label}.Equals(self)", "Mpl.MplSignature",
                qEquals, qs[a], [qs[a]], mEquals, ms[a], [ms[a]], show, show, state, state);

            Assert.Same(sigs[a].P, qs[a].ArgTypes);
            Assert.Same(sigs[a].P, Get(ms[a], "ArgTypes"));

            for (int b = 0; b < sigs.Length; b++)
            {
                MethodComparison.Compare($"{label}.Equals(sig{b})", "Mpl.MplSignature",
                    qEquals, qs[a], [qs[b]], mEquals, ms[a], [ms[b]], show, show, state, state);

                var qCopy = new MplSignature(sigs[b].R!, sigs[b].P?.ToArray()!);
                var mCopy = New(mt, ["Type", "Type[]"], sigs[b].R, sigs[b].P?.ToArray());
                MethodComparison.Compare($"{label}.Equals(copy of sig{b})", "Mpl.MplSignature",
                    qEquals, qs[a], [qCopy], mEquals, ms[a], [mCopy], show, show, state, state);
            }
        }
    }
}
