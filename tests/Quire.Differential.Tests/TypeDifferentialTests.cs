using System.Reflection;
using System.Text;
using static Quire.Differential.Tests.Reflect;
using QType = Quire.Mcd.Type;

namespace Quire.Differential.Tests;

/// <summary>
/// Parses the same signatures with mplrun's <c>Mcd.Type</c> and Quire's, and compares the terms' structure, printed
/// and formatted text, and every public and private member; the cases are listed in docs/mirror/evidence/Mcd.Type.md.
/// </summary>
public class TypeDifferentialTests
{
    private const string Contract = "Mcd.Type";

    public static readonly string[] Signatures =
    [
        // dimension products and value wrappers
        "V{Length 60000}", "V{Mass         60000}", "V{Current-60000 Length 120000 Mass 60000 Time-180000}", "V{}", "{}",
        "{Length 60000 Length -60000}", "{a Length}", "{a 2 b-3 Mass}", "{a}", "{Length}", "{Length -}", "{Length - 5}",
        "{Length 99999999999}", "{Length 007}", "{a Length b Time}", "{Foo 60000}", "{Money 60000 Time 60000}",
        "V a", "V Length", "F{Time 60000}", "R{Length 120000}", "F a", "R Mass", "M(V{Length 60000})", "M(M(V{Time 60000}))",
        "M V{Length 60000}", "M a", "D a", "N{Length 60000}", "V", "M", "{",
        // applications, functions, tuples, lists
        "F a b c", "a->b", "a->b->c", "(a->b)->c", "(a, b)", "()", "(a)", "(a, b, c)", "[a]", "[V{Mass 60000}]", "[[a]]",
        "a -> b", "(I a)->b", "I a->b", "(C a)->b", "C->b", "U->V{Length 60000}", "(a, b)->c", "a->(b, c)->d",
        "Foo (a, b) [c] {d}", "a b", "a-b", "a - > b", "->", "-", "-->a",
        // names and lexing
        "C", "O", "P", "T", "U", "W", "X", "I", "c", "CO", "ab1 cd2", "t1", "t12", "t1x", "tx", "Θ", "θ", "Ünïcode",
        "ΘTemp{x}", "x$y", "$", "", " ", "  V{Length 60000}  ", "\tV{Time 60000}\n", "123", "1a", "_a", "a_b",
        // syntax errors and trailing input
        ")", "]", "}", ",", "(a", "[a", "(a,", "(a b", "a)", "V{Length 60000} junk", "{Length 60000 a}",
    ];

    private static System.Type M => Mathcad.Type("mplrun", "Mcd.Type");
    private static System.Type MNested(string name) => Nested(M, name);
    private static object? MParse(string s) => Call(M, "Parse", s);
    private static object MDefaultSystem() => Get(Mathcad.Type("mplrun", "Mcd.RuntimeOptions"), "s_default_unit_system")!;

    private static string Unmap(string s) => System.Text.RegularExpressions.Regex.Replace(s, @"\bQuire\.(?=(Mcd|Mpl)\.)", "");

    internal static string Dump(object? t)
    {
        if (t == null) return "null";
        string kind = t.GetType().Name;
        return kind switch
        {
            "Constant" or "Variable" => $"{kind}({Text(Get(t, "m_nam"))})",
            "Application" => $"App({Dump(Get(t, "Operator"))}; " +
                (Get(t, "Operands") is Array ops ? string.Join(", ", ops.Cast<object?>().Select(Dump)) : "null") + ")",
            "Product" => $"Prod(v={Text(Get(t, "m_var"))}; " +
                (Get(t, "Powers") is Array pows ? string.Join(" ", pows.Cast<object>().Select(p => $"{Dump(Get(p, "Base"))}^{Get(p, "Exponent")}")) : "null") + ")",
            _ => kind,
        };
    }

    private static string Powers(object? slice) =>
        slice is Array arr ? string.Join(" ", arr.Cast<object>().Select(p => $"{Dump(Get(p, "Base"))}^{Get(p, "Exponent")}")) : "null";

    private static string State(object t)
    {
        if (t is Exception) return Dump(t);
        string kind = t.GetType().Name;
        if (kind is "Constant" or "Variable" or "Application" or "Product") return Dump(t);
        if (kind == "Factory")
        {
            var stk = (System.Collections.ICollection)Get(t, "m_stk")!;
            return $"Factory(count={stk.Count})";
        }
        if (kind == "Lexer") return $"Lexer(off={Get(t, "m_off")})";
        if (kind == "Parser") return $"Parser(tok={Get(t, "m_tok")}, val={Text(Get(t, "m_val"))})";
        if (kind is "Printer" or "Formatter")
        {
            var sb = (StringBuilder)Get(t, "m_out")!;
            return $"{kind}(\"{sb}\")";
        }
        if (t is QType.Visitor || (MNested("Visitor") is Type mVis && mVis.IsAssignableFrom(t.GetType()))) return "Visitor";
        return Unmap(Reflect.Text(t));
    }

    private static MethodInfo ActualOverride(MethodInfo baseMethod, object target)
    {
        var slot = baseMethod.GetBaseDefinition();
        for (var type = target.GetType(); type != null; type = type.BaseType)
        {
            var actual = type.GetMethods(BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.DeclaredOnly)
                .FirstOrDefault(m => m.GetBaseDefinition().MetadataToken == slot.MetadataToken && m.GetBaseDefinition().Module == slot.Module);
            if (actual != null) return actual;
        }
        return baseMethod;
    }

    private static string Show(object? o)
    {
        if (o == null) return "null";
        if (o is Exception ex) return Thrown(ex);
        if (o is QType || o.GetType().IsSubclassOf(M) || o.GetType() == M) return Dump(o);
        if (o is Array arr)
        {
            if (arr.GetType().GetElementType()?.Name == "Power") return Powers(arr);
            return "[" + string.Join(", ", arr.Cast<object?>().Select(Show)) + "]";
        }
        return State(o);
    }

    private static QType.Product.Power[] QPowers() =>
        [new() { Base = new QType.Variable("a"), Exponent = 2 }, new() { Base = new QType.Constant("Length"), Exponent = 60000 }];

    private static Array MPowers()
    {
        var powerType = Nested(MNested("Product"), "Power");
        var array = Array.CreateInstance(powerType, 2);
        (string kind, string name, int e)[] spec = [("Variable", "a", 2), ("Constant", "Length", 60000)];
        for (int i = 0; i < 2; i++)
        {
            object p = Activator.CreateInstance(powerType)!;
            powerType.GetField("Base")!.SetValue(p, New(MNested(spec[i].kind), spec[i].name));
            powerType.GetField("Exponent")!.SetValue(p, spec[i].e);
            array.SetValue(p, i);
        }
        return array;
    }

