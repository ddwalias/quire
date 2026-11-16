namespace Quire.Controls.InputControls;

public partial class ScriptEngineParams : IScriptEngineParams
{
    public global::Quire.EquationEditor.Tree.IInputScriptableControlData Data { get; }

    public bool IsApply { get; set; }

    public bool IsNewInputResults
    {
        get => ((global::Quire.EquationEditor.Tree.IInputScriptableControlDataWithInputs)Data).IsNewInputResults;
        set => ((global::Quire.EquationEditor.Tree.IInputScriptableControlDataWithInputs)Data).IsNewInputResults = value;
    }

    public bool IsInteractChanged { get; set; }

    public bool IsRecalculate { get; set; }

    public bool IsOutputsChanged { get; set; }

    public bool IsInputsExists => ((global::Quire.EquationEditor.Tree.IInputScriptableControlDataWithInputs)Data).IsInputsExists;

    public ScriptEngineParams()
    {
    }

    public ScriptEngineParams(global::Quire.EquationEditor.Tree.IInputScriptableControlData data)
        : this()
    {
        Data = data;
    }
}
