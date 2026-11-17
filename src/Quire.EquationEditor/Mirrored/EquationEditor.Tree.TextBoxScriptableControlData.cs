using System.Windows;

namespace Quire.EquationEditor.Tree;

public partial class TextBoxScriptableControlData
{
    protected const int a = 30;
    protected const int b = 100;

    private bool c;

    private bool d;

    private bool n;

    private TextAlignment o;

    public TextBoxScriptableControlData(string mathNodeName, Quire.Controls.Core.InputControlTypes inputControlTypes)
        : base(mathNodeName, inputControlTypes)
    {
        Width = b;
        Height = a;
        Values = new System.Collections.ObjectModel.Collection<string> { string.Empty };
    }

    public bool AcceptsReturn
    {
        get => c;
        set => c = value;
    }

    public bool IsReadOnly
    {
        get => d;
        set => d = value;
    }

    public bool Border
    {
        get => n;
        set => n = value;
    }

    public TextAlignment TextAlignment
    {
        get => o;
        set => o = value;
    }

    [global::System.Runtime.CompilerServices.SkipLocalsInit]
    public override object Clone()
    {
        return new TextBoxScriptableControlData(base.MathNodeName, base.InputControlType)
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
            AcceptsReturn = AcceptsReturn,
            IsReadOnly = IsReadOnly,
            Border = Border,
            TextAlignment = TextAlignment
        };
    }
}
