using Quire.Controls.Core;

namespace Quire.EquationEditor.Tree;

public static partial class InputControlDataCreator
{
    public static IInputControlData Create(string mathNodeName, InputControlTypes inputControlType)
    {
        IInputControlData result;
        switch (inputControlType)
        {
            case InputControlTypes.ListBoxScriptableControl:
                result = new ListBoxScriptableControlData(mathNodeName, inputControlType);
                break;
            case InputControlTypes.ButtonScriptableControl:
                result = new ButtonScriptableControlData(mathNodeName, inputControlType);
                break;
            case InputControlTypes.CheckBoxScriptableControl:
                result = new CheckBoxScriptableControlData(mathNodeName, inputControlType);
                break;
            case InputControlTypes.TextBoxScriptableControl:
                result = new TextBoxScriptableControlData(mathNodeName, inputControlType);
                break;
            case InputControlTypes.RadioButtonScriptableControl:
                result = new RadioButtonScriptableControlData(mathNodeName, inputControlType);
                break;
            case InputControlTypes.SliderScriptableControl:
                result = new SliderScriptableControlData(mathNodeName, inputControlType);
                break;
            default:
                result = new InputScriptableControlDataWithInputs(mathNodeName, InputControlTypes.EmptyControl);
                DebugTools.DevAssert(true, mathNodeName + " not found in MathNodeNames");
                break;
        }
        return result;
    }
}
