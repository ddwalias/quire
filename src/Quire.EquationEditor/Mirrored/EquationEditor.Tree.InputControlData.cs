namespace Quire.EquationEditor.Tree;

public abstract partial class InputControlData
{
    // Obfuscated original field names: the Release IL keeps them.
    private Quire.Controls.Core.InputControlTypes a;
    private System.Windows.Size b;
    private string c;
    private string d;
    private double e;

    public Quire.Controls.Core.InputControlTypes InputControlType
    {
        get => a;
        set => a = value;
    }

    public System.Windows.Size OutputFieldSize
    {
        get => b;
        set => b = value;
    }

    public string Background
    {
        get => c;
        set => c = value;
    }

    public string Foreground
    {
        get => d;
        set => d = value;
    }

    public double FontSize
    {
        get => e;
        set => e = value;
    }
}
