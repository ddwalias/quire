using System.Reflection;
using static Quire.Differential.Tests.MplRunSides;

namespace Quire.Differential.Tests;

public class PrimitivePredicateDifferentialTests
{
    private static readonly MethodInfo SampleMethod = typeof(string).GetMethod(nameof(string.Contains), [typeof(string)])!;

    [SkippableFact]
    public void IsOpCodePrimitive_checks_code_then_opcode_array_nullness()
    {
        Compare("IsOpCodePrimitive", "null code and empty opcode array", qOpcodes: EmptyOpcodes(), mOpcodes: EmptyOpcodes());
        Compare("IsOpCodePrimitive", "null code and null opcode array");
        Compare("IsOpCodePrimitive", "code suppresses non-null opcode array",
            qCode: New(Q("Mpl.Run.Code"), []), mCode: New(M("Mpl.Run.Code"), []),
            qOpcodes: EmptyOpcodes(), mOpcodes: EmptyOpcodes());
    }

    [SkippableFact]
    public void IsCodePrimitive_checks_only_code_nullness()
    {
        Compare("IsCodePrimitive", "null code", qOpcodes: EmptyOpcodes(), mOpcodes: EmptyOpcodes(),
            qMethod: SampleMethod, mMethod: SampleMethod);
        Compare("IsCodePrimitive", "non-null code despite other fields",
            qCode: New(Q("Mpl.Run.Code"), []), mCode: New(M("Mpl.Run.Code"), []),
            qMethod: SampleMethod, mMethod: SampleMethod, qForeign: "foreign", mForeign: "foreign");
    }

    [SkippableFact]
    public void IsMethodPrimitive_requires_method_and_both_other_fields_null()
    {
        Compare("IsMethodPrimitive", "non-null method and null code and opcodes",
            qMethod: SampleMethod, mMethod: SampleMethod);
        Compare("IsMethodPrimitive", "code suppresses method classification",
            qCode: New(Q("Mpl.Run.Code"), []), mCode: New(M("Mpl.Run.Code"), []),
            qMethod: SampleMethod, mMethod: SampleMethod);
        Compare("IsMethodPrimitive", "opcode array suppresses method classification",
            qOpcodes: EmptyOpcodes(), mOpcodes: EmptyOpcodes(), qMethod: SampleMethod, mMethod: SampleMethod);
        Compare("IsMethodPrimitive", "null method is not method primitive");
    }

    [SkippableFact]
    public void IsForeignPrimitive_checks_nullness_not_empty_string()
    {
        Compare("IsForeignPrimitive", "empty foreign name", qForeign: "", mForeign: "");
        Compare("IsForeignPrimitive", "null foreign name");
    }

    private static void Compare(string name, string caseId, object? qCode = null, object? mCode = null,
        object? qOpcodes = null, object? mOpcodes = null, object? qMethod = null, object? mMethod = null,
        object? qForeign = null, object? mForeign = null)
    {
        var qTarget = New(Q("Mpl.Run.Primitive"), ["String"], "predicate")!;
        var mTarget = New(M("Mpl.Run.Primitive"), ["String"], "predicate")!;
        Set(qTarget, "m_code", qCode);
        Set(mTarget, "m_code", mCode);
        Set(qTarget, "m_opcodes", qOpcodes);
        Set(mTarget, "m_opcodes", mOpcodes);
        Set(qTarget, "m_method", qMethod);
        Set(mTarget, "m_method", mMethod);
        Set(qTarget, "m_foreign", qForeign);
        Set(mTarget, "m_foreign", mForeign);

        var qMethodInfo = Q("Mpl.Run.Primitive").GetMethod(name, All)!;
        var mMethodInfo = M("Mpl.Run.Primitive").GetMethod(name, All)!;
        MethodComparison.Compare(caseId, "Mpl.Run.Primitive", qMethodInfo, qTarget, [], mMethodInfo, mTarget, [],
            quireState: State, originalState: State);
    }

    private static string State(object target)
    {
        string Nullness(string name) => Get(target, name) == null ? "null" : "set";
        return "code=" + Nullness("m_code") + ";opcodes=" + Nullness("m_opcodes") +
            ";method=" + Nullness("m_method") + ";foreign=" + (Get(target, "m_foreign") ?? "<null>");
    }

    private static Array EmptyOpcodes() => Array.CreateInstance(
        typeof(global::Quire.Mpl.Run.Primitive).GetField(nameof(global::Quire.Mpl.Run.Primitive.m_opcodes))!.FieldType.GetElementType()!, 0);
}
