using System.Reflection;
using System.Runtime.CompilerServices;

namespace Quire.Differential.Tests;

/// <summary>Direct reference and state comparisons for the Error getter and setter.</summary>
public class PtcCalculationCoreRequestShowErrorEventArgsMechanicalDifferentialTests
{
    private const string Contract = "Ptc.Calculation.Core.RequestShowErrorEventArgs";
    private static Type QType() => typeof(global::Quire.Calculation.Core.RequestShowErrorEventArgs);
    private static Type MType() => Mathcad.Type("Ptc.Calculation.Core", Contract);
    private static Type QErrorType() => typeof(global::Quire.Calculation.Core.EngineError);
    private static Type MErrorType() => Mathcad.Type("Ptc.Calculation.Core", "Ptc.Calculation.Core.EngineError");
    private static object Error(Type type) => RuntimeHelpers.GetUninitializedObject(type);
    private static void SetError(Type type, object target, object? value) =>
        type.GetProperty("Error")!.GetSetMethod(true)!.Invoke(target, [value]);
    private static string Show(object? value, object? expected) =>
        value == null ? "null" : ReferenceEquals(value, expected) ? "EngineError:expected" : value.GetType().Name;

    [SkippableFact]
    public void Error_get_060000c6()
    {
        object qError = Error(QErrorType());
        object mError = Error(MErrorType());
        object qTarget = Error(QType());
        object mTarget = Error(MType());
        SetError(QType(), qTarget, qError);
        SetError(MType(), mTarget, mError);
        string QState(object target) => Show(QType().GetProperty("Error")!.GetValue(target), qError);
        string MState(object target) => Show(MType().GetProperty("Error")!.GetValue(target), mError);
        MethodComparison.Compare("getter returns the stored EngineError reference", Contract,
            QType().GetProperty("Error")!.GetMethod!, qTarget, [],
            MType().GetProperty("Error")!.GetMethod!, mTarget, [],
            value => Show(value, qError), value => Show(value, mError), QState, MState);
        SetError(QType(), qTarget, null);
        SetError(MType(), mTarget, null);
        MethodComparison.Compare("getter returns null", Contract,
            QType().GetProperty("Error")!.GetMethod!, qTarget, [],
            MType().GetProperty("Error")!.GetMethod!, mTarget, [],
            value => Show(value, qError), value => Show(value, mError), QState, MState);
    }

    [SkippableFact]
    public void Error_set_060000c7()
    {
        object qError = Error(QErrorType());
        object mError = Error(MErrorType());
        object qTarget = Error(QType());
        object mTarget = Error(MType());
        MethodInfo qSetter = QType().GetProperty("Error")!.GetSetMethod(true)!;
        MethodInfo mSetter = MType().GetProperty("Error")!.GetSetMethod(true)!;
        string QState(object target) => Show(QType().GetProperty("Error")!.GetValue(target), qError);
        string MState(object target) => Show(MType().GetProperty("Error")!.GetValue(target), mError);

        MethodComparison.Compare("setter stores the exact EngineError reference", Contract,
            qSetter, qTarget, [qError], mSetter, mTarget, [mError],
            value => Show(value, qError), value => Show(value, mError), QState, MState);
        MethodComparison.Compare("setter stores null", Contract,
            qSetter, qTarget, [null], mSetter, mTarget, [null],
            value => Show(value, qError), value => Show(value, mError), QState, MState);
    }
}

public class PtcCalculationCoreRequestShowErrorEventArgsConstructorTests
{
    [SkippableFact]
    public void ctor_060000c5()
    {
        Type quireType = typeof(global::Quire.Calculation.Core.RequestShowErrorEventArgs);
        Type mathcadType = Mathcad.Type("Ptc.Calculation.Core", "Ptc.Calculation.Core.RequestShowErrorEventArgs");
        Type mathcadErrorType = Mathcad.Type("Ptc.Calculation.Core", "Ptc.Calculation.Core.EngineError");
        ConstructorInfo quire = quireType.GetConstructor([typeof(global::Quire.Calculation.Core.EngineError)])!;
        MethodBase mathcad = mathcadType.Module.ResolveMethod(0x060000c5)!;

        object quireError = RuntimeHelpers.GetUninitializedObject(typeof(global::Quire.Calculation.Core.EngineError));
        object mathcadError = RuntimeHelpers.GetUninitializedObject(mathcadErrorType);
        string QuireShow(object? value) => value switch
        {
            global::Quire.Calculation.Core.EngineError => "EngineError",
            global::Quire.Calculation.Core.RequestShowErrorEventArgs args =>
                $"RequestShowErrorEventArgs:stored={ReferenceEquals(args.Error, quireError)}",
            null => "null",
            _ => value.GetType().Name,
        };
        string MathcadShow(object? value)
        {
            if (value == null) return "null";
            if (value.GetType() == mathcadErrorType) return "EngineError";
            if (value.GetType() == mathcadType)
            {
                object? error = mathcadType.GetProperty("Error")!.GetValue(value);
                return $"RequestShowErrorEventArgs:stored={ReferenceEquals(error, mathcadError)}";
            }
            return value.GetType().Name;
        }

        MethodComparison.Compare("constructor stores non-null EngineError reference", "Ptc.Calculation.Core.RequestShowErrorEventArgs",
            quire, null, [quireError], mathcad, null, [mathcadError], QuireShow, MathcadShow);
        MethodComparison.Compare("constructor stores null EngineError", "Ptc.Calculation.Core.RequestShowErrorEventArgs",
            quire, null, [null], mathcad, null, [null], QuireShow, MathcadShow);
    }
}
