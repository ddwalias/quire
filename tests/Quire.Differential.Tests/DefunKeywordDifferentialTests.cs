using System.Reflection;
using Quire.Mpl.Run;
using static Quire.Differential.Tests.MplRunSides;
using static Quire.Differential.Tests.Reflect;

namespace Quire.Differential.Tests;

public class DefunKeywordDifferentialTests
{
    private const BindingFlags InstancePublic = BindingFlags.Public | BindingFlags.Instance | BindingFlags.DeclaredOnly;
    private const BindingFlags StaticPublic = BindingFlags.Public | BindingFlags.Static | BindingFlags.DeclaredOnly;

    private static object MathcadList(params object?[] items)
    {
        var mSCons = Mathcad.Type("mplrun", "Mpl.Run.SCons");
        var mSexpr = Mathcad.Type("mplrun", "Mpl.Run.Sexpr");
        var ctor = mSCons.GetConstructor(BindingFlags.Public | BindingFlags.Instance, null, [mSexpr, mSexpr], null)!;
        object tail = Reflect.Get(Mathcad.Type("mplrun", "Mpl.Run.Nul"), "singleton")!;
        for (int i = items.Length - 1; i >= 0; i--)
        {
            tail = ctor.Invoke([items[i], tail]);
        }
        return tail;
    }

    private static object MathcadKeywordToken(object keyword, uint handle)
    {
        var mTok = Mathcad.Type("mplrun", "Mpl.Run.Tok");
        var mAtom = Mathcad.Type("mplrun", "Mpl.Run.Atom");
        return mTok.GetConstructor(BindingFlags.Public | BindingFlags.Instance, null, [mAtom, typeof(uint)], null)!
            .Invoke([keyword, handle]);
    }

    private static string EnvState(object env, params object[] symbols)
    {
        var state = new List<string>();
        foreach (object symbol in symbols)
        {
            var binding = MplRunSides.Call(env, "lookup_local", ["Sym"], symbol);
            state.Add(Describe(binding));
        }
        return string.Join(";", state);
    }

    private static string TokenState(object? value, object singleton)
    {
        if (value?.GetType().Name != "Tok") return Describe(value);
        return "singleton=" + ReferenceEquals(Reflect.Get(value, "atom"), singleton) + ";" + Describe(value);
    }

    private static string DefunState(object? value, object singleton, object? expectedTail, object?[]? expectedElements = null)
    {
        if (value?.GetType().Name != "SCons") return Describe(value);
        object token = Reflect.Get(value, "head")!;
        if (token.GetType().Name != "Tok" || !ReferenceEquals(Reflect.Get(token, "atom"), singleton)) return Describe(value);
        object tail = Reflect.Get(value, "tail")!;
        var state = "singleton=True";
        if (expectedTail != null) state += ";tail=" + ReferenceEquals(tail, expectedTail);
        if (expectedElements != null)
        {
            object? cell = tail;
            foreach (object? element in expectedElements)
            {
                state += ";element=" + ReferenceEquals(Reflect.Get(cell!, "head"), element);
                cell = Reflect.Get(cell!, "tail");
            }
        }
        return state + ";" + Describe(value);
    }

    private static string DeclarationState(object? value, object form)
    {
        if (value?.GetType().Name != "Cons") return Describe(value);
        return "form=" + ReferenceEquals(Reflect.Get(value, "Field1"), form) + ";" + Describe(value);
    }

    private static string KeywordEnvState(object target, object env, params object[] symbols)
    {
        return Describe(target) + ";env=" + EnvState(env, symbols);
    }

