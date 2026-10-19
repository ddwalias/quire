using System.Reflection;
using System.Runtime.CompilerServices;

namespace Quire.Differential.Tests;

public class PtcCalculationCoreEngineErrorExtensionsMechanicalDifferentialTests
{
    [SkippableFact]
    public void IsPlaceholderError_06000040()
    {
        Type quireErrorType = typeof(global::Quire.Calculation.Core.EngineError);
        Type mathcadErrorType = Mathcad.Type("Ptc.Calculation.Core", "Ptc.Calculation.Core.EngineError");
        Type mathcadExtensions = Mathcad.Type("Ptc.Calculation.Core", "Ptc.Calculation.Core.EngineErrorExtensions");
        MethodInfo quire = typeof(global::Quire.Calculation.Core.EngineErrorExtensions).GetMethod("IsPlaceholderError")!;
        MethodBase mathcad = mathcadExtensions.Module.ResolveMethod(0x06000040)!;

        object Error(Type type, string code)
        {
            object error = RuntimeHelpers.GetUninitializedObject(type);
            type.GetField("_code", BindingFlags.Instance | BindingFlags.NonPublic)!.SetValue(error, code);
            return error;
        }

        string Show(object? value)
        {
            if (value == null) return "null";
            if (value is bool boolean) return boolean ? "true" : "false";
            string? code = (string?)value.GetType().GetProperty("ErrorCode")!.GetValue(value);
            return $"EngineError:{code ?? "null"}";
        }

        MethodComparison.Compare("null error", "Ptc.Calculation.Core.EngineErrorExtensions",
            quire, null, [null], mathcad, null, [null], Show, Show);
        MethodComparison.Compare("placeholder error code", "Ptc.Calculation.Core.EngineErrorExtensions",
            quire, null, [Error(quireErrorType, "bad%_placeholder")], mathcad, null,
            [Error(mathcadErrorType, "bad%_placeholder")], Show, Show);
        MethodComparison.Compare("different error code", "Ptc.Calculation.Core.EngineErrorExtensions",
            quire, null, [Error(quireErrorType, "other")], mathcad, null,
            [Error(mathcadErrorType, "other")], Show, Show);
    }
}
