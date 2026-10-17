using System.Reflection;
using static Quire.Differential.Tests.MplRunSides;

namespace Quire.Differential.Tests;

public class PrimitiveReferenceDifferentialTests
{
    private const string Contract = "Mpl.Run.Primitive";
    private static readonly System.Type[] Parameters = [typeof(object), typeof(object)];

    [SkippableFact]
    public void Or_ref_selects_the_first_non_null_reference()
    {
        Compare("or_ref", "both null", null, null, null, null);
        Compare("or_ref", "first null returns second", null, new object(), null, new object());
        Compare("or_ref", "second null returns first", new object(), null, new object(), null);
        Compare("or_ref", "two values return first", new object(), new object(), new object(), new object());
    }

    [SkippableFact]
    public void And_ref_selects_the_second_reference_only_when_first_is_non_null()
    {
        Compare("and_ref", "both null", null, null, null, null);
        Compare("and_ref", "first null returns null", null, new object(), null, new object());
        Compare("and_ref", "second null returns null", new object(), null, new object(), null);
        Compare("and_ref", "two values return second", new object(), new object(), new object(), new object());
    }

    private static void Compare(string name, string caseId, object? qX, object? qY, object? mX, object? mY)
    {
        var qMethod = typeof(Mpl.Run.Primitive).GetMethod(name, BindingFlags.Public | BindingFlags.Static,
            null, Parameters, null)!;
        var mMethod = M("Mpl.Run.Primitive").GetMethod(name, BindingFlags.Public | BindingFlags.Static,
            null, Parameters, null)!;
        string QShow(object? value) => Describe(value, qX, qY);
        string MShow(object? value) => Describe(value, mX, mY);
        MethodComparison.Compare(caseId, Contract, qMethod, null, [qX, qY], mMethod, null, [mX, mY], QShow, MShow);
    }

    private static string Describe(object? value, object? first, object? second)
    {
        if (value == null) return "null";
        if (ReferenceEquals(value, first)) return "first";
        if (ReferenceEquals(value, second)) return "second";
        return "other";
    }
}
