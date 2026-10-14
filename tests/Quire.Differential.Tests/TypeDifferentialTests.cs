using System.Text;
using static Quire.Differential.Tests.Reflect;
using QType = Quire.Mcd.Type;

namespace Quire.Differential.Tests;

/// <summary>
/// Parses the same signatures with mplrun's <c>Mcd.Type</c> and Quire's, and compares the terms' structure, printed
/// and formatted text, and every public member; the cases are listed in docs/mirror/evidence/Mcd.Type.md.
/// </summary>
public class TypeDifferentialTests
{
    /// <summary>Signatures covering every construct, plus malformed and edge-case input.</summary>
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

    /// <summary>The term's structure, through either side's public fields.</summary>
    internal static string Dump(object? t)
    {
        if (t == null) return "null";
        string kind = t.GetType().Name;
        return kind switch
        {
            "Constant" or "Variable" => $"{kind}({Text(Get(t, "m_nam"))})",
            "Application" => $"App({Dump(Get(t, "Operator"))}; {string.Join(", ", ((Array)Get(t, "Operands")!).Cast<object>().Select(Dump))})",
            "Product" => $"Prod(v={Text(Get(t, "m_var"))}; " + string.Join(" ",
                ((Array)Get(t, "Powers")!).Cast<object>().Select(p => $"{Dump(Get(p, "Base"))}^{Get(p, "Exponent")}")) + ")",
            _ => kind,
        };
    }

    private static object MDefaultSystem() => Get(Mathcad.Type("mplrun", "Mcd.RuntimeOptions"), "s_default_unit_system")!;

    private static string Members(object? t, Func<object?, object?, string> format, object system)
    {
        if (t == null) return "null";
        var sb = new StringBuilder();
        sb.Append(Dump(t));
        sb.Append(" | ToString ").Append(Outcome(() => t.ToString()));
        sb.Append(" | Name ").Append(Outcome(() => Get(t, "Name")));
        sb.Append(" | Root ").Append(Outcome(() => Get(t, "Root")));
        sb.Append(" | Implicit ").Append(Outcome(() => Get(t, "IsImplicitArgument")));
        sb.Append(" | DimProduct ").Append(Outcome(() => Call(t, "GetDimensionTypeProduct"), Dump));
        sb.Append(" | Format ").Append(Outcome(() => format(t, system)));
        if (t.GetType().Name == "Application") sb.Append(" | Arity ").Append(Outcome(() => Get(t, "Arity")));
        if (t.GetType().Name == "Product")
        {
            foreach (var p in new[] { "IsMonomorphic", "IsPolymorphic", "IsDimensionless" })
                sb.Append($" | {p} ").Append(Outcome(() => Get(t, p)));
            sb.Append(" | Variables ").Append(Outcome(() => Get(t, "Variables"), a => Powers(a, t)));
            sb.Append(" | Constants ").Append(Outcome(() => Get(t, "Constants"), a => Powers(a, t)));
        }
        return sb.ToString();
    }

    /// <summary>A power slice, and whether it is the product's own array (as a full slice is).</summary>
    private static string Powers(object? slice, object product) =>
        (ReferenceEquals(slice, Get(product, "Powers")) ? "same " : "copy ") +
        string.Join(" ", ((Array)slice!).Cast<object>().Select(p => $"{Dump(Get(p, "Base"))}^{Get(p, "Exponent")}"));

    private static string QFormat(object? t, object system)
    {
        var sb = new StringBuilder("<");
        ((QType)t!).Format(sb, (Quire.Mcd.UnitSystem)system);
        return sb.ToString();
    }

    private static string MFormat(object? t, object system)
    {
        var sb = new StringBuilder("<");
        Call(t!, "Format", sb, system);
        return sb.ToString();
    }

