#nullable disable
using System;
using System.Collections;
using System.Collections.Generic;
using System.Xml;
using Quire.Mirror;
using Quire.Native.PtcEquationEditor;

namespace Quire.EquationEditor.SpecTable.Model;

// The whole SpecTableColumn layout in Mathcad IL order: fields, then methods and accessors, then the nested classes.
// Unfinished members are placeholders with [MirrorStub] and [MirrorBlocked], which the gate reports as unfinished.
public partial class SpecTableColumn
{
    [MirrorVerified("Release IL")]
    [MirrorName("a")]
    private readonly IEquationModel a__;

    [MirrorVerified("Release IL")]
    [MirrorName("b")]
    private readonly IEquationModel b__;

    [MirrorVerified("Release IL")]
    [MirrorName("c")]
    private readonly List<IEquationModel> c__;

    [MirrorVerified("Release IL")]
    [MirrorName("d")]
    private readonly Delegate1<IEquationModel> d__;

    [MirrorVerified("Release IL")]
    private List<Pair<IEquationModel, uint>> e;

    [MirrorVerified("Release IL")]
    private bool f;

    [MirrorVerified("Release IL")]
    private IEquationModel g;

    [MirrorVerified("Release IL")]
    private bool h;

    [MirrorVerified("Release IL")]
    public SpecTableColumn() : this(null, 1, false) { }

    [global::Quire.Mirror.MirrorStub]
    [global::Quire.Mirror.MirrorBlocked("unfinished: constructor stores before the base call")]
    public SpecTableColumn(Delegate1<IEquationModel> cellCreatedCallback, int dataCells, bool inCustomInitialization = false) => throw MirrorStub.NotImplemented();

    [MirrorVerified("Release IL")]
    public IColumnMemento CreateMemento() => new ColumnMemento(this);

    [global::Quire.Mirror.MirrorStub]
    [global::Quire.Mirror.MirrorBlocked("unfinished")]
    public void RestoreMemento(ColumnMemento columnMemento) => throw MirrorStub.NotImplemented();

    [global::Quire.Mirror.MirrorStub]
    [global::Quire.Mirror.MirrorBlocked("unfinished")]
    private void a(IList<IEquationModel> cells) => throw MirrorStub.NotImplemented();

    [MirrorVerified("Release IL")]
    public string GetHeaderSExpression() => a__.EquationSExpression;

    [MirrorVerified("Release IL")]
    public string GetScaleSExpression() => b__.EquationSExpression;

    [MirrorVerified("Release IL")]
    public string GetDataSExpression()
    {
        string[] array = new string[c__.Count];
        for (int i = 0; i < c__.Count; i++)
        {
            array[i] = c__[i].EquationSExpression;
        }
        return string.Join(" ", array);
    }

    [MirrorVerified("Release IL")]
    public IEquationModel Header => a__;

    [MirrorVerified("Release IL")]
    public IEquationModel ScalingFactor => b__;

    [MirrorVerified("Release IL")]
    public IEnumerable<IEquationModel> Data => c__;

    [MirrorVerified("Release IL")]
    public int CellCount => c__.Count + 2;

    [MirrorVerified("Release IL")]
    public string Rl => SpecTableColumnToRlTransform.Default.Transform(this, out e);

    [MirrorVerified("Release IL")]
    public int DataCellCount => c__.Count;

    [MirrorVerified("Release IL")]
    public static string GetSExpression(ISpecTableColumn column)
    {
        if (column == null)
        {
            throw new ArgumentNullException(nameof(column));
        }
        return column.SExpression;
    }

    // Mathcad formats these with string.Format; an interpolated string would call DefaultInterpolatedStringHandler.
    [MirrorVerified("Release IL")]
    public string SExpression => string.Format("(:= {0} {1})", Header.EquationSExpression, GetAllValuesScaledSExpression);

