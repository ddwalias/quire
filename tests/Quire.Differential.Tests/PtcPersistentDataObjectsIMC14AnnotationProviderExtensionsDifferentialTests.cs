using System.Collections;
using System.Reflection;
using Quire.PersistentDataObjects;

namespace Quire.Differential.Tests;

public class PtcPersistentDataObjectsIMC14AnnotationProviderExtensionsDifferentialTests
{
    private const string Contract = "Ptc.PersistentDataObjects.IMC14AnnotationProviderExtensions";

    [SkippableFact]
    public void AddMC14ConversionLossAnnotations_single_matches_PtcCore()
    {
        Mathcad.Require();
        Type qExtension = typeof(IMC14AnnotationProviderExtensions);
        Type mExtension = Mathcad.Type("Ptc.Core", "Ptc.PersistentDataObjects.IMC14AnnotationProviderExtensions");
        Type qProvider = typeof(IMC14AnnotationProvider);
        Type mProvider = Mathcad.Type("Ptc.Core", "Ptc.PersistentDataObjects.IMC14AnnotationProvider");
        Type qEnum = typeof(MC14ConversionAnnotationType);
        Type mEnum = Mathcad.Type("Ptc.Core", "Ptc.PersistentDataObjects.MC14ConversionAnnotationType");
        MethodInfo qMethod = qExtension.GetMethod("AddMC14ConversionLossAnnotations", [qProvider, qEnum])!;
        MethodInfo mMethod = mExtension.GetMethod("AddMC14ConversionLossAnnotations", [mProvider, mEnum])!;

        foreach (string[]? initial in new string[]?[] { null, ["Area"], ["Unsupported"] })
        {
            object qTarget = CreateProvider("Quire.PersistentDataObjects.textType", initial);
            object mTarget = CreateProvider("Ptc.PersistentDataObjects.textType", initial);
            string initialName = initial is null ? "null" : string.Join(',', initial);
            MethodComparison.Compare($"single/{initialName}", Contract,
                qMethod, null, [qTarget, Enum.Parse(qEnum, "Area")],
                mMethod, null, [mTarget, Enum.Parse(mEnum, "Area")],
                ShowInput, ShowInput);
        }
    }

    [SkippableFact]
    public void AddMC14ConversionLossAnnotations_many_matches_PtcCore()
    {
        Mathcad.Require();
        Type qExtension = typeof(IMC14AnnotationProviderExtensions);
        Type mExtension = Mathcad.Type("Ptc.Core", "Ptc.PersistentDataObjects.IMC14AnnotationProviderExtensions");
        Type qProvider = typeof(IMC14AnnotationProvider);
        Type mProvider = Mathcad.Type("Ptc.Core", "Ptc.PersistentDataObjects.IMC14AnnotationProvider");
        Type qEnum = typeof(MC14ConversionAnnotationType);
        Type mEnum = Mathcad.Type("Ptc.Core", "Ptc.PersistentDataObjects.MC14ConversionAnnotationType");
        Type qEnums = typeof(IEnumerable<>).MakeGenericType(qEnum);
        Type mEnums = typeof(IEnumerable<>).MakeGenericType(mEnum);
        MethodInfo qMethod = qExtension.GetMethod("AddMC14ConversionLossAnnotations", [qProvider, qEnums])!;
        MethodInfo mMethod = mExtension.GetMethod("AddMC14ConversionLossAnnotations", [mProvider, mEnums])!;
        object qTarget = CreateProvider("Quire.PersistentDataObjects.textType", null);
        object mTarget = CreateProvider("Ptc.PersistentDataObjects.textType", null);
        Array qValues = EnumValues(qEnum, "Area", "Area", "Unsupported", "ChangedRegion");
        Array mValues = EnumValues(mEnum, "Area", "Area", "Unsupported", "ChangedRegion");

        MethodComparison.Compare("many/null-provider-list/duplicates-preserve-first-order", Contract,
            qMethod, null, [qTarget, qValues], mMethod, null, [mTarget, mValues], ShowInput, ShowInput);
    }

    [SkippableFact]
    public void ConvertMC14ConversionLossAnnotationsToStringArray_is_blocked_by_Lane_5_helper()
    {
        Skip.If(true, "Ptc implementation calls Quire.EnumerableExtensions.IsNullOrEmpty, a Lane 5 owned type; direct verification waits for that dependency.");
    }

    private static object CreateProvider(string typeName, string[]? initial)
    {
        Type type = typeName.StartsWith("Ptc.", StringComparison.Ordinal)
            ? Mathcad.Type("Ptc.Core", typeName)
            : typeof(textType);
        object provider = Activator.CreateInstance(type)!;
        type.GetProperty("MC14ConversionLossAnnotationsLocal")!.SetValue(provider, initial is null ? null : new List<string>(initial));
        return provider;
    }

    private static Array EnumValues(Type enumType, params string[] names)
    {
        Array values = Array.CreateInstance(enumType, names.Length);
        for (int i = 0; i < names.Length; i++)
            values.SetValue(Enum.Parse(enumType, names[i]), i);
        return values;
    }

    private static string ShowInput(object? value)
    {
        if (value is null)
            return "null";
        if (value is string text)
            return text;
        if (value is IEnumerable values)
            return string.Join(",", values.Cast<object?>().Select(item => item?.ToString() ?? "null"));
        if (value.GetType().GetProperty("MC14ConversionLossAnnotationsLocal") is PropertyInfo property)
        {
            object? annotations = property.GetValue(value);
            return annotations is null ? "null-provider-list" : string.Join(",", (IEnumerable<string>)annotations);
        }
        return value.ToString()!;
    }
}
