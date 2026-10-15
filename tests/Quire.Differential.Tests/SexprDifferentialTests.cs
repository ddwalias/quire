using System.Reflection;
using Quire.Mpl.Run;
using Xunit;
using static Quire.Differential.Tests.Reflect;
using static Quire.Differential.Tests.MplRunSides;

namespace Quire.Differential.Tests;

/// <summary>
/// Compares Quire's S-expression family (<c>Sexpr</c>, <c>Nul</c>, <c>SCons</c>, <c>Tok</c>, <c>Qt</c>, <c>Bq</c>,
/// <c>Cm</c>, <c>Ca</c>, <c>SAppend</c>, <c>SList</c>, <c>Tag</c>, <c>Atom</c>, <c>Sym</c> and the constants) with
/// mplrun's, member by member using direct <see cref="MethodComparison.Compare"/> calls across all branches and edge cases.
/// </summary>
public class SexprDifferentialTests
{
    private const BindingFlags DeclaredPublic = BindingFlags.Public | BindingFlags.DeclaredOnly;
    private const BindingFlags DeclaredStaticPublic = BindingFlags.Public | BindingFlags.Static | BindingFlags.DeclaredOnly;
    private const BindingFlags DeclaredInstancePublic = BindingFlags.Public | BindingFlags.Instance | BindingFlags.DeclaredOnly;
    private const BindingFlags DeclaredStaticNonPublic = BindingFlags.NonPublic | BindingFlags.Static | BindingFlags.DeclaredOnly;
    private const BindingFlags DeclaredInstanceNonPublic = BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.DeclaredOnly;

    private static Tok S(string? name, uint h = 0) => new(new Sym(name!, null), h);
    private static Tok I(int v, uint h = 0) => new(new int_constant(v), h);
    private static Tok R(double v, uint h = 0) => new(new real_constant(v), h);
    private static Tok Str(string? v, uint h = 0) => new(new str_constant(v!), h);
    private static SCons L(params Sexpr?[] xs)
    {
        Sexpr list = Nul.singleton;
        for (int i = xs.Length - 1; i >= 0; i--) list = new SCons(xs[i]!, list);
        return (SCons)list;
    }

    private static System.Type MType(System.Type qType) =>
        Mathcad.Type("mplrun", qType.FullName!.StartsWith("Quire.", StringComparison.Ordinal) ? qType.FullName[6..] : qType.FullName);

    private static System.Type MParamType(System.Type qParam) =>
        qParam.IsByRef ? MParamType(qParam.GetElementType()!).MakeByRefType() :
        qParam.FullName?.StartsWith("Quire.") == true ? Mathcad.Type("mplrun", qParam.FullName[6..]) : qParam;

    private static (MethodInfo q, MethodInfo m) Method(System.Type qType, string name, BindingFlags flags, params System.Type[] qParamTypes)
    {
        var mType = MType(qType);
        var mParamTypes = qParamTypes.Select(MParamType).ToArray();
        var q = qType.GetMethod(name, flags, qParamTypes)
            ?? throw new HarnessException($"Quire method not found: {qType.Name}.{name}");
        var m = mType.GetMethod(name, flags, mParamTypes)
            ?? throw new HarnessException($"Mathcad method not found: {mType.Name}.{name}");
        return (q, m);
    }

    private static (ConstructorInfo q, ConstructorInfo m) Ctor(System.Type qType, BindingFlags flags, params System.Type[] qParamTypes)
    {
        var mType = MType(qType);
        var mParamTypes = qParamTypes.Select(MParamType).ToArray();
        var q = qType.GetConstructor(flags, null, qParamTypes, null)
            ?? throw new HarnessException($"Quire constructor not found: {qType.Name}");
        var m = mType.GetConstructor(flags, null, mParamTypes, null)
            ?? throw new HarnessException($"Mathcad constructor not found: {mType.Name}");
        return (q, m);
    }

    private static void Compare(string caseId, string contract,
        MethodBase q, object? qTarget, object?[] qArgs,
        MethodBase m, object? mTarget, object?[] mArgs)
    {
        MethodComparison.Compare(caseId, contract,
            q, qTarget, qArgs,
            m, mTarget, mArgs,
            quireShow: o => Describe(o), originalShow: o => Describe(o),
            quireState: o => Describe(o), originalState: o => Describe(o));
    }

    [SkippableFact]
    public void Tag_and_constant_tokens()
    {
        Runtime.Sync();
        // Tag.ctor(int)
        var (qTagCtor, mTagCtor) = Ctor(typeof(Tag), DeclaredInstancePublic, typeof(int));
        foreach (int tag in new[] { 9, -1, 0 })
            Compare($"Tag({tag})", "Mpl.Run.Tag", qTagCtor, null, [tag], mTagCtor, null, [tag]);

        // char_constant
        var (qCharCtor, mCharCtor) = Ctor(typeof(char_constant), DeclaredInstancePublic, typeof(char));
        var (qCharNewTok, mCharNewTok) = Method(typeof(char_constant), "NewTok", DeclaredStaticPublic, typeof(char), typeof(uint));
        var (qCharToString, mCharToString) = Method(typeof(char_constant), "ToString", DeclaredInstancePublic);
        var (qCharCon, mCharCon) = Method(typeof(char_constant), "get_CharCon", DeclaredInstancePublic);

        foreach (char c in new[] { 'c', '\0', 'z' })
        {
            Compare($"char_constant({c})", "Mpl.Run.char_constant", qCharCtor, null, [c], mCharCtor, null, [c]);
            Compare($"char_constant.NewTok({c}, 0)", "Mpl.Run.char_constant", qCharNewTok, null, [c, 0u], mCharNewTok, null, [c, 0u]);
            Compare($"char_constant.NewTok({c}, 5)", "Mpl.Run.char_constant", qCharNewTok, null, [c, 5u], mCharNewTok, null, [c, 5u]);

            var qObj = new char_constant(c);
            var mObj = ToMathcad(qObj)!;
            Compare($"char_constant({c}).ToString()", "Mpl.Run.char_constant", qCharToString, qObj, [], mCharToString, mObj, []);
            Compare($"char_constant({c}).CharCon", "Mpl.Run.char_constant", qCharCon, qObj, [], mCharCon, mObj, []);
        }

        // int_constant
        var (qIntCc, mIntCc) = (typeof(int_constant).TypeInitializer!, MType(typeof(int_constant)).TypeInitializer!);
        MethodComparison.Compare("int_constant.cctor", "Mpl.Run.int_constant",
            qIntCc, null, [],
            mIntCc, null, [],
            quireShow: _ => $"token={Describe(Reflect.Get(typeof(int_constant), "token"))};zero={Describe(Reflect.Get(typeof(int_constant), "zero"))}",
            originalShow: _ => $"token={Describe(Reflect.Get(MType(typeof(int_constant)), "token"))};zero={Describe(Reflect.Get(MType(typeof(int_constant)), "zero"))}",
            quireState: o => Describe(o), originalState: o => Describe(o));

        var (qIntCtorInt, mIntCtorInt) = Ctor(typeof(int_constant), DeclaredInstancePublic, typeof(int));
        var (qIntCtorStr, mIntCtorStr) = Ctor(typeof(int_constant), DeclaredInstancePublic, typeof(string));
        var (qIntNewTokInt, mIntNewTokInt) = Method(typeof(int_constant), "NewTok", DeclaredStaticPublic, typeof(int), typeof(uint));
        var (qIntNewTokStr, mIntNewTokStr) = Method(typeof(int_constant), "NewTok", DeclaredStaticPublic, typeof(string), typeof(uint));
        var (qIntToString, mIntToString) = Method(typeof(int_constant), "ToString", DeclaredInstancePublic);
        var (qIntCon, mIntCon) = Method(typeof(int_constant), "get_IntCon", DeclaredInstancePublic);
        var (qIntQuoteAtom, mIntQuoteAtom) = Method(typeof(int_constant), "get_quote_atom", DeclaredInstancePublic);

        foreach (int v in new[] { 0, -12, int.MaxValue, int.MinValue })
        {
            Compare($"int_constant({v})", "Mpl.Run.int_constant", qIntCtorInt, null, [v], mIntCtorInt, null, [v]);
            Compare($"int_constant.NewTok({v}, 0)", "Mpl.Run.int_constant", qIntNewTokInt, null, [v, 0u], mIntNewTokInt, null, [v, 0u]);
            Compare($"int_constant.NewTok({v}, 300)", "Mpl.Run.int_constant", qIntNewTokInt, null, [v, 300u], mIntNewTokInt, null, [v, 300u]);

            var qObj = new int_constant(v);
            var mObj = ToMathcad(qObj)!;
            Compare($"int_constant({v}).ToString()", "Mpl.Run.int_constant", qIntToString, qObj, [], mIntToString, mObj, []);
            Compare($"int_constant({v}).IntCon", "Mpl.Run.int_constant", qIntCon, qObj, [], mIntCon, mObj, []);
            Compare($"int_constant({v}).quote_atom", "Mpl.Run.int_constant", qIntQuoteAtom, qObj, [], mIntQuoteAtom, mObj, []);
        }

        foreach (string? s in new[] { "12", " 12 ", "-5", "+7", "1e3", "abc", null, "2147483648", "1,000", "0x10", "" })
        {
            Compare($"int_constant({Text(s)})", "Mpl.Run.int_constant", qIntCtorStr, null, [s], mIntCtorStr, null, [s]);
            Compare($"int_constant.NewTok({Text(s)}, 1)", "Mpl.Run.int_constant", qIntNewTokStr, null, [s, 1u], mIntNewTokStr, null, [s, 1u]);
        }

        // real_constant
        var (qRealCc, mRealCc) = (typeof(real_constant).TypeInitializer!, MType(typeof(real_constant)).TypeInitializer!);
        Compare("real_constant.cctor", "Mpl.Run.real_constant", qRealCc, null, [], mRealCc, null, []);

        var (qRealCtorDbl, mRealCtorDbl) = Ctor(typeof(real_constant), DeclaredInstancePublic, typeof(double));
        var (qRealCtorStr, mRealCtorStr) = Ctor(typeof(real_constant), DeclaredInstancePublic, typeof(string));
        var (qRealNewTokDbl, mRealNewTokDbl) = Method(typeof(real_constant), "NewTok", DeclaredStaticPublic, typeof(double), typeof(uint));
        var (qRealNewTokStr, mRealNewTokStr) = Method(typeof(real_constant), "NewTok", DeclaredStaticPublic, typeof(string), typeof(uint));
        var (qRealToString, mRealToString) = Method(typeof(real_constant), "ToString", DeclaredInstancePublic);
        var (qRealCon, mRealCon) = Method(typeof(real_constant), "get_RealCon", DeclaredInstancePublic);
        var (qRealQuoteAtom, mRealQuoteAtom) = Method(typeof(real_constant), "get_quote_atom", DeclaredInstancePublic);

        foreach (double v in new[] { 0.0, -0.0, 1.5, double.NaN, double.NegativeInfinity, 1e300, 1e-320 })
        {
            Compare($"real_constant({v})", "Mpl.Run.real_constant", qRealCtorDbl, null, [v], mRealCtorDbl, null, [v]);
            Compare($"real_constant.NewTok({v}, 0)", "Mpl.Run.real_constant", qRealNewTokDbl, null, [v, 0u], mRealNewTokDbl, null, [v, 0u]);
            Compare($"real_constant.NewTok({v}, 2)", "Mpl.Run.real_constant", qRealNewTokDbl, null, [v, 2u], mRealNewTokDbl, null, [v, 2u]);

            var qObj = new real_constant(v);
            var mObj = ToMathcad(qObj)!;
            Compare($"real_constant({v}).ToString()", "Mpl.Run.real_constant", qRealToString, qObj, [], mRealToString, mObj, []);
            Compare($"real_constant({v}).RealCon", "Mpl.Run.real_constant", qRealCon, qObj, [], mRealCon, mObj, []);
            Compare($"real_constant({v}).quote_atom", "Mpl.Run.real_constant", qRealQuoteAtom, qObj, [], mRealQuoteAtom, mObj, []);
        }

        foreach (string? s in new[] { "1.5", "NaN", "Infinity", "-0", "1e400", "∞", "abc", null, "" })
        {
            Compare($"real_constant({Text(s)})", "Mpl.Run.real_constant", qRealCtorStr, null, [s], mRealCtorStr, null, [s]);
            Compare($"real_constant.NewTok({Text(s)}, 1)", "Mpl.Run.real_constant", qRealNewTokStr, null, [s, 1u], mRealNewTokStr, null, [s, 1u]);
        }

        // str_constant
        var (qStrCc, mStrCc) = (typeof(str_constant).TypeInitializer!, MType(typeof(str_constant)).TypeInitializer!);
        Compare("str_constant.cctor", "Mpl.Run.str_constant", qStrCc, null, [], mStrCc, null, []);

        var (qStrCtor, mStrCtor) = Ctor(typeof(str_constant), DeclaredInstancePublic, typeof(string));
        var (qStrNewTok, mStrNewTok) = Method(typeof(str_constant), "NewTok", DeclaredStaticPublic, typeof(string), typeof(uint));
        var (qStrToString, mStrToString) = Method(typeof(str_constant), "ToString", DeclaredInstancePublic);
        var (qStrCon, mStrCon) = Method(typeof(str_constant), "get_StrCon", DeclaredInstancePublic);
        var (qStrQuoteAtom, mStrQuoteAtom) = Method(typeof(str_constant), "get_quote_atom", DeclaredInstancePublic);

        foreach (string? s in new[] { "a b", "", null })
        {
            Compare($"str_constant({Text(s)})", "Mpl.Run.str_constant", qStrCtor, null, [s], mStrCtor, null, [s]);
            Compare($"str_constant.NewTok({Text(s)}, 0)", "Mpl.Run.str_constant", qStrNewTok, null, [s, 0u], mStrNewTok, null, [s, 0u]);
            Compare($"str_constant.NewTok({Text(s)}, 9)", "Mpl.Run.str_constant", qStrNewTok, null, [s, 9u], mStrNewTok, null, [s, 9u]);

            var qObj = new str_constant(s);
            var mObj = ToMathcad(qObj)!;
            Compare($"str_constant({Text(s)}).ToString()", "Mpl.Run.str_constant", qStrToString, qObj, [], mStrToString, mObj, []);
            Compare($"str_constant({Text(s)}).StrCon", "Mpl.Run.str_constant", qStrCon, qObj, [], mStrCon, mObj, []);
            Compare($"str_constant({Text(s)}).quote_atom", "Mpl.Run.str_constant", qStrQuoteAtom, qObj, [], mStrQuoteAtom, mObj, []);
        }
    }