    [MirrorVerified("Release IL")]
    public string GetAllValuesScaledSExpression
    {
        get
        {
            int count = c__.Count;
            string arg = count == 1 ? c__[0].EquationSExpression : string.Format("(@MATRIX {0} 1 {1})", count, GetDataSExpression());
            return string.Format("(@SCALE {0} {1})", arg, GetScaleSExpression());
        }
    }

    [global::Quire.Mirror.MirrorStub]
    [global::Quire.Mirror.MirrorBlocked("unfinished")]
    public XmlNode EquationXmlNode { get => throw MirrorStub.NotImplemented(); set => throw MirrorStub.NotImplemented(); }

    [global::Quire.Mirror.MirrorStub]
    [global::Quire.Mirror.MirrorBlocked("unfinished")]
    private void a(XmlNode node, XmlNamespaceManager namespaces) => throw MirrorStub.NotImplemented();

    [global::Quire.Mirror.MirrorStub]
    [global::Quire.Mirror.MirrorBlocked("unfinished")]
    private XmlNode a(string namespaceUri) => throw MirrorStub.NotImplemented();

    [MirrorVerified("Release IL")]
    public void SetRestrictedReadonly(bool isRestrictedReadonly)
    {
        Header.IsRestrictedReadOnly = isRestrictedReadonly;
        ScalingFactor.IsRestrictedReadOnly = isRestrictedReadonly;
        foreach (IEquationModel cell in Data)
        {
            cell.IsRestrictedReadOnly = isRestrictedReadonly;
        }
    }

    [global::Quire.Mirror.MirrorStub]
    [global::Quire.Mirror.MirrorBlocked("unfinished: closure over this")]
    public bool IsEmpty { get => throw MirrorStub.NotImplemented(); }

    [MirrorVerified("Release IL")]
    public void AddCell() => InsertDataCell(c__.Count);

    [global::Quire.Mirror.MirrorStub]
    [global::Quire.Mirror.MirrorBlocked("unfinished")]
    private void c() => throw MirrorStub.NotImplemented();

    [MirrorVerified("Release IL")]
    public IEquationModel GetCell(int index)
    {
        if (index < 0 || index >= CellCount)
        {
            return null;
        }
        switch (index)
        {
            case 0:
                return a__;
            case 1:
                return b__;
            default:
                return c__[index - 2];
        }
    }

    [MirrorVerified("Release IL")]
    public IEquationModel GetDataCell(int index) => c__[index];

    [global::Quire.Mirror.MirrorStub]
    [global::Quire.Mirror.MirrorBlocked("unfinished")]
    public void InsertDataCell(int indexToInsert) => throw MirrorStub.NotImplemented();

    [global::Quire.Mirror.MirrorStub]
    [global::Quire.Mirror.MirrorBlocked("unfinished")]
    public void RemoveDataCell(int index) => throw MirrorStub.NotImplemented();

    [global::Quire.Mirror.MirrorStub]
    [global::Quire.Mirror.MirrorBlocked("unfinished")]
    private void c(IEquationModel cell) => throw MirrorStub.NotImplemented();

    [global::Quire.Mirror.MirrorStub]
    [global::Quire.Mirror.MirrorBlocked("unfinished: iterator")]
    public IEnumerator<IEquationModel> GetEnumerator() => throw MirrorStub.NotImplemented();

    [global::Quire.Mirror.MirrorStub]
    [global::Quire.Mirror.MirrorBlocked("unfinished")]
    IEnumerator IEnumerable.GetEnumerator() => throw MirrorStub.NotImplemented();

    [global::Quire.Mirror.MirrorStub]
    [global::Quire.Mirror.MirrorBlocked("unfinished: closure over this")]
    public bool IsExpressionValueModified { get => throw MirrorStub.NotImplemented(); }

    [global::Quire.Mirror.MirrorStub]
    [global::Quire.Mirror.MirrorBlocked("unfinished")]
    public void ClearIsExpressionValueModified() => throw MirrorStub.NotImplemented();

