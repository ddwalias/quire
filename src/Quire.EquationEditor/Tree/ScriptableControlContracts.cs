namespace Quire.EquationEditor.Tree
{
    public partial interface IInputControlData
    {
        global::Quire.Controls.Core.InputControlTypes InputControlType { get; }

        global::System.Windows.Size OutputFieldSize { get; set; }

        string Background { get; set; }

        string Foreground { get; set; }

        double FontSize { get; set; }

        bool IsEquivalentTo(IInputControlData other);
    }

    public partial interface IInputControlDataCalculatedValue
    {
        double SelectedValueIndex { get; set; }

        global::System.Collections.Generic.IList<object> SelectedCalculatedValue { get; set; }

        string RL { get; set; }

        bool HasCalculatedValue { get; }

        bool IsNeedsCalculate { get; set; }

        string MathNodeName { get; set; }

        InputControlEngineTypes EngineType { get; set; }

        string EngineError { get; set; }

        ScriptErrorType ErrorType { get; set; }
    }

    public partial interface IInputScriptableControlData
    {
        string Script { get; set; }

        string ScriptObjectControlName { get; set; }

        string ValueSExpression { get; }
    }

    public partial interface IInputScriptableControlDataWithInputs
    {
        object[] InputsResultValue { get; set; }

        string InputFieldSExpression { get; set; }

        bool IsInputsAreaShown { get; set; }

        bool IsInputsExists { get; set; }

        bool IsNewInputResults { get; set; }

        int? InputFieldSize { get; set; }
    }
}
