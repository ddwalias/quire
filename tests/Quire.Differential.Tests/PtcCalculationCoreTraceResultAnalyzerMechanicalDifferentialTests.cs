using System.Collections;
using System.Globalization;
using System.Reflection;
using System.Windows;

namespace Quire.Differential.Tests;

public class PtcCalculationCoreTraceResultAnalyzerMechanicalDifferentialTests
{
    public class NodeProxy : DispatchProxy
    {
        public object? ListValue { get; set; }
        public object? VectorsValue { get; set; }
        public int CountValue { get; set; }
        public int MeaningfulValuesCountValue { get; set; }

        protected override object? Invoke(MethodInfo? targetMethod, object?[]? args)
        {
            return targetMethod?.Name switch
            {
                "get_List" => ListValue,
                "get_Vectors" => VectorsValue,
                "get_Count" => CountValue,
                "get_MeaningfulValuesCount" => MeaningfulValuesCountValue,
                "IsMeaningfulValue" => args?[0] is string value && !string.IsNullOrEmpty(value),
                _ => throw new NotSupportedException(targetMethod?.Name),
            };
        }
    }

    private const string TypeName = "Ptc.Calculation.Core.Trace2DResults.TraceResultAnalyzer";

    private static MethodBase Original(int token) => Mathcad.Type("Ptc.Calculation.Core", TypeName).Module.ResolveMethod(token)!;

    private static MethodInfo QuireMethod(string name, BindingFlags flags, params Type[] arguments) =>
        typeof(global::Quire.Calculation.Core.Trace2DResults.TraceResultAnalyzer).GetMethod(name, flags, null, arguments, null)!;

    private static object Proxy(Type interfaceType, object? list = null, object? vectors = null, int count = 0, int meaningful = 0)
    {
        object proxy = DispatchProxy.Create(interfaceType, typeof(NodeProxy));
        NodeProxy values = (NodeProxy)proxy;
        values.ListValue = list;
        values.VectorsValue = vectors;
        values.CountValue = count;
        values.MeaningfulValuesCountValue = meaningful;
        return proxy;
    }

    private static string Number(double value) => double.IsNaN(value) ? "NaN" : value.ToString("R", CultureInfo.InvariantCulture);

    private static string Show(object? value)
    {
        if (value == null) return "null";
        if (value is double number) return Number(number);
        if (value is Point point) return $"({Number(point.X)},{Number(point.Y)})";
        if (value is string text) return $"\"{text}\"";
        Type type = value.GetType();
        if (type.GetInterfaces().Any(i => i.Name == "IStringListResultNode"))
        {
            object? list = type.GetProperty("List")!.GetValue(value);
            return "StringNode" + Show(list);
        }
        if (type.GetInterfaces().Any(i => i.Name == "IDoubleListResultNode"))
        {
            object? list = type.GetProperty("List")!.GetValue(value);
            return "DoubleNode" + Show(list);
        }
        if (type.GetInterfaces().Any(i => i.Name == "IVectorListResultNode"))
        {
            object? vectors = type.GetProperty("Vectors")!.GetValue(value);
            return "VectorNode" + Show(vectors);
        }
        if (value is IEnumerable values)
            return "[" + string.Join(",", values.Cast<object?>().Select(Show)) + "]";
        return type.Name;
    }

    private static void Compare(int token, string caseId, MethodInfo quire, object?[] quireArgs, object?[] mathcadArgs) =>
        MethodComparison.Compare(caseId, TypeName, quire, null, quireArgs, Original(token), null, mathcadArgs, Show, Show);

    [SkippableFact]
    public void AddOneDataSegmentWithRepeatedNaNs_060002b9()
    {
        var qPoints = new List<Point>();
        var mPoints = new List<Point>();
        var qValues = new List<double> { 1, double.NaN, double.NaN, 2, double.NaN };
        var mValues = new List<double> { 1, double.NaN, double.NaN, 2, double.NaN };
        var q = QuireMethod("AddOneDataSegment", BindingFlags.Static | BindingFlags.NonPublic,
            typeof(IList<Point>), typeof(double), typeof(IList<double>), typeof(bool));
        Compare(0x060002b9, "repeated NaN separators and trailing NaN", q,
            [qPoints, 3.0, qValues, true], [mPoints, 3.0, mValues, true]);
    }

    [SkippableFact]
    public void StringVectorToIndices_060002ba()
    {
        Type qInterface = typeof(global::Quire.Calculation.Core.IStringListResultNode);
        Type mInterface = Mathcad.Type("Ptc.Calculation.Core", "Ptc.Calculation.Core.IStringListResultNode");
        var qNode = Proxy(qInterface, new List<string> { "a", "", "b" }, count: 3);
        var mNode = Proxy(mInterface, new List<string> { "a", "", "b" }, count: 3);
        var q = QuireMethod("StringVectorToIndices", BindingFlags.Static | BindingFlags.NonPublic, qInterface, typeof(int));
        Compare(0x060002ba, "offsets count meaningful string entries; empty entry becomes NaN", q, [qNode, 4], [mNode, 4]);
    }

