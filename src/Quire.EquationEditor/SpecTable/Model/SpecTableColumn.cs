#nullable disable
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
    public string GetScaleSExpression() => b.EquationSExpression;

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
