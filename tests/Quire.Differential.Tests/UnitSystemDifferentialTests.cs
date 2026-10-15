using System.Reflection;
using static Quire.Differential.Tests.Reflect;
using Q = Quire.Mcd;

namespace Quire.Differential.Tests;

/// <summary>
/// Builds the same units and unit systems with mplrun's <c>Mcd.Unit</c>, <c>Mcd.Units</c> and <c>Mcd.UnitSystem</c>
/// and Quire's, and compares every public and private member over many dimensions; cases are listed in
/// docs/mirror/evidence/Mcd.UnitSystem.md.
/// </summary>
public class UnitSystemDifferentialTests
{
    private const string Contract = "Mcd.UnitSystem";
    private static System.Type MUnit => Mathcad.Type("mplrun", "Mcd.Unit");
    private static System.Type MUnits => Mathcad.Type("mplrun", "Mcd.Units");
    private static System.Type MSystem => Mathcad.Type("mplrun", "Mcd.UnitSystem");
    private static System.Type MType => Mathcad.Type("mplrun", "Mcd.Type");
    private static System.Type MDefault => Mathcad.Type("mplrun", "Mcd.UnitSystem+Default");
    private static System.Type QDefault => typeof(Q.UnitSystem).GetNestedType("Default", BindingFlags.NonPublic)!;

    private record U(string Name, string Symbol, string Quantity, double Magnitude, string Signature);

    private static readonly U[] Base =
    [
        new("meter", "m", "Length", 1, "V{Length 60000}"), new("kilogram", "kg", "Mass", 1, "V{Mass 60000}"),
        new("second", "s", "Time", 1, "V{Time 60000}"), new("ampere", "A", "Current", 1, "V{Current 60000}"),
        new("kelvin", "K", "Temperature", 1, "V{Temperature 60000}"), new("mole", "mol", "Substance", 1, "V{Substance 60000}"),
        new("candela", "cd", "Luminosity", 1, "V{Luminosity 60000}"), new("base_currency", "¤", "Money", 1, "V{Money 60000}"),
    ];

    private static readonly U[] UsBase =
    [
        new("foot", "ft", "Length", 0.3048, "V{Length 60000}"), new("pound", "lb", "Mass", 0.45359237, "V{Mass 60000}"),
        new("second", "s", "Time", 1, "V{Time 60000}"), new("ampere", "A", "Current", 1, "V{Current 60000}"),
        new("rankine", "R", "Temperature", 5.0 / 9, "V{Temperature 60000}"), new("mole", "mol", "Substance", 1, "V{Substance 60000}"),
        new("candela", "cd", "Luminosity", 1, "V{Luminosity 60000}"), new("dollar", "$", "Money", 1, "V{Money 60000}"),
    ];

    private static readonly U[] Derived =
    [
        new("newton", "N", "Force", 1, "V{Length 60000 Mass 60000 Time-120000}"),
        new("kilonewton", "kN", "Force", 1000, "V{Length 60000 Mass 60000 Time-120000}"),
        new("joule", "J", "Energy", 1, "V{Length 120000 Mass 60000 Time-120000}"),
        new("liter", "L", "Volume", 0.001, "V{Length 180000}"), new("hectare", "ha", "Area", 1e4, "V{Length 120000}"),
        new("hertz", "Hz", "Frequency", 1, "V{Time-60000}"), new("minute", "min", "Time", 60, "V{Time 60000}"),
        new("pascal", "Pa", "Pressure", 1, "V{Length-60000 Mass 60000 Time-120000}"),
        new("volt", "V", "Potential", 1, "V{Current-60000 Length 120000 Mass 60000 Time-180000}"),
        new("rootmeter", "rm", "Root length", 1, "V{Length 30000}"), new("odd", "od", "Odd", 2, "V{Length 7 Time-3}"),
        new("nothing", "NOSIMPLIFY", "Velocity", 1, "V{Length 60000 Time-60000}"),
    ];

    private static readonly (string Name, U[] Basis, U[] Defaults)[] Systems =
    [
        ("si", Base, Derived),
        ("us", UsBase, Derived[..6]),
        ("seven", Base[..7], Derived[..3]),
        ("six-no-current", [.. Base[..3], .. Base[4..7]], Derived[..1]),
        ("nine", [.. Base, new U("bit", "b", "Information", 1, "V{Information 60000}")], Derived[..2]),
        ("singular", [.. Base[..7], new U("foot", "ft", "Length", 0.3048, "V{Length 60000}")], Derived[..1]),
        ("composite-basis", [.. Base[..2], new U("hertz", "Hz", "Frequency", 1, "V{Time-60000}"), .. Base[3..]], Derived[..4]),
        ("three-forces", Base, [Derived[0], Derived[1], new U("dyne", "dyn", "Force", 1e-5, "V{Length 60000 Mass 60000 Time-120000}")]),
        ("unknown-default", Base, [new U("foo", "f", "Foo", 1, "V{Foo 60000}")]),
        ("zero-magnitude", Base, [new U("zero", "z", "Force", 0, "V{Length 60000 Mass 60000 Time-120000}")]),
        ("empty", [], []),
    ];

