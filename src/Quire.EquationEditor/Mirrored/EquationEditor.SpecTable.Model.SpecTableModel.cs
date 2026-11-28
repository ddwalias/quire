#nullable disable
using System.Collections.Generic;
using System.Linq;
using System.Xml;
using Quire.Calculation.Core;
using Quire.Core;
using Quire.EquationEditor.Tree;
using Quire.Mirror;
using Quire.PersistentDataObjects;
using Quire.Undo;

namespace Quire.EquationEditor.SpecTable.Model;

public partial class SpecTableModel
{
    [MirrorName("a")]
    internal const int a__ = 3;

    [MirrorName("b")]
    private readonly List<ISpecTableColumn> b__ = new();

    [MirrorName("c")]
    private readonly Dictionary<ISpecTableColumn, SpecTableCalculatableWrapper> c__ = new();

    public event global::Quire.Delegate1<IEquationModel> CellAdded;

    public event global::Quire.Delegate0 TableChanged;

    public event global::Quire.Delegate1<ISpecTableColumn> ColumnAdded;

    public event global::Quire.Delegate1<ISpecTableColumn> ColumnRemoved;

    public event global::Quire.Delegate0 RequestRemove;

    public event global::System.Action RequestClearSelection;

    public event global::System.Action<int, int> RequestAddSelection;

    public event global::System.Action RequestUpdateCellsVisibility;

    public event global::System.Action<bool> RestrictedReadonlyChanged;

    private bool m;

    private static readonly string n;

    private bool o;

    public global::System.Collections.Generic.List<ISpecTableColumn> Columns => b__;

    public int ColumnCount => b__.Count;

    internal int e()
    {
        if (b__.Count != 0)
        {
            return b__[0].DataCellCount;
        }

        return 1;
    }

    internal int d() => b__[0].CellCount;

    public string SExpression
    {
        // The Mathcad getter emits initlocals False for its cached method-group delegate.
        [global::System.Runtime.CompilerServices.SkipLocalsInit]
        get => "(@TABLE " + string.Join(
            " ",
            Columns.Select(k_.a ?? (k_.a = new global::System.Func<ISpecTableColumn, string>(SpecTableColumn.GetSExpression)))) + ")";
    }

    public bool IsRestrictedReadOnly => m;

    public bool IsEmpty => b__.All(l_.g ??= l_.a_.a);

    public bool InCustomInitialization
    {
        get
        {
            DebugTools.DevAssert(Columns == null || Columns.All(a), null);
            return o;
        }
        set
        {
            m_ closure = new();
            closure.a_ = value;
            if (o == closure.a_)
            {
                return;
            }

            o = closure.a_;
            if (Columns != null)
            {
                Columns.Each(closure.a);
            }
        }
    }

    static SpecTableModel()
    {
        n = "@PLACEHOLDER";
        DebugTools.DevAssert(
            n == global::Quire.EquationEditor.Tree.DefaultDescriptorTable.Default["PLACEHOLDER"].OperatorSExpName,
            null);
    }