    private class TestVisitor(StringBuilder sb) : QType.Visitor
    {
        public override void Visit(QType.Constant c) => sb.Append($"C:{c.Name};");
        public override void Visit(QType.Variable v) => sb.Append($"V:{v.Name};");
        public override void Visit(QType.Application a) => sb.Append($"A:{a.Name};");
        public override void Visit(QType.Product p) => sb.Append($"P:{p.Name};");
    }

    [SkippableFact]
    public void Type_core_members_and_parsing()
    {
        var qRoot = typeof(QType).GetMethod("get_Root", BindingFlags.Public | BindingFlags.Instance | BindingFlags.DeclaredOnly)!;
        var mRoot = M.GetMethod("get_Root", BindingFlags.Public | BindingFlags.Instance | BindingFlags.DeclaredOnly)!;
        var qImplicit = typeof(QType).GetMethod("get_IsImplicitArgument", BindingFlags.Public | BindingFlags.Instance | BindingFlags.DeclaredOnly)!;
        var mImplicit = M.GetMethod("get_IsImplicitArgument", BindingFlags.Public | BindingFlags.Instance | BindingFlags.DeclaredOnly)!;
        var qGetItem = typeof(QType).GetMethod("get_Item", BindingFlags.Public | BindingFlags.Instance | BindingFlags.DeclaredOnly, [typeof(int)])!;
        var mGetItem = M.GetMethod("get_Item", BindingFlags.Public | BindingFlags.Instance | BindingFlags.DeclaredOnly, [typeof(int)])!;
        var qSetItem = typeof(QType).GetMethod("set_Item", BindingFlags.Public | BindingFlags.Instance | BindingFlags.DeclaredOnly, [typeof(int), typeof(QType)])!;
        var mSetItem = M.GetMethod("set_Item", BindingFlags.Public | BindingFlags.Instance | BindingFlags.DeclaredOnly, [typeof(int), M])!;
        var qDimProd = typeof(QType).GetMethod("GetDimensionTypeProduct", BindingFlags.Public | BindingFlags.Instance | BindingFlags.DeclaredOnly)!;
        var mDimProd = M.GetMethod("GetDimensionTypeProduct", BindingFlags.Public | BindingFlags.Instance | BindingFlags.DeclaredOnly)!;

        var qTake = typeof(QType).GetMethod("Take", BindingFlags.NonPublic | BindingFlags.Static | BindingFlags.DeclaredOnly)!;
        var mTake = M.GetMethod("Take", BindingFlags.NonPublic | BindingFlags.Static | BindingFlags.DeclaredOnly)!;

        // Direct test for Root on basic terms before any options are touched
        var qConst = new QType.Constant("xyz");
        var mConst = New(MNested("Constant"), "xyz");
        MethodComparison.Compare("Constant.Root", Contract, qRoot, qConst, [], mRoot, mConst, [], Show, Show, State, State);

        // Take helper
        var qPowers = QPowers();
        var mPowers = MPowers();

        string ShowTake(object? o)
        {
            if (o == null) return "null";
            if (o is Array arr)
            {
                bool same = ReferenceEquals(arr, qPowers) || ReferenceEquals(arr, mPowers);
                return $"{(same ? "SAME" : "NEW")}:{Powers(arr)}";
            }
            return Reflect.Text(o);
        }

        MethodComparison.Compare("Take(0, 2)", Contract, qTake, null, [qPowers, 0, 2], mTake, null, [mPowers, 0, 2], ShowTake, ShowTake);
        MethodComparison.Compare("Take(0, 1)", Contract, qTake, null, [qPowers, 0, 1], mTake, null, [mPowers, 0, 1], ShowTake, ShowTake);
        MethodComparison.Compare("Take(0, 0)", Contract, qTake, null, [qPowers, 0, 0], mTake, null, [mPowers, 0, 0], ShowTake, ShowTake);
        MethodComparison.Compare("Take(1, 1)", Contract, qTake, null, [qPowers, 1, 1], mTake, null, [mPowers, 1, 1], ShowTake, ShowTake);
        MethodComparison.Compare("Take(1, 2)", Contract, qTake, null, [qPowers, 1, 2], mTake, null, [mPowers, 1, 2], ShowTake, ShowTake);

        var qParseStr = typeof(QType).GetMethod("Parse", BindingFlags.Public | BindingFlags.Static | BindingFlags.DeclaredOnly, [typeof(string)])!;
        var mParseStr = M.GetMethod("Parse", BindingFlags.Public | BindingFlags.Static | BindingFlags.DeclaredOnly, [typeof(string)])!;
        var qToString = typeof(QType).GetMethod("ToString", BindingFlags.Public | BindingFlags.Instance | BindingFlags.DeclaredOnly, Type.EmptyTypes)!;
        var mToString = M.GetMethod("ToString", BindingFlags.Public | BindingFlags.Instance | BindingFlags.DeclaredOnly, Type.EmptyTypes)!;

        foreach (var s in Signatures)
        {
            MethodComparison.Compare($"Parse({Text(s)})", Contract, qParseStr, null, [s], mParseStr, null, [s], Show, Show);

            object? qTerm = null, mTerm = null;
            try { qTerm = QType.Parse(s); } catch { }
            try { mTerm = MParse(s); } catch { }

            if (qTerm != null && mTerm != null)
            {
                MethodComparison.Compare($"ToString({s})", Contract, qToString, qTerm, [], mToString, mTerm, [], Show, Show, State, State);
                MethodComparison.Compare($"Root({s})", Contract, qRoot, qTerm, [], mRoot, mTerm, [], Show, Show, State, State);
                MethodComparison.Compare($"Implicit({s})", Contract, ActualOverride(qImplicit, qTerm), qTerm, [], ActualOverride(mImplicit, mTerm), mTerm, [], Show, Show, State, State);
                MethodComparison.Compare($"DimProd({s})", Contract, qDimProd, qTerm, [], mDimProd, mTerm, [], Show, Show, State, State);

                foreach (int i in new[] { -1, 0, 1, 2 })
                {
                    MethodComparison.Compare($"get_Item({s}, {i})", Contract, qGetItem, qTerm, [i], mGetItem, mTerm, [i], Show, Show, State, State);

                    var qClone = QType.Parse(s);
                    var mClone = MParse(s)!;
                    var qZ = new QType.Constant("Z");
                    var mZ = New(MNested("Constant"), "Z");
                    MethodComparison.Compare($"set_Item({s}, {i})", Contract, qSetItem, qClone, [i, qZ], mSetItem, mClone, [i, mZ], Show, Show, State, State);
                }
            }
        }

        // Now sync runtime options for Format tests
        Runtime.Sync();
        object qsys = Quire.Mcd.RuntimeOptions.s_default_unit_system;
        object msys = MDefaultSystem();

        var qFormat0 = typeof(QType).GetMethod("Format", BindingFlags.Public | BindingFlags.Instance | BindingFlags.DeclaredOnly, Type.EmptyTypes)!;
        var mFormat0 = M.GetMethod("Format", BindingFlags.Public | BindingFlags.Instance | BindingFlags.DeclaredOnly, Type.EmptyTypes)!;
        var qFormat1 = typeof(QType).GetMethod("Format", BindingFlags.Public | BindingFlags.Instance | BindingFlags.DeclaredOnly, [typeof(StringBuilder)])!;
        var mFormat1 = M.GetMethod("Format", BindingFlags.Public | BindingFlags.Instance | BindingFlags.DeclaredOnly, [typeof(StringBuilder)])!;
        var qFormat2 = typeof(QType).GetMethod("Format", BindingFlags.Public | BindingFlags.Instance | BindingFlags.DeclaredOnly, [typeof(StringBuilder), typeof(Quire.Mcd.UnitSystem)])!;
        var mFormat2 = M.GetMethod("Format", BindingFlags.Public | BindingFlags.Instance | BindingFlags.DeclaredOnly, [typeof(StringBuilder), Mathcad.Type("mplrun", "Mcd.UnitSystem")])!;

        foreach (var s in Signatures)
        {
            object? qTerm = null, mTerm = null;
            try { qTerm = QType.Parse(s); } catch { }
            try { mTerm = MParse(s); } catch { }

            if (qTerm != null && mTerm != null)
            {
                MethodComparison.Compare($"Format0({s})", Contract, qFormat0, qTerm, [], mFormat0, mTerm, [], Show, Show, State, State);

                var qSb1 = new StringBuilder("<");
                var mSb1 = new StringBuilder("<");
                MethodComparison.Compare($"Format1({s})", Contract, qFormat1, qTerm, [qSb1], mFormat1, mTerm, [mSb1], Show, Show, State, State);

                var qSb2 = new StringBuilder("<");
                var mSb2 = new StringBuilder("<");
                MethodComparison.Compare($"Format2({s})", Contract, qFormat2, qTerm, [qSb2, qsys], mFormat2, mTerm, [mSb2, msys], Show, Show, State, State);
            }
        }
    }

