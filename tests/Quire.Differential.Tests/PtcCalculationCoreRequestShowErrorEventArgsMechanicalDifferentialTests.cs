using System.Reflection;
using System.Runtime.CompilerServices;

namespace Quire.Differential.Tests;

/// <summary>
/// Direct comparisons of the <c>Ptc.Calculation.Core.RequestShowErrorEventArgs</c> members that MemberGen reconstructed mechanically
/// (docs/mirror/evidence/Ptc.Calculation.Core.RequestShowErrorEventArgs.md): each compares one member on generated inputs (Mechanical).
/// </summary>
public class PtcCalculationCoreRequestShowErrorEventArgsMechanicalDifferentialTests
{
    [SkippableFact]
    public void Error_get_060000c6() => Mechanical.Compare(typeof(global::Quire.Calculation.Core.RequestShowErrorEventArgs), 0x060000c6);

    [SkippableFact]
    public void Error_set_060000c7() => Mechanical.Compare(typeof(global::Quire.Calculation.Core.RequestShowErrorEventArgs), 0x060000c7);
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
