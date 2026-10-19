namespace Quire.Differential.Tests;

/// <summary>
/// Direct comparisons for <c>Ptc.Calculation.Core.EngineResult</c> (docs/mirror/evidence/Ptc.Calculation.Core.EngineResult.md).
/// All members use artifact-bound MethodComparison calls with explicit field renderers.
/// </summary>
public class PtcCalculationCoreEngineResultMechanicalDifferentialTests
{
    [SkippableFact]
    public void ResultNode_get_06000058() => CompareAccessor(0x06000058);

    [SkippableFact]
    public void ResultNode_set_06000059() => CompareAccessor(0x06000059);

    [SkippableFact]
    public void Errors_get_0600005a() => CompareAccessor(0x0600005a);

    [SkippableFact]
    public void Errors_set_0600005b() => CompareAccessor(0x0600005b);

    [SkippableFact]
    public void Warnings_get_0600005c() => CompareAccessor(0x0600005c);

    [SkippableFact]
    public void Warnings_set_0600005d() => CompareAccessor(0x0600005d);

    [SkippableFact]
    public void SynchronizationStatus_get_0600005e() => CompareAccessor(0x0600005e);

    [SkippableFact]
    public void SynchronizationStatus_set_0600005f() => CompareAccessor(0x0600005f);

    [SkippableFact]
    public void SerializationTag_get_06000062() => CompareAccessor(0x06000062);

    [SkippableFact]
    public void SerializationTag_set_06000063() => CompareAccessor(0x06000063);

    [SkippableFact]
    public void ctor_06000051() => CompareConstructor(0x06000051, typeof(global::Quire.Calculation.Core.SynchronizationStatus), new object?[] { (global::Quire.Calculation.Core.SynchronizationStatus)0 });

    [SkippableFact]
    public void ctor_06000052() => CompareConstructor(0x06000052, typeof(global::Quire.Calculation.Core.IResultNode), new object?[] { null });

    [SkippableFact]
    public void ctor_06000056() => CompareConstructor(0x06000056, typeof(IEnumerable<global::Quire.Calculation.Core.EngineRedefInfo>), new object?[] { Array.Empty<global::Quire.Calculation.Core.EngineRedefInfo>() });

    [SkippableFact]
    public void ctor_06000054() => CompareResultAndErrorsConstructor();

    private static void CompareConstructor(int token, Type secondParameterType, object?[] secondArguments)
    {
        var original = (System.Reflection.ConstructorInfo)Mathcad.Load("Ptc.Calculation.Core").ManifestModule.ResolveMethod(token)!;
        var originalType = original.DeclaringType!;
        var originalInput = Activator.CreateInstance(originalType)!;
        var originalStatus = originalType.GetProperty("SynchronizationStatus")!;
        originalStatus.SetValue(originalInput, Enum.ToObject(originalStatus.PropertyType, 1));
        originalType.GetProperty("SerializationTag")!.SetValue(originalInput, "source-tag");

        var quireInput = new global::Quire.Calculation.Core.EngineResult
        {
            SynchronizationStatus = (global::Quire.Calculation.Core.SynchronizationStatus)1,
            SerializationTag = "source-tag"
        };
        var quireConstructor = typeof(global::Quire.Calculation.Core.EngineResult).GetConstructor(
            new[] { typeof(global::Quire.Calculation.Core.EngineResult), secondParameterType })!;
        var quireArguments = new[] { (object)quireInput, secondArguments[0] };
        var originalArguments = new[] { originalInput, secondArguments[0] == null ? null : ConvertWarnings(secondArguments[0], original.GetParameters()[1].ParameterType) };
        MethodComparison.Compare(
            $"EngineResult {token:x8} with null result node",
            "Ptc.Calculation.Core.EngineResult",
            quireConstructor, null, quireArguments,
            original, null, originalArguments,
            ShowEngineResultValue, ShowEngineResultValue);
    }

    private static object? ConvertWarnings(object? value, Type originalParameterType)
    {
        if (value is not Array source)
            return value;
        Type elementType = originalParameterType.IsArray
            ? originalParameterType.GetElementType()!
            : originalParameterType.GetGenericArguments()[0];
        return Array.CreateInstance(elementType, source.Length);
    }