    [SkippableFact]
    public void Named_subtypes_and_comparisons()
    {
        var qNamedName = typeof(QType.Named).GetMethod("get_Name")!;
        var mNamedName = MNested("Named").GetMethod("get_Name")!;
        var qNamedHash = typeof(QType.Named).GetMethod("GetHashCode")!;
        var mNamedHash = MNested("Named").GetMethod("GetHashCode")!;
        var qNamedEquals = typeof(QType.Named).GetMethod("Equals", [typeof(object)])!;
        var mNamedEquals = MNested("Named").GetMethod("Equals", [typeof(object)])!;
        var qNamedCompare = typeof(QType.Named).GetMethod("CompareTo", [typeof(object)])!;
        var mNamedCompare = MNested("Named").GetMethod("CompareTo", [typeof(object)])!;

        var qConstCtor = typeof(QType.Constant).GetConstructor([typeof(string)])!;
        var mConstCtor = MNested("Constant").GetConstructor([typeof(string)])!;
        var qConstImplicit = typeof(QType.Constant).GetMethod("get_IsImplicitArgument")!;
        var mConstImplicit = MNested("Constant").GetMethod("get_IsImplicitArgument")!;
        var qConstAccept = typeof(QType.Constant).GetMethod("Accept", [typeof(QType.Visitor)])!;
        var mConstAccept = MNested("Constant").GetMethod("Accept", [MNested("Visitor")])!;

        var qVarCtor = typeof(QType.Variable).GetConstructor([typeof(string)])!;
        var mVarCtor = MNested("Variable").GetConstructor([typeof(string)])!;
        var qVarAccept = typeof(QType.Variable).GetMethod("Accept", [typeof(QType.Visitor)])!;
        var mVarAccept = MNested("Variable").GetMethod("Accept", [MNested("Visitor")])!;

        var qAppCtor = typeof(QType.Application).GetConstructor([typeof(QType), typeof(QType[])])!;
        var mAppCtor = MNested("Application").GetConstructor([M, Array.CreateInstance(M, 0).GetType()])!;
        var qAppAccept = typeof(QType.Application).GetMethod("Accept", [typeof(QType.Visitor)])!;
        var mAppAccept = MNested("Application").GetMethod("Accept", [MNested("Visitor")])!;
        var qAppImplicit = typeof(QType.Application).GetMethod("get_IsImplicitArgument")!;
        var mAppImplicit = MNested("Application").GetMethod("get_IsImplicitArgument")!;
        var qAppName = typeof(QType.Application).GetMethod("get_Name")!;
        var mAppName = MNested("Application").GetMethod("get_Name")!;
        var qAppArity = typeof(QType.Application).GetMethod("get_Arity")!;
        var mAppArity = MNested("Application").GetMethod("get_Arity")!;

        var qProdCtor = typeof(QType.Product).GetConstructor([typeof(QType.Product.Power[]), typeof(int)])!;
        var mProdCtor = MNested("Product").GetConstructor([Array.CreateInstance(Nested(MNested("Product"), "Power"), 0).GetType(), typeof(int)])!;
        var qProdAccept = typeof(QType.Product).GetMethod("Accept", [typeof(QType.Visitor)])!;
        var mProdAccept = MNested("Product").GetMethod("Accept", [MNested("Visitor")])!;
        var qProdName = typeof(QType.Product).GetMethod("get_Name")!;
        var mProdName = MNested("Product").GetMethod("get_Name")!;
        var qProdMonomorphic = typeof(QType.Product).GetMethod("get_IsMonomorphic")!;
        var mProdMonomorphic = MNested("Product").GetMethod("get_IsMonomorphic")!;
        var qProdPolymorphic = typeof(QType.Product).GetMethod("get_IsPolymorphic")!;
        var mProdPolymorphic = MNested("Product").GetMethod("get_IsPolymorphic")!;
        var qProdDimensionless = typeof(QType.Product).GetMethod("get_IsDimensionless")!;
        var mProdDimensionless = MNested("Product").GetMethod("get_IsDimensionless")!;
        var qProdVariables = typeof(QType.Product).GetMethod("get_Variables")!;
        var mProdVariables = MNested("Product").GetMethod("get_Variables")!;
        var qProdConstants = typeof(QType.Product).GetMethod("get_Constants")!;
        var mProdConstants = MNested("Product").GetMethod("get_Constants")!;

        string?[] names = ["Length", "Mass", "length", "a", "", "Θ", "COPTUW", "C", "I", null];

        foreach (var n in names)
        {
            MethodComparison.Compare($"Constant.ctor({Text(n)})", Contract, qConstCtor, null, [n], mConstCtor, null, [n], Show, Show);
            MethodComparison.Compare($"Variable.ctor({Text(n)})", Contract, qVarCtor, null, [n], mVarCtor, null, [n], Show, Show);

            if (n != null)
            {
                var qC = new QType.Constant(n);
                var mC = New(MNested("Constant"), n);
                var qV = new QType.Variable(n);
                var mV = New(MNested("Variable"), n);

                MethodComparison.Compare($"Constant({n}).Name", Contract, qNamedName, qC, [], mNamedName, mC, [], Show, Show, State, State);
                MethodComparison.Compare($"Constant({n}).Implicit", Contract, qConstImplicit, qC, [], mConstImplicit, mC, [], Show, Show, State, State);
                MethodComparison.Compare($"Constant({n}).GetHashCode", Contract, qNamedHash, qC, [], mNamedHash, mC, [], Show, Show, State, State);

                MethodComparison.Compare($"Variable({n}).Name", Contract, qNamedName, qV, [], mNamedName, mV, [], Show, Show, State, State);
                MethodComparison.Compare($"Variable({n}).GetHashCode", Contract, qNamedHash, qV, [], mNamedHash, mV, [], Show, Show, State, State);

                foreach (var other in new[] { "Length", "length", "x", "" })
                {
                    var qOtherC = new QType.Constant(other);
                    var mOtherC = New(MNested("Constant"), other);
                    var qOtherV = new QType.Variable(other);
                    var mOtherV = New(MNested("Variable"), other);

                    MethodComparison.Compare($"Constant({n}).Equals(Constant({other}))", Contract, qNamedEquals, qC, [qOtherC], mNamedEquals, mC, [mOtherC], Show, Show, State, State);
                    MethodComparison.Compare($"Constant({n}).Equals(Variable({other}))", Contract, qNamedEquals, qC, [qOtherV], mNamedEquals, mC, [mOtherV], Show, Show, State, State);
                    MethodComparison.Compare($"Constant({n}).CompareTo(Variable({other}))", Contract, qNamedCompare, qC, [qOtherV], mNamedCompare, mC, [mOtherV], Show, Show, State, State);
                }

                MethodComparison.Compare($"Constant({n}).Equals(null)", Contract, qNamedEquals, qC, [null], mNamedEquals, mC, [null], Show, Show, State, State);
                MethodComparison.Compare($"Constant({n}).CompareTo(null)", Contract, qNamedCompare, qC, [null], mNamedCompare, mC, [null], Show, Show, State, State);
            }
        }

        // Application methods
        var qOp = new QType.Constant("F");
        var mOp = New(MNested("Constant"), "F");
        var qOperands = new QType[] { new QType.Variable("a"), new QType.Variable("b") };
        var mOperands = Array.CreateInstance(M, 2);
        mOperands.SetValue(New(MNested("Variable"), "a"), 0);
        mOperands.SetValue(New(MNested("Variable"), "b"), 1);

        MethodComparison.Compare("Application.ctor(F, [a, b])", Contract, qAppCtor, null, [qOp, qOperands], mAppCtor, null, [mOp, mOperands], Show, Show);
        MethodComparison.Compare("Application.ctor(null, null)", Contract, qAppCtor, null, [null, null], mAppCtor, null, [null, null], Show, Show);

        var qApp = new QType.Application(qOp, qOperands);
        var mApp = New(MNested("Application"), mOp, mOperands);
        MethodComparison.Compare("Application.Name", Contract, qAppName, qApp, [], mAppName, mApp, [], Show, Show, State, State);
        MethodComparison.Compare("Application.Arity", Contract, qAppArity, qApp, [], mAppArity, mApp, [], Show, Show, State, State);
        MethodComparison.Compare("Application.Implicit", Contract, qAppImplicit, qApp, [], mAppImplicit, mApp, [], Show, Show, State, State);

        var qAppI = new QType.Application(new QType.Constant("I"), qOperands);
        var mAppI = New(MNested("Application"), New(MNested("Constant"), "I"), mOperands);
        MethodComparison.Compare("Application(I).Implicit", Contract, qAppImplicit, qAppI, [], mAppImplicit, mAppI, [], Show, Show, State, State);

        // Product methods
        var qPowers = QPowers();
        var mPowers = MPowers();
        foreach (int v in new[] { -1, 0, 1, 2, 3 })
        {
            MethodComparison.Compare($"Product.ctor(2 powers, {v})", Contract, qProdCtor, null, [qPowers, v], mProdCtor, null, [mPowers, v], Show, Show);
            var qProd = new QType.Product(qPowers, v);
            var mProd = New(MNested("Product"), mPowers, v);

            MethodComparison.Compare($"Product({v}).Name", Contract, qProdName, qProd, [], mProdName, mProd, [], Show, Show, State, State);
            MethodComparison.Compare($"Product({v}).IsMonomorphic", Contract, qProdMonomorphic, qProd, [], mProdMonomorphic, mProd, [], Show, Show, State, State);
            MethodComparison.Compare($"Product({v}).IsPolymorphic", Contract, qProdPolymorphic, qProd, [], mProdPolymorphic, mProd, [], Show, Show, State, State);
            MethodComparison.Compare($"Product({v}).IsDimensionless", Contract, qProdDimensionless, qProd, [], mProdDimensionless, mProd, [], Show, Show, State, State);
            MethodComparison.Compare($"Product({v}).Variables", Contract, qProdVariables, qProd, [], mProdVariables, mProd, [], Show, Show, State, State);
            MethodComparison.Compare($"Product({v}).Constants", Contract, qProdConstants, qProd, [], mProdConstants, mProd, [], Show, Show, State, State);
        }

        // Empty Product
        var qEmptyPowers = Array.Empty<QType.Product.Power>();
        var mEmptyPowers = Array.CreateInstance(Nested(MNested("Product"), "Power"), 0);
        MethodComparison.Compare("Product.ctor(empty)", Contract, qProdCtor, null, [qEmptyPowers, 0], mProdCtor, null, [mEmptyPowers, 0], Show, Show);
        var qProdEmpty = new QType.Product(qEmptyPowers, 0);
        var mProdEmpty = New(MNested("Product"), mEmptyPowers, 0);
        MethodComparison.Compare("Product(empty).IsDimensionless", Contract, qProdDimensionless, qProdEmpty, [], mProdDimensionless, mProdEmpty, [], Show, Show, State, State);

        // Visitor Accept & Traverse
        var qVisitor = Activator.CreateInstance(typeof(QType).GetNestedType("Printer", BindingFlags.NonPublic)!, new StringBuilder())!;
        var mVisitor = Activator.CreateInstance(MNested("Printer"), new StringBuilder())!;
        var qConstant = new QType.Constant("X");
        var mConstant = New(MNested("Constant"), "X");
        var qVariable = new QType.Variable("y");
        var mVariable = New(MNested("Variable"), "y");

        MethodComparison.Compare("Constant.Accept", Contract, qConstAccept, qConstant, [qVisitor], mConstAccept, mConstant, [mVisitor], Show, Show, State, State);
        MethodComparison.Compare("Variable.Accept", Contract, qVarAccept, qVariable, [qVisitor], mVarAccept, mVariable, [mVisitor], Show, Show, State, State);
        MethodComparison.Compare("Application.Accept", Contract, qAppAccept, qApp, [qVisitor], mAppAccept, mApp, [mVisitor], Show, Show, State, State);
        MethodComparison.Compare("Product.Accept", Contract, qProdAccept, new QType.Product(qPowers, 1), [qVisitor], mProdAccept, New(MNested("Product"), mPowers, 1), [mVisitor], Show, Show, State, State);
    }

