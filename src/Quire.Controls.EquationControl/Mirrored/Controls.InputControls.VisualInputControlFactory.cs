using System.Windows;
using System.Windows.Controls;
using Quire.Controls.Core;
using Quire.EquationEditor.Tree;

namespace Quire.Controls.InputControls;

public static partial class VisualInputControlFactory
{
    public static Control CreateVisualInputControl(string mathNodeName, INodeInfo nodeInfo)
    {
        switch (mathNodeName)
        {
            case "ComboBoxControl":
                return new ComboBoxControl();
            case "ScriptableControl":
                IInputControlDataCalculatedValue data = ((IInputScriptableWithInputNodeInfo)nodeInfo).Value;
                InputControlTypes controlType = (nodeInfo as IInputControlNodeInfo).InputControlType;
                switch (controlType)
                {
                    case InputControlTypes.ListBoxScriptableControl:
                        return new ScriptableInputControl(new ListBoxScriptableInputControl(data));
                    case InputControlTypes.CheckBoxScriptableControl:
                        return new ScriptableInputControl(new CheckBoxControl(data));
                    case InputControlTypes.TextBoxScriptableControl:
                        return new ScriptableInputControl(new TextBoxControl(data));
                    case InputControlTypes.SliderScriptableControl:
                        return new ScriptableInputControl(new SliderControl(data));
                    case InputControlTypes.ButtonScriptableControl:
                        return new ScriptableInputControl(new ButtonControl(data));
                    case InputControlTypes.RadioButtonScriptableControl:
                        return new ScriptableInputControl(new RadioButtonControl(data, null));
                    default:
                        DebugTools.FailAssertion("control " + (nodeInfo as IInputControlNodeInfo).InputControlType.ToString() + " not found");
                        return null;
                }

            default:
                DebugTools.FailAssertion("control " + mathNodeName + " not found");
                return null;
        }
    }

    public static Window CreateInputControlEditor(InputControlTypes controlType, IInputControlViewModel controlViewModel)
    {
        IInputControlEditorModel editorModel = (IInputControlEditorModel)controlViewModel.Model;
        switch (controlType)
        {
            case InputControlTypes.CheckBoxScriptableControl:
                return new InputControlEditor(controlType.ToString(), new CheckBoxPropertiesEditor(editorModel), editorModel);
            case InputControlTypes.ButtonScriptableControl:
                return new InputControlEditor(controlType.ToString(), new ButtonPropertiesEditor(editorModel), editorModel);
            case InputControlTypes.SliderScriptableControl:
                return new InputControlEditor(controlType.ToString(), new SliderPropertiesEditor(editorModel), editorModel);
            case InputControlTypes.RadioButtonScriptableControl:
                return new InputControlEditor(controlType.ToString(), new RadioButtonPropertiesEditor(editorModel), editorModel);
            case InputControlTypes.TextBoxScriptableControl:
                return new InputControlEditor(controlType.ToString(), new TextBoxPropertiesEditor(editorModel), editorModel);
            case InputControlTypes.ListBoxScriptableControl:
                return new InputControlEditor(controlType.ToString(), new ListBoxPropertiesEditor(editorModel), editorModel);
            default:
                DebugTools.FailAssertion("control " + controlType.ToString() + " not found");
                return null;
        }
    }
}