    [SkippableFact]
    public void Atom_base_accessors()
    {
        Runtime.Sync();

        var (qCharCon, mCharCon) = Method(typeof(Atom), "get_CharCon", DeclaredInstancePublic);
        var (qIntCon, mIntCon) = Method(typeof(Atom), "get_IntCon", DeclaredInstancePublic);
        var (qRealCon, mRealCon) = Method(typeof(Atom), "get_RealCon", DeclaredInstancePublic);
        var (qStrCon, mStrCon) = Method(typeof(Atom), "get_StrCon", DeclaredInstancePublic);
        var (qKeyNum, mKeyNum) = Method(typeof(Atom), "get_KeyNum", DeclaredInstancePublic);
        var (qQuoteAtom, mQuoteAtom) = Method(typeof(Atom), "get_quote_atom", DeclaredInstancePublic);
        var (qSym, mSym) = Method(typeof(Atom), "get_sym", DeclaredInstancePublic);
        var (qKey, mKey) = Method(typeof(Atom), "get_Key", DeclaredInstancePublic);

        var qInt = new int_constant(42);
        var mInt = ToMathcad(qInt)!;
        Compare("Atom.CharCon on int_constant", "Mpl.Run.Atom", qCharCon, qInt, [], mCharCon, mInt, []);
        Compare("Atom.RealCon on int_constant", "Mpl.Run.Atom", qRealCon, qInt, [], mRealCon, mInt, []);
        Compare("Atom.StrCon on int_constant", "Mpl.Run.Atom", qStrCon, qInt, [], mStrCon, mInt, []);
        Compare("Atom.KeyNum on int_constant", "Mpl.Run.Atom", qKeyNum, qInt, [], mKeyNum, mInt, []);
        Compare("Atom.sym on int_constant", "Mpl.Run.Atom", qSym, qInt, [], mSym, mInt, []);
        Compare("Atom.Key on int_constant", "Mpl.Run.Atom", qKey, qInt, [], mKey, mInt, []);

        var qReal = new real_constant(2.5);
        var mReal = ToMathcad(qReal)!;
        Compare("Atom.IntCon on real_constant", "Mpl.Run.Atom", qIntCon, qReal, [], mIntCon, mReal, []);

        var qChar = new char_constant('c');
        var mChar = ToMathcad(qChar)!;
        Compare("Atom.quote_atom on char_constant", "Mpl.Run.Atom", qQuoteAtom, qChar, [], mQuoteAtom, mChar, []);

        var qSymInst = new Sym("a", null);
        var mSymInst = ToMathcad(qSymInst)!;
        Compare("Atom.sym on Sym", "Mpl.Run.Atom", qSym, qSymInst, [], mSym, mSymInst, []);
        Compare("Atom.Key on Sym", "Mpl.Run.Atom", qKey, qSymInst, [], mKey, mSymInst, []);
    }