    public bool IsExtraDataCellOrEmptyScale(IEquationModel cell)
    {
        if (cell == null)
        {
            return false;
        }

        if (!cell.EquationRoot.IsPlaceholder)
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
        if (rowIndex < 0 || rowIndex >= e() + 2)
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
            AddColumn(Direction.Right, e() - 1, ColumnCount - 1);
        }
    }

    public void AddRowAtEnd()
    {
        if (ColumnCount <= 0)
        {
            return;
        }

        a(Direction.Down, d() - 1, ColumnCount - 1, 0);
    }

    public ISpecTableMemento CreateMemento() => new SpecTableMemento(Columns);

    public SpecTableCalculatableWrapper GetColumnWrapper(ISpecTableColumn column)
    {
        c__.TryGetValue(column, out SpecTableCalculatableWrapper wrapper);
        return wrapper;
    }

    public global::System.Collections.Generic.IEnumerable<SpecTableCalculatableWrapper> GetWrappedColumns()
        => new n_(-2) { d_ = this };

    public void RemoveColumn(global::Quire.EquationEditor.IEquationModel cell)
    {
        global::Quire.Discrete2DCoordinate coordinate = b(cell);
        h(coordinate.First, coordinate.Second);
    }

    public void RemoveRow(global::Quire.EquationEditor.IEquationModel cell)
    {
        global::Quire.Discrete2DCoordinate coordinate = b(cell);
        i(coordinate.First, coordinate.Second);
    }

    public void HandleOnDelete(global::Quire.EquationEditor.IEquationModel equationModel)
        => equationModel?.Delete(global::Quire.Direction.Right);

    public void HandleClearActiveCell(global::Quire.EquationEditor.IEquationModel equationModel)
        => equationModel?.ClearContent();

    public void HandleNavigateToLastCell()
    {
        global::Quire.Discrete2DCoordinate coordinate = default;
        coordinate.First = d() - 1;
        coordinate.Second = b__.Count - 1;
        a(coordinate, global::Quire.Direction.Right);
    }

    private global::Quire.EquationEditor.IEquationModel a(
        global::Quire.Discrete2DCoordinate coordinate,
        global::Quire.Direction direction)
    {
        global::Quire.EquationEditor.IEquationModel equationModel = e(coordinate.First, coordinate.Second);
        if (equationModel == null)
        {
            return null;
        }

        equationModel.MoveToRegionExtreme(direction);
        f(equationModel);
        return equationModel;
    }

    private global::Quire.EquationEditor.IEquationModel e(int rowIndex, int columnIndex)
    {
        if (columnIndex >= b__.Count || columnIndex < 0)
        {
            return null;
        }

        return GetColumn(columnIndex).GetCell(rowIndex);
    }

    private global::Quire.Discrete2DCoordinate b(
        global::Quire.Discrete2DCoordinate coordinate,
        global::Quire.Direction direction)
    {
        global::Quire.Discrete2DCoordinate result = coordinate;
        switch (direction)
        {
            case global::Quire.Direction.Right:
                result.Second++;
                break;
            case global::Quire.Direction.Up:
                result.First--;
                break;
            case global::Quire.Direction.Left:
                result.Second--;
                break;
            case global::Quire.Direction.Down:
                result.First++;
                break;
        }

        if (result.Second < 0)
        {
            result.Second = b__.Count - 1;
            result.First--;
        }
        else if (result.Second >= b__.Count)
        {
            result.Second = 0;
            result.First++;
        }

        return result;
    }

    private global::Quire.EquationEditor.IEquationModel a(
        global::Quire.Direction direction,
        global::Quire.Discrete2DCoordinate coordinate)
    {
        global::Quire.Direction reverseDirection = global::Quire.Direction.Left;
        if (direction == global::Quire.Direction.Left)
        {
            reverseDirection = global::Quire.Direction.Right;
        }

        return a(b(coordinate, direction), reverseDirection);
    }

    private global::Quire.Discrete2DCoordinate b(global::Quire.EquationEditor.IEquationModel equationModel)
    {
        global::Quire.Discrete2DCoordinate coordinate = default;
        foreach (ISpecTableColumn column in b__)
        {
            coordinate.First = 0;
            foreach (global::Quire.EquationEditor.IEquationModel cell in column)
            {
                if (cell == equationModel)
                {
                    return coordinate;
                }

                coordinate.First++;
            }

            coordinate.Second++;
        }

        throw new global::System.ArgumentException("Cell not found", "cell");
    }

    public bool CanNavigationBetweenCells(
        global::Quire.EquationEditor.IEquationModel cell,
        global::Quire.Direction navigationDirection,
        bool isJumpNavigation)
    {
        if (cell == null)
        {
            return false;
        }

        if (!isJumpNavigation)
        {
            global::Quire.Discrete2DCoordinate coordinate = b(cell);
            global::Quire.Discrete2DCoordinate nextCoordinate = b(coordinate, navigationDirection);
            ISpecTableColumn firstColumn = GetColumn(0);
            if (nextCoordinate.Second < 0
                || nextCoordinate.Second >= b__.Count
                || nextCoordinate.First < 0
                || nextCoordinate.First >= firstColumn.CellCount)
            {
                return false;
            }

            return true;
        }

        return true;
    }

    public void HandleNavigationBetweenCells(
        global::Quire.EquationEditor.IEquationModel cell,
        global::Quire.Direction navigationDirection,
        bool isJumpNavigation)
    {
        global::Quire.Discrete2DCoordinate coordinate = b(cell);
        if (!isJumpNavigation)
        {
            a(navigationDirection, coordinate);
            return;
        }

        if ((navigationDirection == global::Quire.Direction.Right && coordinate.Second == b__.Count - 1)
            || (navigationDirection == global::Quire.Direction.Left && coordinate.Second == 0))
        {
            return;
        }

        a(navigationDirection, coordinate).MoveToRegionExtreme(navigationDirection);
    }

    public void HandleOnTraverseForward(global::Quire.EquationEditor.IEquationModel equationModel)
    {
        global::Quire.Discrete2DCoordinate coordinate = b(equationModel);
        if (!IsRestrictedReadOnly && coordinate.Second == b__.Count - 1 && coordinate.First == e() + 1)
        {
            AddRowAtEnd();
            coordinate.Second = 0;
            coordinate.First++;
            a(coordinate, global::Quire.Direction.Right);
        }
        else
        {
            a(global::Quire.Direction.Right, coordinate);
        }
    }

    public void HandleOnTraverseBackward(global::Quire.EquationEditor.IEquationModel equationModel)
    {
        global::Quire.Discrete2DCoordinate coordinate = b(equationModel);
        a(global::Quire.Direction.Left, coordinate).MoveToRegionStart();
    }

    public MathNode CreateMatrixFromTableCells(global::Quire.DiscreteRect selected)
    {
        MathNode[] cells = new MathNode[(selected.Height + 1) * (selected.Width + 1)];
        int cellIndex = 0;
        for (int rowIndex = selected.Top; rowIndex <= selected.Bottom; rowIndex++)
        {
            for (int columnIndex = selected.Left; columnIndex <= selected.Right; columnIndex++)
            {
                cells[cellIndex++] = e(rowIndex, columnIndex).Root.Children[0].Clone();
            }
        }

        return global::Quire.Native.PtcEquationEditor.r.a_.a(selected.Height + 1, selected.Width + 1, cells);
    }

    private CompoundCommand f()
    {
        CompoundCommand command = new();
        using (new CommandScope(command))
        {
            for (int rowIndex = d(); rowIndex > 2; rowIndex--)
            {
                if (IsRowEmpty(rowIndex))
                {
                    i(rowIndex, 0);
                }
            }
        }

        return command;
    }

    public static SpecTableModel DefinitionsListToSpecTableModel(List<MathNode> definitionsMathNodesList)
    {
        List<MathNode> definitions = definitionsMathNodesList.Where(l_.b_ ??= l_.a_.a).ToList();
        if (definitions.Count == 0)
        {
            return null;
        }

        return new SpecTableModel(definitions);
    }

    private SpecTableCalculatableWrapper g(ISpecTableColumn column)
    {
        b__.Add(column);
        return f(column);
    }

    private SpecTableCalculatableWrapper f(ISpecTableColumn column)
    {
        SpecTableCalculatableWrapper wrapper = new(column);
        c__[column] = wrapper;
        return wrapper;
    }

    private static bool a(IEnumerable<ISpecTableColumn> columns)
    {
        if (columns.IsNullOrEmpty())
        {
            return false;
        }

        foreach (ISpecTableColumn column in columns)
        {
            if (column == null)
            {
                return false;
            }

            if (column.Header.Root[0].DescriptorName != "IDENTIFIER")
            {
                return false;
            }
        }

        return true;
    }

    private void e(ISpecTableColumn column) => c__.Remove(column);

    private void d(ISpecTableColumn column)
    {
        if (ColumnAdded != null)
        {
            ColumnAdded(column);
        }

        b();
    }

    private void c(ISpecTableColumn column)
    {
        if (ColumnAdded != null)
        {
            ColumnAdded(column);
        }
    }

    private void e(IEquationModel equationModel)
    {
        d(equationModel);
        a(equationModel);
    }

    private void d(IEquationModel equationModel) => equationModel.RequestRemoveEquation += a;

    private void c(IEquationModel equationModel) => equationModel.RequestRemoveEquation -= a;

    public void FireRequestUpdateCellsVisibility()
    {
        if (RequestUpdateCellsVisibility != null)
        {
            RequestUpdateCellsVisibility();
        }
    }

    public mathType[] GetColumnsRegionPersistentData()
    {
        IList<mathType> serializedColumns = new List<mathType>();
        foreach (ISpecTableColumn column in Columns)
        {
            mathType serializedColumn = new()
            {
                InnerXml = new XmlNode[] { column.EquationXmlNode },
                InnerXmlForXamlSerialization = new XmlNode[] { column.EquationXmlNode }
            };
            int resultRefId = ResultManager.Instance.CreateResultRefId(GetColumnWrapper(column));
            if (IResultManagerHelper.IsResultRefIdSpecified(resultRefId))
            {
                serializedColumn.resultRefSpecified = true;
                serializedColumn.resultRef = resultRefId;
            }

            serializedColumns.Add(serializedColumn);
        }

        return serializedColumns.ToArray();
    }

    public void SetColumnsRegionPersistentData(mathType[] columns)
    {
        c();
        foreach (mathType serializedColumn in columns)
        {
            ISpecTableColumn column = new SpecTableColumn(e, 0, InCustomInitialization)
            {
                EquationXmlNode = serializedColumn.InnerXml != null
                    ? serializedColumn.InnerXml[0]
                    : serializedColumn.InnerXmlForXamlSerialization[0]
            };
            column.SetRestrictedReadonly(IsRestrictedReadOnly);
            SpecTableCalculatableWrapper wrapper = g(column);
            if (serializedColumn.resultRefSpecified)
            {
                ResultManager.Instance.SetResultByRefId(serializedColumn.resultRef, wrapper);
            }

            d(column);
        }

        b();
    }

    public void RestoreMemento(SpecTableMemento modelMemento)
    {
        if (DebugTools.AssertNotNull(modelMemento, null).Failed)
        {
            return;
        }

        c();
        foreach (ISpecTableColumn column in modelMemento.RestoreData())
        {
            column.SetRestrictedReadonly(IsRestrictedReadOnly);
            g(column);
            d(column);
        }

        b();
    }

    public bool SubstituteAndCommitInputValue(
        string sExprId,
        string valueSExpr,
        string scalingSExpr,
        bool shouldMoveFocus = false)
    {
        if (valueSExpr.IsNullOrEmpty() || sExprId.IsNullOrEmpty())
        {
            return false;
        }

        return CommandBuffer.Execute(new a_(this, sExprId, valueSExpr, scalingSExpr, shouldMoveFocus));
    }

    public void Commit()
    {
        c__.Values.Each(l_.f ??= l_.a_.a);
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
        static l_() => a_ = new l_();

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

        internal bool a(MathNode mathNode) => mathNode.DescriptorName == "DEFINITION";
    }

    partial class m_
    {
        [MirrorVerified("Release IL")]
        public m_() { }

        internal void a(ISpecTableColumn column) => column.InCustomInitialization = a_;
    }

    private bool a(ISpecTableColumn column) => column.InCustomInitialization == o;


    [MirrorOf("Ptc.EquationEditor.SpecTable.Model.SpecTableModel+n, Ptc.EquationEditor")]
    private sealed partial class n_ :
        global::System.Collections.Generic.IEnumerable<SpecTableCalculatableWrapper>,
        global::System.Collections.IEnumerable,
        global::System.Collections.Generic.IEnumerator<SpecTableCalculatableWrapper>,
        global::System.Collections.IEnumerator,
        global::System.IDisposable
    {
        [MirrorName("a")]
        private int a_;

        [MirrorName("b")]
        private SpecTableCalculatableWrapper b_;

        [MirrorName("c")]
        private int c_;

        [MirrorName("d")]
        public SpecTableModel d_;

        [MirrorName("e")]
        private global::System.Collections.Generic.List<ISpecTableColumn>.Enumerator e_;

        public n_(int A_0)
        {
            a_ = A_0;
            c_ = global::System.Environment.CurrentManagedThreadId;
        }

        void global::System.IDisposable.Dispose()
            => throw global::Quire.Mirror.MirrorStub.NotImplemented();

        bool global::System.Collections.IEnumerator.MoveNext()
            => throw global::Quire.Mirror.MirrorStub.NotImplemented();

        private void a()
            => throw global::Quire.Mirror.MirrorStub.NotImplemented();

        void global::System.Collections.IEnumerator.Reset()
            => throw global::Quire.Mirror.MirrorStub.NotImplemented();

        SpecTableCalculatableWrapper
            global::System.Collections.Generic.IEnumerator<SpecTableCalculatableWrapper>.Current
            => throw global::Quire.Mirror.MirrorStub.NotImplemented();

        object global::System.Collections.IEnumerator.Current
            => throw global::Quire.Mirror.MirrorStub.NotImplemented();

        global::System.Collections.Generic.IEnumerator<SpecTableCalculatableWrapper>
            global::System.Collections.Generic.IEnumerable<SpecTableCalculatableWrapper>.GetEnumerator()
            => throw global::Quire.Mirror.MirrorStub.NotImplemented();

        global::System.Collections.IEnumerator global::System.Collections.IEnumerable.GetEnumerator()
            => throw global::Quire.Mirror.MirrorStub.NotImplemented();
    }
}
