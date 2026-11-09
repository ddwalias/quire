using Quire.Controls.Core;
using Quire.EquationEditor.Tree.Transform;

namespace Quire.EquationEditor.Tree;

public abstract partial class InputScriptableControlData
{
    protected byte[] b;
    private string c;
    private System.Collections.ObjectModel.Collection<string> d;

    public InputScriptableControlData()
    {
        base.EngineType = InputControlEngineTypes.DefaultScript;
    }

    public InputScriptableControlData(string mathNodeName) : this()
    {
        base.MathNodeName = "ScriptableControl";
    }

    public InputScriptableControlData(string mathNodeName, InputControlTypes inputControlTypes) : this(mathNodeName)
    {
        InputControlType = inputControlTypes;
        base.MathNodeName = mathNodeName;
    }

    public string ScriptObjectControlName
    {
        get => c;
        set => c = value;
    }

    public System.Collections.ObjectModel.Collection<string> Values
    {
        get => d;
        set => d = value;
    }

    public byte[] ScriptHashCode => b;

    public string ValueSExpression => ComboBoxInputControlDataToSExpressionTransform.Transform(this);

    public override bool IsEquivalentTo(IInputControlData other)
    {
        InputScriptableControlDataWithInputs control = other as InputScriptableControlDataWithInputs;
        return control != null && ScriptHashCode.Equals(control.ScriptHashCode);
    }

    public InputScriptableControlData(string mathNodeName, InputControlTypes inputControlTypes, ParsedSExpression parsedSExp)
        : this(mathNodeName, inputControlTypes)
    {
        IInputControlData control = ParsedSExpressionToComboBoxInputControlData.Transform(
            mathNodeName, inputControlTypes, parsedSExp.SExpList[parsedSExp.SExpList.Count - 1]);
        IInputControlDataWithValues valuedControl = control as IInputControlDataWithValues;
        if (valuedControl != null)
            Values = valuedControl.Values;
        IInputScriptableControlData scriptableControl = control as IInputScriptableControlData;
        if (scriptableControl != null)
            Script = scriptableControl.Script;
    }
}
