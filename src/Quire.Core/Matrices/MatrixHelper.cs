#nullable disable
using Quire.Mirror;
using Quire.Mirror;

namespace Quire.Matrices;

public partial class MatrixHelper
{
    [MirrorVerified("PtcMatricesMatrixHelperMechanicalDifferentialTests")]
    public MatrixHelper() { }

    [MirrorVerified("Release IL")]
    public static int RowOf(int index, int columns) => index / columns;

    [MirrorVerified("Release IL")]
    public static int ColumnOf(int index, int columns) => index % columns;

    [MirrorVerified("Release IL")]
    public static T[] SwapRowsColumns<T>(int rows, int columns, T[] elements)
    {
        T[] array = new T[elements.Length];
        for (int i = 0; i < elements.Length; i++)
        {
            array[SwapIndex(i, rows, columns)] = elements[i];
        }
        return array;
    }

    [MirrorVerified("Release IL")]
    public static int IndexOf(int row, int column, int numOfColumns)
    {
        if (!DebugTools.DevAssert(0 <= column && column < numOfColumns, null).Passed)
        {
            return -1;
        }
        return row * numOfColumns + column;
    }

    [MirrorVerified("Release IL")]
    public static int SwapIndex(int index, int rows, int columns)
    {
        int column = index / columns;
        return IndexOf(index % columns, column, rows);
    }

    [MirrorVerified("Release IL")]
    private static void Swap<T>(ref T first, ref T second)
    {
        T val = second;
        second = first;
        first = val;
    }

    [MirrorVerified("Release IL")]
    public static DiscreteRect RectFromCornerIndices(int corner1, int corner2, int columns) => new DiscreteRect(RowOf(corner1, columns), ColumnOf(corner1, columns), RowOf(corner2, columns), ColumnOf(corner2, columns));
}
