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

    [global::System.Runtime.CompilerServices.SkipLocalsInit]
    public override object Clone()
    {
        return new ListBoxScriptableControlData(base.MathNodeName, base.InputControlType)
        {
            SelectedValueIndex = base.SelectedValueIndex,
            Script = base.Script,
            EngineType = base.EngineType,
            Width = base.Width,
            Height = base.Height,
            IsInputsExists = base.IsInputsExists,
            IsInputsAreaShown = base.IsInputsAreaShown,
            InputFieldSExpression = base.InputFieldSExpression,
            InputsResultValue = base.InputsResultValue,
            Values = base.Values,
            EngineError = base.EngineError,
            SelectedCalculatedValue = base.SelectedCalculatedValue,
            Sort = Sort,
            FlowDirection = FlowDirection
        };
    }
}