    private static string ShowEngineResultValue(object? value)
    {
        if (value == null) return "null";
        var type = value.GetType();
        if (type.IsEnum) return Convert.ToInt64(value, System.Globalization.CultureInfo.InvariantCulture).ToString(System.Globalization.CultureInfo.InvariantCulture);
        if (type.Name == "EngineResult")
        {
            string Read(string name) => ShowEngineResultValue(type.GetProperty(name)!.GetValue(value));
            return $"EngineResult(ResultNode={Read("ResultNode")}; Errors={Read("Errors")}; Warnings={Read("Warnings")}; Sync={Read("SynchronizationStatus")}; Tag={Read("SerializationTag")})";
        }
        if (value is Array array) return $"{type.GetElementType()!.Name}[{array.Length}]";
        if (value is string text) return $"string:{text}";
        if (type.GetProperty("Count")?.GetValue(value) is int count) return $"{type.Name}[{count}]";
        return type.Name;
    }
    private static void CompareAccessor(int token)
    {
        const System.Reflection.BindingFlags flags = System.Reflection.BindingFlags.Public
            | System.Reflection.BindingFlags.NonPublic
            | System.Reflection.BindingFlags.Instance
            | System.Reflection.BindingFlags.Static
            | System.Reflection.BindingFlags.DeclaredOnly;
        var original = (System.Reflection.MethodInfo)Mathcad.Load("Ptc.Calculation.Core").ManifestModule.ResolveMethod(token)!;
        var parameters = original.GetParameters();
        var quire = typeof(global::Quire.Calculation.Core.EngineResult).GetMethods(flags)
            .Single(method => method.Name == original.Name && method.GetParameters().Length == parameters.Length);
        var quireArguments = new object?[parameters.Length];
        var originalArguments = new object?[parameters.Length];
        if (parameters.Length != 0)
        {
            Type quireParameter = quire.GetParameters()[0].ParameterType;
            Type originalParameter = parameters[0].ParameterType;
            if (originalParameter.IsEnum)
            {
                quireArguments[0] = Enum.ToObject(quireParameter, 1);
                originalArguments[0] = Enum.ToObject(originalParameter, 1);
            }
            else if (originalParameter == typeof(string))
            {
                quireArguments[0] = "property-value";
                originalArguments[0] = "property-value";
            }
            else if (originalParameter.IsArray)
            {
                quireArguments[0] = Array.CreateInstance(quireParameter.GetElementType()!, 0);
                originalArguments[0] = Array.CreateInstance(originalParameter.GetElementType()!, 0);
            }
        }
        object quireTarget = default(global::Quire.Calculation.Core.EngineResult);
        object originalTarget = Activator.CreateInstance(original.DeclaringType!)!;
        MethodComparison.Compare(
            $"EngineResult accessor {token:x8}",
            "Ptc.Calculation.Core.EngineResult",
            quire, quireTarget, quireArguments,
            original, originalTarget, originalArguments,
            ShowEngineResultValue, ShowEngineResultValue,
            ShowEngineResultValue, ShowEngineResultValue);
    }

    private static void CompareResultAndErrorsConstructor()
    {
        const int status = 1;
        var original = (System.Reflection.ConstructorInfo)Mathcad.Load("Ptc.Calculation.Core").ManifestModule.ResolveMethod(0x06000054)!;
        var quire = typeof(global::Quire.Calculation.Core.EngineResult).GetConstructor(new[]
        {
            typeof(global::Quire.Calculation.Core.IResultNode),
            typeof(global::Quire.Calculation.Core.EngineErrorCollection),
            typeof(global::Quire.Calculation.Core.SynchronizationStatus)
        })!;
        var originalNodeType = Mathcad.Type("Ptc.Calculation.Core", "Ptc.Calculation.Core.ComplexSimpleResultNode");
        var originalErrorsType = Mathcad.Type("Ptc.Calculation.Core", "Ptc.Calculation.Core.EngineErrorCollection");
        var originalErrorType = Mathcad.Type("Ptc.Calculation.Core", "Ptc.Calculation.Core.EngineError");

        void CompareCase(string caseId, bool useNode, int errorCount)
        {
            object? quireNode = useNode
                ? System.Runtime.CompilerServices.RuntimeHelpers.GetUninitializedObject(typeof(global::Quire.Calculation.Core.ComplexSimpleResultNode))
                : null;
            object? originalNode = useNode ? System.Runtime.CompilerServices.RuntimeHelpers.GetUninitializedObject(originalNodeType) : null;
            object? quireErrors = errorCount < 0 ? null : new global::Quire.Calculation.Core.EngineErrorCollection();
            object? originalErrors = errorCount < 0 ? null : Activator.CreateInstance(originalErrorsType);
            for (int i = 0; i < errorCount; i++)
            {
                ((global::Quire.Calculation.Core.EngineErrorCollection)quireErrors!).Add(null!);
                originalErrorsType.GetMethod("Add", new[] { originalErrorType })!.Invoke(originalErrors, new object?[] { null });
            }
            object?[] quireArguments = { quireNode, quireErrors, (global::Quire.Calculation.Core.SynchronizationStatus)status };
            object?[] originalArguments = { originalNode, originalErrors, Enum.ToObject(original.GetParameters()[2].ParameterType, status) };
            MethodComparison.Compare(
                caseId,
                "Ptc.Calculation.Core.EngineResult",
                quire, null, quireArguments,
                original, null, originalArguments,
                ShowEngineResultValue, ShowEngineResultValue);
        }

        CompareCase("EngineResult null result and errors", useNode: false, errorCount: -1);
        CompareCase("EngineResult non-empty result with empty errors", useNode: true, errorCount: 0);
        CompareCase("EngineResult non-empty result with errors", useNode: true, errorCount: 1);
    }
}
