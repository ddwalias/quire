using System.Collections;
using System.Reflection;
using Quire.Mpl.Run;
using Xunit;
using static Quire.Differential.Tests.MplRunSides;

namespace Quire.Differential.Tests;

/// <summary>
/// Compares Quire's <see cref="MplEnv"/>, <see cref="BindingInfo"/>, <see cref="MacroBindingInfo"/>,
/// <see cref="LocalEnv"/>, <see cref="MplMacro"/>, <see cref="MplSymbolMacro"/> and <see cref="MplQuoteMacro"/> with mplrun's on the same inputs.
/// Evidence records: docs/mirror/evidence/Mpl.Run.MplEnv.md, Mpl.Run.BindingInfo.md, Mpl.Run.LocalEnv.md,
/// Mpl.Run.MplMacro.md, Mpl.Run.MplSymbolMacro.md, Mpl.Run.MplQuoteMacro.md.
/// </summary>
public class EnvDifferentialTests
{
    private const BindingFlags DeclaredInstancePublic = BindingFlags.Public | BindingFlags.Instance | BindingFlags.DeclaredOnly;
    private const BindingFlags DeclaredStaticPublic = BindingFlags.Public | BindingFlags.Static | BindingFlags.DeclaredOnly;

    private static System.Type MType(System.Type qType) =>
        Mathcad.Type("mplrun", qType.FullName!.StartsWith("Quire.", StringComparison.Ordinal) ? qType.FullName[6..] : qType.FullName);

    private static System.Type MParamType(System.Type qParam) =>
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

    private static string EnvState(object? o)
    {
        if (o == null) return "null";
        var level = Get(o, "Level");
        var parent = Get(o, "m_parent");
        string parentDesc = parent == null ? "null" : parent.GetType().Name;
        var table = (IDictionary)Get(o, "m_table")!;
        var entries = new List<string>();
        foreach (DictionaryEntry entry in table)
            entries.Add($"{entry.Key}:{Describe(entry.Value)}");
        return $"{o.GetType().Name}(Level={level},parent={parentDesc},entries=[{string.Join(",", entries)}])";
    }

    [SkippableFact]
    public void BindingInfo_and_MacroBindingInfo_members()
    {
        Runtime.Sync();
        var (qCtor0, mCtor0) = Ctor(typeof(BindingInfo), DeclaredInstancePublic);
        var (qCtor1, mCtor1) = Ctor(typeof(BindingInfo), DeclaredInstancePublic, typeof(uint));
        var (qCtor2, mCtor2) = Ctor(typeof(BindingInfo), DeclaredInstancePublic, typeof(uint), typeof(int));
        var (qMacro, mMacro) = Method(typeof(BindingInfo), "get_macro", DeclaredInstancePublic);

        Func<object?, string> show = o => Describe(o);
        Func<object, string> state = o => Describe(o);

        MethodComparison.Compare("BindingInfo()", "Mpl.Run.BindingInfo",
            qCtor0, null, [], mCtor0, null, [], show, show);

        foreach (uint h in new[] { 0u, 1u, 42u, uint.MaxValue })
        {
            MethodComparison.Compare($"BindingInfo({h})", "Mpl.Run.BindingInfo",
                qCtor1, null, [h], mCtor1, null, [h], show, show);
        }

        foreach (var (h, z) in new[] { (0u, 0), (10u, 1), (100u, -1), (uint.MaxValue, int.MaxValue) })
        {
            MethodComparison.Compare($"BindingInfo({h}, {z})", "Mpl.Run.BindingInfo",
                qCtor2, null, [h, z], mCtor2, null, [h, z], show, show);
        }

        var qB0 = new BindingInfo();
        var mB0 = New(M("Mpl.Run.BindingInfo"), [])!;
        MethodComparison.Compare("BindingInfo.macro default", "Mpl.Run.BindingInfo",
            qMacro, qB0, [], mMacro, mB0, [], show, show, state, state);

        var qB1 = new BindingInfo(42u, 5);
        var mB1 = New(M("Mpl.Run.BindingInfo"), ["UInt32", "Int32"], 42u, 5)!;
        MethodComparison.Compare("BindingInfo(42, 5).macro", "Mpl.Run.BindingInfo",
            qMacro, qB1, [], mMacro, mB1, [], show, show, state, state);

        // MacroBindingInfo
        var (qMacroCtor, mMacroCtor) = Ctor(typeof(MacroBindingInfo), DeclaredInstancePublic, typeof(uint), typeof(MplMacro));
        var (qMacroProp, mMacroProp) = Method(typeof(MacroBindingInfo), "get_macro", DeclaredInstancePublic);

        var qSym = new Sym("x", null);
        var mSym = ToMathcad(qSym)!;
        var qRep = int_constant.NewTok(10, 0u);
        var mRep = ToMathcad(qRep)!;
        var qMacroObj = new MplSymbolMacro(qSym, qRep);
        var mMacroObj = New(M("Mpl.Run.MplSymbolMacro"), ["Sym", "Sexpr"], mSym, mRep)!;

        MethodComparison.Compare("MacroBindingInfo.ctor", "Mpl.Run.BindingInfo",
            qMacroCtor, null, [77u, qMacroObj], mMacroCtor, null, [77u, mMacroObj], show, show);

        var qMb = new MacroBindingInfo(77u, qMacroObj);
        var mMb = New(M("Mpl.Run.MacroBindingInfo"), ["UInt32", "MplMacro"], 77u, mMacroObj)!;
        MethodComparison.Compare("MacroBindingInfo.macro", "Mpl.Run.BindingInfo",
            qMacroProp, qMb, [], mMacroProp, mMb, [], show, show, state, state);
    }