    [SkippableFact]
    public void ToDoubleDataList_060002bb()
    {
        Type qVectorInterface = typeof(global::Quire.Calculation.Core.IVectorListResultNode);
        Type qListInterface = typeof(global::Quire.Calculation.Core.IListResultNode);
        Type mVectorInterface = Mathcad.Type("Ptc.Calculation.Core", "Ptc.Calculation.Core.IVectorListResultNode");
        Type mListInterface = Mathcad.Type("Ptc.Calculation.Core", "Ptc.Calculation.Core.IListResultNode");
        object qDouble = Proxy(typeof(global::Quire.Calculation.Core.IDoubleListResultNode), new List<double> { 7, 8 }, count: 2);
        object qString1 = Proxy(typeof(global::Quire.Calculation.Core.IStringListResultNode), new List<string> { "a", "" }, count: 2);
        object qString2 = Proxy(typeof(global::Quire.Calculation.Core.IStringListResultNode), new List<string> { "b", "c" }, count: 2);
        var qVectors = new List<global::Quire.Calculation.Core.IListResultNode>
        {
            (global::Quire.Calculation.Core.IListResultNode)qDouble,
            (global::Quire.Calculation.Core.IListResultNode)qString1,
            (global::Quire.Calculation.Core.IListResultNode)qString2,
        };
        Type mDoubleInterface = Mathcad.Type("Ptc.Calculation.Core", "Ptc.Calculation.Core.IDoubleListResultNode");
        Type mStringInterface = Mathcad.Type("Ptc.Calculation.Core", "Ptc.Calculation.Core.IStringListResultNode");
        var mDouble = Proxy(mDoubleInterface, new List<double> { 7, 8 }, count: 2);
        var mString1 = Proxy(mStringInterface, new List<string> { "a", "" }, count: 2);
        var mString2 = Proxy(mStringInterface, new List<string> { "b", "c" }, count: 2);
        IList mVectors = (IList)Activator.CreateInstance(typeof(List<>).MakeGenericType(mListInterface))!;
        mVectors.Add(mDouble);
        mVectors.Add(mString1);
        mVectors.Add(mString2);
        object qResult = Proxy(qVectorInterface, vectors: qVectors, count: 3);
        object mResult = Proxy(mVectorInterface, vectors: mVectors, count: 3);
        var q = QuireMethod("ToDoubleDataList", BindingFlags.Static | BindingFlags.Public, qVectorInterface);
        Compare(0x060002bb, "double vectors pass through and string vector indices continue across vectors", q,
            [qResult], [mResult]);
    }

    [SkippableFact]
    public void BuildPointsForBoxes_060002bd()
    {
        var qPoints = new List<Point>();
        var mPoints = new List<Point>();
        var qRows = new List<IList<double>>
        {
            new List<double> { 1, double.NaN, 3 },
            new List<double> { 4, 5 },
            null!,
        };
        var mRows = new List<IList<double>>
        {
            new List<double> { 1, double.NaN, 3 },
            new List<double> { 4, 5 },
            null!,
        };
        var q = QuireMethod("BuildPointsForBoxes", BindingFlags.Static | BindingFlags.Public,
            typeof(IList<Point>), typeof(IList<double>), typeof(IList<IList<double>>));
        Compare(0x060002bd, "skip NaN x values and null y rows; append NaN between boxes", q,
            [qPoints, new List<double> { 2, double.NaN, 6 }, qRows],
            [mPoints, new List<double> { 2, double.NaN, 6 }, mRows]);
    }

    [SkippableFact]
    public void BuildTuplesOfPoints_060002be()
    {
        var qPoints = new List<Point>();
        var mPoints = new List<Point>();
        IList<double>[] qYVectors =
        [
            new List<double> { 2, 3, double.NaN },
            new List<double> { 4, double.NaN, 6 },
        ];
        IList<double>[] mYVectors =
        [
            new List<double> { 2, 3, double.NaN },
            new List<double> { 4, double.NaN, 6 },
        ];
        var q = QuireMethod("BuildTuplesOfPoints", BindingFlags.Static | BindingFlags.Public,
            typeof(IList<Point>), typeof(IList<double>), typeof(IList<double>[]), typeof(IList<double>));
        Compare(0x060002be, "skip NaN x and y values; apply per-vector x offsets and delimit rows", q,
            [qPoints, new List<double> { 1, double.NaN, 3 },
                qYVectors, new List<double> { 0.5, 1.0 }],
            [mPoints, new List<double> { 1, double.NaN, 3 },
                mYVectors, new List<double> { 0.5, 1.0 }]);
    }
}