    [SkippableFact]
    public void Parse_print_and_inspect_every_signature()
    {
        Runtime.Sync();
        var diffs = new List<string>();
        object qsys = Quire.Mcd.RuntimeOptions.s_default_unit_system, msys = MDefaultSystem();
        foreach (var s in Signatures)
        {
            string q = Outcome(() => QType.Parse(s), t => Members(t, QFormat, qsys));
            string m = Outcome(() => MParse(s), t => Members(t, MFormat, msys));
            Same(diffs, $"Parse({Text(s)})", q, m);
            // Format() and Format(StringBuilder) use the thread's unit system
            Same(diffs, $"Parse({Text(s)}).Format()", () => QType.Parse(s).Format(), () => Call(MParse(s)!, "Format"));
            Same(diffs, $"Parse({Text(s)}).Format(sb)", () => { var sb = new StringBuilder("<"); QType.Parse(s).Format(sb); return sb.ToString(); },
                () => { var sb = new StringBuilder("<"); Call(MParse(s)!, "Format", sb); return sb.ToString(); });
        }
        NoDiffs(diffs);
    }

    [SkippableFact]
    public void Constructors_and_named_comparisons()
    {
        Runtime.Sync();
        var diffs = new List<string>();
        string?[] names = ["Length", "Mass", "length", "a", "", "Θ", "COPTUW", "C", "I", null];
        foreach (var n in names)
        {
            Same(diffs, $"new Constant({Text(n)})", () => new QType.Constant(n!), () => New(MNested("Constant"), n), t => Members(t, (_, _) => "", ""));
            Same(diffs, $"new Variable({Text(n)})", () => new QType.Variable(n!), () => New(MNested("Variable"), n), t => Members(t, (_, _) => "", ""));
            foreach (var o in names)
            {
                Same(diffs, $"Constant({Text(n)}).Equals(Constant({Text(o)}))",
                    () => new QType.Constant(n!).Equals(new QType.Constant(o!)), () => New(MNested("Constant"), n).Equals(New(MNested("Constant"), o)));
                Same(diffs, $"Constant({Text(n)}).Equals(Variable({Text(o)}))",
                    () => new QType.Constant(n!).Equals(new QType.Variable(o!)), () => New(MNested("Constant"), n).Equals(New(MNested("Variable"), o)));
                Same(diffs, $"Constant({Text(n)}).CompareTo(Variable({Text(o)}))",
                    () => new QType.Constant(n!).CompareTo(new QType.Variable(o!)), () => Call(New(MNested("Constant"), n), "CompareTo", New(MNested("Variable"), o)));
            }
            Same(diffs, $"Constant({Text(n)}).Equals(null)", () => new QType.Constant(n!).Equals(null), () => New(MNested("Constant"), n).Equals(null));
            Same(diffs, $"Constant({Text(n)}).Equals(product)", () => new QType.Constant(n!).Equals(QType.Parse("{}")), () => New(MNested("Constant"), n).Equals(MParse("{}")));
            Same(diffs, $"Constant({Text(n)}).GetHashCode", () => new QType.Constant(n!).GetHashCode(), () => New(MNested("Constant"), n).GetHashCode());
        }
        // Application and Product constructors, including null and malformed arguments
        Same(diffs, "new Application(C, [])", () => new QType.Application(new QType.Constant("F"), []),
            () => New(MNested("Application"), New(MNested("Constant"), "F"), Array.CreateInstance(M, 0)), t => Members(t, (_, _) => "", ""));
        Same(diffs, "new Application(null, null).Arity", () => new QType.Application(null!, null!).Arity,
            () => Get(New(MNested("Application"), null, null), "Arity"));
        Same(diffs, "new Product(null, 0)", () => new QType.Product(null!, 0), () => New(MNested("Product"), null, 0), Dump);
        foreach (int v in new[] { -1, 0, 1, 2, 3 })
        {
            Same(diffs, $"new Product(2 powers, {v})", () => new QType.Product(QPowers(), v), () => New(MNested("Product"), MPowers(), v),
                t => Members(t, (_, _) => "", ""));
        }
        NoDiffs(diffs);
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

    [SkippableFact]
    public void Indexer_reads_and_writes_operands()
    {
        Runtime.Sync();
        var diffs = new List<string>();
        foreach (var s in new[] { "F a b", "a->b", "V{Length 60000}", "a", "{}", "(a, b, c)" })
            foreach (int i in new[] { -1, 0, 1, 2, 3, 4 })
            {
                Same(diffs, $"Parse({s})[{i}]", () => QType.Parse(s)[i], () => M.GetProperty("Item")!.GetValue(MParse(s), [i]), Dump);
                Same(diffs, $"Parse({s})[{i}] = Z", () => { var t = QType.Parse(s); t[i] = new QType.Constant("Z"); return t; },
                    () => { var t = MParse(s)!; M.GetProperty("Item")!.SetValue(t, New(MNested("Constant"), "Z"), [i]); return t; }, Dump);
            }
        NoDiffs(diffs);
    }

    /// <summary>One script of factory calls, run on either side; each step's result is dumped.</summary>
    private static string FactoryScript(object fac, Func<string, object> constant, Func<object[], Array> types)
    {
        var log = new List<string>();
        void Step(string what, Func<object?> f) => log.Add(what + "=" + Outcome(f, Dump));
        Step("NewConstant", () => Call(fac, "NewConstant", "K"));
        Step("NewVariable", () => Call(fac, "NewVariable", "k"));
        Step("NewApplication(t, null)", () => CallSig(fac, "NewApplication", ["Type", "Type[]"], constant("F"), null));
        Step("NewApplication(t, [])", () => CallSig(fac, "NewApplication", ["Type", "Type[]"], constant("F"), types([])));
        Step("NewApplication(t, [a])", () => CallSig(fac, "NewApplication", ["Type", "Type[]"], constant("F"), types([constant("A")])));
        Step("NewFunction", () => Call(fac, "NewFunction", constant("A"), constant("B")));
        Step("NewList", () => Call(fac, "NewList", constant("A")));
        Step("NewTuple(0)", () => Call(fac, "NewTuple", 0));
        Call(fac, "Push", constant("P1"));
        Step("NewTuple(1)", () => Call(fac, "NewTuple", 1));
        foreach (var n in new[] { "P1", "P2", "P3" }) Call(fac, "Push", constant(n));
        Step("NewTuple(3)", () => Call(fac, "NewTuple", 3));
        foreach (var n in new[] { "G", "X", "Y" }) Call(fac, "Push", constant(n));
        Step("NewApplication(3)", () => CallSig(fac, "NewApplication", ["Int32"], 3));
        foreach (var n in new[] { "X", "Y" }) Call(fac, "Push", constant(n));
        Step("NewApplication(t, 2)", () => CallSig(fac, "NewApplication", ["Type", "Int32"], constant("H"), 2));
        Step("NewApplication(t, 0)", () => CallSig(fac, "NewApplication", ["Type", "Int32"], constant("H"), 0));
        CallSig(fac, "Push", ["Type", "Int32"], constant("Length"), 60000);
        CallSig(fac, "Push", ["Type", "Int32"], constant("Time"), -2);
        Step("NewProduct(0, 2)", () => Call(fac, "NewProduct", 0, 2));
        foreach (var n in new[] { "Q1", "Q2" }) Call(fac, "Push", constant(n));
        Step("Pop(2)", () => CallSig(fac, "Pop", ["Int32"], 2));
        Step("Pop() empty", () => CallSig(fac, "Pop", []));
        Step("Pop(-1)", () => CallSig(fac, "Pop", ["Int32"], -1));
        CallSig(fac, "Push", ["Type", "Int32"], constant("Length"), 1);
        Step("Pop() exponent", () => CallSig(fac, "Pop", []));
        Step("NewProduct(1, 0) mismatched", () => Call(fac, "NewProduct", 1, 0));
        Step("NewTuple(-1)", () => Call(fac, "NewTuple", -1));
        Step("NewApplication(0)", () => CallSig(fac, "NewApplication", ["Int32"], 0));
        return string.Join("\n", log);
    }

    [SkippableFact]
    public void Factory_builds_terms_from_its_stack()
    {
        Runtime.Sync();
        string q = FactoryScript(new QType.Factory(), n => new QType.Constant(n), a => a.Cast<QType>().ToArray());
        string m = FactoryScript(New(MNested("Factory")), n => New(MNested("Constant"), n), a =>
        {
            var array = Array.CreateInstance(M, a.Length);
            a.CopyTo(array, 0);
            return array;
        });
        Assert.Equal(m, q);
    }
}