    [SkippableFact]
    public void Sym_members()
    {
        Runtime.Sync();
        var (qCc, mCc) = (typeof(Sym).TypeInitializer!, MType(typeof(Sym)).TypeInitializer!);
        Compare("Sym.cctor", "Mpl.Run.Sym", qCc, null, [], mCc, null, []);

        var (qCtorObj, mCtorObj) = Ctor(typeof(Sym), DeclaredInstancePublic, typeof(object));
        var (qCtorStr2, mCtorStr2) = Ctor(typeof(Sym), DeclaredInstancePublic, typeof(string), typeof(string));
        var (qCtorStrInt, mCtorStrInt) = Ctor(typeof(Sym), DeclaredInstancePublic, typeof(string), typeof(string), typeof(int));
        var (qCtorStrUint, mCtorStrUint) = Ctor(typeof(Sym), DeclaredInstancePublic, typeof(string), typeof(string), typeof(uint));

        foreach (object? o in new object?[] { "a", null, 5, "" })
            Compare($"Sym(object {Text(o)})", "Mpl.Run.Sym", qCtorObj, null, [o], mCtorObj, null, [o]);

        foreach (var (nm, q) in new[] { ("a", "b"), ("a", null), (null, "b"), ("", "") })
        {
            Compare($"Sym({Text(nm)}, {Text(q)})", "Mpl.Run.Sym", qCtorStr2, null, [nm, q], mCtorStr2, null, [nm, q]);
            Compare($"Sym({Text(nm)}, {Text(q)}, 5)", "Mpl.Run.Sym", qCtorStrInt, null, [nm, q, 5], mCtorStrInt, null, [nm, q, 5]);
            Compare($"Sym({Text(nm)}, {Text(q)}, 42u)", "Mpl.Run.Sym", qCtorStrUint, null, [nm, q, 42u], mCtorStrUint, null, [nm, q, 42u]);
        }

        var (qMakeSym, mMakeSym) = Method(typeof(Sym), "MakeSym", DeclaredStaticPublic, typeof(string));
        var (qMakeSymQ, mMakeSymQ) = Method(typeof(Sym), "MakeSymQ", DeclaredStaticPublic, typeof(string), typeof(string));
        var (qNewTok2, mNewTok2) = Method(typeof(Sym), "NewTok", DeclaredStaticPublic, typeof(string), typeof(uint));
        var (qNewTok3a, mNewTok3a) = Method(typeof(Sym), "NewTok", DeclaredStaticPublic, typeof(string), typeof(string), typeof(uint));
        var (qNewTok3b, mNewTok3b) = Method(typeof(Sym), "NewTok", DeclaredStaticPublic, typeof(string), typeof(uint), typeof(uint));
        var (qNewTok4, mNewTok4) = Method(typeof(Sym), "NewTok", DeclaredStaticPublic, typeof(string), typeof(string), typeof(uint), typeof(uint));

        foreach (var (nm, q) in new[] { ("a", "b"), ("a", null), (null, null), ("", "") })
        {
            Compare($"MakeSym({Text(nm)})", "Mpl.Run.Sym", qMakeSym, null, [nm], mMakeSym, null, [nm]);
            Compare($"MakeSymQ({Text(nm)}, {Text(q)})", "Mpl.Run.Sym", qMakeSymQ, null, [nm, q], mMakeSymQ, null, [nm, q]);
            Compare($"NewTok({Text(nm)}, 3u)", "Mpl.Run.Sym", qNewTok2, null, [nm, 3u], mNewTok2, null, [nm, 3u]);
            Compare($"NewTok({Text(nm)}, {Text(q)}, 3u)", "Mpl.Run.Sym", qNewTok3a, null, [nm, q, 3u], mNewTok3a, null, [nm, q, 3u]);
            Compare($"NewTok({Text(nm)}, 8u, 3u)", "Mpl.Run.Sym", qNewTok3b, null, [nm, 8u, 3u], mNewTok3b, null, [nm, 8u, 3u]);
            Compare($"NewTok({Text(nm)}, {Text(q)}, 8u, 3u)", "Mpl.Run.Sym", qNewTok4, null, [nm, q, 8u, 3u], mNewTok4, null, [nm, q, 8u, 3u]);
        }

        var (qToString, mToString) = Method(typeof(Sym), "ToString", DeclaredInstancePublic);
        var (qName, mName) = Method(typeof(Sym), "get_name", DeclaredInstancePublic);
        var (qQualifier, mQualifier) = Method(typeof(Sym), "get_qualifier", DeclaredInstancePublic);
        var (qLongName, mLongName) = Method(typeof(Sym), "get_long_name", DeclaredInstancePublic);
        var (qUpId, mUpId) = Method(typeof(Sym), "get_UpId", DeclaredInstancePublic);
        var (qLowId, mLowId) = Method(typeof(Sym), "get_LowId", DeclaredInstancePublic);
        var (qKeyNum, mKeyNum) = Method(typeof(Sym), "get_KeyNum", DeclaredInstancePublic);
        var (qGetUid, mGetUid) = Method(typeof(Sym), "get_uid", DeclaredInstancePublic);
        var (qSetUid, mSetUid) = Method(typeof(Sym), "set_uid", DeclaredInstancePublic, typeof(uint));
        var (qQuoteAtom, mQuoteAtom) = Method(typeof(Sym), "get_quote_atom", DeclaredInstancePublic);

        Sym[] syms =
        [
            new Sym("abc", null), new Sym("Abc", null), new Sym("", null), new Sym(null!, null), new Sym("_x", null),
            new Sym("n", "q", 0), new Sym("k", "q", 5), new Sym("u", "q", 9u), new Sym("1a", null)
        ];

        foreach (var s in syms)
        {
            var m = ToMathcad(s)!;
            string label = Describe(s);
            Compare($"{label}.ToString()", "Mpl.Run.Sym", qToString, s, [], mToString, m, []);
            Compare($"{label}.name", "Mpl.Run.Sym", qName, s, [], mName, m, []);
            Compare($"{label}.qualifier", "Mpl.Run.Sym", qQualifier, s, [], mQualifier, m, []);
            Compare($"{label}.long_name", "Mpl.Run.Sym", qLongName, s, [], mLongName, m, []);
            Compare($"{label}.UpId", "Mpl.Run.Sym", qUpId, s, [], mUpId, m, []);
            Compare($"{label}.LowId", "Mpl.Run.Sym", qLowId, s, [], mLowId, m, []);
            Compare($"{label}.KeyNum", "Mpl.Run.Sym", qKeyNum, s, [], mKeyNum, m, []);
            Compare($"{label}.uid", "Mpl.Run.Sym", qGetUid, s, [], mGetUid, m, []);
            Compare($"{label}.quote_atom", "Mpl.Run.Sym", qQuoteAtom, s, [], mQuoteAtom, m, []);
        }

        var sMutQ = new Sym("v", null);
        var sMutM = ToMathcad(sMutQ)!;
        Compare("set_uid(0xFFFFFFFF)", "Mpl.Run.Sym", qSetUid, sMutQ, [0xFFFF_FFFFu], mSetUid, sMutM, [0xFFFF_FFFFu]);
        Compare("get_uid() after 0xFFFFFFFF", "Mpl.Run.Sym", qGetUid, sMutQ, [], mGetUid, sMutM, []);
        Compare("set_uid(42)", "Mpl.Run.Sym", qSetUid, sMutQ, [42u], mSetUid, sMutM, [42u]);
        Compare("get_uid() after 42", "Mpl.Run.Sym", qGetUid, sMutQ, [], mGetUid, sMutM, []);

        var qEnv = new LocalEnv();
        var mEnv = New(M("Mpl.Run.LocalEnv"), [])!;
        var form = L(I(1, 6), I(2));
        var mForm = ToMathcad(form);

        var (qWalkDeclAs, mWalkDeclAs) = Method(typeof(Sym), "walk_declaration_as", DeclaredInstancePublic, typeof(MplEnv), typeof(SCons));
        var (qWalkDefAs, mWalkDefAs) = Method(typeof(Sym), "walk_definition_as", DeclaredInstancePublic, typeof(MplEnv), typeof(SCons));
        var (qWalkExprAs, mWalkExprAs) = Method(typeof(Sym), "walk_expression_as", DeclaredInstancePublic, typeof(MplEnv), typeof(SCons));

        var symUpperQ = new Sym("Foo", null);
        var symUpperM = ToMathcad(symUpperQ)!;
        Compare("Sym.walk_definition_as", "Mpl.Run.Sym", qWalkDefAs, symUpperQ, [qEnv, form], mWalkDefAs, symUpperM, [mEnv, mForm]);
        Compare("Sym.walk_expression_as upper", "Mpl.Run.Sym", qWalkExprAs, symUpperQ, [qEnv, form], mWalkExprAs, symUpperM, [mEnv, mForm]);

        // An upper-case symbol walks the arguments without the environment: unchanged arguments keep the form,
        // a nested list (walked into an apply) gives a new form with the original head.
        var formQ = L(S("Foo", 2), I(1), Str("s"));
        var formM = ToMathcad(formQ)!;
        var changedQ = L(S("Foo", 2), I(1), L(I(2), I(3)));
        var changedM = ToMathcad(changedQ)!;
        foreach (var (fQ, fM, label) in new[] { (formQ, formM, "unchanged"), (changedQ, changedM, "changed") })
        {
            MethodComparison.Compare($"Sym.walk_expression_as upper {label}", "Mpl.Run.Sym",
                qWalkExprAs, symUpperQ, [null, fQ], mWalkExprAs, symUpperM, [null, fM],
                quireShow: res => ReferenceEquals(res, fQ) ? "same form" : res is SCons r && ReferenceEquals(r.head, fQ.head) ? "same head: " + Describe(res) : Describe(res),
                originalShow: res => ReferenceEquals(res, fM) ? "same form" : res != null && ReferenceEquals(Reflect.Get(res, "Field1"), Reflect.Get(fM, "Field1")) ? "same head: " + Describe(res) : Describe(res),
                quireState: o => Describe(o), originalState: o => Describe(o));
        }
        Compare("Sym.walk_expression_as upper null form", "Mpl.Run.Sym", qWalkExprAs, symUpperQ, [qEnv, null], mWalkExprAs, symUpperM, [mEnv, null]);
        Compare("Sym.walk_definition_as null form", "Mpl.Run.Sym", qWalkDefAs, symUpperQ, [qEnv, null], mWalkDefAs, symUpperM, [mEnv, null]);

        // Lower-case symbols and declarations consult the environment first; without one both sides fail there.
        var symLowerQ = new Sym("foo", null);
        var symLowerM = ToMathcad(symLowerQ)!;
        var lowerForm = L(S("foo", 4), I(1));
        var mLowerForm = ToMathcad(lowerForm);
        Compare("Sym.walk_expression_as lower, no env", "Mpl.Run.Sym", qWalkExprAs, symLowerQ, [null, lowerForm], mWalkExprAs, symLowerM, [null, mLowerForm]);
        Compare("Sym.walk_declaration_as lower, no env", "Mpl.Run.Sym", qWalkDeclAs, symLowerQ, [null, lowerForm], mWalkDeclAs, symLowerM, [null, mLowerForm]);
        Compare("Sym.walk_declaration_as upper, no env", "Mpl.Run.Sym", qWalkDeclAs, symUpperQ, [null, form], mWalkDeclAs, symUpperM, [null, mForm]);
    }

