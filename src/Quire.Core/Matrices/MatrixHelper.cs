#nullable disable
using Quire.Mirror;
using Quire.Mirror;

namespace Quire.Matrices;

public partial class MatrixHelper
{
    [MirrorVerified("Release IL")]
    public static T[] SwapRowsColumns<T>(int rows, int columns, global::System.Collections.Generic.IEnumerable<T> elements)
    {
        T[] array = new T[rows * columns];
        int num = 0;
        foreach (T element in elements)
        {
            array[num++] = element;
        }
        if (array.Length > num)
        {
            throw new global::System.ArgumentException("Too few elements passed in");
        }
        return SwapRowsColumns(rows, columns, array);
    }

    [MirrorVerified("Release IL")]
    public static T[] AddRow<T>(int rowToAdd, int rows, int cols, T[] elements, global::Quire.DelegateFunction0<T> createDefaultElement)
    {
        if (rowToAdd < 0 || rowToAdd > rows)
        {
            throw new global::System.ArgumentException(global::System.String.Format("rowToAdd({0}) must be in the range 0 - # of rows({1})", rowToAdd, rows));
        }
        if (rows * cols != elements.Length)
        {
            throw new global::System.ArgumentException(global::System.String.Format("There must be exactly rows x cols ({0}) but there were {1}", rows * cols, elements.Length));
        }
        T[] array = new T[(rows + 1) * cols];
        int num = IndexOf(rowToAdd, 0, cols);
        if (num > 0)
        {
            global::System.Array.Copy(elements, 0, array, 0, num);
        }
        if (createDefaultElement != null)
        {
            for (int i = num; i < num + cols; i++)
            {
                array[i] = createDefaultElement();
            }
        }
        if (num < elements.Length)
        {
            global::System.Array.Copy(elements, num, array, num + cols, elements.Length - num);
        }
        return array;
    }

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
    public static T[] AddColumn<T>(int colToAdd, int rows, int cols, T[] elements, global::Quire.DelegateFunction0<T> createDefaultElement)
    {
        if (colToAdd < 0 || colToAdd > cols)
        {
            throw new global::System.ArgumentException(global::System.String.Format("colToAdd({0}) must be in the range 0 - # of columns({1})", colToAdd, cols));
        }
        if (rows * cols != elements.Length)
        {
            throw new global::System.ArgumentException(global::System.String.Format("There must be exactly rows x cols ({0}) but there were {1}", rows * cols, elements.Length));
        }
        T[] array = new T[rows * (cols + 1)];
        for (int i = 0; i < rows; i++)
        {
            if (colToAdd > 0)
            {
                global::System.Array.Copy(elements, IndexOf(i, 0, cols), array, IndexOf(i, 0, cols + 1), colToAdd);
            }
            if (createDefaultElement != null)
            {
                array[IndexOf(i, colToAdd, cols + 1)] = createDefaultElement();
            }
            if (colToAdd < cols)
            {
                global::System.Array.Copy(elements, IndexOf(i, colToAdd, cols), array, IndexOf(i, colToAdd + 1, cols + 1), cols - colToAdd);
            }
        }
        return array;
    }

    [MirrorVerified("Release IL")]
    public static T[] DeleteRow<T>(int rowToDelete, int rows, int cols, T[] elements)
    {
        if (rowToDelete < 0 || rowToDelete >= rows)
        {
            throw new global::System.ArgumentException(global::System.String.Format("rowToDelete({0}) must be in the range 0 - # of rows - 1({1})", rowToDelete, rows - 1));
        }
        if (rows * cols != elements.Length)
        {
            throw new global::System.ArgumentException(global::System.String.Format("There must be exactly rows x cols ({0}) but there were {1}", rows * cols, elements.Length));
        }
        T[] array = new T[(rows - 1) * cols];
        int num = IndexOf(rowToDelete, 0, cols);
        if (num > 0)
        {
            global::System.Array.Copy(elements, 0, array, 0, num);
        }
        if (num < array.Length)
        {
            global::System.Array.Copy(elements, num + cols, array, num, array.Length - num);
        }
        return array;
    }

    [MirrorVerified("Release IL")]
    public static T[] DeleteColumn<T>(int colToDelete, int rows, int cols, T[] elements)
    {
        if (colToDelete < 0 || colToDelete >= cols)
        {
            throw new global::System.ArgumentException(global::System.String.Format("colToDelete({0}) must be in the range 0 - # of columns - 1({1})", colToDelete, cols - 1));
        }
        if (rows * cols != elements.Length)
        {
            throw new global::System.ArgumentException(global::System.String.Format("There must be exactly rows x cols ({0}) but there were {1}", rows * cols, elements.Length));
        }
        int num = cols - 1;
        T[] array = new T[rows * num];
        for (int i = 0; i < rows; i++)
        {
            if (colToDelete > 0)
            {
                global::System.Array.Copy(elements, IndexOf(i, 0, cols), array, IndexOf(i, 0, num), colToDelete);
            }
            if (colToDelete < cols - 1)
            {
                global::System.Array.Copy(elements, IndexOf(i, colToDelete + 1, cols), array, IndexOf(i, colToDelete, num), num - colToDelete);
            }
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