    [SkippableFact]
    public void MplSymbolMacro_members()
    {
        Runtime.Sync();
        var (qCtor, mCtor) = Ctor(typeof(MplSymbolMacro), DeclaredInstancePublic, typeof(Sym), typeof(Sexpr));
        var (qMake, mMake) = Method(typeof(MplSymbolMacro), "make", DeclaredStaticPublic, typeof(Atom), typeof(int));
        var (qExpandRef, mExpandRef) = Method(typeof(MplSymbolMacro), "ExpandRef", DeclaredInstancePublic, typeof(MplEnv), typeof(Tok));
        var (qExpandCall, mExpandCall) = Method(typeof(MplSymbolMacro), "ExpandCall", DeclaredInstancePublic, typeof(MplEnv), typeof(Sexpr));

        Func<object?, string> show = o => Describe(o);
        Func<object, string> state = o => Describe(o);

        var qSymX = new Sym("x", null);
        var mSymX = ToMathcad(qSymX)!;
        var qRep1 = int_constant.NewTok(42, 0u);
        var mRep1 = ToMathcad(qRep1)!;

        MethodComparison.Compare("MplSymbolMacro.ctor", "Mpl.Run.MplSymbolMacro",
            qCtor, null, [qSymX, qRep1], mCtor, null, [mSymX, mRep1], show, show);

        var qMacroX = new MplSymbolMacro(qSymX, qRep1);
        var mMacroX = New(M("Mpl.Run.MplSymbolMacro"), ["Sym", "Sexpr"], mSymX, mRep1)!;

        var qTokX = new Tok(qSymX, 5u);
        var mTokX = ToMathcad(qTokX)!;
        MethodComparison.Compare("MplSymbolMacro.ExpandRef", "Mpl.Run.MplSymbolMacro",
            qExpandRef, qMacroX, [null, qTokX], mExpandRef, mMacroX, [null, mTokX], show, show, state, state);

        var qCall1 = SCons.list2(qTokX, int_constant.NewTok(1, 0u));
        var mCall1 = ToMathcad(qCall1)!;
        MethodComparison.Compare("MplSymbolMacro.ExpandCall list2", "Mpl.Run.MplSymbolMacro",
            qExpandCall, qMacroX, [null, qCall1], mExpandCall, mMacroX, [null, mCall1], show, show, state, state);

        var qCallTailNull = new SCons(qTokX, null);
        var mCallTailNull = New(M("Mpl.Run.SCons"), ["Sexpr", "Sexpr"], mTokX, null)!;
        MethodComparison.Compare("MplSymbolMacro.ExpandCall tail null", "Mpl.Run.MplSymbolMacro",
            qExpandCall, qMacroX, [null, qCallTailNull], mExpandCall, mMacroX, [null, mCallTailNull], show, show, state, state);

        // ExpandCall non-SCons throws NullReferenceException on both
        MethodComparison.Compare("MplSymbolMacro.ExpandCall non-SCons", "Mpl.Run.MplSymbolMacro",
            qExpandCall, qMacroX, [null, qRep1], mExpandCall, mMacroX, [null, mRep1], show, show, state, state);

        // ExpandCall null form throws NullReferenceException on both
        MethodComparison.Compare("MplSymbolMacro.ExpandCall null", "Mpl.Run.MplSymbolMacro",
            qExpandCall, qMacroX, [null, null], mExpandCall, mMacroX, [null, null], show, show, state, state);

        // make
        foreach (int n in new[] { 0, 1, -10, 42 })
        {
            MethodComparison.Compare($"MplSymbolMacro.make(sym, {n})", "Mpl.Run.MplSymbolMacro",
                qMake, null, [qSymX, n], mMake, null, [mSymX, n], show, show);
        }

        var qNonSym = new int_constant(99);
        var mNonSym = ToMathcad(qNonSym)!;
        MethodComparison.Compare("MplSymbolMacro.make(non-sym, 5)", "Mpl.Run.MplSymbolMacro",
            qMake, null, [qNonSym, 5], mMake, null, [mNonSym, 5], show, show);
    }