    [SkippableFact]
    public void Tok_members()
    {
        Runtime.Sync();
        var (qCc, mCc) = (typeof(Tok).TypeInitializer!, MType(typeof(Tok)).TypeInitializer!);
        Compare("Tok.cctor", "Mpl.Run.Tok", qCc, null, [], mCc, null, []);

        var (qCtorObjInt, mCtorObjInt) = Ctor(typeof(Tok), DeclaredInstancePublic, typeof(object), typeof(int));
        var (qCtorAtomInt, mCtorAtomInt) = Ctor(typeof(Tok), DeclaredInstancePublic, typeof(Atom), typeof(int));
        var (qCtorAtomUint, mCtorAtomUint) = Ctor(typeof(Tok), DeclaredInstancePublic, typeof(Atom), typeof(uint));

        var atomQ = new Sym("a", null);
        var atomM = ToMathcad(atomQ)!;
        foreach (var (aQ, aM) in new[] { (atomQ, atomM), ((object)null!, (object)null!) })
        {
            Compare("Tok(atom, -1)", "Mpl.Run.Tok", qCtorObjInt, null, [aQ, -1], mCtorObjInt, null, [aM, -1]);
            Compare("Tok(atom, 16)", "Mpl.Run.Tok", qCtorObjInt, null, [aQ, 16], mCtorObjInt, null, [aM, 16]);
            Compare("Tok(Atom, int.MinValue)", "Mpl.Run.Tok", qCtorAtomInt, null, [aQ, int.MinValue], mCtorAtomInt, null, [aM, int.MinValue]);
            Compare("Tok(Atom, uint.MaxValue)", "Mpl.Run.Tok", qCtorAtomUint, null, [aQ, uint.MaxValue], mCtorAtomUint, null, [aM, uint.MaxValue]);
            Compare("Tok(Atom, 0u)", "Mpl.Run.Tok", qCtorAtomUint, null, [aQ, 0u], mCtorAtomUint, null, [aM, 0u]);
        }
        Compare("Tok('not an atom', 0)", "Mpl.Run.Tok", qCtorObjInt, null, ["not an atom", 0], mCtorObjInt, null, ["not an atom", 0]);

        var (qHandle, mHandle) = Method(typeof(Tok), "get_handle", DeclaredInstancePublic);
        var (qAtom, mAtom) = Method(typeof(Tok), "get_atom", DeclaredInstancePublic);
        var (qKey, mKey) = Method(typeof(Tok), "get_Key", DeclaredInstancePublic);
        var (qSym, mSym) = Method(typeof(Tok), "get_sym", DeclaredInstancePublic);
        var (qStr, mStr) = Method(typeof(Tok), "get_str", DeclaredInstancePublic);
        var (qQuote, mQuote) = Method(typeof(Tok), "get_quote", DeclaredInstancePublic);
        var (qToString, mToString) = Method(typeof(Tok), "ToString", DeclaredInstancePublic);
        var (qWalkExpr, mWalkExpr) = Method(typeof(Tok), "walk_expression", DeclaredInstancePublic, typeof(MplEnv));
        var (qWalkPat, mWalkPat) = Method(typeof(Tok), "walk_pattern", DeclaredInstancePublic, typeof(MplEnv));

        var qEnv = new LocalEnv();
        var mEnv = New(M("Mpl.Run.LocalEnv"), [])!;

        Tok[] toks =
        [
            S("x", 5), S("X", 0x1F), S(""), S(null), I(42), I(-7, 300), R(1.5), Str("a b"), Str(null),
            new Tok(new char_constant('c'), 4), new Tok((Atom)null!, 0u)
        ];

        foreach (var t in toks)
        {
            var m = ToMathcad(t)!;
            string label = Describe(t);
            Compare($"{label}.handle", "Mpl.Run.Tok", qHandle, t, [], mHandle, m, []);
            Compare($"{label}.atom", "Mpl.Run.Tok", qAtom, t, [], mAtom, m, []);
            Compare($"{label}.Key", "Mpl.Run.Tok", qKey, t, [], mKey, m, []);
            Compare($"{label}.sym", "Mpl.Run.Tok", qSym, t, [], mSym, m, []);
            Compare($"{label}.str", "Mpl.Run.Tok", qStr, t, [], mStr, m, []);
            Compare($"{label}.quote", "Mpl.Run.Tok", qQuote, t, [], mQuote, m, []);
            Compare($"{label}.ToString()", "Mpl.Run.Tok", qToString, t, [], mToString, m, []);
            Compare($"{label}.walk_expression", "Mpl.Run.Tok", qWalkExpr, t, [qEnv], mWalkExpr, m, [mEnv]);
            if (t.Key == null || string.IsNullOrEmpty(t.Key.Field1))
                Compare($"{label}.walk_pattern", "Mpl.Run.Tok", qWalkPat, t, [qEnv], mWalkPat, m, [mEnv]);
            // Without an environment every path that needs one fails at its first call (LoadMacro or AddLocal).
            Compare($"{label}.walk_expression, no env", "Mpl.Run.Tok", qWalkExpr, t, [null], mWalkExpr, m, [null]);
            Compare($"{label}.walk_pattern, no env", "Mpl.Run.Tok", qWalkPat, t, [null], mWalkPat, m, [null]);
        }

        // Dedicated comparisons for Tok.walk_expression with macros:
        // 1. Bound local variable: LoadMacro returns null -> returns this
        var qEnvLocal = new LocalEnv();
        var mEnvLocal = New(M("Mpl.Run.LocalEnv"), [])!;
        var qSymLoc = new Sym("loc", null);
        var mSymLoc = ToMathcad(qSymLoc)!;
        qEnvLocal.AddLocal(qSymLoc, 77u);
        Call(mEnvLocal, "AddLocal", ["Sym", "UInt32"], mSymLoc, 77u);
        var qTokLoc = new Tok(qSymLoc, 99u);
        var mTokLoc = ToMathcad(qTokLoc)!;
        Compare("Tok.walk_expression bound local returns this", "Mpl.Run.Tok",
            qWalkExpr, qTokLoc, [qEnvLocal], mWalkExpr, mTokLoc, [mEnvLocal]);

        // 2. Bound symbol macro: returns expanded and walked replacement
        var qEnvMacro = new LocalEnv();
        var mEnvMacro = New(M("Mpl.Run.LocalEnv"), [])!;
        var qSymA = new Sym("a", null);
        var mSymA = ToMathcad(qSymA)!;
        var qRepA = I(42, 7u);
        var mRepA = ToMathcad(qRepA)!;
        qEnvMacro.AddSymbolMacro(qSymA, 1u, qRepA);
        Call(mEnvMacro, "AddSymbolMacro", ["Sym", "UInt32", "Sexpr"], mSymA, 1u, mRepA);
        var qTokA = new Tok(qSymA, 10u);
        var mTokA = ToMathcad(qTokA)!;
        Compare("Tok.walk_expression bound symbol macro", "Mpl.Run.Tok",
            qWalkExpr, qTokA, [qEnvMacro], mWalkExpr, mTokA, [mEnvMacro]);

        // 3. Chained symbol macros: macro B expands to macro C which expands to a constant
        var qEnvChain = new LocalEnv();
        var mEnvChain = New(M("Mpl.Run.LocalEnv"), [])!;
        var qSymB = new Sym("b", null);
        var mSymB = ToMathcad(qSymB)!;
        var qSymC = new Sym("c", null);
        var mSymC = ToMathcad(qSymC)!;
        var qConst = I(99, 12u);
        var mConst = ToMathcad(qConst)!;
        var qTokC = new Tok(qSymC, 11u);
        var mTokC = ToMathcad(qTokC)!;
        qEnvChain.AddSymbolMacro(qSymB, 2u, qTokC);
        Call(mEnvChain, "AddSymbolMacro", ["Sym", "UInt32", "Sexpr"], mSymB, 2u, mTokC);
        qEnvChain.AddSymbolMacro(qSymC, 3u, qConst);
        Call(mEnvChain, "AddSymbolMacro", ["Sym", "UInt32", "Sexpr"], mSymC, 3u, mConst);
        var qTokB = new Tok(qSymB, 15u);
        var mTokB = ToMathcad(qTokB)!;
        Compare("Tok.walk_expression chained symbol macros", "Mpl.Run.Tok",
            qWalkExpr, qTokB, [qEnvChain], mWalkExpr, mTokB, [mEnvChain]);

        // 4. Bound function macro: base MplMacro.ExpandRef throws AppException("illegal macro reference", 0u, handle)
        var qEnvFn = new LocalEnv();
        var mEnvFn = New(M("Mpl.Run.LocalEnv"), [])!;
        var qSymFn = new Sym("fn", null);
        var mSymFn = ToMathcad(qSymFn)!;
        var qQuoteMacro = (MplMacro)New(typeof(MplQuoteMacro));
        var mQuoteMacro = New(M("Mpl.Run.MplQuoteMacro"))!;
        var qTableFn = (SortedDictionary<string, BindingInfo>)Reflect.Get(qEnvFn, "m_table")!;
        qTableFn["fn"] = new MacroBindingInfo(42u, qQuoteMacro);
        var mTableFn = (System.Collections.IDictionary)Reflect.Get(mEnvFn, "m_table")!;
        var mMacroBinding = New(M("Mpl.Run.MacroBindingInfo"), ["UInt32", "MplMacro"], 42u, mQuoteMacro)!;
        mTableFn["fn"] = mMacroBinding;
        var qTokFn = new Tok(qSymFn, 88u);
        var mTokFn = ToMathcad(qTokFn)!;
        Compare("Tok.walk_expression bound function macro throws illegal macro ref", "Mpl.Run.Tok",
            qWalkExpr, qTokFn, [qEnvFn], mWalkExpr, mTokFn, [mEnvFn]);

        // 5. Scoped LocalEnv(parent): shadowing and parent resolution
        var qParent = new LocalEnv();
        var mParent = New(M("Mpl.Run.LocalEnv"), [])!;
        var qSymParent = new Sym("p", null);
        var mSymParent = ToMathcad(qSymParent)!;
        var qSymShadow = new Sym("s", null);
        var mSymShadow = ToMathcad(qSymShadow)!;
        qParent.AddSymbolMacro(qSymParent, 1u, I(100, 1u));
        Call(mParent, "AddSymbolMacro", ["Sym", "UInt32", "Sexpr"], mSymParent, 1u, ToMathcad(I(100, 1u))!);
        qParent.AddSymbolMacro(qSymShadow, 2u, I(200, 2u));
        Call(mParent, "AddSymbolMacro", ["Sym", "UInt32", "Sexpr"], mSymShadow, 2u, ToMathcad(I(200, 2u))!);

        var qChild = new LocalEnv(qParent);
        var mChild = New(M("Mpl.Run.LocalEnv"), ["MplEnv"], mParent)!;
        var qSymChild = new Sym("k", null);
        var mSymChild = ToMathcad(qSymChild)!;
        qChild.AddSymbolMacro(qSymShadow, 3u, I(300, 3u));
        Call(mChild, "AddSymbolMacro", ["Sym", "UInt32", "Sexpr"], mSymShadow, 3u, ToMathcad(I(300, 3u))!);
        qChild.AddSymbolMacro(qSymChild, 4u, I(400, 4u));
        Call(mChild, "AddSymbolMacro", ["Sym", "UInt32", "Sexpr"], mSymChild, 4u, ToMathcad(I(400, 4u))!);

        var qTokParent = new Tok(qSymParent, 50u);
        var mTokParent = ToMathcad(qTokParent)!;
        var qTokShadow = new Tok(qSymShadow, 51u);
        var mTokShadow = ToMathcad(qTokShadow)!;
        var qTokChild = new Tok(qSymChild, 52u);
        var mTokChild = ToMathcad(qTokChild)!;

        // In child, parent symbol resolves through parent
        Compare("Tok.walk_expression scoped parent resolution", "Mpl.Run.Tok",
            qWalkExpr, qTokParent, [qChild], mWalkExpr, mTokParent, [mChild]);
        // In child, shadowed symbol resolves to child macro (300)
        Compare("Tok.walk_expression scoped shadowing in child", "Mpl.Run.Tok",
            qWalkExpr, qTokShadow, [qChild], mWalkExpr, mTokShadow, [mChild]);
        // In parent, shadowed symbol resolves to parent macro (200)
        Compare("Tok.walk_expression scoped resolution in parent", "Mpl.Run.Tok",
            qWalkExpr, qTokShadow, [qParent], mWalkExpr, mTokShadow, [mParent]);
        // In child, child-only symbol resolves in child
        Compare("Tok.walk_expression scoped child resolution", "Mpl.Run.Tok",
            qWalkExpr, qTokChild, [qChild], mWalkExpr, mTokChild, [mChild]);
    }

    [SkippableFact]
    public void Nul_and_Cons_members()
    {
        Runtime.Sync();
        // Nul
        var (qNulCc, mNulCc) = (typeof(Nul).TypeInitializer!, MType(typeof(Nul)).TypeInitializer!);
        Compare("Nul.cctor", "Mpl.Run.Nul", qNulCc, null, [], mNulCc, null, []);

        var (qNulCtor, mNulCtor) = Ctor(typeof(Nul), DeclaredInstancePublic);
        Compare("new Nul()", "Mpl.Run.Nul", qNulCtor, null, [], mNulCtor, null, []);

        var (qNulToString, mNulToString) = Method(typeof(Nul), "ToString", DeclaredInstancePublic);
        var (qNulFormatTail, mNulFormatTail) = Method(typeof(Nul), "FormatTail", DeclaredInstancePublic);
        var (qNulScons, mNulScons) = Method(typeof(Nul), "get_scons", DeclaredInstancePublic);
        var (qNulQuote, mNulQuote) = Method(typeof(Nul), "get_quote", DeclaredInstancePublic);

        var qNul = Nul.singleton;
        var mNul = ToMathcad(qNul)!;
        Compare("Nul.ToString()", "Mpl.Run.Nul", qNulToString, qNul, [], mNulToString, mNul, []);
        Compare("Nul.FormatTail()", "Mpl.Run.Nul", qNulFormatTail, qNul, [], mNulFormatTail, mNul, []);
        Compare("Nul.scons", "Mpl.Run.Nul", qNulScons, qNul, [], mNulScons, mNul, []);
        Compare("Nul.quote", "Mpl.Run.Nul", qNulQuote, qNul, [], mNulQuote, mNul, []);

        // Cons
        var (qConsCtor, mConsCtor) = Ctor(typeof(Cons), DeclaredInstancePublic, typeof(object), typeof(object));
        Compare("Cons(1, 2)", "Mpl.Run.Cons", qConsCtor, null, [1, 2], mConsCtor, null, [1, 2]);
        Compare("Cons(null, null)", "Mpl.Run.Cons", qConsCtor, null, [null, null], mConsCtor, null, [null, null]);

        var (qHead, mHead) = Method(typeof(Cons), "get_head", DeclaredInstancePublic);
        var (qTail, mTail) = Method(typeof(Cons), "get_tail", DeclaredInstancePublic);
        var (qLength, mLength) = Method(typeof(Cons), "Length", DeclaredStaticPublic, typeof(Cons));
        var (qAppend, mAppend) = Method(typeof(Cons), "destructive_append", DeclaredStaticPublic, typeof(Cons), typeof(Cons));
        var (qConsToString, mConsToString) = Method(typeof(Cons), "ToString", DeclaredInstancePublic);
        var (qFormatItems, mFormatItems) = Method(typeof(Cons), "FormatItems", DeclaredInstanceNonPublic);

        Cons[] consList =
        [
            new Cons(1, 2), new Cons(1, "tail"), new Cons(null, null),
            new Cons(1, new Cons(2, new Cons(3, null)))
        ];

        foreach (var c in consList)
        {
            var m = ToMathcad(c)!;
            string label = Describe(c);
            Compare($"{label}.head", "Mpl.Run.Cons", qHead, c, [], mHead, m, []);
            Compare($"{label}.tail", "Mpl.Run.Cons", qTail, c, [], mTail, m, []);
            Compare($"{label}.ToString()", "Mpl.Run.Cons", qConsToString, c, [], mConsToString, m, []);
            Compare($"{label}.FormatItems()", "Mpl.Run.Cons", qFormatItems, c, [], mFormatItems, m, []);
            Compare($"Cons.Length({label})", "Mpl.Run.Cons", qLength, null, [c], mLength, null, [m]);
        }
        Compare("Cons.Length(null)", "Mpl.Run.Cons", qLength, null, [null], mLength, null, [null]);

        // destructive_append
        foreach (var (a, b) in new[]
        {
            ((Cons)null!, (Cons)null!),
            (new Cons(1, null), (Cons)null!),
            ((Cons)null!, new Cons(2, null)),
            (new Cons(1, null), new Cons(2, null)),
            (new Cons(1, new Cons(2, null)), new Cons(3, new Cons(4, null)))
        })
        {
            var aQ = a != null ? new Cons(a.Field1, a.Field2) : null;
            var aM = ToMathcad(aQ);
            var bQ = b != null ? new Cons(b.Field1, b.Field2) : null;
            var bM = ToMathcad(bQ);
            Compare($"destructive_append({Describe(aQ)}, {Describe(bQ)})", "Mpl.Run.Cons", qAppend, null, [aQ, bQ], mAppend, null, [aM, bM]);
        }
    }

