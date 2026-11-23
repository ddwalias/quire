#nullable disable
using Quire.Mirror;

namespace Quire.EquationEditor.SpecTable.Model;

public partial class SpecTableMemento
{
    [MirrorVerified("Release IL")]
    public void Restore(ISpecTableModel model) => model.RestoreMemento(this);

    partial class a_
    {
        [MirrorVerified("Release IL")]
        public a_() { }

        [MirrorVerified("Release IL")]
        internal IColumnMemento a(ISpecTableColumn specTableColumn) => specTableColumn.CreateMemento();

        [MirrorVerified("Release IL")]
        internal ISpecTableColumn a(IColumnMemento columnMemento) => columnMemento.RestoreData();
    }
}
