#nullable disable
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Windows.Input;
using Quire.Matrices;
using Quire.Mirror;

namespace Quire.Controls.Core;

public partial class MatrixFormat
{
    private TableSize _size;

    private TableCoordinate _offset;

    private int _origin;

    private bool _showIndices;

    private bool _collapseNestedMatrices;

    private NestedMatrixFormats _nestedMatrixFormats;

    public TableSize Size
    {
        get => _size;
        set
        {
            if (_size == value)
            {
                return;
            }
            _size.Rows = Math.Max(value.Rows, 1);
            _size.Columns = Math.Max(value.Columns, 1);
            HandleMatrixFormatChanged("Size", _size);
        }
    }

    public TableCoordinate Offset
    {
        get => _offset;
        set
        {
            if (_offset == value)
            {
                return;
            }
            _offset = value;
            HandleMatrixFormatChanged("Offset", value);
        }
    }

    public int Origin
    {
        get => _origin;
        set
        {
            if (_origin == value)
            {
                return;
            }
            _origin = value;
            foreach (TableCoordinate populatedPosition in NestedMatrixFormats.PopulatedPositions)
            {
                NestedMatrixFormats[populatedPosition]._origin = Origin;
            }
            HandleMatrixFormatChanged("Origin", value);
        }
    }

    public bool ShowIndices
    {
        get => _showIndices;
        set
        {
            if (_showIndices == value)
            {
                return;
            }
            _showIndices = value;
            HandleMatrixFormatChanged("ShowIndices", value);
        }
    }

    public bool CollapseNestedMatrices
    {
        get => _collapseNestedMatrices;
        set
        {
            if (_collapseNestedMatrices == value)
            {
                return;
            }
            _collapseNestedMatrices = value;
            HandleMatrixFormatChanged("CollapseNestedMatrices", value);
        }
    }

    public NestedMatrixFormats NestedMatrixFormats
    {
        get => _nestedMatrixFormats;
        set
        {
            if (_nestedMatrixFormats == value)
            {
                return;
            }
            _nestedMatrixFormats = value;
            HandleMatrixFormatChanged("NestedMatrixFormats", value);
        }
    }

    [MirrorVerified("PtcControlsCoreMatrixFormatMechanicalDifferentialTests")]
    public event ActionByPropertyNameAndValue FormatChanged;

    private void HandleMatrixFormatChanged(string propertyName, object newValue)
    {
        FirePropertyChanged(propertyName);
        if (FormatChanged != null)
        {
            FormatChanged(propertyName, newValue);
        }
    }

    public MatrixFormat()
    {
        _collapseNestedMatrices = true;
        _size = new TableSize(1, 1);
        NestedMatrixFormats = new NestedMatrixFormats();
        NestedMatrixFormats.Changed += HandleNestedMatrixChanged;
    }

    private void HandleNestedMatrixChanged(object sender, SparseMatrix<MatrixFormat>.SparseMatrixItemChangedEventArgs e)
    {
        if (sender != NestedMatrixFormats)
        {
            throw new InvalidOperationException("Logic Error: the wrong NestedMatrixFormats Changed event is hooked up to MatrixFormat.");
        }
        if (e.Position.Row < 0)
        {
            return;
        }
        MatrixFormat oldItem = e.OldItem;
        if (oldItem != NestedMatrixFormats.EmptyPosition)
        {
            oldItem.RemoveFormatChangedHandlerFromNestedMatrixFormat(FormatChanged);
        }
        MatrixFormat newItem = e.NewItem;
        if (newItem != NestedMatrixFormats.EmptyPosition)
        {
            newItem.AddFormatChangedHandlerToNestedMatrixFormat(FormatChanged);
        }
    }

    public object GetValueByName(string propertyName)
    {
        switch (propertyName)
        {
        case "CollapseNestedMatrices":
            return CollapseNestedMatrices;
        case "NestedMatrixFormats":
            return NestedMatrixFormats;
        case "Offset":
            return Offset;
        case "Origin":
            return Origin;
        case "ShowIndices":
            return ShowIndices;
        case "Size":
            return Size;
        default:
            // Mathcad formats via string.Format(string, object) before calling FailAssertion(string).
            DebugTools.FailAssertion(string.Format("{0} property doesn't exist in MatrixFormat class.", propertyName));
            return null;
        }
    }

    public void SetValueByName(string propertyName, object value)
    {
        if (propertyName == "NestedMatrixFormats")
        {
            NestedMatrixFormats = this.ValidateReferenceType<NestedMatrixFormats>(value, propertyName);
        }
        else
        {
            if (value == null)
            {
                return;
            }
            switch (propertyName)
            {
            case "CollapseNestedMatrices":
                if (this.IsValidateValueType<bool>(propertyName, value))
                {
                    CollapseNestedMatrices = (bool)value;
                }
                return;
            case "Offset":
                if (this.IsValidateValueType<TableCoordinate>(propertyName, value))
                {
                    Offset = (TableCoordinate)value;
                }
                return;
            case "Origin":
                if (this.IsValidateValueType<int>(propertyName, value))
                {
                    Origin = (int)value;
                }
                return;
            case "ShowIndices":
                if (this.IsValidateValueType<bool>(propertyName, value))
                {
                    ShowIndices = (bool)value;
                }
                return;
            case "Size":
                if (this.IsValidateValueType<TableSize>(propertyName, value))
                {
                    Size = (TableSize)value;
                }
                return;
            }
        }
    }

