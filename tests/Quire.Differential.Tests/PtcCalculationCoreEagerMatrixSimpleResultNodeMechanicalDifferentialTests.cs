using System.Collections;
using System.Reflection;

namespace Quire.Differential.Tests;

/// <summary>
/// Direct comparisons of the <c>Ptc.Calculation.Core.EagerMatrixSimpleResultNode</c> members that MemberGen reconstructed mechanically
/// (docs/mirror/evidence/Ptc.Calculation.Core.EagerMatrixSimpleResultNode.md): each compares one member on generated inputs (Mechanical).
/// </summary>
public class PtcCalculationCoreEagerMatrixSimpleResultNodeMechanicalDifferentialTests
{
    private const BindingFlags DeclaredInstance = BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.DeclaredOnly;

    private static System.Type MathcadType => Mathcad.Type("Ptc.Calculation.Core", "Ptc.Calculation.Core.EagerMatrixSimpleResultNode");

    private static string Render(object? value)
    {
        if (value is int number) return number.ToString(System.Globalization.CultureInfo.InvariantCulture);
        if (value == null) return "null";
        var children = ((IEnumerable)Reflect.Get(value, "_children")!).Cast<object?>();
        return $"columns={Reflect.Get(value, "Columns")};children={string.Join(",", children.Select(child => child == null ? "null" : child.GetType().Name))}";
    }

    [SkippableFact]
    public void Accept_060001d0() => Mechanical.Compare(typeof(global::Quire.Calculation.Core.EagerMatrixSimpleResultNode), 0x060001d0);

    [SkippableFact]
    public void Count_get_060001c6() => Mechanical.Compare(typeof(global::Quire.Calculation.Core.EagerMatrixSimpleResultNode), 0x060001c6);

    [SkippableFact]
    public void Columns_get_060001c8() => Mechanical.Compare(typeof(global::Quire.Calculation.Core.EagerMatrixSimpleResultNode), 0x060001c8);

    [SkippableFact]
    public void Columns_set_060001c9() => Mechanical.Compare(typeof(global::Quire.Calculation.Core.EagerMatrixSimpleResultNode), 0x060001c9);

    [SkippableFact]
    public void SetElementAt_060001ce() => Mechanical.Compare(typeof(global::Quire.Calculation.Core.EagerMatrixSimpleResultNode), 0x060001ce);

    [SkippableFact]
    public void ToString_060001cf() => Mechanical.Compare(typeof(global::Quire.Calculation.Core.EagerMatrixSimpleResultNode), 0x060001cf);

    [SkippableFact]
    public void Rows_get_060001ca() => Mechanical.Compare(typeof(global::Quire.Calculation.Core.EagerMatrixSimpleResultNode), 0x060001ca);

    [SkippableFact]
    public void Item_get_060001cb() => Mechanical.Compare(typeof(global::Quire.Calculation.Core.EagerMatrixSimpleResultNode), 0x060001cb);

    [SkippableFact]
    public void Item_set_060001cc() => Mechanical.Compare(typeof(global::Quire.Calculation.Core.EagerMatrixSimpleResultNode), 0x060001cc);

    [SkippableFact]
    public void ctor_060001c4()
    {
        var quireCtor = typeof(global::Quire.Calculation.Core.EagerMatrixSimpleResultNode)
            .GetConstructor(DeclaredInstance, null, [typeof(int), typeof(int)], null)!;
        var mathcadCtor = MathcadType.GetConstructor(DeclaredInstance, null, [typeof(int), typeof(int)], null)!;
        foreach (var (rows, columns) in new[] { (0, 0), (0, 4), (1, 2), (2, 3), (-1, 1) })
        {
            MethodComparison.Compare($"new({rows},{columns})", "Ptc.Calculation.Core.EagerMatrixSimpleResultNode",
                quireCtor, null, [rows, columns], mathcadCtor, null, [rows, columns], Render, Render);
        }
    }

}