    [SkippableFact]
    public void Factory_Lexer_Parser_Printer_Formatter()
    {
        // Factory methods
        var qFacCtor = typeof(QType.Factory).GetConstructor([])!;
        var mFacCtor = MNested("Factory").GetConstructor([])!;
        MethodComparison.Compare("Factory.ctor()", Contract, qFacCtor, null, [], mFacCtor, null, [], Show, Show);

        var qFacPush1 = typeof(QType.Factory).GetMethod("Push", [typeof(QType)])!;
        var mFacPush1 = MNested("Factory").GetMethod("Push", [M])!;
        var qFacPush2 = typeof(QType.Factory).GetMethod("Push", [typeof(QType), typeof(int)])!;
        var mFacPush2 = MNested("Factory").GetMethod("Push", [M, typeof(int)])!;
        var qFacPop0 = typeof(QType.Factory).GetMethod("Pop", Type.EmptyTypes)!;
        var mFacPop0 = MNested("Factory").GetMethod("Pop", Type.EmptyTypes)!;
        var qFacPop1 = typeof(QType.Factory).GetMethod("Pop", [typeof(int)])!;
        var mFacPop1 = MNested("Factory").GetMethod("Pop", [typeof(int)])!;
        var qFacNewConst = typeof(QType.Factory).GetMethod("NewConstant", [typeof(string)])!;
        var mFacNewConst = MNested("Factory").GetMethod("NewConstant", [typeof(string)])!;
        var qFacNewVar = typeof(QType.Factory).GetMethod("NewVariable", [typeof(string)])!;
        var mFacNewVar = MNested("Factory").GetMethod("NewVariable", [typeof(string)])!;
        var qFacNewAppArr = typeof(QType.Factory).GetMethod("NewApplication", [typeof(QType), typeof(QType[])])!;
        var mFacNewAppArr = MNested("Factory").GetMethod("NewApplication", [M, Array.CreateInstance(M, 0).GetType()])!;
        var qFacNewAppInt = typeof(QType.Factory).GetMethod("NewApplication", [typeof(int)])!;
        var mFacNewAppInt = MNested("Factory").GetMethod("NewApplication", [typeof(int)])!;
        var qFacNewAppTypeInt = typeof(QType.Factory).GetMethod("NewApplication", [typeof(QType), typeof(int)])!;
        var mFacNewAppTypeInt = MNested("Factory").GetMethod("NewApplication", [M, typeof(int)])!;
        var qFacNewTuple = typeof(QType.Factory).GetMethod("NewTuple", [typeof(int)])!;
        var mFacNewTuple = MNested("Factory").GetMethod("NewTuple", [typeof(int)])!;
        var qFacNewFunc = typeof(QType.Factory).GetMethod("NewFunction", [typeof(QType), typeof(QType)])!;
        var mFacNewFunc = MNested("Factory").GetMethod("NewFunction", [M, M])!;
        var qFacNewList = typeof(QType.Factory).GetMethod("NewList", [typeof(QType)])!;
        var mFacNewList = MNested("Factory").GetMethod("NewList", [M])!;
        var qFacNewProd = typeof(QType.Factory).GetMethod("NewProduct", [typeof(int), typeof(int)])!;
        var mFacNewProd = MNested("Factory").GetMethod("NewProduct", [typeof(int), typeof(int)])!;

        var qFac = new QType.Factory();
        var mFac = New(MNested("Factory"));

        MethodComparison.Compare("NewConstant(K)", Contract, qFacNewConst, qFac, ["K"], mFacNewConst, mFac, ["K"], Show, Show, State, State);
        MethodComparison.Compare("NewVariable(k)", Contract, qFacNewVar, qFac, ["k"], mFacNewVar, mFac, ["k"], Show, Show, State, State);

        var qConstF = new QType.Constant("F");
        var mConstF = New(MNested("Constant"), "F");
        MethodComparison.Compare("NewApplication(t, null)", Contract, qFacNewAppArr, qFac, [qConstF, null], mFacNewAppArr, mFac, [mConstF, null], Show, Show, State, State);
        MethodComparison.Compare("NewApplication(t, [])", Contract, qFacNewAppArr, qFac, [qConstF, Array.Empty<QType>()], mFacNewAppArr, mFac, [mConstF, Array.CreateInstance(M, 0)], Show, Show, State, State);

        MethodComparison.Compare("NewFunction(A, B)", Contract, qFacNewFunc, qFac, [new QType.Constant("A"), new QType.Constant("B")], mFacNewFunc, mFac, [New(MNested("Constant"), "A"), New(MNested("Constant"), "B")], Show, Show, State, State);
        MethodComparison.Compare("NewList(A)", Contract, qFacNewList, qFac, [new QType.Constant("A")], mFacNewList, mFac, [New(MNested("Constant"), "A")], Show, Show, State, State);

        MethodComparison.Compare("NewTuple(0)", Contract, qFacNewTuple, qFac, [0], mFacNewTuple, mFac, [0], Show, Show, State, State);

        MethodComparison.Compare("Push(P1)", Contract, qFacPush1, qFac, [new QType.Constant("P1")], mFacPush1, mFac, [New(MNested("Constant"), "P1")], Show, Show, State, State);
        MethodComparison.Compare("NewTuple(1)", Contract, qFacNewTuple, qFac, [1], mFacNewTuple, mFac, [1], Show, Show, State, State);

        foreach (var name in new[] { "P1", "P2", "P3" })
        {
            MethodComparison.Compare($"Push({name})", Contract, qFacPush1, qFac, [new QType.Constant(name)], mFacPush1, mFac, [New(MNested("Constant"), name)], Show, Show, State, State);
        }
        MethodComparison.Compare("NewTuple(3)", Contract, qFacNewTuple, qFac, [3], mFacNewTuple, mFac, [3], Show, Show, State, State);

        foreach (var name in new[] { "G", "X", "Y" })
        {
            MethodComparison.Compare($"Push({name})", Contract, qFacPush1, qFac, [new QType.Constant(name)], mFacPush1, mFac, [New(MNested("Constant"), name)], Show, Show, State, State);
        }
        MethodComparison.Compare("NewApplication(3)", Contract, qFacNewAppInt, qFac, [3], mFacNewAppInt, mFac, [3], Show, Show, State, State);

        foreach (var name in new[] { "X", "Y" })
        {
            MethodComparison.Compare($"Push({name})", Contract, qFacPush1, qFac, [new QType.Constant(name)], mFacPush1, mFac, [New(MNested("Constant"), name)], Show, Show, State, State);
        }
        MethodComparison.Compare("NewApplication(t, 2)", Contract, qFacNewAppTypeInt, qFac, [new QType.Constant("H"), 2], mFacNewAppTypeInt, mFac, [New(MNested("Constant"), "H"), 2], Show, Show, State, State);
        MethodComparison.Compare("NewApplication(t, 0)", Contract, qFacNewAppTypeInt, qFac, [new QType.Constant("H"), 0], mFacNewAppTypeInt, mFac, [New(MNested("Constant"), "H"), 0], Show, Show, State, State);

        MethodComparison.Compare("Push(Length, 60000)", Contract, qFacPush2, qFac, [new QType.Constant("Length"), 60000], mFacPush2, mFac, [New(MNested("Constant"), "Length"), 60000], Show, Show, State, State);
        MethodComparison.Compare("Push(Time, -2)", Contract, qFacPush2, qFac, [new QType.Constant("Time"), -2], mFacPush2, mFac, [New(MNested("Constant"), "Time"), -2], Show, Show, State, State);
        MethodComparison.Compare("NewProduct(0, 2)", Contract, qFacNewProd, qFac, [0, 2], mFacNewProd, mFac, [0, 2], Show, Show, State, State);

        foreach (var name in new[] { "Q1", "Q2" })
        {
            MethodComparison.Compare($"Push({name})", Contract, qFacPush1, qFac, [new QType.Constant(name)], mFacPush1, mFac, [New(MNested("Constant"), name)], Show, Show, State, State);
        }
        MethodComparison.Compare("Pop(2)", Contract, qFacPop1, qFac, [2], mFacPop1, mFac, [2], Show, Show, State, State);
        MethodComparison.Compare("Pop() empty", Contract, qFacPop0, qFac, [], mFacPop0, mFac, [], Show, Show, State, State);

        // Lexer & Parser methods
        var qLexType = typeof(QType).GetNestedType("Lexer", BindingFlags.NonPublic)!;
        var mLexType = MNested("Lexer");
        var qLexCtor = qLexType.GetConstructor([typeof(string)])!;
        var mLexCtor = mLexType.GetConstructor([typeof(string)])!;
        var qLexGetToken = qLexType.GetMethod("GetToken")!;
        var mLexGetToken = mLexType.GetMethod("GetToken")!;

        MethodComparison.Compare("Lexer.ctor(V{Length})", Contract, qLexCtor, null, ["V{Length}"], mLexCtor, null, ["V{Length}"], Show, Show);
        var qLex = Activator.CreateInstance(qLexType, "V{Length 60000}")!;
        var mLex = Activator.CreateInstance(mLexType, "V{Length 60000}")!;
        for (int i = 0; i < 5; i++)
        {
            MethodComparison.Compare($"Lexer.GetToken({i})", Contract, qLexGetToken, qLex, [null], mLexGetToken, mLex, [null], Show, Show, State, State);
        }

        var qLex2 = Activator.CreateInstance(qLexType, "-> , -12 abc123   spaces   () [] {} $")!;
        var mLex2 = Activator.CreateInstance(mLexType, "-> , -12 abc123   spaces   () [] {} $")!;
        for (int i = 0; i < 15; i++)
        {
            MethodComparison.Compare($"Lexer.GetToken2({i})", Contract, qLexGetToken, qLex2, [null], mLexGetToken, mLex2, [null], Show, Show, State, State);
        }

        var qParserType = typeof(QType).GetNestedType("Parser", BindingFlags.NonPublic)!;
        var mParserType = MNested("Parser");
        var qParserCtor = qParserType.GetConstructor([typeof(string)])!;
        var mParserCtor = mParserType.GetConstructor([typeof(string)])!;
        var qParserParse = qParserType.GetMethod("Parse", BindingFlags.Public | BindingFlags.Instance, Type.EmptyTypes)!;
        var mParserParse = mParserType.GetMethod("Parse", BindingFlags.Public | BindingFlags.Instance, Type.EmptyTypes)!;
        var qParseApp = qParserType.GetMethod("ParseApplication", BindingFlags.NonPublic | BindingFlags.Instance, Type.EmptyTypes)!;
        var mParseApp = mParserType.GetMethod("ParseApplication", BindingFlags.NonPublic | BindingFlags.Instance, Type.EmptyTypes)!;
        var qParseAtomic = qParserType.GetMethod("ParseAtomic", BindingFlags.NonPublic | BindingFlags.Instance, Type.EmptyTypes)!;
        var mParseAtomic = mParserType.GetMethod("ParseAtomic", BindingFlags.NonPublic | BindingFlags.Instance, Type.EmptyTypes)!;
        var qParseExp = qParserType.GetMethod("ParseExponent", BindingFlags.NonPublic | BindingFlags.Instance, Type.EmptyTypes)!;
        var mParseExp = mParserType.GetMethod("ParseExponent", BindingFlags.NonPublic | BindingFlags.Instance, Type.EmptyTypes)!;
        var qIsNext = qParserType.GetMethod("IsNextToken", BindingFlags.NonPublic | BindingFlags.Instance, [typeof(string)])!;
        var mIsNext = mParserType.GetMethod("IsNextToken", BindingFlags.NonPublic | BindingFlags.Instance, [typeof(string)])!;
        var qGetNext = qParserType.GetMethod("GetNextToken", BindingFlags.NonPublic | BindingFlags.Instance, Type.EmptyTypes)!;
        var mGetNext = mParserType.GetMethod("GetNextToken", BindingFlags.NonPublic | BindingFlags.Instance, Type.EmptyTypes)!;
        var qGetNextIf = qParserType.GetMethod("GetNextTokenIf", BindingFlags.NonPublic | BindingFlags.Instance, [typeof(string)])!;
        var mGetNextIf = mParserType.GetMethod("GetNextTokenIf", BindingFlags.NonPublic | BindingFlags.Instance, [typeof(string)])!;
        var qSkipToken = qParserType.GetMethod("SkipToken", BindingFlags.NonPublic | BindingFlags.Instance, [typeof(char)])!;
        var mSkipToken = mParserType.GetMethod("SkipToken", BindingFlags.NonPublic | BindingFlags.Instance, [typeof(char)])!;

        MethodComparison.Compare("Parser.ctor(V{Length})", Contract, qParserCtor, null, ["V{Length 60000}"], mParserCtor, null, ["V{Length 60000}"], Show, Show);
        var qParser = Activator.CreateInstance(qParserType, "V{Length 60000}")!;
        var mParser = Activator.CreateInstance(mParserType, "V{Length 60000}")!;
        MethodComparison.Compare("Parser.IsNextToken(V)", Contract, qIsNext, qParser, ["V"], mIsNext, mParser, ["V"], Show, Show, State, State);
        MethodComparison.Compare("Parser.GetNextTokenIf(V)", Contract, qGetNextIf, qParser, ["V"], mGetNextIf, mParser, ["V"], Show, Show, State, State);
        MethodComparison.Compare("Parser.GetNextToken()", Contract, qGetNext, qParser, [], mGetNext, mParser, [], Show, Show, State, State);
        MethodComparison.Compare("Parser.SkipToken({)", Contract, qSkipToken, qParser, ['{'], mSkipToken, mParser, ['{'], Show, Show, State, State);

        foreach (var expStr in new[] { "-12", "5", "", "0", "99999999999", "junk" })
        {
            var qEp = Activator.CreateInstance(qParserType, expStr)!;
            var mEp = Activator.CreateInstance(mParserType, expStr)!;
            MethodComparison.Compare($"Parser.ParseExponent({expStr})", Contract, qParseExp, qEp, [], mParseExp, mEp, [], Show, Show, State, State);
        }

        foreach (var appStr in new[] { "F a b", "a->b", "a->b->c", "a", "(a, b)" })
        {
            var qAp = Activator.CreateInstance(qParserType, appStr)!;
            var mAp = Activator.CreateInstance(mParserType, appStr)!;
            MethodComparison.Compare($"Parser.ParseApplication({appStr})", Contract, qParseApp, qAp, [], mParseApp, mAp, [], Show, Show, State, State);
        }

        foreach (var atomStr in new[] { "[Length]", "(a, b)", "{a 2 Length 60000}", "a", "Length", "()", "{}", "{a 2 b-3 Mass}", "(a)", "[[a]]", "{a}", "{Length -60000}" })
        {
            var qAt = Activator.CreateInstance(qParserType, atomStr)!;
            var mAt = Activator.CreateInstance(mParserType, atomStr)!;
            MethodComparison.Compare($"Parser.ParseAtomic({atomStr})", Contract, qParseAtomic, qAt, [], mParseAtomic, mAt, [], Show, Show, State, State);
        }

        foreach (var fullStr in new[] { "a->b", "{Length 60000}", "(a, b)", "[a]", "F a b", "V{Length 60000}", "{a 2 b-3}" })
        {
            var qFp = Activator.CreateInstance(qParserType, fullStr)!;
            var mFp = Activator.CreateInstance(mParserType, fullStr)!;
            MethodComparison.Compare($"Parser.Parse({fullStr})", Contract, qParserParse, qFp, [], mParserParse, mFp, [], Show, Show, State, State);
        }

        // Visitor methods
        var qVisType = typeof(QType.Visitor);
        var mVisType = MNested("Visitor");

        var qVisTraverse = qVisType.GetMethod("Traverse", [typeof(QType)])!;
        var mVisTraverse = mVisType.GetMethod("Traverse", [M])!;

        var qVisitor = Activator.CreateInstance(typeof(QType).GetNestedType("Printer", BindingFlags.NonPublic)!, new StringBuilder())!;
        var mVisitor = Activator.CreateInstance(MNested("Printer"), new StringBuilder())!;
        MethodComparison.Compare("Visitor.Traverse", Contract, qVisTraverse, qVisitor, [new QType.Constant("C")], mVisTraverse, mVisitor, [New(MNested("Constant"), "C")], Show, Show, State, State);

        // Printer methods
        var qPrintType = typeof(QType).GetNestedType("Printer", BindingFlags.NonPublic)!;
        var mPrintType = MNested("Printer");
        var qPrintCtor = qPrintType.GetConstructor([typeof(StringBuilder)])!;
        var mPrintCtor = mPrintType.GetConstructor([typeof(StringBuilder)])!;
        MethodComparison.Compare("Printer.ctor()", Contract, qPrintCtor, null, [new StringBuilder()], mPrintCtor, null, [new StringBuilder()], Show, Show);

        var qPrintConst = qPrintType.GetMethod("Visit", [typeof(QType.Constant)])!;
        var mPrintConst = mPrintType.GetMethod("Visit", [MNested("Constant")])!;
        var qPrintVar = qPrintType.GetMethod("Visit", [typeof(QType.Variable)])!;
        var mPrintVar = mPrintType.GetMethod("Visit", [MNested("Variable")])!;
        var qPrintApp = qPrintType.GetMethod("Visit", [typeof(QType.Application)])!;
        var mPrintApp = mPrintType.GetMethod("Visit", [MNested("Application")])!;
        var qPrintProd = qPrintType.GetMethod("Visit", [typeof(QType.Product)])!;
        var mPrintProd = mPrintType.GetMethod("Visit", [MNested("Product")])!;

        var qPrinter = Activator.CreateInstance(qPrintType, new StringBuilder())!;
        var mPrinter = Activator.CreateInstance(mPrintType, new StringBuilder())!;
        MethodComparison.Compare("Printer.Visit(Constant)", Contract, qPrintConst, qPrinter, [new QType.Constant("X")], mPrintConst, mPrinter, [New(MNested("Constant"), "X")], Show, Show, State, State);
        MethodComparison.Compare("Printer.Visit(Variable)", Contract, qPrintVar, qPrinter, [new QType.Variable("y")], mPrintVar, mPrinter, [New(MNested("Variable"), "y")], Show, Show, State, State);

        foreach (var s in new[] { "F a b", "(a, b)", "a->b", "[a]" })
        {
            var qT = QType.Parse(s);
            var mT = MParse(s);
            MethodComparison.Compare($"Printer.Visit(Application {s})", Contract, qPrintApp, qPrinter, [qT], mPrintApp, mPrinter, [mT], Show, Show, State, State);
        }

        foreach (var s in new[] { "{}", "{a}", "{Length -60000}", "{a 2 Length 60000}", "{a 2 b-3 Mass}" })
        {
            var qT = QType.Parse(s);
            var mT = MParse(s);
            MethodComparison.Compare($"Printer.Visit(Product {s})", Contract, qPrintProd, qPrinter, [qT], mPrintProd, mPrinter, [mT], Show, Show, State, State);
        }

        // Formatter methods
        var qFormType = typeof(QType).GetNestedType("Formatter", BindingFlags.NonPublic)!;
        var mFormType = MNested("Formatter");
        var qFormCtor = qFormType.GetConstructor([typeof(StringBuilder), typeof(Quire.Mcd.UnitSystem)])!;
        var mFormCtor = mFormType.GetConstructor([typeof(StringBuilder), Mathcad.Type("mplrun", "Mcd.UnitSystem")])!;
        MethodComparison.Compare("Formatter.ctor()", Contract, qFormCtor, null, [new StringBuilder(), Quire.Mcd.RuntimeOptions.s_default_unit_system], mFormCtor, null, [new StringBuilder(), MDefaultSystem()], Show, Show);

        var qFormVar = qFormType.GetMethod("Visit", [typeof(QType.Variable)])!;
        var mFormVar = mFormType.GetMethod("Visit", [MNested("Variable")])!;
        var qFormApp = qFormType.GetMethod("Visit", [typeof(QType.Application)])!;
        var mFormApp = mFormType.GetMethod("Visit", [MNested("Application")])!;
        var qFormOnFunc = qFormType.GetMethod("OnFunction", BindingFlags.NonPublic | BindingFlags.Instance, [typeof(QType), typeof(int)])!;
        var mFormOnFunc = mFormType.GetMethod("OnFunction", BindingFlags.NonPublic | BindingFlags.Instance, [M, typeof(int)])!;
        var qFormProd = qFormType.GetMethod("Visit", [typeof(QType.Product)])!;
        var mFormProd = mFormType.GetMethod("Visit", [MNested("Product")])!;

        var qFormatter = Activator.CreateInstance(qFormType, new StringBuilder(), Quire.Mcd.RuntimeOptions.s_default_unit_system)!;
        var mFormatter = Activator.CreateInstance(mFormType, new StringBuilder(), MDefaultSystem())!;

        foreach (var vName in new[] { "a", "b", "t1", "t12", "custom" })
        {
            MethodComparison.Compare($"Formatter.Visit(Variable {vName})", Contract, qFormVar, qFormatter, [new QType.Variable(vName)], mFormVar, mFormatter, [New(MNested("Variable"), vName)], Show, Show, State, State);
        }

        foreach (var s in new[] { "M(V{Length 60000})", "(a, b)", "a->b", "[a]", "F a b", "(I a)->b", "M(M(V{Time 60000}))", "D a", "N{Length 60000}", "V a" })
        {
            var qT = QType.Parse(s);
            var mT = MParse(s);
            MethodComparison.Compare($"Formatter.Visit(Application {s})", Contract, qFormApp, qFormatter, [qT], mFormApp, mFormatter, [mT], Show, Show, State, State);
        }

        foreach (var s in new[] { "{}", "{a}", "{a 2}", "{a b}", "{a 2 b-3}", "{a Length 60000}", "{Length 60000}", "{Length 120000}", "{Length -60000}", "{Length 60000 Time-120000}", "{Mass 60000 Length 120000 Time-180000 Current-60000}", "{Length 60000 Mass 60000}" })
        {
            var qT = QType.Parse(s);
            var mT = MParse(s);
            MethodComparison.Compare($"Formatter.Visit(Product {s})", Contract, qFormProd, qFormatter, [qT], mFormProd, mFormatter, [mT], Show, Show, State, State);
        }
    }

