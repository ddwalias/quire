#nullable disable
using System.Collections.Generic;
using System.Linq;
using System.Xml;
using Quire.Calculation.Core;
using Quire.Core;
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

    [field: MirrorName("d")]
    public event global::Quire.Delegate1<IEquationModel> CellAdded;

    [field: MirrorName("e")]
    public event global::Quire.Delegate0 TableChanged;

    [field: MirrorName("f")]
    public event global::Quire.Delegate1<ISpecTableColumn> ColumnAdded;

    [field: MirrorName("g")]
    public event global::Quire.Delegate1<ISpecTableColumn> ColumnRemoved;

    [field: MirrorName("h")]
    public event global::Quire.Delegate0 RequestRemove = () => { };

    [field: MirrorName("i")]
    public event global::System.Action RequestClearSelection;

    [field: MirrorName("j")]
    public event global::System.Action<int, int> RequestAddSelection;

    [field: MirrorName("k")]
    public event global::System.Action RequestUpdateCellsVisibility;

    [field: MirrorName("l")]
    public event global::System.Action<bool> RestrictedReadonlyChanged;

    private bool m;

    private static readonly string n;

    private bool o;

    public global::System.Collections.Generic.List<ISpecTableColumn> Columns => b__;

    public int ColumnCount => b__.Count;

    internal int NumberOfDataCells => b__.Count != 0 ? b__[0].DataCellCount : 1;

    internal int CellCount => b__[0].CellCount;

    public string SExpression => "(@TABLE " + string.Join(" ", Columns.Select(SpecTableColumn.GetSExpression)) + ")";

    public bool IsEmpty => b__.All(column => column.IsEmpty);

    static SpecTableModel()
    {
        n = "@PLACEHOLDER";
        DebugTools.DevAssert(
            n == global::Quire.EquationEditor.Tree.DefaultDescriptorTable.Default["PLACEHOLDER"].OperatorSExpName,
            null);
    }

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

    public ISpecTableMemento CreateMemento() => new SpecTableMemento(Columns);

    public SpecTableCalculatableWrapper GetColumnWrapper(ISpecTableColumn column)
    {
        c__.TryGetValue(column, out SpecTableCalculatableWrapper wrapper);
        return wrapper;
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
        c__.Values.Each(wrapper => wrapper.Commit());
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

        public SpecTableModel d;

        [MirrorName("e")]
        private global::System.Collections.Generic.List<ISpecTableColumn>.Enumerator e_;

        public n_(int A_0) => throw global::Quire.Mirror.MirrorStub.NotImplemented();

        void global::System.IDisposable.Dispose() => throw global::Quire.Mirror.MirrorStub.NotImplemented();

        bool global::System.Collections.IEnumerator.MoveNext() => throw global::Quire.Mirror.MirrorStub.NotImplemented();

        private void a() => throw global::Quire.Mirror.MirrorStub.NotImplemented();

        void global::System.Collections.IEnumerator.Reset() => throw global::Quire.Mirror.MirrorStub.NotImplemented();

        global::System.Collections.Generic.IEnumerator<SpecTableCalculatableWrapper>
            global::System.Collections.Generic.IEnumerable<SpecTableCalculatableWrapper>.GetEnumerator()
            => throw global::Quire.Mirror.MirrorStub.NotImplemented();

        global::System.Collections.IEnumerator global::System.Collections.IEnumerable.GetEnumerator()
            => throw global::Quire.Mirror.MirrorStub.NotImplemented();

        SpecTableCalculatableWrapper
            global::System.Collections.Generic.IEnumerator<SpecTableCalculatableWrapper>.Current
            => throw global::Quire.Mirror.MirrorStub.NotImplemented();

        object global::System.Collections.IEnumerator.Current
            => throw global::Quire.Mirror.MirrorStub.NotImplemented();
    }
}
