using System.Reflection;
using System.Runtime.CompilerServices;
using System.Text;
using System.Xml;
using Quire.Calculation.Core;

namespace Quire.Differential.Tests;

/// <summary>Direct comparisons for the private matrix writers used by ResultNodeToXmcdTransform visitors.</summary>
public class PtcCalculationCoreResultNodeToXmcdTransformMatrixWriter_MechanicalDifferentialTests
{
    private const string Contract = "Ptc.Calculation.Core.ResultNodeToXmcdTransform";
    private static Type Q => typeof(ResultNodeToXmcdTransform);
    private static Type M => Mathcad.Type("Ptc.Calculation.Core", Contract);

    [SkippableFact]
    public void WriteMatrixXml_06000171()
    {
        Mathcad.Require();
        MethodInfo qMethod = Q.GetMethod("WriteMatrixXml", BindingFlags.NonPublic | BindingFlags.Instance)!;
        MethodInfo mMethod = M.GetMethod("WriteMatrixXml", BindingFlags.NonPublic | BindingFlags.Instance)!;
        var q = Transform(Q);
        var m = Transform(M);
        object qMatrix = EmptyMatrix(typeof(EagerMatrixSimpleResultNode));
        Type mMatrixType = Mathcad.Type("Ptc.Calculation.Core", "Ptc.Calculation.Core.EagerMatrixSimpleResultNode");
        object mMatrix = EmptyMatrix(mMatrixType);
        MethodComparison.Compare("WriteMatrixXml empty 0x0 matrix", Contract, qMethod, q.Target, [qMatrix],
            mMethod, m.Target, [mMatrix], ShowMatrix, ShowMatrix,
            target => WriterState(target, q.Output), target => WriterState(target, m.Output));
    }

    [SkippableFact]
    public void WriteDisplayMatrixXml_06000172()
    {
        Mathcad.Require();
        MethodInfo qMethod = Q.GetMethod("WriteDisplayMatrixXml", BindingFlags.NonPublic | BindingFlags.Instance)!;
        MethodInfo mMethod = M.GetMethod("WriteDisplayMatrixXml", BindingFlags.NonPublic | BindingFlags.Instance)!;
        var q = Transform(Q);
        var m = Transform(M);
        MethodComparison.Compare("WriteDisplayMatrixXml null argument", Contract, qMethod, q.Target, [null],
            mMethod, m.Target, [null], ShowDisplayMatrix, ShowDisplayMatrix,
            target => WriterState(target, q.Output), target => WriterState(target, m.Output));

        object qDisplay = RuntimeHelpers.GetUninitializedObject(typeof(DisplayMatrixSimpleResultNode));
        Type mDisplayType = Mathcad.Type("Ptc.Calculation.Core", "Ptc.Calculation.Core.DisplayMatrixSimpleResultNode");
        object mDisplay = RuntimeHelpers.GetUninitializedObject(mDisplayType);
        SetField(qDisplay, "<Viewport>k__BackingField", null);
        SetField(mDisplay, "<Viewport>k__BackingField", null);
        q = Transform(Q);
        m = Transform(M);
        MethodComparison.Compare("WriteDisplayMatrixXml null viewport", Contract, qMethod, q.Target, [qDisplay],
            mMethod, m.Target, [mDisplay], ShowDisplayMatrix, ShowDisplayMatrix,
            target => WriterState(target, q.Output), target => WriterState(target, m.Output));
    }

    private static (object Target, StringBuilder Output) Transform(Type type)
    {
        var output = new StringBuilder();
        XmlWriter writer = XmlWriter.Create(output, new XmlWriterSettings
        {
            OmitXmlDeclaration = true,
            ConformanceLevel = ConformanceLevel.Fragment,
        });
        object target = RuntimeHelpers.GetUninitializedObject(type);
        SetField(target, "<Writer>k__BackingField", writer);
        return (target, output);
    }

    private static object EmptyMatrix(Type type)
    {
        object matrix = RuntimeHelpers.GetUninitializedObject(type);
        Type elementType = type.GetField("_children", BindingFlags.Instance | BindingFlags.NonPublic)!.FieldType.GetGenericArguments()[0];
        object children = Activator.CreateInstance(typeof(List<>).MakeGenericType(elementType))!;
        SetField(matrix, "_children", children);
        SetField(matrix, "<Columns>k__BackingField", 1);
        return matrix;
    }

    private static string WriterState(object target, StringBuilder output)
    {
        ((XmlWriter)Field(target, "<Writer>k__BackingField")!).Flush();
        return output.ToString();
    }

    private static string ShowMatrix(object? value) => value is null ? "null"
        : $"{value.GetType().Name}: {Read(value, "Rows")}x{Read(value, "Columns")}";

    private static string ShowDisplayMatrix(object? value) => value?.GetType().Name ?? "null";

    private static object? Read(object target, string name) =>
        target.GetType().GetProperty(name, BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic)!.GetValue(target);

    private static object? Field(object target, string name)
    {
        for (Type? type = target.GetType(); type != null; type = type.BaseType)
        {
            FieldInfo? field = type.GetField(name, BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.DeclaredOnly);
            if (field != null) return field.GetValue(target);
        }

        throw new MissingFieldException(target.GetType().FullName, name);
    }

    private static void SetField(object target, string name, object? value)
    {
        for (Type? type = target.GetType(); type != null; type = type.BaseType)
        {
            FieldInfo? field = type.GetField(name, BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.DeclaredOnly);
            if (field == null) continue;
            field.SetValue(target, value);
            return;
        }

        throw new MissingFieldException(target.GetType().FullName, name);
    }
}