    private static readonly string[] Signatures =
    [
        "V{Length 60000}", "V{}", "V{Length 60000 Time-120000}", "V{Current-60000 Length 120000 Mass 60000 Time-180000}",
        "V{Unknown 60000}", "M(V{Length 60000})", "a->b", "F{Time 60000}", "V{Length 7 Time-3}", "{Length 60000}",
    ];

    private static object MkUnit(bool mathcad, U u) => mathcad
        ? New(MUnit, u.Name, u.Symbol, u.Quantity, u.Magnitude, u.Signature)
        : new Q.Unit(u.Name, u.Symbol, u.Quantity, u.Magnitude, u.Signature);

    private static Array Units(bool mathcad, U[] us)
    {
        var array = Array.CreateInstance(mathcad ? MUnit : typeof(Q.Unit), us.Length);
        for (int i = 0; i < us.Length; i++) array.SetValue(MkUnit(mathcad, us[i]), i);
        return array;
    }

    private static string Unmap(string s) => System.Text.RegularExpressions.Regex.Replace(s, @"\bQuire\.(?=(Mcd|Mpl)\.)", "");

    internal static string UnitText(object? u) => u == null ? "null"
        : $"{Get(u, "Symbol")}({Get(u, "Name")}, {Get(u, "Quantity")}, {Text(Get(u, "Magnitude"))}, {Get(u, "Type")})";

    internal static string UnitsText(object? u) => u == null ? "null"
        : $"{u} m={Text(Get(u, "Magnitude"))}/{Text(Get(u, "magnitude"))} [" + string.Join(" ", ((Array)Get(u, "Powers")!).Cast<object>()
            .Select(p => $"{Get(Get(p, "Base")!, "Symbol")}^{Get(Get(p, "Exponent")!, "Numerator")}/{Get(Get(p, "Exponent")!, "Denominator")}")) + "]";

    private static string Dims(object? a) => a == null ? "null" : string.Join(" ", ((Array)a).Cast<object?>().Select(t => t?.ToString() ?? "null"));

    private static string UnitsDetailed(object? a) => a == null ? "null" : string.Join(" ", ((Array)a).Cast<object?>().Select(UnitText));

    private static string SystemState(object s)
    {
        string name = (string)Get(s, "Name")!;
        uint code = (uint)Get(s, "Code")!;
        string guid = (string)Get(s, "Guid")!;
        int ro = (int)Get(s, "RO_number")!;
        int rank = (int)Get(s, "Rank")!;
        string dims = Dims(Get(s, "Dimensions"));
        string basis = UnitsDetailed(Get(s, "Basis"));
        string defaults;
        try { defaults = UnitsDetailed(Get(s, "Defaults")); }
        catch (Exception e) { defaults = Thrown(e); }
        return $"UnitSystem({name}, code={code}, guid={guid}, ro={ro}, rank={rank}, dims=[{dims}], basis=[{basis}], defaults=[{defaults}])";
    }

    private static string DefaultState(object d)
    {
        var vec = (int[])Get(d, "Vector")!;
        int scalar = (int)Get(d, "Scalar")!;
        int order = (int)Get(d, "Order")!;
        var unit = Get(d, "Unit");
        return $"Default(vec=[{string.Join(",", vec)}], scalar={scalar}, order={order}, unit={(unit == null ? "null" : UnitText(unit))})";
    }

    private static string Show(object? o)
    {
        if (o == null) return "null";
        if (o.GetType().Name == "Units") return UnitsText(o);
        if (o.GetType().Name == "UnitSystem") return SystemState(o);
        if (o.GetType().Name == "Default") return DefaultState(o);
        if (o.GetType().Name == "Product") return $"Product(v={Get(o, "m_var")}, {o})";
        if (o.GetType().Name == "Constant" || o.GetType().Name == "Variable" || o.GetType().Name == "Application") return o.ToString()!;
        if (o is Exception ex) return Thrown(ex);
        return Unmap(Reflect.Text(o));
    }

    private static object? ProductOf(string signature, bool mathcad)
    {
        try { return Call(Call(mathcad ? MType : typeof(Q.Type), "Parse", signature)!, "GetDimensionTypeProduct"); }
        catch { return null; }
    }