    [SkippableFact]
    public void Quote_wrappers()
    {
        Runtime.Sync();
        var tok = S("e", 1);
        var mTok = ToMathcad(tok);

        var qEnv = new LocalEnv();
        var mEnv = New(M("Mpl.Run.LocalEnv"), [])!;

        // Qt
        var (qQtCc, mQtCc) = (typeof(Qt).TypeInitializer!, MType(typeof(Qt)).TypeInitializer!);
        Compare("Qt.cctor", "Mpl.Run.Qt", qQtCc, null, [], mQtCc, null, []);

        var (qQtCtorObj, mQtCtorObj) = Ctor(typeof(Qt), DeclaredInstancePublic, typeof(object));
        var (qQtCtorSex, mQtCtorSex) = Ctor(typeof(Qt), DeclaredInstancePublic, typeof(Sexpr));
        var (qQtToString, mQtToString) = Method(typeof(Qt), "ToString", DeclaredInstancePublic);
        var (qQtQuotation, mQtQuotation) = Method(typeof(Qt), "get_quotation", DeclaredInstancePublic);
        var (qQtQuote, mQtQuote) = Method(typeof(Qt), "get_quote", DeclaredInstancePublic);
        var (qQtWalkExpr, mQtWalkExpr) = Method(typeof(Qt), "walk_expression", DeclaredInstancePublic, typeof(MplEnv));

        Compare("Qt(tok obj)", "Mpl.Run.Qt", qQtCtorObj, null, [tok], mQtCtorObj, null, [mTok]);
        Compare("Qt('not sexpr')", "Mpl.Run.Qt", qQtCtorObj, null, ["not sexpr"], mQtCtorObj, null, ["not sexpr"]);
        Compare("Qt(null obj)", "Mpl.Run.Qt", qQtCtorObj, null, [null], mQtCtorObj, null, [null]);
        Compare("Qt(tok sexpr)", "Mpl.Run.Qt", qQtCtorSex, null, [tok], mQtCtorSex, null, [mTok]);
        Compare("Qt(null sexpr)", "Mpl.Run.Qt", qQtCtorSex, null, [null], mQtCtorSex, null, [null]);

        var qQt = new Qt(tok);
        var mQt = ToMathcad(qQt)!;
        Compare("Qt.ToString()", "Mpl.Run.Qt", qQtToString, qQt, [], mQtToString, mQt, []);
        Compare("Qt.quotation", "Mpl.Run.Qt", qQtQuotation, qQt, [], mQtQuotation, mQt, []);
        Compare("Qt.quote", "Mpl.Run.Qt", qQtQuote, qQt, [], mQtQuote, mQt, []);
        Compare("Qt.walk_expression", "Mpl.Run.Qt", qQtWalkExpr, qQt, [qEnv], mQtWalkExpr, mQt, [mEnv]);

        // Bq
        var (qBqCtorObj, mBqCtorObj) = Ctor(typeof(Bq), DeclaredInstancePublic, typeof(object));
        var (qBqCtorSex, mBqCtorSex) = Ctor(typeof(Bq), DeclaredInstancePublic, typeof(Sexpr));
        var (qBqToString, mBqToString) = Method(typeof(Bq), "ToString", DeclaredInstancePublic);
        var (qBqWalkExpr, mBqWalkExpr) = Method(typeof(Bq), "walk_expression", DeclaredInstancePublic, typeof(MplEnv));

        Compare("Bq(tok obj)", "Mpl.Run.Bq", qBqCtorObj, null, [tok], mBqCtorObj, null, [mTok]);
        Compare("Bq(null obj)", "Mpl.Run.Bq", qBqCtorObj, null, [null], mBqCtorObj, null, [null]);
        Compare("Bq(tok sexpr)", "Mpl.Run.Bq", qBqCtorSex, null, [tok], mBqCtorSex, null, [mTok]);
        Compare("Bq(null sexpr)", "Mpl.Run.Bq", qBqCtorSex, null, [null], mBqCtorSex, null, [null]);

        var qBq = new Bq(tok);
        var mBq = ToMathcad(qBq)!;
        Compare("Bq.ToString()", "Mpl.Run.Bq", qBqToString, qBq, [], mBqToString, mBq, []);
        Compare("Bq.walk_expression", "Mpl.Run.Bq", qBqWalkExpr, qBq, [qEnv], mBqWalkExpr, mBq, [mEnv]);

        // Cm
        var (qCmCtorObj, mCmCtorObj) = Ctor(typeof(Cm), DeclaredInstancePublic, typeof(object));
        var (qCmCtorSex, mCmCtorSex) = Ctor(typeof(Cm), DeclaredInstancePublic, typeof(Sexpr));
        var (qCmToString, mCmToString) = Method(typeof(Cm), "ToString", DeclaredInstancePublic);

        Compare("Cm(tok obj)", "Mpl.Run.Cm", qCmCtorObj, null, [tok], mCmCtorObj, null, [mTok]);
        Compare("Cm(null obj)", "Mpl.Run.Cm", qCmCtorObj, null, [null], mCmCtorObj, null, [null]);
        Compare("Cm(tok sexpr)", "Mpl.Run.Cm", qCmCtorSex, null, [tok], mCmCtorSex, null, [mTok]);
        Compare("Cm(null sexpr)", "Mpl.Run.Cm", qCmCtorSex, null, [null], mCmCtorSex, null, [null]);

        var qCm = new Cm(tok);
        var mCm = ToMathcad(qCm)!;
        Compare("Cm.ToString()", "Mpl.Run.Cm", qCmToString, qCm, [], mCmToString, mCm, []);

        // Ca
        var (qCaCtorObj, mCaCtorObj) = Ctor(typeof(Ca), DeclaredInstancePublic, typeof(object));
        var (qCaCtorSex, mCaCtorSex) = Ctor(typeof(Ca), DeclaredInstancePublic, typeof(Sexpr));
        var (qCaToString, mCaToString) = Method(typeof(Ca), "ToString", DeclaredInstancePublic);

        Compare("Ca(tok obj)", "Mpl.Run.Ca", qCaCtorObj, null, [tok], mCaCtorObj, null, [mTok]);
        Compare("Ca(null obj)", "Mpl.Run.Ca", qCaCtorObj, null, [null], mCaCtorObj, null, [null]);
        Compare("Ca(tok sexpr)", "Mpl.Run.Ca", qCaCtorSex, null, [tok], mCaCtorSex, null, [mTok]);
        Compare("Ca(null sexpr)", "Mpl.Run.Ca", qCaCtorSex, null, [null], mCaCtorSex, null, [null]);

        var qCa = new Ca(tok);
        var mCa = ToMathcad(qCa)!;
        Compare("Ca.ToString()", "Mpl.Run.Ca", qCaToString, qCa, [], mCaToString, mCa, []);
    }

