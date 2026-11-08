namespace Quire.EquationEditor.Tree;

public partial class ListBoxScriptableControlData
{
    // Obfuscated original names: the Release IL keeps these field and constant names.
    private const int a = 70;
    private const int b = 140;
    private bool c;
    private System.Windows.FlowDirection d;

    public bool Sort
    {
        get => c;
        set => c = value;
    }

    public System.Windows.FlowDirection FlowDirection
    {
        get => d;
        set => d = value;
    }

    public ListBoxScriptableControlData(string mathNodeName, Quire.Controls.Core.InputControlTypes inputControlTypes)
        : base(mathNodeName, inputControlTypes)
    {
        Height = a;
        Width = b;
    }
}
