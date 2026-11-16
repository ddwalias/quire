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

    public InputControlEngineTypes EngineType
    {
        get => f;
        set => f = value;
    }

    public string EngineError
    {
        get => g;
        set => g = value;
    }

    public ScriptErrorType ErrorType
    {
        get => h;
        set => h = value;
    }

    public double SelectedValueIndex
    {
        get => i;
        set => i = value;
    }

    public System.Collections.Generic.IList<object> SelectedCalculatedValue
    {
        get => j;
        set => j = value;
    }

    public string RL
    {
        get => k;
        set => k = value;
    }

    public bool IsNeedsCalculate
    {
        get => l;
        set => l = value;
    }

    public string MathNodeName
    {
        get => m;
        set => m = value;
    }

    public bool HasCalculatedValue
    {
        get
        {
            System.Collections.Generic.IList<object> selectedCalculatedValue = SelectedCalculatedValue;
            if (selectedCalculatedValue != null)
            {
                return selectedCalculatedValue.Count > 0;
            }
            return false;
        }
    }

    protected InputControlDataCalculatedValue()
    {
    }
}