    [SkippableFact]
    public void SAppend_and_SList_members()
    {
        Runtime.Sync();
        var tok = S("e", 1);
        var mTok = ToMathcad(tok);
        var nul = Nul.singleton;
        var mNul = ToMathcad(nul);

        // SAppend
        var (qAppCtorObj, mAppCtorObj) = Ctor(typeof(SAppend), DeclaredInstancePublic, typeof(object), typeof(object));
        var (qAppCtorSex, mAppCtorSex) = Ctor(typeof(SAppend), DeclaredInstancePublic, typeof(Sexpr), typeof(Sexpr));
        var (qAppFormat, mAppFormat) = Method(typeof(SAppend), "FormatAppend", DeclaredInstancePublic);
        var (qAppToString, mAppToString) = Method(typeof(SAppend), "ToString", DeclaredInstancePublic);

        Compare("SAppend(tok, nul obj)", "Mpl.Run.SAppend", qAppCtorObj, null, [tok, nul], mAppCtorObj, null, [mTok, mNul]);
        Compare("SAppend(tok, 'x' obj)", "Mpl.Run.SAppend", qAppCtorObj, null, [tok, "x"], mAppCtorObj, null, [mTok, "x"]);
        Compare("SAppend(null, null obj)", "Mpl.Run.SAppend", qAppCtorObj, null, [null, null], mAppCtorObj, null, [null, null]);
        Compare("SAppend(tok, nul sexpr)", "Mpl.Run.SAppend", qAppCtorSex, null, [tok, nul], mAppCtorSex, null, [mTok, mNul]);
        Compare("SAppend(tok, tok sexpr)", "Mpl.Run.SAppend", qAppCtorSex, null, [tok, tok], mAppCtorSex, null, [mTok, mTok]);
        Compare("SAppend(null, null sexpr)", "Mpl.Run.SAppend", qAppCtorSex, null, [null, null], mAppCtorSex, null, [null, null]);

        var qApp = new SAppend(tok, new SAppend(S("b"), nul));
        var mApp = ToMathcad(qApp)!;
        Compare("SAppend.FormatAppend()", "Mpl.Run.SAppend", qAppFormat, qApp, [], mAppFormat, mApp, []);
        Compare("SAppend.ToString()", "Mpl.Run.SAppend", qAppToString, qApp, [], mAppToString, mApp, []);

        var qAppDotted = new SAppend(tok, S("b", 1));
        var mAppDotted = ToMathcad(qAppDotted)!;
        Compare("SAppend dotted ToString()", "Mpl.Run.SAppend", qAppToString, qAppDotted, [], mAppToString, mAppDotted, []);

        // SList
        var (qListCtorObj, mListCtorObj) = Ctor(typeof(SList), DeclaredInstancePublic, typeof(object), typeof(object));
        var (qListCtorCons, mListCtorCons) = Ctor(typeof(SList), DeclaredInstancePublic, typeof(Cons), typeof(Sexpr));
        var (qListFormatCons, mListFormatCons) = Method(typeof(SList), "FormatCons", DeclaredStaticNonPublic, typeof(Cons));
        var (qListToString, mListToString) = Method(typeof(SList), "ToString", DeclaredInstancePublic);

        var cons = new Cons(tok, new Cons(5, null));
        var mCons = ToMathcad(cons);

        Compare("SList(cons, nul obj)", "Mpl.Run.SList", qListCtorObj, null, [cons, nul], mListCtorObj, null, [mCons, mNul]);
        Compare("SList('x', 'y' obj)", "Mpl.Run.SList", qListCtorObj, null, ["x", "y"], mListCtorObj, null, ["x", "y"]);
        Compare("SList(null, nul obj)", "Mpl.Run.SList", qListCtorObj, null, [null, nul], mListCtorObj, null, [null, mNul]);
        Compare("SList(cons, nul sexpr)", "Mpl.Run.SList", qListCtorCons, null, [cons, nul], mListCtorCons, null, [mCons, mNul]);
        Compare("SList(cons, tok sexpr)", "Mpl.Run.SList", qListCtorCons, null, [cons, tok], mListCtorCons, null, [mCons, mTok]);
        Compare("SList(null, nul sexpr)", "Mpl.Run.SList", qListCtorCons, null, [null, nul], mListCtorCons, null, [null, mNul]);

        Compare("SList.FormatCons(null)", "Mpl.Run.SList", qListFormatCons, null, [null], mListFormatCons, null, [null]);
        Compare("SList.FormatCons(cons)", "Mpl.Run.SList", qListFormatCons, null, [cons], mListFormatCons, null, [mCons]);

        var qListSingleton = new SList(cons, Nul.singleton);
        var mListSingleton = ToMathcad(qListSingleton)!;
        Compare("SList.ToString() singleton", "Mpl.Run.SList", qListToString, qListSingleton, [], mListToString, mListSingleton, []);

        var qListNewNul = new SList(cons, new Nul());
        var mListNewNul = ToMathcad(qListNewNul)!;
        Compare("SList.ToString() new Nul", "Mpl.Run.SList", qListToString, qListNewNul, [], mListToString, mListNewNul, []);

        var qListDotted = new SList(cons, S("t", 2));
        var mListDotted = ToMathcad(qListDotted)!;
        Compare("SList.ToString() dotted", "Mpl.Run.SList", qListToString, qListDotted, [], mListToString, mListDotted, []);
    }