    private static IEnumerable<int[]> Vectors(int rank)
    {
        int[] steps = [0, 60000, -60000, 120000, -120000, 180000, 30000, -20000, 7, -1];
        yield return new int[rank];
        for (int i = 0; i < rank; i++)
            foreach (int s in steps)
            {
                var v = new int[rank];
                v[i] = s;
                yield return v;
            }
        var rng = new Random(20261014);
        for (int n = 0; n < 20; n++)
        {
            var v = new int[rank];
            for (int i = 0; i < rank; i++) v[i] = steps[rng.Next(steps.Length)];
            yield return v;
        }
    }

    [SkippableFact]
    public void UnitSystem_methods_and_conversions()
    {
        var qCtor = typeof(Q.UnitSystem).GetConstructor([typeof(string), typeof(uint), typeof(string), typeof(Q.Unit[]), typeof(Q.Unit[])])!;
        var mCtor = MSystem.GetConstructor([typeof(string), typeof(uint), typeof(string), Array.CreateInstance(MUnit, 0).GetType(), Array.CreateInstance(MUnit, 0).GetType()])!;

        var qGetName = typeof(Q.UnitSystem).GetMethod("get_Name")!;
        var mGetName = MSystem.GetMethod("get_Name")!;
        var qGetCode = typeof(Q.UnitSystem).GetMethod("get_Code")!;
        var mGetCode = MSystem.GetMethod("get_Code")!;
        var qGetGuid = typeof(Q.UnitSystem).GetMethod("get_Guid")!;
        var mGetGuid = MSystem.GetMethod("get_Guid")!;
        var qGetRank = typeof(Q.UnitSystem).GetMethod("get_Rank")!;
        var mGetRank = MSystem.GetMethod("get_Rank")!;
        var qGetDims = typeof(Q.UnitSystem).GetMethod("get_Dimensions")!;
        var mGetDims = MSystem.GetMethod("get_Dimensions")!;
        var qGetBasis = typeof(Q.UnitSystem).GetMethod("get_Basis")!;
        var mGetBasis = MSystem.GetMethod("get_Basis")!;
        var qSetBasis = typeof(Q.UnitSystem).GetMethod("set_Basis", [typeof(Q.Unit[])])!;
        var mSetBasis = MSystem.GetMethod("set_Basis", [Array.CreateInstance(MUnit, 0).GetType()])!;
        var qGetDefs = typeof(Q.UnitSystem).GetMethod("get_Defaults")!;
        var mGetDefs = MSystem.GetMethod("get_Defaults")!;
        var qSetDefs = typeof(Q.UnitSystem).GetMethod("set_Defaults", [typeof(Q.Unit[])])!;
        var mSetDefs = MSystem.GetMethod("set_Defaults", [Array.CreateInstance(MUnit, 0).GetType()])!;
        var qGetRO = typeof(Q.UnitSystem).GetMethod("get_RO_number")!;
        var mGetRO = MSystem.GetMethod("get_RO_number")!;
        var qSetRO = typeof(Q.UnitSystem).GetMethod("set_RO_number", [typeof(int)])!;
        var mSetRO = MSystem.GetMethod("set_RO_number", [typeof(int)])!;
        var qToString = typeof(Q.UnitSystem).GetMethod("ToString", Type.EmptyTypes)!;
        var mToString = MSystem.GetMethod("ToString", Type.EmptyTypes)!;
        var qReset = typeof(Q.UnitSystem).GetMethod("Reset", Type.EmptyTypes)!;
        var mReset = MSystem.GetMethod("Reset", Type.EmptyTypes)!;

        var qSimpStr = typeof(Q.UnitSystem).GetMethod("Simplify", [typeof(string)])!;
        var mSimpStr = MSystem.GetMethod("Simplify", [typeof(string)])!;
        var qSimpProd = typeof(Q.UnitSystem).GetMethod("Simplify", [typeof(Q.Type.Product)])!;
        var mSimpProd = MSystem.GetMethod("Simplify", [MType.GetNestedType("Product")!])!;
        var qSimpInt = typeof(Q.UnitSystem).GetMethod("Simplify", [typeof(int[])])!;
        var mSimpInt = MSystem.GetMethod("Simplify", [typeof(int[])])!;

        var qNormStr = typeof(Q.UnitSystem).GetMethod("Normalize", [typeof(string)])!;
        var mNormStr = MSystem.GetMethod("Normalize", [typeof(string)])!;
        var qNormProd = typeof(Q.UnitSystem).GetMethod("Normalize", [typeof(Q.Type.Product)])!;
        var mNormProd = MSystem.GetMethod("Normalize", [MType.GetNestedType("Product")!])!;
        var qNormInt = typeof(Q.UnitSystem).GetMethod("Normalize", [typeof(int[])])!;
        var mNormInt = MSystem.GetMethod("Normalize", [typeof(int[])])!;

        var qGetCoords = typeof(Q.UnitSystem).GetMethod("GetCoordinates", [typeof(Q.Type.Product)])!;
        var mGetCoords = MSystem.GetMethod("GetCoordinates", [MType.GetNestedType("Product")!])!;
        var qGetUnits = typeof(Q.UnitSystem).GetMethod("GetUnits", [typeof(double[])])!;
        var mGetUnits = MSystem.GetMethod("GetUnits", [typeof(double[])])!;
        var qGetDimInt = typeof(Q.UnitSystem).GetMethod("GetDimensions", [typeof(int[])])!;
        var mGetDimInt = MSystem.GetMethod("GetDimensions", [typeof(int[])])!;
        var qChangeBasisInt = typeof(Q.UnitSystem).GetMethod("ChangeBasis", [typeof(int[])])!;
        var mChangeBasisInt = MSystem.GetMethod("ChangeBasis", [typeof(int[])])!;
        var qChangeBasisDbl = typeof(Q.UnitSystem).GetMethod("ChangeBasis", [typeof(double[])])!;
        var mChangeBasisDbl = MSystem.GetMethod("ChangeBasis", [typeof(double[])])!;
        var qIsUnitSym = typeof(Q.UnitSystem).GetMethod("IsUnitSymbol", [typeof(string)])!;
        var mIsUnitSym = MSystem.GetMethod("IsUnitSymbol", [typeof(string)])!;

        // Test constructors for all 11 system configs
        foreach (var (name, basis, defaults) in Systems)
        {
            var qB = Units(false, basis);
            var mB = Units(true, basis);
            var qD = Units(false, defaults);
            var mD = Units(true, defaults);
            MethodComparison.Compare($"new UnitSystem({name})", Contract, qCtor, null, [name, 7u, "guid", qB, qD], mCtor, null, [name, 7u, "guid", mB, mD], Show, Show);
        }

        // Use standard SI system for member comparisons
        var qSys = new Q.UnitSystem("si", 7u, "guid", (Q.Unit[])Units(false, Base), (Q.Unit[])Units(false, Derived));
        var mSys = New(MSystem, "si", 7u, "guid", Units(true, Base), Units(true, Derived));

        // Getters
        MethodComparison.Compare("get_Name", Contract, qGetName, qSys, [], mGetName, mSys, [], Show, Show, SystemState, SystemState);
        MethodComparison.Compare("get_Code", Contract, qGetCode, qSys, [], mGetCode, mSys, [], Show, Show, SystemState, SystemState);
        MethodComparison.Compare("get_Guid", Contract, qGetGuid, qSys, [], mGetGuid, mSys, [], Show, Show, SystemState, SystemState);
        MethodComparison.Compare("get_Rank", Contract, qGetRank, qSys, [], mGetRank, mSys, [], Show, Show, SystemState, SystemState);
        MethodComparison.Compare("get_Dimensions", Contract, qGetDims, qSys, [], mGetDims, mSys, [], Show, Show, SystemState, SystemState);
        MethodComparison.Compare("get_Basis", Contract, qGetBasis, qSys, [], mGetBasis, mSys, [], Show, Show, SystemState, SystemState);
        MethodComparison.Compare("get_Defaults", Contract, qGetDefs, qSys, [], mGetDefs, mSys, [], Show, Show, SystemState, SystemState);
        MethodComparison.Compare("get_RO_number", Contract, qGetRO, qSys, [], mGetRO, mSys, [], Show, Show, SystemState, SystemState);
        MethodComparison.Compare("ToString", Contract, qToString, qSys, [], mToString, mSys, [], Show, Show, SystemState, SystemState);

        // RO_number setter
        MethodComparison.Compare("set_RO_number(42)", Contract, qSetRO, qSys, [42], mSetRO, mSys, [42], Show, Show, SystemState, SystemState);

        // IsUnitSymbol
        foreach (var sym in new[] { "m", "kg", "A", "N", "kN", "NOSIMPLIFY", "ft", "x", "", null })
            MethodComparison.Compare($"IsUnitSymbol({Text(sym)})", Contract, qIsUnitSym, qSys, [sym], mIsUnitSym, mSys, [sym], Show, Show, SystemState, SystemState);

        // Vectors
        int rank = (int)Get(qSys, "Rank")!;
        foreach (var v in Vectors(rank))
        {
            string vs = string.Join(",", v);
            MethodComparison.Compare($"Simplify([{vs}])", Contract, qSimpInt, qSys, [v], mSimpInt, mSys, [v], Show, Show, SystemState, SystemState);
            MethodComparison.Compare($"Normalize([{vs}])", Contract, qNormInt, qSys, [v], mNormInt, mSys, [v], Show, Show, SystemState, SystemState);
            MethodComparison.Compare($"GetDimensions([{vs}])", Contract, qGetDimInt, qSys, [v], mGetDimInt, mSys, [v], Show, Show, SystemState, SystemState);
            MethodComparison.Compare($"ChangeBasis([{vs}])", Contract, qChangeBasisInt, qSys, [v], mChangeBasisInt, mSys, [v], Show, Show, SystemState, SystemState);

            var local = v.Select(x => x / 60000.0).ToArray();
            MethodComparison.Compare($"ChangeBasis(local [{vs}])", Contract, qChangeBasisDbl, qSys, [local], mChangeBasisDbl, mSys, [local], Show, Show, SystemState, SystemState);
            MethodComparison.Compare($"GetUnits(local [{vs}])", Contract, qGetUnits, qSys, [local], mGetUnits, mSys, [local], Show, Show, SystemState, SystemState);
        }

        // Shorter/longer vectors
        MethodComparison.Compare("ChangeBasis(short)", Contract, qChangeBasisInt, qSys, [new int[3]], mChangeBasisInt, mSys, [new int[3]], Show, Show, SystemState, SystemState);
        MethodComparison.Compare("ChangeBasis(long)", Contract, qChangeBasisInt, qSys, [new int[10]], mChangeBasisInt, mSys, [new int[10]], Show, Show, SystemState, SystemState);

        // Signatures
        foreach (var s in Signatures)
        {
            MethodComparison.Compare($"Simplify({s})", Contract, qSimpStr, qSys, [s], mSimpStr, mSys, [s], Show, Show, SystemState, SystemState);
            MethodComparison.Compare($"Normalize({s})", Contract, qNormStr, qSys, [s], mNormStr, mSys, [s], Show, Show, SystemState, SystemState);

            object? qProd = ProductOf(s, false);
            object? mProd = ProductOf(s, true);
            MethodComparison.Compare($"Simplify(product {s})", Contract, qSimpProd, qSys, [qProd], mSimpProd, mSys, [mProd], Show, Show, SystemState, SystemState);
            MethodComparison.Compare($"Normalize(product {s})", Contract, qNormProd, qSys, [qProd], mNormProd, mSys, [mProd], Show, Show, SystemState, SystemState);
            MethodComparison.Compare($"GetCoordinates({s})", Contract, qGetCoords, qSys, [qProd], mGetCoords, mSys, [mProd], Show, Show, SystemState, SystemState);
        }

        // Setters and Reset
        var qNewBasis = Units(false, UsBase);
        var mNewBasis = Units(true, UsBase);
        MethodComparison.Compare("set_Basis(US)", Contract, qSetBasis, qSys, [qNewBasis], mSetBasis, mSys, [mNewBasis], Show, Show, SystemState, SystemState);

        var qPaddedBasis = Units(false, Base[..7]);
        var mPaddedBasis = Units(true, Base[..7]);
        MethodComparison.Compare("set_Basis(padded 7)", Contract, qSetBasis, qSys, [qPaddedBasis], mSetBasis, mSys, [mPaddedBasis], Show, Show, SystemState, SystemState);

        var qSingularBasis = Units(false, [.. Base[..7], new U("foot", "ft", "Length", 0.3048, "V{Length 60000}")]);
        var mSingularBasis = Units(true, [.. Base[..7], new U("foot", "ft", "Length", 0.3048, "V{Length 60000}")]);
        MethodComparison.Compare("set_Basis(singular)", Contract, qSetBasis, qSys, [qSingularBasis], mSetBasis, mSys, [mSingularBasis], Show, Show, SystemState, SystemState);

        var qNewDefs = Units(false, Derived[..4]);
        var mNewDefs = Units(true, Derived[..4]);
        MethodComparison.Compare("set_Defaults(4)", Contract, qSetDefs, qSys, [qNewDefs], mSetDefs, mSys, [mNewDefs], Show, Show, SystemState, SystemState);

        var qMultiDefs = Units(false, [Derived[0], Derived[1], new U("dyne", "dyn", "Force", 1e-5, "V{Length 60000 Mass 60000 Time-120000}")]);
        var mMultiDefs = Units(true, [Derived[0], Derived[1], new U("dyne", "dyn", "Force", 1e-5, "V{Length 60000 Mass 60000 Time-120000}")]);
        MethodComparison.Compare("set_Defaults(multi-force)", Contract, qSetDefs, qSys, [qMultiDefs], mSetDefs, mSys, [mMultiDefs], Show, Show, SystemState, SystemState);

        var qEmptyDefs = Units(false, []);
        var mEmptyDefs = Units(true, []);
        MethodComparison.Compare("set_Defaults(empty)", Contract, qSetDefs, qSys, [qEmptyDefs], mSetDefs, mSys, [mEmptyDefs], Show, Show, SystemState, SystemState);

        MethodComparison.Compare("Reset()", Contract, qReset, qSys, [], mReset, mSys, [], Show, Show, SystemState, SystemState);
    }

