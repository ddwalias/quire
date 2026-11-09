namespace Quire.Controls.InputControls;

public partial interface IInputControlEditorModel
{
    global::Quire.EquationEditor.Tree.InputControlEngineTypes EngineType { get; set; }
    bool IsEnabled { get; }
    bool AutoRecalculate { get; set; }
}
