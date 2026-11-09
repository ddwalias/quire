using System.Windows;

namespace Quire.EquationEditor.Tree;

public partial class CheckBoxScriptableControlData
{
    public CheckBoxScriptableControlData(string mathNodeName, Quire.Controls.Core.InputControlTypes inputControlTypes)
        : base(mathNodeName, inputControlTypes)
    {
        Height = a;
        Width = b;
    }

    private FlowDirection c;

    public FlowDirection FlowDirection
    {
        get => c;
        set => c = value;
    }
}
