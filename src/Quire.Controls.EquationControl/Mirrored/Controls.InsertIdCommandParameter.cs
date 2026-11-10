using Quire.Controls.Core;
using Quire.EquationEditor.Tree;

namespace Quire.Controls;

public partial class InsertIdCommandParameter
{
    private FormattedString _name;
    private IdLabels? _label;

    public InsertIdCommandParameter(FormattedString name, IdLabels? label)
    {
        _name = name;
        _label = label;
    }

    public FormattedString Name
    {
        get => _name;
        set => _name = value;
    }

    public IdLabels? Label
    {
        get => _label;
        set => _label = value;
    }
}
