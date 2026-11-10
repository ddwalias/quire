using Quire.EquationEditor.Equation;

namespace Quire.Controls;

public partial class ScaledEquationControl
{
    public ScaledEquationControl()
        : this(true)
    {
    }

    public ScaledEquationControl(bool allowExpressionsOnly)
        : base(new EquationModel("SPEC_TABLE_SCALING_CELL"))
    {
        AllowExpressionsOnly = allowExpressionsOnly;
    }

    public override string GetHelpContext()
    {
        string helpContext = base.GetHelpContext();
        return helpContext == EquationControlConstants.GetPlaceholderHelpContext("EquationControl")
            ? EquationControlConstants.GetPlaceholderHelpContext("ScaledEquationControl")
            : helpContext;
    }
}
