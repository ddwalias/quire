using Quire.Controls.Core;

namespace Quire.EquationEditor.Tree;

public partial class InputScriptableControlDataWithInputs
{
    private string e;
    protected int f;
    private bool g;
    private bool h;
    private bool i;
    private int? j;
    private object[] k;
    private double l;
    private double m;

    public InputScriptableControlDataWithInputs(string mathNodeName, InputControlTypes inputControlTypes)
        : base(mathNodeName, inputControlTypes)
    {
    }

    public InputScriptableControlDataWithInputs(string mathNodeName, InputControlTypes inputControlTypes, Transform.ParsedSExpression parsedSExp)
        : base(mathNodeName, inputControlTypes, parsedSExp)
    {
        IInputControlData control = Transform.ParsedSExpressionToComboBoxInputControlData.Transform(
            mathNodeName, inputControlTypes, parsedSExp);
        IInputResizableControlData resizableControl = control as IInputResizableControlData;
        if (resizableControl != null)
        {
            Height = resizableControl.Height;
            Width = resizableControl.Width;
        }
        IInputScriptableControlDataWithInputs scriptableControl = control as IInputScriptableControlDataWithInputs;
        if (scriptableControl != null)
            InputFieldSExpression = scriptableControl.InputFieldSExpression;
    }

    public string InputFieldSExpression
    {
        get => e;
        set => e = value;
    }

    public bool IsInputsAreaShown
    {
        get => g;
        set => g = value;
    }

    public bool IsInputsExists
    {
        get => h;
        set => h = value;
    }

    public bool IsNewInputResults
    {
        get => i;
        set => i = value;
    }

    public int? InputFieldSize
    {
        get => j;
        set => j = value;
    }

    public object[] InputsResultValue
    {
        get => k;
        set => k = value;
    }

    public double Height
    {
        get => l;
        set => l = value;
    }

    public double Width
    {
        get => m;
        set => m = value;
    }

    public int Count
    {
        get => Math.Max(Values?.Count ?? 0, f);
        set => f = value;
    }

    public override bool IsEquivalentTo(IInputControlData other)
    {
        InputScriptableControlDataWithInputs control = other as InputScriptableControlDataWithInputs;
        return control != null && ScriptHashCode.Equals(control.ScriptHashCode) &&
            Height == control.Height && Width == control.Width;
    }
}
