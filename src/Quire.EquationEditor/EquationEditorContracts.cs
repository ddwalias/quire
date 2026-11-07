namespace Quire.EquationEditor
{
    public partial interface IEquationEditorSettings
    {
        bool ParenthesizeOperands { get; }

        global::Quire.TableSize ResultMatrixMaxDisplaySize { get; }
    }

    public partial interface IEquationTriadBuilder
    {
        global::Quire.Controls.Core.IEquationControl BuildEquationControl(IEquationModel eqModel);
    }
}