    [SkippableFact]
    public void UnitSystem_private_methods_and_Default()
    {
        // Private static methods on UnitSystem
        var qGetDimsStr = typeof(Q.UnitSystem).GetMethod("GetDimensions", BindingFlags.NonPublic | BindingFlags.Static, [typeof(string)])!;
        var mGetDimsStr = MSystem.GetMethod("GetDimensions", BindingFlags.NonPublic | BindingFlags.Static, [typeof(string)])!;
        var qInverse = typeof(Q.UnitSystem).GetMethod("Inverse", BindingFlags.NonPublic | BindingFlags.Static, [typeof(double[,])])!;
        var mInverse = MSystem.GetMethod("Inverse", BindingFlags.NonPublic | BindingFlags.Static, [typeof(double[,])])!;
        var qIsSmall = typeof(Q.UnitSystem).GetMethod("IsSmall", BindingFlags.NonPublic | BindingFlags.Static, [typeof(double)])!;
        var mIsSmall = MSystem.GetMethod("IsSmall", BindingFlags.NonPublic | BindingFlags.Static, [typeof(double)])!;
        var qCountInt = typeof(Q.UnitSystem).GetMethod("Count", BindingFlags.NonPublic | BindingFlags.Static, [typeof(int[])])!;
        var mCountInt = MSystem.GetMethod("Count", BindingFlags.NonPublic | BindingFlags.Static, [typeof(int[])])!;
        var qCountDbl = typeof(Q.UnitSystem).GetMethod("Count", BindingFlags.NonPublic | BindingFlags.Static, [typeof(double[])])!;
        var mCountDbl = MSystem.GetMethod("Count", BindingFlags.NonPublic | BindingFlags.Static, [typeof(double[])])!;

        // Inverse(double[,])
        double[][,] testMatrices =
        [
            new double[,] { { 5.0 } },
            new double[,] { { 0.0 } },
            new double[,] { { 1, 0 }, { 0, 1 } },
            new double[,] { { 2, 1 }, { 1, 1 } },
            new double[,] { { 1, 1 }, { 1, 1 } },
            new double[,] { { 0, 1 }, { 1, 0 } },
            new double[,] { { 0, 2 }, { 3, 4 } },
            new double[,] { { 1e-13, 1 }, { 0, 1 } },
            new double[,] { { 0, 0 }, { 1, 1 } },
            new double[,] { { 0, 0, 1 }, { 1, 0, 0 }, { 0, 1, 0 } },
            new double[,] { { 0, 2, 1 }, { 1, 0, 1 }, { 1, 1, 0 } },
            new double[,] { { 1, 2, 3 }, { 0, 1, 4 }, { 5, 6, 0 } },
            new double[,] { { 0, 0, 1 }, { 0, 2, 0 }, { 3, 0, 0 } },
            new double[,] { { 0, 0, 1 }, { 0, 0, 2 }, { 1, 2, 3 } },
            new double[,] { { 0, 0, 0, 1 }, { 0, 0, 2, 0 }, { 0, 3, 0, 0 }, { 4, 0, 0, 0 } },
            new double[,] { { 1, 2, 3, 4 }, { 2, 3, 4, 1 }, { 3, 4, 1, 2 }, { 4, 1, 2, 3 } },
        ];
        for (int i = 0; i < testMatrices.Length; i++)
        {
            var mat = testMatrices[i];
            MethodComparison.Compare($"Inverse(mat_{i})", Contract, qInverse, null, [mat], mInverse, null, [mat], Show, Show);
        }

        // IsSmall(double)
        foreach (double d in new[] { 0.0, 1e-13, -1e-13, 1e-11, -1e-11, 1e-12, -1e-12, 9.99e-13, -9.99e-13, 1.01e-12, -1.01e-12, 1.0, -1.0, double.NaN })
            MethodComparison.Compare($"IsSmall({d:R})", Contract, qIsSmall, null, [d], mIsSmall, null, [d], Show, Show);

        // Count
        MethodComparison.Compare("Count([0,0])", Contract, qCountInt, null, [new int[] { 0, 0 }], mCountInt, null, [new int[] { 0, 0 }], Show, Show);
        MethodComparison.Compare("Count([1,0,2])", Contract, qCountInt, null, [new int[] { 1, 0, 2 }], mCountInt, null, [new int[] { 1, 0, 2 }], Show, Show);
        MethodComparison.Compare("Count([0.0, 0.0])", Contract, qCountDbl, null, [new double[] { 0.0, 0.0 }], mCountDbl, null, [new double[] { 0.0, 0.0 }], Show, Show);
        MethodComparison.Compare("Count([1.5, 0.0, NaN])", Contract, qCountDbl, null, [new double[] { 1.5, 0.0, double.NaN }], mCountDbl, null, [new double[] { 1.5, 0.0, double.NaN }], Show, Show);

        // GetDimensions(string)
        MethodComparison.Compare("GetDimensions(V{Length 60000})", Contract, qGetDimsStr, null, ["V{Length 60000}"], mGetDimsStr, null, ["V{Length 60000}"], Show, Show);
        MethodComparison.Compare("GetDimensions(plain)", Contract, qGetDimsStr, null, ["Length"], mGetDimsStr, null, ["Length"], Show, Show);

        // GetDimensionIndex (tested with uninitialized instance so no constructor side-effects occur)
        var qGetDimIdx = typeof(Q.UnitSystem).GetMethod("GetDimensionIndex", BindingFlags.NonPublic | BindingFlags.Instance, [typeof(Q.Type)])!;
        var mGetDimIdx = MSystem.GetMethod("GetDimensionIndex", BindingFlags.NonPublic | BindingFlags.Instance, [MType])!;
        var qSys = (Q.UnitSystem)System.Runtime.CompilerServices.RuntimeHelpers.GetUninitializedObject(typeof(Q.UnitSystem));
        var mSys = System.Runtime.CompilerServices.RuntimeHelpers.GetUninitializedObject(MSystem);
        var testDims = new[] { "Current", "Length", "Luminosity", "Mass", "Substance", "Temperature", "Time", "Angle" };
        var qDims = testDims.Select(Q.Type.Parse).OrderBy(t => t.ToString()).ToArray();
        var mDims = Array.CreateInstance(MType, testDims.Length);
        for (int i = 0; i < testDims.Length; i++)
            mDims.SetValue(Call(MType, "Parse", qDims[i].ToString()), i);
        Set(qSys, "m_dims", qDims);
        Set(mSys, "m_dims", mDims);

        string DimState(object s) => Dims(Get(s, "Dimensions"));

        foreach (var dName in new[] { "Current", "Angle", "Length", "Luminosity", "Mass", "Substance", "Temperature", "Time", "Unknown", "Foo" })
        {
            var qDim = Q.Type.Parse(dName);
            var mDim = Call(MType, "Parse", dName);
            MethodComparison.Compare($"GetDimensionIndex({dName})", Contract, qGetDimIdx, qSys, [qDim], mGetDimIdx, mSys, [mDim], Show, Show, DimState, DimState);
        }
    }

