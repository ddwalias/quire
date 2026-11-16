using Quire.Controls.Core;
using Quire.EquationEditor.Tree;

namespace Quire.Controls;

public partial class VisualIdDescriptor
{
    protected IdLabels? _label;
    protected FormattedString _idName;

    public VisualIdDescriptor()
    {
        _command = EquationCommands.InsertId;
    }
}
