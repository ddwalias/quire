namespace Quire.EquationEditor.Tree;

public abstract partial class InputControlDataCalculatedValue
{
    // Obfuscated original field names: the Release IL keeps them.
    protected InputControlEngineTypes f;
    private string g;
    private ScriptErrorType h;
    private double i;
    private System.Collections.Generic.IList<object> j;
    private string k;
    private bool l;
    private string m;

    public virtual InputControlEngineTypes EngineType
    {
        get => f;
        set => f = value;
    }

    public virtual string EngineError
    {
        get => g;
        set => g = value;
    }

    public virtual ScriptErrorType ErrorType
    {
        get => h;
        set => h = value;
    }

    public virtual double SelectedValueIndex
    {
        get => i;
        set => i = value;
    }

    public virtual System.Collections.Generic.IList<object> SelectedCalculatedValue
    {
        get => j;
        set => j = value;
    }

    public virtual string RL
    {
        get => k;
        set => k = value;
    }

    public virtual bool IsNeedsCalculate
    {
        get => l;
        set => l = value;
    }

    public virtual string MathNodeName
    {
        get => m;
        set => m = value;
    }

    protected InputControlDataCalculatedValue()
    {
    }
}