    public static bool operator ==(MatrixFormat lhsFormat, MatrixFormat rhsFormat)
    {
        if ((object)lhsFormat == null && (object)rhsFormat == null)
        {
            return true;
        }
        if ((object)lhsFormat == null || (object)rhsFormat == null)
        {
            return false;
        }
        if (lhsFormat.NestedMatrixFormats.PopulatedCount != rhsFormat.NestedMatrixFormats.PopulatedCount)
        {
            return false;
        }
        if (lhsFormat.NestedMatrixFormats.PopulatedPositions.Any((TableCoordinate pos) => lhsFormat.NestedMatrixFormats[pos] != rhsFormat.NestedMatrixFormats[pos]))
        {
            return false;
        }
        if (lhsFormat.Size == rhsFormat.Size && lhsFormat.Offset == rhsFormat.Offset && lhsFormat.Origin == rhsFormat.Origin && lhsFormat.ShowIndices == rhsFormat.ShowIndices)
        {
            return lhsFormat.CollapseNestedMatrices == rhsFormat.CollapseNestedMatrices;
        }
        return false;
    }

    public static bool operator !=(MatrixFormat format, MatrixFormat other)
    {
        return !(format == other);
    }

    public MatrixFormat Clone()
    {
        MatrixFormat clone = new MatrixFormat
        {
            Size = Size,
            Offset = Offset,
            Origin = Origin,
            ShowIndices = ShowIndices,
            CollapseNestedMatrices = CollapseNestedMatrices
        };
        NestedMatrixFormats.PopulatedPositions.Each((TableCoordinate pos) =>
        {
            clone.NestedMatrixFormats[pos] = NestedMatrixFormats[pos].Clone();
        });
        return clone;
    }

    object ICloneable.Clone() => Clone();

    public static MatrixFormat GetFormatAdjustedForActualMatrixSize(TableSize actualSize, MatrixFormat format)
    {
        if (actualSize.Rows <= 0 || actualSize.Columns <= 0)
        {
            throw new InvalidOperationException("logic error: The actual size of a matrix did not contain any elements (rows and/or columns was zero)");
        }
        TableCoordinate tableCoordinate = new TableCoordinate(format.Offset.Row + format.Size.Rows - 1, format.Offset.Column + format.Size.Columns - 1);
        MatrixFormat matrixFormat = format.Clone();
        int rowOffsetDelta = 0;
        int rowSizeDelta = 0;
        int colOffsetDelta = 0;
        int colSizeDelta = 0;
        if (tableCoordinate.Row >= actualSize.Rows)
        {
            int excessRows = tableCoordinate.Row - actualSize.Rows + 1;
            rowOffsetDelta = Math.Min(format.Offset.Row, excessRows);
            rowSizeDelta = excessRows - rowOffsetDelta;
        }
        if (tableCoordinate.Column >= actualSize.Columns)
        {
            int excessCols = tableCoordinate.Column - actualSize.Columns + 1;
            colOffsetDelta = Math.Min(format.Offset.Column, excessCols);
            colSizeDelta = excessCols - colOffsetDelta;
        }
        if (rowOffsetDelta + colOffsetDelta > 0)
        {
            matrixFormat.Offset = new TableCoordinate(matrixFormat.Offset.Row - rowOffsetDelta, matrixFormat.Offset.Column - colOffsetDelta);
        }
        if (rowSizeDelta + colSizeDelta > 0)
        {
            matrixFormat.Size = new TableSize(matrixFormat.Size.Rows - rowSizeDelta, matrixFormat.Size.Columns - colSizeDelta);
        }
        return matrixFormat;
    }

    public void AddFormatChangedHandlerToNestedMatrixFormat(ActionByPropertyNameAndValue action)
    {
        FormatChanged += action;
        foreach (TableCoordinate populatedPosition in NestedMatrixFormats.PopulatedPositions)
        {
            NestedMatrixFormats[populatedPosition].AddFormatChangedHandlerToNestedMatrixFormat(action);
        }
    }

    public void RemoveFormatChangedHandlerFromNestedMatrixFormat(ActionByPropertyNameAndValue action)
    {
        foreach (TableCoordinate populatedPosition in NestedMatrixFormats.PopulatedPositions)
        {
            NestedMatrixFormats[populatedPosition].RemoveFormatChangedHandlerFromNestedMatrixFormat(action);
        }
        FormatChanged -= action;
    }

    public static MatrixFormat GetCommonMatrixFormat(IEnumerable<MatrixFormat> formats, MatrixFormat compareTo)
    {
        return PropertyAggregateExtension.GetCommonFormat(formats, compareTo, "ShowIndices", "CollapseNestedMatrices");
    }

    [MirrorVerified("PtcControlsCoreMatrixFormatMechanicalDifferentialTests")]
    public event PropertyChangedEventHandler PropertyChanged;

    protected void FirePropertyChanged(string propertyName)
    {
        if (PropertyChanged != null)
        {
            PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
        }
    }

    // Declared after op_Equality and Clone to preserve DisplayClass ordinals 32_0 and 34_0.
    public static readonly RoutedCommand SetMatrixFormatFieldCommand = new RoutedUICommand("Set the result matrix format field", "SetMatrixFormatField", typeof(MatrixFormat));
}