    [global::Quire.Mirror.MirrorStub]
    [global::Quire.Mirror.MirrorBlocked("unfinished: the setter is Mathcad method b")]
    public IEquationModel ErrorCell { get => throw MirrorStub.NotImplemented(); private set => throw MirrorStub.NotImplemented(); }

    [global::Quire.Mirror.MirrorStub]
    [global::Quire.Mirror.MirrorBlocked("unfinished")]
    private void b(IEquationModel value) => throw MirrorStub.NotImplemented();

    [global::Quire.Mirror.MirrorStub]
    [global::Quire.Mirror.MirrorBlocked("unfinished")]
    private bool b() => throw MirrorStub.NotImplemented();

    [global::Quire.Mirror.MirrorStub]
    [global::Quire.Mirror.MirrorBlocked("unfinished")]
    private void a(bool isRestrictedReadonly) => throw MirrorStub.NotImplemented();

    [global::Quire.Mirror.MirrorStub]
    [global::Quire.Mirror.MirrorBlocked("unfinished")]
    private void a() => throw MirrorStub.NotImplemented();

    [global::Quire.Mirror.MirrorStub]
    [global::Quire.Mirror.MirrorBlocked("unfinished: calls the unfinished helpers")]
    public void HandleError(Quire.Calculation.Core.EngineError calculationError) => throw MirrorStub.NotImplemented();

    [global::Quire.Mirror.MirrorStub]
    [global::Quire.Mirror.MirrorBlocked("unfinished")]
    private void a(IEquationModel cell, int index) => throw MirrorStub.NotImplemented();

    [global::Quire.Mirror.MirrorStub]
    [global::Quire.Mirror.MirrorBlocked("unfinished")]
    private IEquationModel a(int index) => throw MirrorStub.NotImplemented();

    [global::Quire.Mirror.MirrorStub]
    [global::Quire.Mirror.MirrorBlocked("unfinished")]
    public void Dispose() => throw MirrorStub.NotImplemented();

    [global::Quire.Mirror.MirrorStub]
    [global::Quire.Mirror.MirrorBlocked("unfinished")]
    public bool InCustomInitialization { get => throw MirrorStub.NotImplemented(); set => throw MirrorStub.NotImplemented(); }

    [MirrorVerified("Release IL")]
    private void a(int index, IEquationModel equationModel) => a(equationModel, index);

    [global::Quire.Mirror.MirrorStub]
    [global::Quire.Mirror.MirrorBlocked("unfinished")]
    private bool a(IEquationModel cell) => throw MirrorStub.NotImplemented();

    private partial class a_ : global::Quire.Undo.IUndoableCommand
    {
        [MirrorVerified("Release IL")]
        private readonly SpecTableColumn a;

        [MirrorVerified("Release IL")]
        private readonly int b;

        [MirrorVerified("Release IL")]
        private IEquationModel c;

        [global::Quire.Mirror.MirrorStub]
        [global::Quire.Mirror.MirrorBlocked("unfinished")]
        public string Name { get => throw MirrorStub.NotImplemented(); }

        [global::Quire.Mirror.MirrorStub]
        [global::Quire.Mirror.MirrorBlocked("unfinished")]
        public a_(SpecTableColumn column, int index) { }

        [global::Quire.Mirror.MirrorStub]
        [global::Quire.Mirror.MirrorBlocked("unfinished")]
        public bool Do() => throw MirrorStub.NotImplemented();

        [global::Quire.Mirror.MirrorStub]
        [global::Quire.Mirror.MirrorBlocked("unfinished")]
        public void Undo() => throw MirrorStub.NotImplemented();

        [MirrorVerified("Release IL")]
        public void Redo() => Do();
    }

    private partial class b_ : global::Quire.Undo.IUndoableCommand
    {
        [MirrorVerified("Release IL")]
        private readonly SpecTableColumn a;

        [MirrorVerified("Release IL")]
        private readonly int b;

        [MirrorVerified("Release IL")]
        private IEquationModel c;

        [global::Quire.Mirror.MirrorStub]
        [global::Quire.Mirror.MirrorBlocked("unfinished")]
        public string Name { get => throw MirrorStub.NotImplemented(); }