    [SkippableFact]
    public void SCons_members()
    {
        Runtime.Sync();
        var tok = S("e", 1);
        var mTok = ToMathcad(tok);
        var nul = Nul.singleton;
        var mNul = ToMathcad(nul);

        var (qCc, mCc) = (typeof(SCons).TypeInitializer!, MType(typeof(SCons)).TypeInitializer!);
        Compare("SCons.cctor", "Mpl.Run.SCons", qCc, null, [], mCc, null, []);

        var (qCtorObj, mCtorObj) = Ctor(typeof(SCons), DeclaredInstancePublic, typeof(object), typeof(object));
        var (qCtorSex, mCtorSex) = Ctor(typeof(SCons), DeclaredInstancePublic, typeof(Sexpr), typeof(Sexpr));

        Compare("SCons(tok, nul obj)", "Mpl.Run.SCons", qCtorObj, null, [tok, nul], mCtorObj, null, [mTok, mNul]);
        Compare("SCons(tok, 'x' obj)", "Mpl.Run.SCons", qCtorObj, null, [tok, "x"], mCtorObj, null, [mTok, "x"]);
        Compare("SCons(null, null obj)", "Mpl.Run.SCons", qCtorObj, null, [null, null], mCtorObj, null, [null, null]);
        Compare("SCons(tok, nul sexpr)", "Mpl.Run.SCons", qCtorSex, null, [tok, nul], mCtorSex, null, [mTok, mNul]);
        Compare("SCons(tok, tok sexpr)", "Mpl.Run.SCons", qCtorSex, null, [tok, tok], mCtorSex, null, [mTok, mTok]);
        Compare("SCons(null, null sexpr)", "Mpl.Run.SCons", qCtorSex, null, [null, null], mCtorSex, null, [null, null]);

        var (qList1, mList1) = Method(typeof(SCons), "list1", DeclaredStaticPublic, typeof(Sexpr));
        var (qList2, mList2) = Method(typeof(SCons), "list2", DeclaredStaticPublic, typeof(Sexpr), typeof(Sexpr));
        var (qList3, mList3) = Method(typeof(SCons), "list3", DeclaredStaticPublic, typeof(Sexpr), typeof(Sexpr), typeof(Sexpr));
        var (qList4, mList4) = Method(typeof(SCons), "list4", DeclaredStaticPublic, typeof(Sexpr), typeof(Sexpr), typeof(Sexpr), typeof(Sexpr));
        var (qAsSexpr, mAsSexpr) = Method(typeof(SCons), "as_Sexpr", DeclaredStaticPublic, typeof(Cons));

        Compare("list1(tok)", "Mpl.Run.SCons", qList1, null, [tok], mList1, null, [mTok]);
        Compare("list2(tok, null)", "Mpl.Run.SCons", qList2, null, [tok, null], mList2, null, [mTok, null]);
        Compare("list3(tok, I(2), tok)", "Mpl.Run.SCons", qList3, null, [tok, I(2), tok], mList3, null, [mTok, ToMathcad(I(2)), mTok]);
        Compare("list4(tok, I(2), nul, L(tok))", "Mpl.Run.SCons", qList4, null, [tok, I(2), nul, L(tok)], mList4, null, [mTok, ToMathcad(I(2)), mNul, ToMathcad(L(tok))]);

        var cons = new Cons(tok, new Cons(7, null));
        var mCons = ToMathcad(cons);
        Compare("as_Sexpr(null)", "Mpl.Run.SCons", qAsSexpr, null, [null], mAsSexpr, null, [null]);
        Compare("as_Sexpr(cons)", "Mpl.Run.SCons", qAsSexpr, null, [cons], mAsSexpr, null, [mCons]);

        var (qLength, mLength) = Method(typeof(SCons), "get_Length", DeclaredInstancePublic);
        var (qHead, mHead) = Method(typeof(SCons), "get_head", DeclaredInstancePublic);
        var (qCar, mCar) = Method(typeof(SCons), "get_car", DeclaredInstancePublic);
        var (qFirst, mFirst) = Method(typeof(SCons), "get_first", DeclaredInstancePublic);
        var (qTail, mTail) = Method(typeof(SCons), "get_tail", DeclaredInstancePublic);
        var (qCdr, mCdr) = Method(typeof(SCons), "get_cdr", DeclaredInstancePublic);
        var (qSecond, mSecond) = Method(typeof(SCons), "get_second", DeclaredInstancePublic);
        var (qThird, mThird) = Method(typeof(SCons), "get_third", DeclaredInstancePublic);
        var (qFourth, mFourth) = Method(typeof(SCons), "get_fourth", DeclaredInstancePublic);
        var (qFifth, mFifth) = Method(typeof(SCons), "get_fifth", DeclaredInstancePublic);
        var (qHandle, mHandle) = Method(typeof(SCons), "get_handle", DeclaredInstancePublic);
        var (qScons, mScons) = Method(typeof(SCons), "get_scons", DeclaredInstancePublic);
        var (qQuote, mQuote) = Method(typeof(SCons), "get_quote", DeclaredInstancePublic);
        var (qKey, mKey) = Method(typeof(SCons), "get_Key", DeclaredInstancePublic);
        var (qFormatTail, mFormatTail) = Method(typeof(SCons), "FormatTail", DeclaredInstancePublic);
        var (qToString, mToString) = Method(typeof(SCons), "ToString", DeclaredInstancePublic);

        var fiveItems = L(I(1), I(2), I(3), I(4), I(5));
        var mList5 = ToMathcad(fiveItems)!;
        Compare("list5.Length", "Mpl.Run.SCons", qLength, fiveItems, [], mLength, mList5, []);
        Compare("list5.head", "Mpl.Run.SCons", qHead, fiveItems, [], mHead, mList5, []);
        Compare("list5.car", "Mpl.Run.SCons", qCar, fiveItems, [], mCar, mList5, []);
        Compare("list5.first", "Mpl.Run.SCons", qFirst, fiveItems, [], mFirst, mList5, []);
        Compare("list5.tail", "Mpl.Run.SCons", qTail, fiveItems, [], mTail, mList5, []);
        Compare("list5.cdr", "Mpl.Run.SCons", qCdr, fiveItems, [], mCdr, mList5, []);
        Compare("list5.second", "Mpl.Run.SCons", qSecond, fiveItems, [], mSecond, mList5, []);
        Compare("list5.third", "Mpl.Run.SCons", qThird, fiveItems, [], mThird, mList5, []);
        Compare("list5.fourth", "Mpl.Run.SCons", qFourth, fiveItems, [], mFourth, mList5, []);
        Compare("list5.fifth", "Mpl.Run.SCons", qFifth, fiveItems, [], mFifth, mList5, []);
        Compare("list5.scons", "Mpl.Run.SCons", qScons, fiveItems, [], mScons, mList5, []);
        Compare("list5.quote", "Mpl.Run.SCons", qQuote, fiveItems, [], mQuote, mList5, []);
        Compare("list5.FormatTail()", "Mpl.Run.SCons", qFormatTail, fiveItems, [], mFormatTail, mList5, []);
        Compare("list5.ToString()", "Mpl.Run.SCons", qToString, fiveItems, [], mToString, mList5, []);

        var singleItem = L(tok);
        var mList1Inst = ToMathcad(singleItem)!;
        Compare("list1.tail", "Mpl.Run.SCons", qTail, singleItem, [], mTail, mList1Inst, []);
        Compare("list1.second throws", "Mpl.Run.SCons", qSecond, singleItem, [], mSecond, mList1Inst, []);
        Compare("list1.third throws", "Mpl.Run.SCons", qThird, singleItem, [], mThird, mList1Inst, []);
        Compare("list1.fourth throws", "Mpl.Run.SCons", qFourth, singleItem, [], mFourth, mList1Inst, []);
        Compare("list1.fifth throws", "Mpl.Run.SCons", qFifth, singleItem, [], mFifth, mList1Inst, []);

        var sImproper = new SCons(tok, tok);
        var mImproper = ToMathcad(sImproper)!;
        Compare("improper.Length", "Mpl.Run.SCons", qLength, sImproper, [], mLength, mImproper, []);
        Compare("improper.ToString()", "Mpl.Run.SCons", qToString, sImproper, [], mToString, mImproper, []);

        var sNullHead = new SCons((Sexpr)null!, nul);
        var mNullHead = ToMathcad(sNullHead)!;
        Compare("nullHead.handle throws", "Mpl.Run.SCons", qHandle, sNullHead, [], mHandle, mNullHead, []);
        Compare("nullHead.Key throws", "Mpl.Run.SCons", qKey, sNullHead, [], mKey, mNullHead, []);

        var sHandle7 = new SCons(S("a", 7u), nul);
        var mHandle7 = ToMathcad(sHandle7)!;
        Compare("sHandle7.handle", "Mpl.Run.SCons", qHandle, sHandle7, [], mHandle, mHandle7, []);

        var sNestedHandle = new SCons(L(S("a", 12u)), nul);
        var mNestedHandle = ToMathcad(sNestedHandle)!;
        Compare("sNestedHandle.handle", "Mpl.Run.SCons", qHandle, sNestedHandle, [], mHandle, mNestedHandle, []);

        var sKeySym = new SCons(S("a"), nul);
        var mKeySym = ToMathcad(sKeySym)!;
        Compare("sKeySym.Key", "Mpl.Run.SCons", qKey, sKeySym, [], mKey, mKeySym, []);

        var sKeyInt = new SCons(I(1), nul);
        var mKeyInt = ToMathcad(sKeyInt)!;
        Compare("sKeyInt.Key", "Mpl.Run.SCons", qKey, sKeyInt, [], mKey, mKeyInt, []);

        var sNullTail = new SCons(tok, (Sexpr)null!);
        var mNullTail = ToMathcad(sNullTail)!;
        Compare("sNullTail.ToString() throws", "Mpl.Run.SCons", qToString, sNullTail, [], mToString, mNullTail, []);

        // walks
        var qEnv = new LocalEnv();
        var mEnv = New(M("Mpl.Run.LocalEnv"), [])!;

        var (qWalkDecl, mWalkDecl) = Method(typeof(SCons), "walk_declaration_lhs", DeclaredInstancePublic, typeof(MplEnv));
        var (qWalkPat, mWalkPat) = Method(typeof(SCons), "walk_pattern", DeclaredInstancePublic, typeof(MplEnv));
        var (qWalkExpr, mWalkExpr) = Method(typeof(SCons), "walk_expression", DeclaredInstancePublic, typeof(MplEnv));
        var (qWalkPats, mWalkPats) = Method(typeof(SCons), "walk_patterns", DeclaredStaticPublic, typeof(MplEnv), typeof(SCons));
        var (qWalkExprs, mWalkExprs) = Method(typeof(SCons), "walk_expressions", DeclaredStaticPublic, typeof(MplEnv), typeof(SCons));

        var patSymQ = L(S("Foo"), I(1));
        var patSymM = ToMathcad(patSymQ)!;
        var patIntQ = L(I(1), I(2));
        var patIntM = ToMathcad(patIntQ)!;
        Compare("SCons.walk_declaration_lhs non-sym", "Mpl.Run.SCons", qWalkDecl, patIntQ, [qEnv], mWalkDecl, patIntM, [mEnv]);
        Compare("SCons.walk_pattern non-sym", "Mpl.Run.SCons", qWalkPat, patIntQ, [qEnv], mWalkPat, patIntM, [mEnv]);
        Compare("SCons.walk_pattern upper", "Mpl.Run.SCons", qWalkPat, patSymQ, [qEnv], mWalkPat, patSymM, [mEnv]);

        var patRealQ = L(R(3.14), I(1));
        var patRealM = ToMathcad(patRealQ)!;
        Compare("SCons.walk_pattern real head", "Mpl.Run.SCons", qWalkPat, patRealQ, [qEnv], mWalkPat, patRealM, [mEnv]);

        var patStrQ = L(Str("hello"), I(1));
        var patStrM = ToMathcad(patStrQ)!;
        Compare("SCons.walk_pattern str head", "Mpl.Run.SCons", qWalkPat, patStrQ, [qEnv], mWalkPat, patStrM, [mEnv]);

        var patNestedQ = L(L(I(1), I(2)), I(3));
        var patNestedM = ToMathcad(patNestedQ)!;
        Compare("SCons.walk_pattern nested head", "Mpl.Run.SCons", qWalkPat, patNestedQ, [qEnv], mWalkPat, patNestedM, [mEnv]);

        var patEmptySymQ = L(S(""), I(1));
        var patEmptySymM = ToMathcad(patEmptySymQ)!;
        Compare("SCons.walk_pattern empty sym head", "Mpl.Run.SCons", qWalkPat, patEmptySymQ, [qEnv], mWalkPat, patEmptySymM, [mEnv]);

        var patNullSymQ = L(S(null), I(1));
        var patNullSymM = ToMathcad(patNullSymQ)!;
        Compare("SCons.walk_pattern null sym head", "Mpl.Run.SCons", qWalkPat, patNullSymQ, [qEnv], mWalkPat, patNullSymM, [mEnv]);

        var patMultiUpperQ = L(S("Bar"), I(1), Str("s"));
        var patMultiUpperM = ToMathcad(patMultiUpperQ)!;
        Compare("SCons.walk_pattern multi upper", "Mpl.Run.SCons", qWalkPat, patMultiUpperQ, [qEnv], mWalkPat, patMultiUpperM, [mEnv]);

        Compare("SCons.walk_patterns(null)", "Mpl.Run.SCons", qWalkPats, null, [qEnv, null], mWalkPats, null, [mEnv, null]);
        MethodComparison.Compare("SCons.walk_patterns(pats) reused", "Mpl.Run.SCons",
            qWalkPats, null, [qEnv, patIntQ],
            mWalkPats, null, [mEnv, patIntM],
            quireShow: res => ReferenceEquals(res, patIntQ) ? "reused" : Describe(res),
            originalShow: res => ReferenceEquals(res, patIntM) ? "reused" : Describe(res),
            quireState: o => Describe(o), originalState: o => Describe(o));

        Compare("SCons.walk_expression sym", "Mpl.Run.SCons", qWalkExpr, patSymQ, [qEnv], mWalkExpr, patSymM, [mEnv]);
        Compare("SCons.walk_expression non-sym", "Mpl.Run.SCons", qWalkExpr, patIntQ, [qEnv], mWalkExpr, patIntM, [mEnv]);
        Compare("SCons.walk_expression null-head", "Mpl.Run.SCons", qWalkExpr, sNullHead, [qEnv], mWalkExpr, mNullHead, [mEnv]);

        var sImproperWalkQ = new SCons(I(1, 1), I(2));
        var sImproperWalkM = ToMathcad(sImproperWalkQ)!;
        Compare("SCons.walk_expression improper", "Mpl.Run.SCons", qWalkExpr, sImproperWalkQ, [qEnv], mWalkExpr, sImproperWalkM, [mEnv]);

        Compare("SCons.walk_expressions(null)", "Mpl.Run.SCons", qWalkExprs, null, [qEnv, null], mWalkExprs, null, [mEnv, null]);
        MethodComparison.Compare("SCons.walk_expressions(exps) reused", "Mpl.Run.SCons",
            qWalkExprs, null, [qEnv, patSymQ],
            mWalkExprs, null, [mEnv, patSymM],
            quireShow: res => ReferenceEquals(res, patSymQ) ? "reused" : Describe(res),
            originalShow: res => ReferenceEquals(res, patSymM) ? "reused" : Describe(res),
            quireState: o => Describe(o), originalState: o => Describe(o));

        var nestedApplyQ = L(L(I(1), I(2)));
        var nestedApplyM = ToMathcad(nestedApplyQ)!;
        MethodComparison.Compare("SCons.walk_expressions(exps) changed", "Mpl.Run.SCons",
            qWalkExprs, null, [qEnv, nestedApplyQ],
            mWalkExprs, null, [mEnv, nestedApplyM],
            quireShow: res => ReferenceEquals(res, nestedApplyQ) ? "reused" : Describe(res),
            originalShow: res => ReferenceEquals(res, nestedApplyM) ? "reused" : Describe(res),
            quireState: o => Describe(o), originalState: o => Describe(o));

        // An upper-case pattern list always walks to a new cell, so it changes the head, the tail or both.
        var patLists = new[]
        {
            L(L(S("Foo"), I(1)), I(2)),
            L(I(1), L(S("Foo"), I(2))),
            L(L(S("Foo")), L(S("Bar"), I(3))),
            L(I(1), Str("s"), I(2)),
        };
        foreach (var patsQ in patLists)
        {
            var patsM = ToMathcad(patsQ)!;
            MethodComparison.Compare($"SCons.walk_patterns({Describe(patsQ)})", "Mpl.Run.SCons",
                qWalkPats, null, [qEnv, patsQ],
                mWalkPats, null, [mEnv, patsM],
                quireShow: res => ReferenceEquals(res, patsQ) ? "reused" : Describe(res),
                originalShow: res => ReferenceEquals(res, patsM) ? "reused" : Describe(res),
                quireState: o => Describe(o), originalState: o => Describe(o));
        }

        // A null head fails in Key before anything else.
        Compare("SCons.walk_pattern null head", "Mpl.Run.SCons", qWalkPat, sNullHead, [qEnv], mWalkPat, mNullHead, [mEnv]);
        Compare("SCons.walk_declaration_lhs null head", "Mpl.Run.SCons", qWalkDecl, sNullHead, [qEnv], mWalkDecl, mNullHead, [mEnv]);

        // Upper-case heads in pattern and expression position never touch the environment, so a null one still
        // gives the ordinary result.
        Compare("SCons.walk_pattern upper, no env", "Mpl.Run.SCons", qWalkPat, patSymQ, [null], mWalkPat, patSymM, [null]);
        Compare("SCons.walk_expression upper, no env", "Mpl.Run.SCons", qWalkExpr, patSymQ, [null], mWalkExpr, patSymM, [null]);

        // Paths that need the environment: with none, both sides fail with NullReferenceException at the first
        // environment call (MplEnv.LoadMacro, via Sym.walk_declaration_as for declarations). This shows only that the
        // environment is dereferenced before any other observable effect, not which branch follows; the rest needs
        // MplEnv, LocalEnv and MplMacro.
        var lowerQ = L(S("foo", 3), I(1));
        var lowerM = ToMathcad(lowerQ)!;
        Compare("SCons.walk_declaration_lhs upper, no env", "Mpl.Run.SCons", qWalkDecl, patSymQ, [null], mWalkDecl, patSymM, [null]);
        Compare("SCons.walk_declaration_lhs lower, no env", "Mpl.Run.SCons", qWalkDecl, lowerQ, [null], mWalkDecl, lowerM, [null]);
        Compare("SCons.walk_pattern lower, no env", "Mpl.Run.SCons", qWalkPat, lowerQ, [null], mWalkPat, lowerM, [null]);
        Compare("SCons.walk_expression lower, no env", "Mpl.Run.SCons", qWalkExpr, lowerQ, [null], mWalkExpr, lowerM, [null]);
    }