    [SkippableFact]
    public void Singleton_factories_and_walks()
    {
        Runtime.Sync();
        var qType = typeof(defun_keyword);
        var mType = Mathcad.Type("mplrun", "Mpl.Run.defun_keyword");

        MethodComparison.Compare("defun_keyword.cctor", "Mpl.Run.defun_keyword",
            qType.TypeInitializer!, null, [], mType.TypeInitializer!, null, [],
            _ => Describe(Reflect.Get(qType, "singleton")), _ => Describe(Reflect.Get(mType, "singleton")));

        var qCtor = qType.GetConstructor(Type.EmptyTypes)!;
        var mCtor = mType.GetConstructor(Type.EmptyTypes)!;
        MethodComparison.Compare("defun_keyword.ctor", "Mpl.Run.defun_keyword",
            qCtor, null, [], mCtor, null, [], o => Describe(o), o => Describe(o));

        var qSingleton = Reflect.Get(qType, "singleton")!;
        var mSingleton = Reflect.Get(mType, "singleton")!;
        var qNewTok = qType.GetMethod("NewTok", StaticPublic, [typeof(uint)])!;
        var mNewTok = mType.GetMethod("NewTok", StaticPublic, [typeof(uint)])!;
        foreach (uint handle in new[] { 0u, 13u, uint.MaxValue })
        {
            MethodComparison.Compare($"defun_keyword.NewTok({handle})", "Mpl.Run.defun_keyword",
                qNewTok, null, [handle], mNewTok, null, [handle],
                o => TokenState(o, qSingleton), o => TokenState(o, mSingleton));
        }

        var qMakeForms = qType.GetMethod("MakeDefun", StaticPublic, [typeof(uint), typeof(Sexpr)])!;
        var mSexpr = Mathcad.Type("mplrun", "Mpl.Run.Sexpr");
        var mMakeForms = mType.GetMethod("MakeDefun", StaticPublic, [typeof(uint), mSexpr])!;
        var qMakeParts = qType.GetMethod("MakeDefun", StaticPublic,
            [typeof(uint), typeof(Sexpr), typeof(Sexpr), typeof(Sexpr)])!;
        var mMakeParts = mType.GetMethod("MakeDefun", StaticPublic, [typeof(uint), mSexpr, mSexpr, mSexpr])!;

        var qNameSym = new Sym("f", "f", 1);
        var qName = new Tok(qNameSym, 21u);
        var qPatterns = SCons.list1(new Tok(new Sym("x", "x", 2), 22u));
        var qExpression = new Tok(new Sym("x", "x", 2), 23u);
        var mName = ToMathcad(qName)!;
        var mPatterns = ToMathcad(qPatterns)!;
        var mExpression = ToMathcad(qExpression)!;
        var qForms = SCons.list3(qName, qPatterns, qExpression);
        var mForms = MathcadList(mName, mPatterns, mExpression);
        foreach (uint handle in new[] { 0u, 19u, uint.MaxValue })
        {
            MethodComparison.Compare($"defun_keyword.MakeDefun(forms,{handle})", "Mpl.Run.defun_keyword",
                qMakeForms, null, [handle, qForms], mMakeForms, null, [handle, mForms],
                o => DefunState(o, qSingleton, qForms), o => DefunState(o, mSingleton, mForms));
            MethodComparison.Compare($"defun_keyword.MakeDefun(parts,{handle})", "Mpl.Run.defun_keyword",
                qMakeParts, null, [handle, qName, qPatterns, qExpression], mMakeParts, null, [handle, mName, mPatterns, mExpression],
                o => DefunState(o, qSingleton, null, [qName, qPatterns, qExpression]),
                o => DefunState(o, mSingleton, null, [mName, mPatterns, mExpression]));
        }

        var qWalkDeclaration = qType.GetMethod("walk_declaration_as", InstancePublic, [typeof(MplEnv), typeof(SCons)])!;
        var mWalkDeclaration = mType.GetMethod("walk_declaration_as", InstancePublic,
            [Mathcad.Type("mplrun", "Mpl.Run.MplEnv"), Mathcad.Type("mplrun", "Mpl.Run.SCons")])!;
        var qEnv = new LocalEnv();
        var mEnv = New(Mathcad.Type("mplrun", "Mpl.Run.LocalEnv"), [])!;
        var qDeclName = new Sym("declared", "declared", 3);
        var qDeclTok = new Tok(qDeclName, 43u);
        var qDeclaration = new SCons(new Tok((Atom)qSingleton, 41u), SCons.list1(qDeclTok));
        var mDeclName = ToMathcad(qDeclName)!;
        var mDeclaration = MathcadList(MathcadKeywordToken(mSingleton, 41u), ToMathcad(qDeclTok)!);
        Assert.Equal(Describe(mDeclaration), Describe(qDeclaration));
        MethodComparison.Compare("defun_keyword.walk_declaration_as registers name", "Mpl.Run.defun_keyword",
            qWalkDeclaration, qSingleton, [qEnv, qDeclaration], mWalkDeclaration, mSingleton, [mEnv, mDeclaration],
            o => DeclarationState(o, qDeclaration), o => DeclarationState(o, mDeclaration),
            target => KeywordEnvState(target, qEnv, qDeclName),
            target => KeywordEnvState(target, mEnv, mDeclName));

        var qWalkDefinition = qType.GetMethod("walk_definition_as", InstancePublic, [typeof(MplEnv), typeof(SCons)])!;
        var mWalkDefinition = mType.GetMethod("walk_definition_as", InstancePublic,
            [Mathcad.Type("mplrun", "Mpl.Run.MplEnv"), Mathcad.Type("mplrun", "Mpl.Run.SCons")])!;
        var qPatternSym = new Sym("x", "x", 2);
        var qPattern = new Tok(qPatternSym, 22u);
        var qDefinition = new SCons(new Tok((Atom)qSingleton, 42u), SCons.list3(qName, SCons.list1(qPattern), qExpression));
        var mDefinition = MathcadList(MathcadKeywordToken(mSingleton, 42u), mName,
            MathcadList(ToMathcad(qPattern)!), mExpression);
        qEnv = new LocalEnv();
        mEnv = New(Mathcad.Type("mplrun", "Mpl.Run.LocalEnv"), [])!;
        MethodComparison.Compare("defun_keyword.walk_definition_as walks bindings and body", "Mpl.Run.defun_keyword",
            qWalkDefinition, qSingleton, [qEnv, qDefinition], mWalkDefinition, mSingleton, [mEnv, mDefinition],
            o => DefunState(o, qSingleton, null), o => DefunState(o, mSingleton, null),
            target => KeywordEnvState(target, qEnv, qNameSym, qPatternSym),
            target => KeywordEnvState(target, mEnv, ToMathcad(qNameSym)!, ToMathcad(qPatternSym)!));

        var qBadDefinition = new SCons(new Tok((Atom)qSingleton, 44u), SCons.list2(qName, qPatterns));
        var mBadDefinition = MathcadList(MathcadKeywordToken(mSingleton, 44u), mName, mPatterns);
        qEnv = new LocalEnv();
        mEnv = New(Mathcad.Type("mplrun", "Mpl.Run.LocalEnv"), [])!;
        MethodComparison.Compare("defun_keyword.walk_definition_as rejects malformed form", "Mpl.Run.defun_keyword",
            qWalkDefinition, qSingleton, [qEnv, qBadDefinition], mWalkDefinition, mSingleton, [mEnv, mBadDefinition],
            o => Describe(o), o => Describe(o),
            target => KeywordEnvState(target, qEnv, qNameSym, qPatternSym),
            target => KeywordEnvState(target, mEnv, ToMathcad(qNameSym)!, ToMathcad(qPatternSym)!));
    }
}