    [SkippableFact]
    public void MplMacro_members()
    {
        Runtime.Sync();
        var (qNotAtTop, mNotAtTop) = Method(typeof(MplMacro), "NotAtTopLevel", DeclaredStaticPublic, typeof(Sym), typeof(uint));
        var (qCanNotExpand, mCanNotExpand) = Method(typeof(MplMacro), "CanNotExpand", DeclaredStaticPublic, typeof(uint));
        var (qIllegalCall, mIllegalCall) = Method(typeof(MplMacro), "IllegalMacroCall", DeclaredInstancePublic, typeof(Sexpr));
        var (qIllegalRef, mIllegalRef) = Method(typeof(MplMacro), "IllegalMacroRef", DeclaredInstancePublic, typeof(Tok));

        Func<object?, string> show = o => Describe(o);
        Func<object, string> state = o => Describe(o);

        var qSym = new Sym("m", null);
        var mSym = ToMathcad(qSym)!;

        foreach (uint h in new[] { 0u, 15u, uint.MaxValue })
        {
            MethodComparison.Compare($"MplMacro.NotAtTopLevel({h})", "Mpl.Run.MplMacro",
                qNotAtTop, null, [qSym, h], mNotAtTop, null, [mSym, h], show, show);
            MethodComparison.Compare($"MplMacro.CanNotExpand({h})", "Mpl.Run.MplMacro",
                qCanNotExpand, null, [h], mCanNotExpand, null, [h], show, show);
        }

        // IllegalMacroCall and IllegalMacroRef are non-virtual on MplMacro; target can be MplSymbolMacro
        var qRep = int_constant.NewTok(1, 0u);
        var mRep = ToMathcad(qRep)!;
        var qMacroObj = new MplSymbolMacro(qSym, qRep);
        var mMacroObj = New(M("Mpl.Run.MplSymbolMacro"), ["Sym", "Sexpr"], mSym, mRep)!;

        var qForm = new SCons(new Tok(qSym, 88u), null);
        var mForm = New(M("Mpl.Run.SCons"), ["Sexpr", "Sexpr"], ToMathcad(new Tok(qSym, 88u))!, null)!;
        MethodComparison.Compare("MplMacro.IllegalMacroCall", "Mpl.Run.MplMacro",
            qIllegalCall, qMacroObj, [qForm], mIllegalCall, mMacroObj, [mForm], show, show, state, state);

        var qTok = new Tok(qSym, 99u);
        var mTok = ToMathcad(qTok)!;
        MethodComparison.Compare("MplMacro.IllegalMacroRef", "Mpl.Run.MplMacro",
            qIllegalRef, qMacroObj, [qTok], mIllegalRef, mMacroObj, [mTok], show, show, state, state);
    }

