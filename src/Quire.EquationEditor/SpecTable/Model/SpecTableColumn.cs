#nullable disable
using System;
using System.Collections.Generic;
using Quire.Mirror;
using Quire.Native.PtcEquationEditor;

namespace Quire.EquationEditor.SpecTable.Model;

public partial class SpecTableColumn
{
    [MirrorVerified("Release IL")]
    public SpecTableColumn() : this(null, 1, false) { }

    [MirrorVerified("Release IL")]
    public IColumnMemento CreateMemento() => new ColumnMemento(this);

    [MirrorVerified("Release IL")]
    private readonly IEquationModel b;

    [MirrorVerified("Release IL")]
    [MirrorName("a")]
    private readonly IEquationModel a__;

    [MirrorVerified("Release IL")]
    [MirrorName("c")]
    private readonly List<IEquationModel> c__;

    [MirrorVerified("Release IL")]
    [MirrorName("d")]
    private readonly Delegate1<IEquationModel> d__;

    [MirrorVerified("Release IL")]
    public string GetScaleSExpression() => b.EquationSExpression;

    [MirrorVerified("Release IL")]
    public IEquationModel Header => a__;

    [MirrorVerified("Release IL")]
    public IEnumerable<IEquationModel> Data => c__;

    [MirrorVerified("Release IL")]
    public int CellCount => c__.Count + 2;

    [MirrorVerified("Release IL")]
    public int DataCellCount => c__.Count;

    [MirrorVerified("Release IL")]
    public string Rl => SpecTableColumnToRlTransform.Default.Transform(this, out e);

    [MirrorVerified("Release IL")]
    public string SExpression => $"(:= {Header.EquationSExpression} {GetAllValuesScaledSExpression})";

    [MirrorVerified("Release IL")]
    public string GetAllValuesScaledSExpression
    {
        get
        {
            int count = c__.Count;
            string arg = count == 1 ? c__[0].EquationSExpression : $"(@MATRIX {count} 1 {GetDataSExpression()})";
            return $"(@SCALE {arg} {GetScaleSExpression()})";
        }
    }

    [MirrorVerified("Release IL")]
    public string GetHeaderSExpression() => a__.EquationSExpression;

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
    public static string GetSExpression(ISpecTableColumn column)
    {
        if (column == null)
        {
            throw new ArgumentNullException(nameof(column));
        }
        return column.SExpression;
    }

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
                return b;
            default:
                return c__[index - 2];
        }
    }

    [MirrorVerified("Release IL")]
    public IEquationModel GetDataCell(int index) => c__[index];

    [MirrorVerified("Release IL")]
    private void a(int index, IEquationModel equationModel) => a(equationModel, index);

    [MirrorVerified("Release IL")]
    public IEquationModel ScalingFactor => b;

    partial class a_
    {
        [MirrorVerified("Release IL")]
        public void Redo() => Do();
    }

    partial class b_
    {
        [MirrorVerified("Release IL")]
        private readonly SpecTableColumn a;

        [MirrorVerified("Release IL")]
        private readonly int b;

        [MirrorVerified("Release IL")]
        private IEquationModel c;

        [MirrorVerified("Release IL")]
        public void Undo() => a.a(c, b);

        [MirrorVerified("Release IL")]
        public void Redo() => Do();
    }

    partial class c_
    {
        [MirrorVerified("Release IL")]
        public c_() { }

        [MirrorVerified("Release IL")]
        internal IEquationModel a(k first) => first.a();

        [MirrorVerified("Release IL")]
        internal bool a(IEquationModel equationModel) => equationModel.IsExpressionValueModified;
    }

    partial class d_
    {
        [MirrorVerified("Release IL")]
        public d_() { }
    }
}
