using System.Collections;
using System.Reflection;
using System.Runtime.CompilerServices;

namespace Quire.Differential.Tests;

/// <summary>Direct comparisons for EngineErrorCollection construction from an existing sequence plus one error.</summary>
public class PtcCalculationCoreEngineErrorCollectionMechanicalDifferentialTests
{
    private const string Contract = "Ptc.Calculation.Core.EngineErrorCollection";
    private const int Token = 0x06000050;
    private const BindingFlags PublicInstance = BindingFlags.Instance | BindingFlags.Public;

    private static Type QType() => typeof(global::Quire.Calculation.Core.EngineErrorCollection);
    private static Type MType() => Mathcad.Type("Ptc.Calculation.Core", Contract);
    private static Type QErrorType() => typeof(global::Quire.Calculation.Core.EngineError);
    private static Type MErrorType() => Mathcad.Type("Ptc.Calculation.Core", "Ptc.Calculation.Core.EngineError");

    private static object Error(Type type) => RuntimeHelpers.GetUninitializedObject(type);

    private static Array Errors(Type errorType, params object?[] values)
    {
        Array errors = Array.CreateInstance(errorType, values.Length);
        for (int i = 0; i < values.Length; i++) errors.SetValue(values[i], i);
        return errors;
    }

    private static string Show(object? value)
    {
        if (value == null) return "null";
        if (value.GetType().Name == "EngineError") return "EngineError";
        if (value is IEnumerable sequence)
        {
            List<string> items = [];
            foreach (object? item in sequence) items.Add(Show(item));
            return "[" + string.Join(",", items) + "]";
        }
        return value.GetType().Name;
    }

    [SkippableFact]
    public void ctor_06000050()
    {
        ConstructorInfo qCtor = QType().GetConstructor(PublicInstance, null,
            [typeof(IEnumerable<global::Quire.Calculation.Core.EngineError>), QErrorType()], null)!;
        ConstructorInfo mCtor = (ConstructorInfo)MType().Module.ResolveMethod(Token)!;

        object qFirst = Error(QErrorType());
        object qSecond = Error(QErrorType());
        object mFirst = Error(MErrorType());
        object mSecond = Error(MErrorType());

        MethodComparison.Compare("empty sequence appends the error", Contract, qCtor, null,
            [Errors(QErrorType()), qFirst], mCtor, null, [Errors(MErrorType()), mFirst], Show, Show);
        MethodComparison.Compare("copies multiple errors and appends one", Contract, qCtor, null,
            [Errors(QErrorType(), qFirst, qSecond), Error(QErrorType())],
            mCtor, null, [Errors(MErrorType(), mFirst, mSecond), Error(MErrorType())], Show, Show);
        MethodComparison.Compare("preserves null items and appends null", Contract, qCtor, null,
            [Errors(QErrorType(), new object?[] { null }), null], mCtor, null, [Errors(MErrorType(), new object?[] { null }), null], Show, Show);
        MethodComparison.Compare("null sequence throws", Contract, qCtor, null,
            [null, qFirst], mCtor, null, [null, mFirst], Show, Show);
    }

    [SkippableFact]
    public void ctor_0600004f() => Mechanical.Compare(QType(), 0x0600004f);
}