    [SkippableFact]
    public void MplEnv_members()
    {
        Runtime.Sync();
        var (qCanNotResolve, mCanNotResolve) = Method(typeof(MplEnv), "CanNotResolve", DeclaredStaticPublic, typeof(string));
        var (qMayNotModify, mMayNotModify) = Method(typeof(MplEnv), "MayNotModify", DeclaredStaticPublic);
        var (qLoadMacro, mLoadMacro) = Method(typeof(MplEnv), "LoadMacro", DeclaredInstancePublic, typeof(Sym));

        Func<object?, string> show = o => Describe(o);
        Func<object, string> envState = o => EnvState(o);

        foreach (string? sig in new[] { "foo(int)", "", null })
        {
            MethodComparison.Compare($"MplEnv.CanNotResolve({sig})", "Mpl.Run.MplEnv",
                qCanNotResolve, null, [sig], mCanNotResolve, null, [sig], show, show);
        }

        MethodComparison.Compare("MplEnv.MayNotModify", "Mpl.Run.MplEnv",
            qMayNotModify, null, [], mMayNotModify, null, [], show, show);

        // LoadMacro on LocalEnv target
        var qEnv = new LocalEnv();
        var mEnv = New(M("Mpl.Run.LocalEnv"), [])!;

        // 1. Keyword returns null
        var qKey = new define_keyword { Field1 = "define", Field2 = "Keyword", m_tag = 5 };
        Set(qKey, "m_keynum", 1);
        var mKey = New(M("Mpl.Run.define_keyword"), [])!;
        MethodComparison.Compare("MplEnv.LoadMacro(Keyword)", "Mpl.Run.MplEnv",
            qLoadMacro, qEnv, [qKey], mLoadMacro, mEnv, [mKey], show, show, envState, envState);

        // 2. Unbound symbol with null parent throws NullReferenceException
        var qMissing = new Sym("unbound", null);
        var mMissing = ToMathcad(qMissing)!;
        MethodComparison.Compare("MplEnv.LoadMacro(unbound)", "Mpl.Run.MplEnv",
            qLoadMacro, qEnv, [qMissing], mLoadMacro, mEnv, [mMissing], show, show, envState, envState);

        // 3. Bound local value has macro == null
        var qSymA = new Sym("a", null);
        var mSymA = ToMathcad(qSymA)!;
        qEnv.AddLocal(qSymA, 10u);
        Call(mEnv, "AddLocal", ["Sym", "UInt32"], mSymA, 10u);
        MethodComparison.Compare("MplEnv.LoadMacro(local value)", "Mpl.Run.MplEnv",
            qLoadMacro, qEnv, [qSymA], mLoadMacro, mEnv, [mSymA], show, show, envState, envState);

        // 4. Bound symbol macro returns macro
        var qSymM = new Sym("m", null);
        var mSymM = ToMathcad(qSymM)!;
        var qRep = int_constant.NewTok(123, 0u);
        var mRep = ToMathcad(qRep)!;
        qEnv.AddSymbolMacro(qSymM, 20u, qRep);
        Call(mEnv, "AddSymbolMacro", ["Sym", "UInt32", "Sexpr"], mSymM, 20u, mRep);
        MethodComparison.Compare("MplEnv.LoadMacro(symbol macro)", "Mpl.Run.MplEnv",
            qLoadMacro, qEnv, [qSymM], mLoadMacro, mEnv, [mSymM], show, show, envState, envState);
    }

