namespace Quire.EquationEditor.Tree;

public partial class FunctionOperatorDescriptor
{
    // Obfuscated original field names: the Release IL keeps them.
    private string a;
    private int b;
    private string c;
    private System.Collections.Generic.IList<Quire.Controls.Core.IdLabels?> d;

    public string Name
    {
        get => a;
        set => a = value;
    }

    public int ArgumentCount
    {
        get => b;
        set => b = value;
    }

    public string RlFormat
    {
        get => c;
        set => c = value;
    }

    public System.Collections.Generic.IList<Quire.Controls.Core.IdLabels?> ArgumentLabels
    {
        get => d;
        set => d = value;
    }

    public FunctionOperatorDescriptor()
    {
    }
}
