namespace Quire.EquationEditor.Tree
{
    public partial interface IInputControlDataWithValues
    {
        global::System.Collections.ObjectModel.Collection<string> Values { get; set; }

        int Count { get; set; }
    }

    public partial interface IInputResizableControlData
    {
        double Height { get; set; }

        double Width { get; set; }
    }

    public partial interface IListBoxScriptableControlData
    {
        bool Sort { get; set; }

        global::System.Windows.FlowDirection FlowDirection { get; set; }
    }
}