    [SkippableFact]
    public void LocalEnv_members()
    {
        Runtime.Sync();
        var (qCtor0, mCtor0) = Ctor(typeof(LocalEnv), DeclaredInstancePublic);
        var (qCtor1, mCtor1) = Ctor(typeof(LocalEnv), DeclaredInstancePublic, typeof(MplEnv));
        var (qLevel, mLevel) = Method(typeof(LocalEnv), "get_Level", DeclaredInstancePublic);
        var (qLookupLocal, mLookupLocal) = Method(typeof(LocalEnv), "lookup_local", DeclaredInstancePublic, typeof(Sym));
        var (qLookup, mLookup) = Method(typeof(LocalEnv), "lookup", DeclaredInstancePublic, typeof(Sym));
        var (qAddLocal, mAddLocal) = Method(typeof(LocalEnv), "AddLocal", DeclaredInstancePublic, typeof(Sym), typeof(uint));
        var (qAddLocalEq, mAddLocalEq) = Method(typeof(LocalEnv), "AddLocalEquation", DeclaredInstancePublic, typeof(Sym), typeof(uint));
        var (qAddSymbolMacro, mAddSymbolMacro) = Method(typeof(LocalEnv), "AddSymbolMacro", DeclaredInstancePublic, typeof(Sym), typeof(uint), typeof(Sexpr));
        var (qLoadForeign, mLoadForeign) = Method(typeof(LocalEnv), "LoadForeignMacro", DeclaredInstancePublic, typeof(string));

        Func<object?, string> show = o => Describe(o);
        Func<object?, string> envState = o => EnvState(o);

        MethodComparison.Compare("LocalEnv()", "Mpl.Run.LocalEnv",
            qCtor0, null, [], mCtor0, null, [], envState, envState);

        var qRoot = new LocalEnv();
        var mRoot = New(M("Mpl.Run.LocalEnv"), [])!;
        MethodComparison.Compare("LocalEnv(parent root)", "Mpl.Run.LocalEnv",
            qCtor1, null, [qRoot], mCtor1, null, [mRoot], envState, envState);

        var qChild = new LocalEnv(qRoot);
        var mChild = New(M("Mpl.Run.LocalEnv"), ["MplEnv"], mRoot)!;
        MethodComparison.Compare("LocalEnv(parent child)", "Mpl.Run.LocalEnv",
            qCtor1, null, [qChild], mCtor1, null, [mChild], envState, envState);

        var qGrandchild = new LocalEnv(qChild);
        var mGrandchild = New(M("Mpl.Run.LocalEnv"), ["MplEnv"], mChild)!;

        MethodComparison.Compare("LocalEnv.Level root", "Mpl.Run.LocalEnv",
            qLevel, qRoot, [], mLevel, mRoot, [], show, show, envState, envState);
        MethodComparison.Compare("LocalEnv.Level child", "Mpl.Run.LocalEnv",
            qLevel, qChild, [], mLevel, mChild, [], show, show, envState, envState);
        MethodComparison.Compare("LocalEnv.Level grandchild", "Mpl.Run.LocalEnv",
            qLevel, qGrandchild, [], mLevel, mGrandchild, [], show, show, envState, envState);

        // AddLocal
        var qSymX = new Sym("x", null);
        var mSymX = ToMathcad(qSymX)!;
        MethodComparison.Compare("LocalEnv.AddLocal(x, 10)", "Mpl.Run.LocalEnv",
            qAddLocal, qRoot, [qSymX, 10u], mAddLocal, mRoot, [mSymX, 10u], show, show, envState, envState);

        // AddLocal duplicate throws ArgumentException
        MethodComparison.Compare("LocalEnv.AddLocal duplicate", "Mpl.Run.LocalEnv",
            qAddLocal, qRoot, [qSymX, 99u], mAddLocal, mRoot, [mSymX, 99u], show, show, envState, envState);

        // AddLocalEquation adds and overwrites
        var qSymY = new Sym("y", null);
        var mSymY = ToMathcad(qSymY)!;
        MethodComparison.Compare("LocalEnv.AddLocalEquation(y, 20)", "Mpl.Run.LocalEnv",
            qAddLocalEq, qRoot, [qSymY, 20u], mAddLocalEq, mRoot, [mSymY, 20u], show, show, envState, envState);
        MethodComparison.Compare("LocalEnv.AddLocalEquation overwrite y", "Mpl.Run.LocalEnv",
            qAddLocalEq, qRoot, [qSymY, 30u], mAddLocalEq, mRoot, [mSymY, 30u], show, show, envState, envState);

        // AddSymbolMacro
        var qSymZ = new Sym("z", null);
        var mSymZ = ToMathcad(qSymZ)!;
        var qRep = int_constant.NewTok(555, 0u);
        var mRep = ToMathcad(qRep)!;
        MethodComparison.Compare("LocalEnv.AddSymbolMacro(z, 555)", "Mpl.Run.LocalEnv",
            qAddSymbolMacro, qRoot, [qSymZ, 40u, qRep], mAddSymbolMacro, mRoot, [mSymZ, 40u, mRep], show, show, envState, envState);

        // AddSymbolMacro duplicate throws ArgumentException
        MethodComparison.Compare("LocalEnv.AddSymbolMacro duplicate", "Mpl.Run.LocalEnv",
            qAddSymbolMacro, qRoot, [qSymZ, 50u, qRep], mAddSymbolMacro, mRoot, [mSymZ, 50u, mRep], show, show, envState, envState);

        // lookup_local
        MethodComparison.Compare("LocalEnv.lookup_local(x)", "Mpl.Run.LocalEnv",
            qLookupLocal, qRoot, [qSymX], mLookupLocal, mRoot, [mSymX], show, show, envState, envState);
        MethodComparison.Compare("LocalEnv.lookup_local(y)", "Mpl.Run.LocalEnv",
            qLookupLocal, qRoot, [qSymY], mLookupLocal, mRoot, [mSymY], show, show, envState, envState);
        MethodComparison.Compare("LocalEnv.lookup_local(z)", "Mpl.Run.LocalEnv",
            qLookupLocal, qRoot, [qSymZ], mLookupLocal, mRoot, [mSymZ], show, show, envState, envState);

        var qUnbound = new Sym("unbound", null);
        var mUnbound = ToMathcad(qUnbound)!;
        MethodComparison.Compare("LocalEnv.lookup_local(unbound)", "Mpl.Run.LocalEnv",
            qLookupLocal, qRoot, [qUnbound], mLookupLocal, mRoot, [mUnbound], show, show, envState, envState);

        // lookup: on local env finds local binding
        MethodComparison.Compare("LocalEnv.lookup(x) on root", "Mpl.Run.LocalEnv",
            qLookup, qRoot, [qSymX], mLookup, mRoot, [mSymX], show, show, envState, envState);

        // lookup: on child env finds parent binding
        MethodComparison.Compare("LocalEnv.lookup(x) on child via parent", "Mpl.Run.LocalEnv",
            qLookup, qChild, [qSymX], mLookup, mChild, [mSymX], show, show, envState, envState);

        // lookup: unbound with null parent throws NullReferenceException
        MethodComparison.Compare("LocalEnv.lookup(unbound) root throws NullRef", "Mpl.Run.LocalEnv",
            qLookup, qRoot, [qUnbound], mLookup, mRoot, [mUnbound], show, show, envState, envState);

        // LoadForeignMacro: on child env delegates to parent, eventually throwing AppException("Can not resolve signature")
        MethodComparison.Compare("LocalEnv.LoadForeignMacro on child", "Mpl.Run.LocalEnv",
            qLoadForeign, qChild, ["sig_foreign"], mLoadForeign, mChild, ["sig_foreign"], show, show, envState, envState);

        // LoadForeignMacro: on root env (null parent) throws NullReferenceException
        MethodComparison.Compare("LocalEnv.LoadForeignMacro on root throws NullRef", "Mpl.Run.LocalEnv",
            qLoadForeign, qRoot, ["sig_foreign"], mLoadForeign, mRoot, ["sig_foreign"], show, show, envState, envState);
    }

