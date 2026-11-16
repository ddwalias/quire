namespace Quire.EquationEditor.Tree;

public partial class ButtonScriptableControlData
{
    public ButtonScriptableControlData(string mathNodeName, Quire.Controls.Core.InputControlTypes inputControlTypes)
        : base(mathNodeName, inputControlTypes)
    {
        Height = a;
        Width = b;
    }

    [global::System.Runtime.CompilerServices.SkipLocalsInit]
    public override object Clone()
    {
        return new ButtonScriptableControlData(base.MathNodeName, base.InputControlType)
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
            SelectedCalculatedValue = base.SelectedCalculatedValue
        };
    }
}
