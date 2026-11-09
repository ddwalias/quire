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
}
