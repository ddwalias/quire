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
    public void WriteRealXml_0600016f()
    {
        Mathcad.Require();
        MethodInfo qMethod = Q.GetMethod("WriteRealXml", BindingFlags.NonPublic | BindingFlags.Instance)!;
        MethodInfo mMethod = M.GetMethod("WriteRealXml", BindingFlags.NonPublic | BindingFlags.Instance)!;
        var q = Transform(Q);
        var m = Transform(M);
        MethodComparison.Compare("WriteRealXml round-trip real", Contract, qMethod, q.Target, [2.5],
            mMethod, m.Target, [2.5], state => Reflect.Text(state), state => Reflect.Text(state),
            target => WriterState(target, q.Output), target => WriterState(target, m.Output));
    }

    [SkippableFact]
    public void WriteImaginaryXml_06000170()
    {
        Mathcad.Require();
        MethodInfo qMethod = Q.GetMethod("WriteImaginaryXml", BindingFlags.NonPublic | BindingFlags.Instance)!;
        MethodInfo mMethod = M.GetMethod("WriteImaginaryXml", BindingFlags.NonPublic | BindingFlags.Instance)!;
        var q = Transform(Q);
        var m = Transform(M);
        MethodComparison.Compare("WriteImaginaryXml round-trip imaginary", Contract, qMethod, q.Target, [-4.25],
            mMethod, m.Target, [-4.25], state => Reflect.Text(state), state => Reflect.Text(state),
            target => WriterState(target, q.Output), target => WriterState(target, m.Output));
    }

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
        MethodComparison.Compare("WriteMatrixXml empty 0x1 matrix", Contract, qMethod, q.Target, [qMatrix],
            mMethod, m.Target, [mMatrix], ShowMatrix, ShowMatrix,
            target => WriterState(target, q.Output), target => WriterState(target, m.Output));

        q = Transform(Q);
        m = Transform(M);
        Type mMatrixInterface = Mathcad.Type("Ptc.Calculation.Core", "Ptc.Calculation.Core.IMatrixResultNode");
        Type mRealType = Mathcad.Type("Ptc.Calculation.Core", "Ptc.Calculation.Core.RealSimpleResultNode");
        qMatrix = MatrixProxy(typeof(IMatrixResultNode), typeof(RealSimpleResultNode), 2.5);
        mMatrix = MatrixProxy(mMatrixInterface, mRealType, 2.5);
        MethodComparison.Compare("WriteMatrixXml one real value", Contract, qMethod, q.Target, [qMatrix],
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
        SetProperty(qDisplay, "Viewport", null);
        SetProperty(mDisplay, "Viewport", null);
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
        SetProperty(target, "Writer", writer);
        return (target, output);
    }

    private static object EmptyMatrix(Type type)
    {
        object matrix = RuntimeHelpers.GetUninitializedObject(type);
        Type elementType = type.GetField("_children", BindingFlags.Instance | BindingFlags.NonPublic)!.FieldType.GetGenericArguments()[0];
        object children = Activator.CreateInstance(typeof(List<>).MakeGenericType(elementType))!;
        SetField(matrix, "_children", children);
        SetProperty(matrix, "Columns", 1);
        return matrix;
    }

    private static string WriterState(object target, StringBuilder output)
    {
        ((XmlWriter)Read(target, "Writer")!).Flush();
        return output.ToString();
    }

    private static string ShowMatrix(object? value) => value is null ? "null"
        : $"matrix {Read(value, "Rows")}x{Read(value, "Columns")}";

    private static string ShowDisplayMatrix(object? value) => value?.GetType().Name ?? "null";

    private static object? Read(object target, string name) =>
        target.GetType().GetProperty(name, BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic)!.GetValue(target);

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

    private static void SetProperty(object target, string name, object? value) =>
        target.GetType().GetProperty(name, BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic)!
            .SetValue(target, value);

    private static object MatrixProxy(Type matrixInterface, Type realNodeType, double value)
    {
        MethodInfo create = typeof(DispatchProxy).GetMethods(BindingFlags.Public | BindingFlags.Static)
            .Single(method => method.Name == "Create" && method.IsGenericMethodDefinition
                && method.GetGenericArguments().Length == 2 && method.GetParameters().Length == 0)
            .MakeGenericMethod(matrixInterface, typeof(MatrixNodeDispatchProxy));
        var proxy = (MatrixNodeDispatchProxy)create.Invoke(null, null)!;
        object realNode = Activator.CreateInstance(realNodeType)!;
        realNodeType.GetProperty("Value")!.SetValue(realNode, value);
        proxy.Handler = method => method.Name switch
        {
            "get_Rows" or "get_Columns" => 1,
            "get_Item" => realNode,
            _ => method.ReturnType.IsValueType ? Activator.CreateInstance(method.ReturnType) : null,
        };
        return proxy;
    }

    public class MatrixNodeDispatchProxy : DispatchProxy
    {
        public Func<MethodInfo, object?>? Handler { get; set; }

        protected override object? Invoke(MethodInfo? targetMethod, object?[]? args) => Handler!(targetMethod!);
    }
}
