namespace Quire.Controls;

public partial class VisualIdDescriptor
{
    public global::System.Nullable<global::Quire.Controls.Core.IdLabels> Label
    {
        get => _label;
        set => _label = value;
    }

    public global::Quire.EquationEditor.Tree.FormattedString IdName
    {
        get => _idName;
        set => _idName = value;
    }
}
