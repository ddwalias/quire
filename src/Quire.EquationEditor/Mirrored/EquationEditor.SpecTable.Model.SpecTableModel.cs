#nullable disable
using Quire.Mirror;

namespace Quire.EquationEditor.SpecTable.Model;

public partial class SpecTableModel
{
    public global::System.Collections.Generic.List<ISpecTableColumn> Columns => b__;

    public int ColumnCount => b__.Count;

    internal int NumberOfDataCells => b__.Count != 0 ? b__[0].DataCellCount : 1;

    internal int CellCount => b__[0].CellCount;

    public bool IsExtraDataCellOrEmptyScale(IEquationModel cell)
    {
        if (cell == null || !cell.EquationRoot.IsPlaceholder)
        {
            return false;
        }

        if (cell.Root.DescriptorName == "SPEC_TABLE_HEADER")
        {
            return false;
        }

        if (cell.Root.DescriptorName == "SPEC_TABLE_SCALING_CELL")
        {
            return true;
        }

        foreach (ISpecTableColumn column in b__)
        {
            for (int dataCellIndex = column.DataCellCount - 1; dataCellIndex > 0; dataCellIndex--)
            {
                IEquationModel dataCell = column.GetDataCell(dataCellIndex);
                if (!dataCell.EquationRoot.IsPlaceholder)
                {
                    break;
                }

                if (dataCell == cell)
                {
                    return true;
                }
            }
        }

        return false;
    }

    public void RemoveEmptyColumns()
    {
        for (int columnIndex = ColumnCount - 1; columnIndex >= 0; columnIndex--)
        {
            if (GetColumn(columnIndex).IsEmpty)
            {
                h(0, columnIndex);
            }
        }
    }

    public bool IsRowEmpty(int rowIndex)
    {
        if (rowIndex < 0 || rowIndex >= NumberOfDataCells + 2)
        {
            return true;
        }

        foreach (ISpecTableColumn column in b__)
        {
            if (!column.GetCell(rowIndex).EquationRoot.IsPlaceholder)
            {
                return false;
            }
        }

        return true;
    }

    public bool IsColumnEmpty(int colIndex)
    {
        if (colIndex < 0 || colIndex >= ColumnCount)
        {
            return true;
        }

        return b__[colIndex].IsEmpty;
    }

    public ISpecTableColumn GetColumn(int i) => b__[i];

    public void AddColumnAtEnd()
    {
        if (ColumnCount == 0)
        {
            AddColumn(Direction.Right, 0, 0);
        }
        else
        {
            AddColumn(Direction.Right, NumberOfDataCells - 1, ColumnCount - 1);
        }
    }

    public void AddRowAtEnd()
    {
        if (ColumnCount > 0)
        {
            a(Direction.Down, CellCount - 1, ColumnCount - 1, 0);
        }
    }

    [MirrorVerified("Release IL")]
    public SpecTableModel() : this(1, 1) { }

    [MirrorVerified("Release IL")]
    private static void f(IEquationModel equationModel) => equationModel.FireRequestFocus();

    [MirrorVerified("Release IL")]
    public void HandleNavigateToFirstCell() => a(Discrete2DCoordinate.Origin, Direction.Left);

    [MirrorVerified("Release IL")]
    public ISpecTableColumn this[int index] => GetColumn(index);

    public void HandleInputTagChanged(bool isInput) => a(isInput);

    private void a(bool isRestrictedReadonly)
    {
        if (m == isRestrictedReadonly)
        {
            return;
        }
        m = isRestrictedReadonly;
        foreach (ISpecTableColumn column in Columns)
        {
            column.SetRestrictedReadonly(m);
        }
        if (RestrictedReadonlyChanged != null)
        {
            RestrictedReadonlyChanged(m);
        }
    }

    partial class a_
    {
        [MirrorVerified("Release IL")]
        public void Redo() => Do();
    }

    partial class d_
    {
        [MirrorVerified("Release IL")]
        public void Redo() => Do();
    }

    partial class i_
    {
        [MirrorVerified("Release IL")]
        public void Redo() => Do();
    }

    partial class l_
    {
        [MirrorVerified("Release IL")]
        public l_() { }

        [MirrorVerified("Release IL")]
        internal void c() { }

        [MirrorVerified("Release IL")]
        internal void b() { }

        [MirrorVerified("Release IL")]
        internal void a() { }

        [MirrorVerified("Release IL")]
        internal void a(SpecTableCalculatableWrapper specTableCalculatableWrapper) => specTableCalculatableWrapper.Commit();

        [MirrorVerified("Release IL")]
        internal bool a(ISpecTableColumn specTableColumn) => specTableColumn.IsEmpty;
    }

    partial class m_
    {
        [MirrorVerified("Release IL")]
        public m_() { }
    }
}