    [SkippableFact]
    public void Sexpr_base_and_errors()
    {
        Runtime.Sync();

        var (qHandle, mHandle) = Method(typeof(Sexpr), "get_handle", DeclaredInstancePublic);
        var (qKey, mKey) = Method(typeof(Sexpr), "get_Key", DeclaredInstancePublic);
        var (qSym, mSym) = Method(typeof(Sexpr), "get_sym", DeclaredInstancePublic);
        var (qStr, mStr) = Method(typeof(Sexpr), "get_str", DeclaredInstancePublic);
        var (qAsSym, mAsSym) = Method(typeof(Sexpr), "get_AsSym", DeclaredInstancePublic);
        var (qAsStr, mAsStr) = Method(typeof(Sexpr), "get_AsStr", DeclaredInstancePublic);
        var (qAsKey, mAsKey) = Method(typeof(Sexpr), "get_AsKey", DeclaredInstancePublic);
        var (qQuotation, mQuotation) = Method(typeof(Sexpr), "get_quotation", DeclaredInstancePublic);
        var (qQuote, mQuote) = Method(typeof(Sexpr), "get_quote", DeclaredInstancePublic);
        var (qScons, mScons) = Method(typeof(Sexpr), "get_scons", DeclaredInstancePublic);
        var (qFormatTail, mFormatTail) = Method(typeof(Sexpr), "FormatTail", DeclaredInstancePublic);
        var (qFormatAppend, mFormatAppend) = Method(typeof(Sexpr), "FormatAppend", DeclaredInstancePublic);
        var (qWalkDecl, mWalkDecl) = Method(typeof(Sexpr), "walk_declaration_lhs", DeclaredInstancePublic, typeof(MplEnv));
        var (qWalkPat, mWalkPat) = Method(typeof(Sexpr), "walk_pattern", DeclaredInstancePublic, typeof(MplEnv));
        var (qWalkExpr, mWalkExpr) = Method(typeof(Sexpr), "walk_expression", DeclaredInstancePublic, typeof(MplEnv));
        var (qWalkWithBind, mWalkWithBind) = Method(typeof(Sexpr), "walk_with_bindings", DeclaredInstancePublic, typeof(MplEnv), typeof(SCons), typeof(Sexpr).MakeByRefType());
        var (qWalk, mWalk) = Method(typeof(Sexpr), "walk", DeclaredInstancePublic, typeof(MplEnv), typeof(MplEnv).MakeByRefType());
        var (qWalkStatic, mWalkStatic) = Method(typeof(Sexpr), "Walk", DeclaredStaticPublic, typeof(MplEnv), typeof(Sexpr));
        var (qWalkDefs, mWalkDefs) = Method(typeof(Sexpr), "walk_definitions", DeclaredStaticNonPublic, typeof(Cons), typeof(MplEnv));
        var (qWalkDef, mWalkDef) = Method(typeof(Sexpr), "walk_definition", DeclaredStaticNonPublic, typeof(SCons), typeof(MplEnv));

        var qNul = Nul.singleton;
        var mNul = ToMathcad(qNul)!;
        Compare("Sexpr.handle on Nul", "Mpl.Run.Sexpr", qHandle, qNul, [], mHandle, mNul, []);
        Compare("Sexpr.Key on Nul", "Mpl.Run.Sexpr", qKey, qNul, [], mKey, mNul, []);
        Compare("Sexpr.sym on Nul", "Mpl.Run.Sexpr", qSym, qNul, [], mSym, mNul, []);
        Compare("Sexpr.str on Nul", "Mpl.Run.Sexpr", qStr, qNul, [], mStr, mNul, []);
        Compare("Sexpr.quotation on Nul", "Mpl.Run.Sexpr", qQuotation, qNul, [], mQuotation, mNul, []);

        var qTokSym = S("foo");
        var mTokSym = ToMathcad(qTokSym)!;
        Compare("Sexpr.AsSym on Tok(Sym)", "Mpl.Run.Sexpr", qAsSym, qTokSym, [], mAsSym, mTokSym, []);
        Compare("Sexpr.AsSym on Nul throws", "Mpl.Run.Sexpr", qAsSym, qNul, [], mAsSym, mNul, []);
        Compare("Sexpr.AsKey on Tok(Sym)", "Mpl.Run.Sexpr", qAsKey, qTokSym, [], mAsKey, mTokSym, []);
        Compare("Sexpr.AsKey on Nul throws", "Mpl.Run.Sexpr", qAsKey, qNul, [], mAsKey, mNul, []);
        Compare("Sexpr.scons on Tok throws", "Mpl.Run.Sexpr", qScons, qTokSym, [], mScons, mTokSym, []);
        Compare("Sexpr.FormatTail on Tok", "Mpl.Run.Sexpr", qFormatTail, qTokSym, [], mFormatTail, mTokSym, []);
        Compare("Sexpr.FormatAppend on Tok", "Mpl.Run.Sexpr", qFormatAppend, qTokSym, [], mFormatAppend, mTokSym, []);

        var qTokStr = Str("s");
        var mTokStr = ToMathcad(qTokStr)!;
        Compare("Sexpr.AsStr on Tok(Str)", "Mpl.Run.Sexpr", qAsStr, qTokStr, [], mAsStr, mTokStr, []);
        Compare("Sexpr.AsStr on Nul throws", "Mpl.Run.Sexpr", qAsStr, qNul, [], mAsStr, mNul, []);

        var qCm = new Cm(qTokSym);
        var mCm = ToMathcad(qCm)!;
        Compare("Sexpr.quote on Cm throws", "Mpl.Run.Sexpr", qQuote, qCm, [], mQuote, mCm, []);

        var qEnv = new LocalEnv();
        var mEnv = New(M("Mpl.Run.LocalEnv"), [])!;
        Compare("Sexpr.walk_declaration_lhs on Nul", "Mpl.Run.Sexpr", qWalkDecl, qNul, [qEnv], mWalkDecl, mNul, [mEnv]);
        Compare("Sexpr.walk_pattern on Nul throws", "Mpl.Run.Sexpr", qWalkPat, qNul, [qEnv], mWalkPat, mNul, [mEnv]);
        Compare("Sexpr.walk_expression on Nul throws", "Mpl.Run.Sexpr", qWalkExpr, qNul, [qEnv], mWalkExpr, mNul, [mEnv]);

        // walk_with_bindings
        var qTokUpper = S("Foo");
        var mTokUpper = ToMathcad(qTokUpper)!;
        var pats = L(I(1), Str("s"));
        var mPats = ToMathcad(pats);
        object?[] qBindArgs1 = [qEnv, null, null], mBindArgs1 = [mEnv, null, null];
        Compare("Sexpr.walk_with_bindings null pats", "Mpl.Run.Sexpr", qWalkWithBind, qTokUpper, qBindArgs1, mWalkWithBind, mTokUpper, mBindArgs1);

        object?[] qBindArgs2 = [qEnv, pats, null], mBindArgs2 = [mEnv, mPats, null];
        Compare("Sexpr.walk_with_bindings pats", "Mpl.Run.Sexpr", qWalkWithBind, qTokUpper, qBindArgs2, mWalkWithBind, mTokUpper, mBindArgs2);

        // walk and Walk
        object?[] qWalkArgs = [qEnv, null], mWalkArgs = [mEnv, null];
        Compare("Sexpr.walk on Nul", "Mpl.Run.Sexpr", qWalk, qNul, qWalkArgs, mWalk, mNul, mWalkArgs);
        Compare("Sexpr.Walk(qEnv, Nul)", "Mpl.Run.Sexpr", qWalkStatic, null, [qEnv, qNul], mWalkStatic, null, [mEnv, mNul]);
        Compare("Sexpr.Walk(qEnv, Tok)", "Mpl.Run.Sexpr", qWalkStatic, null, [qEnv, qTokUpper], mWalkStatic, null, [mEnv, mTokUpper]);

        // WalkModule: a block that fails while walking. A successful walk needs block_keyword.NewTok (a stub).
        var (qWalkModule, mWalkModule) = Method(typeof(Sexpr), "WalkModule", DeclaredStaticPublic, typeof(MplEnv), typeof(Sexpr));
        var badBlock = L(I(1, 8), I(2));
        var mBadBlock = ToMathcad(badBlock);
        Compare("Sexpr.WalkModule non-sym declaration throws", "Mpl.Run.Sexpr", qWalkModule, null, [qEnv, badBlock], mWalkModule, null, [mEnv, mBadBlock]);
        Compare("Sexpr.WalkModule null block throws", "Mpl.Run.Sexpr", qWalkModule, null, [qEnv, null], mWalkModule, null, [mEnv, null]);

        // walk_definitions and walk_definition
        Compare("Sexpr.walk_definitions(null)", "Mpl.Run.Sexpr", qWalkDefs, null, [null, qEnv], mWalkDefs, null, [null, mEnv]);

        var declCons = new Cons(L(S("foo"), I(1)), null);
        var mDeclCons = ToMathcad(declCons);
        Compare("Sexpr.walk_definitions(decls)", "Mpl.Run.Sexpr", qWalkDefs, null, [declCons, qEnv], mWalkDefs, null, [mDeclCons, mEnv]);

        var declForm = L(S("foo"), I(1));
        var mDeclForm = ToMathcad(declForm);
        Compare("Sexpr.walk_definition sym", "Mpl.Run.Sexpr", qWalkDef, null, [declForm, qEnv], mWalkDef, null, [mDeclForm, mEnv]);

        var nonSymDecl = L(I(1), I(2));
        var mNonSymDecl = ToMathcad(nonSymDecl);
        Compare("Sexpr.walk_definition non-sym throws", "Mpl.Run.Sexpr", qWalkDef, null, [nonSymDecl, qEnv], mWalkDef, null, [mNonSymDecl, mEnv]);

        // Error helpers (12 methods)
        string[] helpers =
        [
            "BadDeclaration", "BadPattern", "BadExpression", "ExpectedAList", "SyntaxError", "BadApplication",
            "NotASymbol", "NotAString", "NotAKeyword", "NotImplemented", "NotAQuotation", "QuotationNotImplemented",
        ];

        var nonZeroHandle = I(1, 7);
        var mNonZeroHandle = ToMathcad(nonZeroHandle);
        foreach (string h in helpers)
        {
            var (qErr, mErr) = Method(typeof(Sexpr), h, DeclaredStaticPublic, typeof(Sexpr));
            Compare($"Sexpr.{h}(handle 7)", "Mpl.Run.Sexpr", qErr, null, [nonZeroHandle], mErr, null, [mNonZeroHandle]);
            Compare($"Sexpr.{h}(Nul)", "Mpl.Run.Sexpr", qErr, null, [qNul], mErr, null, [mNul]);
            Compare($"Sexpr.{h}(null) throws", "Mpl.Run.Sexpr", qErr, null, [null], mErr, null, [null]);
        }
    }
}