    [SkippableFact]
    public void UnitSystem_Default_methods()
    {
        // Default nested class
        var qDefCtor1 = QDefault.GetConstructor(BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance, null, [typeof(int[])], null)!;
        var mDefCtor1 = MDefault.GetConstructor(BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance, null, [typeof(int[])], null)!;
        var qDefCtor2 = QDefault.GetConstructor(BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance, null, [typeof(int[]), typeof(Q.Unit)], null)!;
        var mDefCtor2 = MDefault.GetConstructor(BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance, null, [typeof(int[]), MUnit], null)!;

        var qDefHash = QDefault.GetMethod("GetHashCode")!;
        var mDefHash = MDefault.GetMethod("GetHashCode")!;
        var qDefEquals = QDefault.GetMethod("Equals", [typeof(object)])!;
        var mDefEquals = MDefault.GetMethod("Equals", [typeof(object)])!;
        var qDefCompare = QDefault.GetMethod("CompareTo", [typeof(object)])!;
        var mDefCompare = MDefault.GetMethod("CompareTo", [typeof(object)])!;

        int[][] defaultVectors =
        [
            [],
            [0],
            [1],
            [-1],
            [5],
            [-5],
            [0, 0, 0],
            [60000, 0, 0],
            [-60000, 120000, 0],
            [120000, 60000, -120000],
            [-12, 18, -24],
            [12, 18, 24],
            [1, 2, 3],
            [-1, 2, 3],
            [7, 0, 0],
            [0, -1, 3],
            [0, 0, -6],
            [17, 33, 49, 65, 81, 97, 113, 129],
            [10, 20, 30, 40, 50, 60, 70, 80, 90, 100],
            [-15, 25, -35, 45, -55, 65, -75, 85, -95, 105],
            [1, 0, 2, 0, 3, 0, 4, 0, 5, 0],
            [255, 127, 63, 31, 15, 7, 3, 1],
            [-255, -127, -63, -31, -15, -7, -3, -1],
            [60000, 120000, 180000, 240000, 300000, 360000, 420000],
        ];

        var qUnit = new Q.Unit("meter", "m", "Length", 1.0, "V{Length 60000}");
        var mUnit = New(MUnit, "meter", "m", "Length", 1.0, "V{Length 60000}");

        foreach (var v in defaultVectors)
        {
            string vs = string.Join(",", v);
            MethodComparison.Compare($"Default.ctor([{vs}])", Contract, qDefCtor1, null, [v], mDefCtor1, null, [v], Show, Show);
            MethodComparison.Compare($"Default.ctor([{vs}], unit)", Contract, qDefCtor2, null, [v, qUnit], mDefCtor2, null, [v, mUnit], Show, Show);

            var qD = Activator.CreateInstance(QDefault, (object)v)!;
            var mD = Activator.CreateInstance(MDefault, (object)v)!;
            MethodComparison.Compare($"Default.GetHashCode([{vs}])", Contract, qDefHash, qD, [], mDefHash, mD, [], Show, Show, DefaultState, DefaultState);
        }

        // Default.Equals and CompareTo
        var qD1 = Activator.CreateInstance(QDefault, (object)new int[] { 60000, 120000, 0 })!;
        var mD1 = Activator.CreateInstance(MDefault, (object)new int[] { 60000, 120000, 0 })!;
        var qD2 = Activator.CreateInstance(QDefault, (object)new int[] { 120000, 240000, 0 })!;
        var mD2 = Activator.CreateInstance(MDefault, (object)new int[] { 120000, 240000, 0 })!;
        var qD3 = Activator.CreateInstance(QDefault, (object)new int[] { 60000, 0, 0 })!;
        var mD3 = Activator.CreateInstance(MDefault, (object)new int[] { 60000, 0, 0 })!;
        var qD4 = Activator.CreateInstance(QDefault, (object)new int[] { 0, 120000, 0 })!;
        var mD4 = Activator.CreateInstance(MDefault, (object)new int[] { 0, 120000, 0 })!;
        var qD5 = Activator.CreateInstance(QDefault, (object)new int[] { 60000, 120000, 60000 })!;
        var mD5 = Activator.CreateInstance(MDefault, (object)new int[] { 60000, 120000, 60000 })!;

        MethodComparison.Compare("Default.Equals(self)", Contract, qDefEquals, qD1, [qD1], mDefEquals, mD1, [mD1], Show, Show, DefaultState, DefaultState);
        MethodComparison.Compare("Default.Equals(same direction)", Contract, qDefEquals, qD1, [qD2], mDefEquals, mD1, [mD2], Show, Show, DefaultState, DefaultState);
        MethodComparison.Compare("Default.Equals(diff direction)", Contract, qDefEquals, qD1, [qD3], mDefEquals, mD1, [mD3], Show, Show, DefaultState, DefaultState);
        MethodComparison.Compare("Default.Equals(diff index 0)", Contract, qDefEquals, qD1, [qD4], mDefEquals, mD1, [mD4], Show, Show, DefaultState, DefaultState);
        MethodComparison.Compare("Default.Equals(diff last index)", Contract, qDefEquals, qD1, [qD5], mDefEquals, mD1, [mD5], Show, Show, DefaultState, DefaultState);

        var qD_one1 = Activator.CreateInstance(QDefault, (object)new int[] { 1 })!;
        var mD_one1 = Activator.CreateInstance(MDefault, (object)new int[] { 1 })!;
        var qD_one2 = Activator.CreateInstance(QDefault, (object)new int[] { 2 })!;
        var mD_one2 = Activator.CreateInstance(MDefault, (object)new int[] { 2 })!;
        var qD_one0 = Activator.CreateInstance(QDefault, (object)new int[] { 0 })!;
        var mD_one0 = Activator.CreateInstance(MDefault, (object)new int[] { 0 })!;

        MethodComparison.Compare("Default.Equals(1-elem diff 1 vs 2)", Contract, qDefEquals, qD_one1, [qD_one2], mDefEquals, mD_one1, [mD_one2], Show, Show, DefaultState, DefaultState);
        MethodComparison.Compare("Default.Equals(1-elem same 1 vs 1)", Contract, qDefEquals, qD_one1, [qD_one1], mDefEquals, mD_one1, [mD_one1], Show, Show, DefaultState, DefaultState);
        MethodComparison.Compare("Default.Equals(1-elem diff 1 vs 0)", Contract, qDefEquals, qD_one1, [qD_one0], mDefEquals, mD_one1, [mD_one0], Show, Show, DefaultState, DefaultState);

        MethodComparison.Compare("Default.CompareTo(larger)", Contract, qDefCompare, qD1, [qD2], mDefCompare, mD1, [mD2], Show, Show, DefaultState, DefaultState);
        MethodComparison.Compare("Default.CompareTo(smaller)", Contract, qDefCompare, qD2, [qD1], mDefCompare, mD2, [mD1], Show, Show, DefaultState, DefaultState);
        MethodComparison.Compare("Default.CompareTo(equal)", Contract, qDefCompare, qD1, [qD1], mDefCompare, mD1, [mD1], Show, Show, DefaultState, DefaultState);
    }
}
