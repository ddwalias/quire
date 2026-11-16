using System.Windows;
using System.Windows.Controls;

namespace Quire.EquationEditor.Tree;

public partial class RadioButtonScriptableControlData
{
    private Orientation c = Orientation.Vertical;

    private FlowDirection d;

    public RadioButtonScriptableControlData(string mathNodeName, Quire.Controls.Core.InputControlTypes inputControlTypes)
        : base(mathNodeName, inputControlTypes)
    {
        Height = a;
        Width = b;
    }

    public Orientation Orientation
    {
        get => c;
        set => c = value;
    }

    public FlowDirection FlowDirection
    {
        get => d;
        set => d = value;
    }

    [global::System.Runtime.CompilerServices.SkipLocalsInit]
    public override object Clone()
    {
        return new RadioButtonScriptableControlData(base.MathNodeName, base.InputControlType)
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
            Orientation = Orientation,
            Values = base.Values,
            EngineError = base.EngineError,
            SelectedCalculatedValue = base.SelectedCalculatedValue,
            FlowDirection = FlowDirection
        };
    }
}
