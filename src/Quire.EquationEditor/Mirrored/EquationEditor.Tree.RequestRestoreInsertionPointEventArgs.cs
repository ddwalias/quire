namespace Quire.EquationEditor.Tree;

public partial class RequestRestoreInsertionPointEventArgs
{
    private readonly View.IVisualMathNode a;
    private bool b;

    public RequestRestoreInsertionPointEventArgs(View.IVisualMathNode originalSource)
    {
        a = originalSource;
    }

    public View.IVisualMathNode OriginalSource => a;

    public bool RequestHandled
    {
        get => b;
        set => b = value;
    }
}