        [global::Quire.Mirror.MirrorStub]
        [global::Quire.Mirror.MirrorBlocked("unfinished")]
        public b_(SpecTableColumn column, int index) { }

        [global::Quire.Mirror.MirrorStub]
        [global::Quire.Mirror.MirrorBlocked("unfinished")]
        public bool Do() => throw MirrorStub.NotImplemented();

        [MirrorVerified("Release IL")]
        public void Undo() => a.a(c, b);

        [MirrorVerified("Release IL")]
        public void Redo() => Do();
    }

    private sealed partial class c_
    {
        [MirrorVerified("Release IL")]
        [MirrorName("a")]
        public static readonly c_ a_ = new c_();

        [global::Quire.Mirror.MirrorStub]
        [global::Quire.Mirror.MirrorName("b")]
        public static Func<Quire.Native.PtcEquationEditor.k, IEquationModel> b_;

        [global::Quire.Mirror.MirrorStub]
        public static Func<IEquationModel, bool> c;

        [global::Quire.Mirror.MirrorStub]
        public static Predicate<IEquationModel> d;

        [MirrorVerified("Release IL")]
        public c_() { }

        [MirrorVerified("Release IL")]
        internal IEquationModel a(k first) => first.a();

        [global::Quire.Mirror.MirrorStub]
        [global::Quire.Mirror.MirrorBlocked("unfinished")]
        internal bool b(IEquationModel cell) => throw MirrorStub.NotImplemented();

        [MirrorVerified("Release IL")]
        internal bool a(IEquationModel equationModel) => equationModel.IsExpressionValueModified;
    }

    private sealed partial class d_
    {
        [MirrorVerified("Release IL")]
        public d_() { }

        [global::Quire.Mirror.MirrorStub]
        [global::Quire.Mirror.MirrorBlocked("unfinished")]
        internal void a(IEquationModel cell) => throw MirrorStub.NotImplemented();
    }

    private sealed partial class e_
    {
        [global::Quire.Mirror.MirrorStub]
        [global::Quire.Mirror.MirrorName("a")]
        private int a_;

        [global::Quire.Mirror.MirrorStub]
        [global::Quire.Mirror.MirrorName("b")]
        private IEquationModel b_;

        [global::Quire.Mirror.MirrorStub]
        [global::Quire.Mirror.MirrorName("c")]
        public SpecTableColumn c_;

        [global::Quire.Mirror.MirrorStub]
        private IEnumerator<IEquationModel> d;

        [global::Quire.Mirror.MirrorStub]
        [global::Quire.Mirror.MirrorBlocked("unfinished: iterator")]
        public e_(int state) => throw MirrorStub.NotImplemented();

        [global::Quire.Mirror.MirrorStub]
        [global::Quire.Mirror.MirrorBlocked("unfinished")]
        void IDisposable.Dispose() => throw MirrorStub.NotImplemented();

        [global::Quire.Mirror.MirrorStub]
        [global::Quire.Mirror.MirrorBlocked("unfinished: iterator")]
        bool IEnumerator.MoveNext() => throw MirrorStub.NotImplemented();

        [global::Quire.Mirror.MirrorStub]
        [global::Quire.Mirror.MirrorBlocked("unfinished")]
        private void a() => throw MirrorStub.NotImplemented();

        [global::Quire.Mirror.MirrorStub]
        [global::Quire.Mirror.MirrorBlocked("unfinished")]
        void IEnumerator.Reset() => throw MirrorStub.NotImplemented();

        [global::Quire.Mirror.MirrorStub]
        [global::Quire.Mirror.MirrorBlocked("unfinished")]
        IEquationModel IEnumerator<IEquationModel>.Current => throw MirrorStub.NotImplemented();

        [global::Quire.Mirror.MirrorStub]
        [global::Quire.Mirror.MirrorBlocked("unfinished")]
        object IEnumerator.Current => throw MirrorStub.NotImplemented();
    }
}
