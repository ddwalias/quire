namespace Quire.Controls.Internal;

internal partial class VisualEmptyResult
{
    public override string Text => global::Quire.Controls.EquationProperties.Settings.Default.EvalPlaceholder;

    public VisualEmptyResult()
    {
        MathFormat.IsItalic = true;
    }
}
