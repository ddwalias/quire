namespace Quire.Controls.InputControls;

public partial interface IScriptEngineParams
{
    global::Quire.EquationEditor.Tree.IInputScriptableControlData Data { get; }
    bool IsApply { get; }
    bool IsNewInputResults { get; set; }
    bool IsInteractChanged { get; set; }
    bool IsRecalculate { get; set; }
    bool IsOutputsChanged { get; set; }
}
