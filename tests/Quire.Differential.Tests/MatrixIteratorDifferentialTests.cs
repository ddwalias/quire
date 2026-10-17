using System.Reflection;
using Xunit;
using static Quire.Differential.Tests.Reflect;

namespace Quire.Differential.Tests;

public class MatrixIteratorDifferentialTests
{
    private const string Contract = "Mcd.RngVal";
    private const BindingFlags DeclaredInstance = BindingFlags.Public | BindingFlags.Instance | BindingFlags.DeclaredOnly;

    private static System.Type QIterator => typeof(global::Quire.Mcd.Iterator.MatrixIterator);
    private static System.Type MIterator => Mathcad.Type("mplrun", "Mcd.Iterator+MatrixIterator");
    private static System.Type QIteratorBase => typeof(global::Quire.Mcd.Iterator);
    private static System.Type MIteratorBase => Mathcad.Type("mplrun", "Mcd.Iterator");
    private static System.Type QMatVal => typeof(global::Quire.Mcd.MatVal);
    private static System.Type MMatVal => Mathcad.Type("mplrun", "Mcd.MatVal");

    private static ConstructorInfo Constructor(Side side) =>
        (side == Side.Quire ? QIterator : MIterator).GetConstructor(DeclaredInstance, null,
            [side == Side.Quire ? QMatVal : MMatVal], null)
        ?? throw new HarnessException($"{side.Name} MatrixIterator constructor not found");

    private static MethodInfo Method(Side side, string name)
    {
        var method = (side == Side.Quire ? QIterator : MIterator).GetMethod(name, DeclaredInstance, Type.EmptyTypes);
        return method ?? throw new HarnessException($"{side.Name} MatrixIterator.{name}() not found");
    }

    private static object Matrix(Side side, double[] values) =>
        New(side == Side.Quire ? QMatVal : MMatVal, values, null, 2, 3);

    private static string State(object iterator) =>
        $"MatrixIterator[r={Get(iterator, "m_row")},c={Get(iterator, "m_col")}]";

    private static string ShowAdvance(object? value, object iterator) => value == null ? "null"
        : ReferenceEquals(value, iterator) ? $"same {State(value)}"
        : $"other {value.GetType().FullName} {State(value)}";

    private static string ShowCursor(Side side, object? value) => value == null ? "null"
        : value.GetType().Name == "MatrixIterator" ? $"{side.Show(value)} {State(value)}"
        : side.Show(value);

    [SkippableFact]
    public void Matrix_iterator_traverses_rows_then_columns()
    {
        Runtime.Sync();
        var qMatrix = Matrix(Side.Quire, [10, 20, 30, 40, 50, 60]);
        var mMatrix = Matrix(Side.Mathcad, [10, 20, 30, 40, 50, 60]);
        var qCtor = Constructor(Side.Quire);
        var mCtor = Constructor(Side.Mathcad);

        MethodComparison.Compare("MatrixIterator(2x3)", Contract,
            qCtor, null, [qMatrix], mCtor, null, [mMatrix],
            value => ShowCursor(Side.Quire, value), value => ShowCursor(Side.Mathcad, value));

        var qForValue = QIteratorBase.GetMethod("forValue", BindingFlags.Public | BindingFlags.Static | BindingFlags.DeclaredOnly,
            [typeof(object)])!;
        var mForValue = MIteratorBase.GetMethod("forValue", BindingFlags.Public | BindingFlags.Static | BindingFlags.DeclaredOnly,
            [typeof(object)])!;
        MethodComparison.Compare("Iterator.forValue(2x3) initial cursor", Contract,
            qForValue, null, [qMatrix], mForValue, null, [mMatrix],
            value => ShowCursor(Side.Quire, value), value => ShowCursor(Side.Mathcad, value));

        var qIterator = New(QIterator, qMatrix);
        var mIterator = New(MIterator, mMatrix);
        var qFirst = Method(Side.Quire, "first");
        var mFirst = Method(Side.Mathcad, "first");
        var qNext = Method(Side.Quire, "next");
        var mNext = Method(Side.Mathcad, "next");
        for (int i = 0; i < 6; i++)
        {
            MethodComparison.Compare($"MatrixIterator.first[{i}]", Contract,
                qFirst, qIterator, [], mFirst, mIterator, [], Side.Quire.Show, Side.Mathcad.Show, State, State);
            MethodComparison.Compare($"MatrixIterator.next[{i}]", Contract,
                qNext, qIterator, [], mNext, mIterator, [],
                value => ShowAdvance(value, qIterator), value => ShowAdvance(value, mIterator), State, State);
        }
    }
}
