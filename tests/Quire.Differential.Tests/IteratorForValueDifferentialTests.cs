using System.Reflection;
using Xunit;

namespace Quire.Differential.Tests;

public sealed class IteratorForValueDifferentialTests
{
    private const string Contract = "Mcd.RngVal";
    private const BindingFlags StaticPublic = BindingFlags.Public | BindingFlags.Static | BindingFlags.DeclaredOnly;
    private const BindingFlags InstancePublic = BindingFlags.Public | BindingFlags.Instance | BindingFlags.DeclaredOnly;

    [SkippableFact]
    public void forValue_checks_each_matrix_axis_and_retains_nonempty_matrix()
    {
        Runtime.Sync();
        var qMethod = typeof(global::Quire.Mcd.Iterator).GetMethod("forValue", StaticPublic, [typeof(object)])!;
        var mIterator = Mathcad.Type("mplrun", "Mcd.Iterator");
        var mMethod = mIterator.GetMethod("forValue", StaticPublic, [typeof(object)])!;
        var qMat = typeof(global::Quire.Mcd.MatVal);
        var mMat = Mathcad.Type("mplrun", "Mcd.MatVal");
        var qCtor = qMat.GetConstructor(InstancePublic, null, [typeof(int), typeof(int)], null)!;
        var mCtor = mMat.GetConstructor(InstancePublic, null, [typeof(int), typeof(int)], null)!;

        foreach (var (rows, cols) in new[] { (0, 3), (3, 0), (2, 2) })
        {
            object qValue = qCtor.Invoke([rows, cols]);
            object mValue = mCtor.Invoke([rows, cols]);
            MethodComparison.Compare($"forValue(matrix {rows}x{cols})", Contract,
                qMethod, null, [qValue], mMethod, null, [mValue],
                value => Show(value, "Quire", qValue), value => Show(value, "Mathcad", mValue));
            if (rows > 0 && cols > 0)
            {
                object qIterator = qMethod.Invoke(null, [qValue])!;
                object mResultIterator = mMethod.Invoke(null, [mValue])!;
                Assert.Same(qValue, qIterator.GetType().GetField("m_mat", BindingFlags.NonPublic | BindingFlags.Instance)!.GetValue(qIterator));
                Assert.Same(mValue, mResultIterator.GetType().GetField("m_mat", BindingFlags.NonPublic | BindingFlags.Instance)!.GetValue(mResultIterator));
            }
        }
    }

    private static string Show(object? value, string side, object input)
    {
        if (value == null) return "null";
        if (value.GetType().Name == "MatVal")
        {
            var type = side == "Quire" ? typeof(global::Quire.Mcd.MatVal) : Mathcad.Type("mplrun", "Mcd.MatVal");
            var rows = type.GetProperty("Rows")!.GetValue(value);
            var cols = type.GetProperty("Cols")!.GetValue(value);
            return $"MatVal[{rows}x{cols}]";
        }
        if (value.GetType().Name == "MatrixIterator")
        {
            var matrix = value.GetType().GetField("m_mat", BindingFlags.NonPublic | BindingFlags.Instance)!.GetValue(value);
            var row = value.GetType().GetField("m_row", BindingFlags.NonPublic | BindingFlags.Instance)!.GetValue(value);
            var col = value.GetType().GetField("m_col", BindingFlags.NonPublic | BindingFlags.Instance)!.GetValue(value);
            return $"MatrixIterator[input={ReferenceEquals(matrix, input)},r={row},c={col}]";
        }
        return value.GetType().Name;
    }
}
