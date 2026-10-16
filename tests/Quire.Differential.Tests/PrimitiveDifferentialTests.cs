using System.Reflection;
using System.Runtime.CompilerServices;
using Quire.Mpl.Run;
using Xunit;
using static Quire.Differential.Tests.Reflect;

namespace Quire.Differential.Tests;

public class PrimitiveDifferentialTests
{
    private const string Contract = "Mpl.Run.Primitive";
    private const BindingFlags PublicInstance = BindingFlags.Public | BindingFlags.Instance | BindingFlags.DeclaredOnly;

    private static (Primitive q, object m) Targets(bool hasCode = false, bool hasOpcodes = false, bool hasMethod = false, string? foreign = null)
    {
        var q = (Primitive)RuntimeHelpers.GetUninitializedObject(typeof(Primitive));
        var mType = Mathcad.Type("mplrun", "Mpl.Run.Primitive");
        var m = RuntimeHelpers.GetUninitializedObject(mType);
        object? qCode = hasCode ? RuntimeHelpers.GetUninitializedObject(typeof(Code)) : null;
        object? mCode = hasCode ? RuntimeHelpers.GetUninitializedObject(Mathcad.Type("mplrun", "Mpl.Run.Code")) : null;
        object? qOpcodes = hasOpcodes
            ? Array.CreateInstance(typeof(Primitive).GetField("m_opcodes")!.FieldType.GetElementType()!, 0)
            : null;
        object? mOpcodes = hasOpcodes
            ? Array.CreateInstance(mType.GetField("m_opcodes")!.FieldType.GetElementType()!, 0)
            : null;
        MethodInfo? method = hasMethod ? typeof(string).GetMethod("ToString", Type.EmptyTypes) : null;
        Set(q, "m_code", qCode);
        Set(m, "m_code", mCode);
        Set(q, "m_opcodes", qOpcodes);
        Set(m, "m_opcodes", mOpcodes);
        Set(q, "m_method", method);
        Set(m, "m_method", method);
        Set(q, "m_foreign", foreign);
        Set(m, "m_foreign", foreign);
        return (q, m);
    }

    private static string State(object target) => string.Format("code={0};opcodes={1};method={2};foreign={3}",
        Get(target, "m_code") != null, Get(target, "m_opcodes") != null,
        Get(target, "m_method") != null, Get(target, "m_foreign") ?? "null");

    [SkippableFact]
    public void Primitive_kind_predicates()
    {
        var mType = Mathcad.Type("mplrun", "Mpl.Run.Primitive");
        var qCode = typeof(Primitive).GetMethod("IsOpCodePrimitive", PublicInstance, [])!;
        var mCode = mType.GetMethod("IsOpCodePrimitive", PublicInstance, [])!;
        foreach (var (hasCode, hasOpcodes) in new[] { (false, false), (false, true), (true, true) })
        {
            var (qTarget, mTarget) = Targets(hasCode, hasOpcodes);
            MethodComparison.Compare($"IsOpCodePrimitive({hasCode},{hasOpcodes})", Contract,
                qCode, qTarget, [], mCode, mTarget, [], quireState: State, originalState: State);
        }

        var qIsCode = typeof(Primitive).GetMethod("IsCodePrimitive", PublicInstance, [])!;
        var mIsCode = mType.GetMethod("IsCodePrimitive", PublicInstance, [])!;
        foreach (bool hasCode in new[] { false, true })
        {
            var (qTarget, mTarget) = Targets(hasCode);
            MethodComparison.Compare($"IsCodePrimitive({hasCode})", Contract,
                qIsCode, qTarget, [], mIsCode, mTarget, [], quireState: State, originalState: State);
        }

        var qIsMethod = typeof(Primitive).GetMethod("IsMethodPrimitive", PublicInstance, [])!;
        var mIsMethod = mType.GetMethod("IsMethodPrimitive", PublicInstance, [])!;
        foreach (var (hasCode, hasOpcodes, hasMethod) in new[]
        {
            (false, false, false), (false, false, true), (true, false, true), (false, true, true)
        })
        {
            var (qTarget, mTarget) = Targets(hasCode, hasOpcodes, hasMethod);
            MethodComparison.Compare($"IsMethodPrimitive({hasCode},{hasOpcodes},{hasMethod})", Contract,
                qIsMethod, qTarget, [], mIsMethod, mTarget, [], quireState: State, originalState: State);
        }

        var qMethod = typeof(Primitive).GetMethod("IsForeignPrimitive", PublicInstance, [])!;
        var mMethod = mType.GetMethod("IsForeignPrimitive", PublicInstance, [])!;

        foreach (string? foreign in new string?[] { null, "", "[eficore_]Efi.Core::add" })
        {
            var (qTarget, mTarget) = Targets(foreign: foreign);

            MethodComparison.Compare($"IsForeignPrimitive({foreign ?? "null"})", Contract,
                qMethod, qTarget, [], mMethod, mTarget, [],
                quireState: State, originalState: State);
        }
    }
}