    [SkippableFact]
    public void Formatter_OnFunction()
    {
        var qFormType = typeof(QType).GetNestedType("Formatter", BindingFlags.NonPublic)!;
        var mFormType = MNested("Formatter");
        var qFormOnFunc = qFormType.GetMethod("OnFunction", BindingFlags.NonPublic | BindingFlags.Instance, [typeof(QType), typeof(int)])!;
        var mFormOnFunc = mFormType.GetMethod("OnFunction", BindingFlags.NonPublic | BindingFlags.Instance, [M, typeof(int)])!;

        var qFormatter = Activator.CreateInstance(qFormType, new StringBuilder(), Quire.Mcd.RuntimeOptions.s_default_unit_system)!;
        var mFormatter = Activator.CreateInstance(mFormType, new StringBuilder(), MDefaultSystem())!;

        var qQuestionTerm = new QType.Constant("?");
        var mQuestionTerm = New(MNested("Constant"), "?");
        MethodComparison.Compare("Formatter.OnFunction(?)", Contract, qFormOnFunc, qFormatter, [qQuestionTerm, 0], mFormOnFunc, mFormatter, [mQuestionTerm, 0], Show, Show, State, State);

        var qConstTerm = new QType.Constant("c");
        var mConstTerm = New(MNested("Constant"), "c");
        MethodComparison.Compare("Formatter.OnFunction(c, 0)", Contract, qFormOnFunc, qFormatter, [qConstTerm, 0], mFormOnFunc, mFormatter, [mConstTerm, 0], Show, Show, State, State);

        var qFuncTerm = QType.Parse("a->b->c");
        var mFuncTerm = MParse("a->b->c");
        MethodComparison.Compare("Formatter.OnFunction(a->b->c)", Contract, qFormOnFunc, qFormatter, [qFuncTerm, 0], mFormOnFunc, mFormatter, [mFuncTerm, 0], Show, Show, State, State);
    }
}