    [SkippableFact]
    public void MplQuoteMacro_members()
    {
        Runtime.Sync();
        var qSingleton = MplQuoteMacro.singleton;
        var mSingleton = Get(M("Mpl.Run.MplQuoteMacro"), "singleton")!;

        Assert.Equal(Describe(mSingleton), Describe(qSingleton));

        var (qExpandCall, mExpandCall) = Method(typeof(MplQuoteMacro), "ExpandCall", DeclaredInstancePublic, typeof(MplEnv), typeof(Sexpr));

        Func<object?, string> show = o => Describe(o);
        Func<object, string> state = o => Describe(o);

        // 1. Valid length-2 list: (quote x) -> Qt(x)
        var qSymQuote = new Sym("quote", null);
        var qTokQuote = new Tok(qSymQuote, 10u);
        var qArgX = int_constant.NewTok(42, 20u);
        var qCallValid = SCons.list2(qTokQuote, qArgX);
        var mCallValid = ToMathcad(qCallValid)!;

        MethodComparison.Compare("MplQuoteMacro.ExpandCall (quote x)", "Mpl.Run.MplQuoteMacro",
            qExpandCall, qSingleton, [null, qCallValid], mExpandCall, mSingleton, [null, mCallValid], show, show, state, state);

        var qArgSym = new Tok(new Sym("y", null), 30u);
        var qCallSym = SCons.list2(qTokQuote, qArgSym);
        var mCallSym = ToMathcad(qCallSym)!;
        MethodComparison.Compare("MplQuoteMacro.ExpandCall (quote y)", "Mpl.Run.MplQuoteMacro",
            qExpandCall, qSingleton, [null, qCallSym], mExpandCall, mSingleton, [null, mCallSym], show, show, state, state);

        var qArgList = SCons.list2(int_constant.NewTok(1, 0u), int_constant.NewTok(2, 0u));
        var qCallList = SCons.list2(qTokQuote, qArgList);
        var mCallList = ToMathcad(qCallList)!;
        MethodComparison.Compare("MplQuoteMacro.ExpandCall (quote (1 2))", "Mpl.Run.MplQuoteMacro",
            qExpandCall, qSingleton, [null, qCallList], mExpandCall, mSingleton, [null, mCallList], show, show, state, state);

        // 2. Invalid length list: (quote x y) (length 3) throws AppException("illegal macro call")
        var qCallLen3 = SCons.list3(qTokQuote, qArgX, qArgSym);
        var mCallLen3 = ToMathcad(qCallLen3)!;
        MethodComparison.Compare("MplQuoteMacro.ExpandCall length 3 throws", "Mpl.Run.MplQuoteMacro",
            qExpandCall, qSingleton, [null, qCallLen3], mExpandCall, mSingleton, [null, mCallLen3], show, show, state, state);

        // 3. Length-1 list: (quote) throws AppException("illegal macro call")
        var qCallLen1 = SCons.list1(qTokQuote);
        var mCallLen1 = ToMathcad(qCallLen1)!;
        MethodComparison.Compare("MplQuoteMacro.ExpandCall length 1 throws", "Mpl.Run.MplQuoteMacro",
            qExpandCall, qSingleton, [null, qCallLen1], mExpandCall, mSingleton, [null, mCallLen1], show, show, state, state);

        // 4. Non-SCons: Tok throws NullReferenceException
        var qTokNonSCons = qTokQuote;
        var mTokNonSCons = ToMathcad(qTokNonSCons)!;
        MethodComparison.Compare("MplQuoteMacro.ExpandCall non-SCons throws NullRef", "Mpl.Run.MplQuoteMacro",
            qExpandCall, qSingleton, [null, qTokNonSCons], mExpandCall, mSingleton, [null, mTokNonSCons], show, show, state, state);

        // 5. Null form throws NullReferenceException
        MethodComparison.Compare("MplQuoteMacro.ExpandCall null throws NullRef", "Mpl.Run.MplQuoteMacro",
            qExpandCall, qSingleton, [null, null], mExpandCall, mSingleton, [null, null], show, show, state, state);
    }
}
