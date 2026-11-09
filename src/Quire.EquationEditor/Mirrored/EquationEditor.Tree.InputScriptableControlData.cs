using Quire.Controls.Core;
using Quire.EquationEditor.Tree.Transform;

namespace Quire.EquationEditor.Tree;

public abstract partial class InputScriptableControlData
{
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
