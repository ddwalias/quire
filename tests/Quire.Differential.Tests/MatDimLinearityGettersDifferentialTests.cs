using System.Reflection;
using static Quire.Differential.Tests.Reflect;

namespace Quire.Differential.Tests;

public class MatDimLinearityGettersDifferentialTests
{
    private const string Contract = "Mcd.MatDim";
    private static readonly Guid Mvid = new("17a0933b-4988-4c18-963d-563998321cc8");
    private const BindingFlags DeclaredInstance = BindingFlags.Public | BindingFlags.NonPublic |
        BindingFlags.Instance | BindingFlags.DeclaredOnly;
    private static readonly string[] FieldNames = ["m_dim", "m_rowdim", "m_coldim"];

    private sealed record Snapshot(int Rows, int Cols, Dictionary<string, object?> Fields,
        Dictionary<string, object?[]> ArrayValues);

    private static Type Nested(Side side, string name) => side.T($"Mcd.MatDim+{name}");

    private static Array Dims(Side side, params string[] names)
    {
        var result = Array.CreateInstance(side.Dim, names.Length);
        for (int i = 0; i < names.Length; i++) result.SetValue(side.D(names[i]), i);
        return result;
    }

    private static object Uniform(Side side) => Call(Nested(side, "uniform_dim"), "make", side.D("T"), 2, 3)!;

    private static object Row(Side side) => Call(Nested(side, "r_dim"), "make", Dims(side, "L", "T"), 3)!;

    private static object Column(Side side) => Call(Nested(side, "c_dim"), "make", Dims(side, "L", "N"), 2)!;

    private static object RowColumn(Side side) => Call(Nested(side, "rc_dim"), "make_rc",
        Dims(side, "L", "T"), Dims(side, "N", "v"))!;

    private static MethodInfo Method(Side side, string kind) =>
        Nested(side, kind).GetMethod("get_IsLinear", DeclaredInstance)
        ?? throw new HarnessException($"Missing {side.Name} MatDim.{kind}.get_IsLinear()");

    private static object? Field(object target, string name) =>
        target.GetType().GetField(name, DeclaredInstance)?.GetValue(target);

    private static Snapshot Capture(object target)
    {
        var fields = FieldNames.ToDictionary(name => name, name => Field(target, name));
        var arrays = fields.ToDictionary(pair => pair.Key, pair => pair.Value is Array values
            ? values.Cast<object?>().ToArray()
            : []);
        return new((int)Get(target, "Rows")!, (int)Get(target, "Cols")!, fields, arrays);
    }

    private static string Ref(object? before, object? after) => ReferenceEquals(before, after) ? "same" : "changed";

    private static string FieldState(Side side, Snapshot before, object target, string name)
    {
        object? prior = before.Fields[name], current = Field(target, name);
        string reference = Ref(prior, current);
        if (prior is not Array && current is not Array)
            return $"{name}-ref={reference}:{side.DimText(prior)}->{side.DimText(current)}";

        var currentValues = current is Array array ? array.Cast<object?>().ToArray() : [];
        var priorValues = before.ArrayValues[name];
        int count = Math.Max(priorValues.Length, currentValues.Length);
        string values = string.Join(",", Enumerable.Range(0, count).Select(index =>
        {
            object? oldValue = index < priorValues.Length ? priorValues[index] : null;
            object? newValue = index < currentValues.Length ? currentValues[index] : null;
            return $"{index}:{side.DimText(oldValue)}->{side.DimText(newValue)}:{Ref(oldValue, newValue)}";
        }));
        return $"{name}-ref={reference}:[{values}]";
    }

    private static string State(Side side, Snapshot before, object target) =>
        $"{target.GetType().Name}:{Get(target, "Rows")}x{Get(target, "Cols")}:" +
        string.Join(":", FieldNames.Select(name => FieldState(side, before, target, name)));

    private static string Result(object? result, object target) =>
        ReferenceEquals(result, target) ? "same receiver" : result == null ? "null" : MplRunSides.Describe(result);

    private static void Compare(string kind, int token, object qTarget, object mTarget)
    {
        MethodInfo qMethod = Method(Side.Quire, kind), mMethod = Method(Side.Mathcad, kind);
        Assert.Equal(token, mMethod.MetadataToken);
        Assert.Equal(Mvid, mMethod.Module.ModuleVersionId);
        Snapshot qBefore = Capture(qTarget), mBefore = Capture(mTarget);
        MethodComparison.Compare($"{kind}.get_IsLinear returns its receiver", Contract,
            qMethod, qTarget, [], mMethod, mTarget, [],
            result => Result(result, qTarget), result => Result(result, mTarget),
            target => State(Side.Quire, qBefore, target), target => State(Side.Mathcad, mBefore, target));
    }

    [SkippableFact]
    public void IsLinear_overrides_return_the_exact_receiver_without_changing_dimensions()
    {
        Runtime.Sync();
        Compare("uniform_dim", 0x060007a1, Uniform(Side.Quire), Uniform(Side.Mathcad));
        Compare("rc_dim", 0x060007b7, RowColumn(Side.Quire), RowColumn(Side.Mathcad));
        Compare("r_dim", 0x060007c7, Row(Side.Quire), Row(Side.Mathcad));
        Compare("c_dim", 0x060007d9, Column(Side.Quire), Column(Side.Mathcad));
    }
}
