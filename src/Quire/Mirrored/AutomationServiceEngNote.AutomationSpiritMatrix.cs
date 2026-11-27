namespace Quire.AutomationServiceEngNote;

public partial class AutomationSpiritMatrix
{
    private readonly double[][] _matrix;

    public AutomationSpiritMatrix(int rows, int columns)
    {
        _matrix = new double[rows][];
        for (int row = 0; row < rows; row++)
        {
            _matrix[row] = new double[columns];
            for (int column = 0; column < columns; column++)
            {
                _matrix[row][column] = double.NaN;
            }
        }
    }

    public AutomationSpiritMatrix(double[][] matrix)
    {
        _matrix = matrix;
    }

    public double[][] Matrix => _matrix;

    public double this[int rowInd, int colInd]
    {
        get
        {
            double value;
            if (GetMatrixElement(rowInd, colInd, out value) != -1)
            {
                return value;
            }

            return double.NaN;
        }
    }

    public double Rows
    {
        get
        {
            GetMatrixSize(out int rows, out int columns);
            return rows;
        }
    }

    public double Columns
    {
        get
        {
            GetMatrixSize(out int rows, out int columns);
            return columns;
        }
    }

    private void GetMatrixSize(out int rows, out int columns)
    {
        try
        {
            rows = _matrix != null ? _matrix.Length : -1;
            columns = _matrix != null ? _matrix[0].Length : -1;
        }
        catch
        {
            rows = 0;
            columns = 0;
        }
    }

    public int SetMatrixElement(int rowIndexArg, int columnIndexArg, double valueArg)
    {
        try
        {
            if (rowIndexArg >= 0 && rowIndexArg < _matrix.Length && columnIndexArg >= 0 && columnIndexArg < _matrix[rowIndexArg].Length)
            {
                _matrix[rowIndexArg][columnIndexArg] = valueArg;
                return 0;
            }
        }
        catch
        {
        }

        return -1;
    }

    public int GetMatrixElement(int rowIndexArg, int columnIndexArg, out double valueArg)
    {
        try
        {
            if (rowIndexArg >= 0 && rowIndexArg < _matrix.Length && columnIndexArg >= 0 && columnIndexArg < _matrix[rowIndexArg].Length)
            {
                valueArg = _matrix[rowIndexArg][columnIndexArg];
                return 0;
            }
        }
        catch
        {
        }

        valueArg = double.NaN;
        return -1;
    }
}
