namespace Quire.EquationEditor.Tree
{
    public partial interface INodeInfo
    {
        bool IsEquivalentTo(INodeInfo other);
    }

    public partial interface INodeInfoNotifyModel
    {
        event global::System.Action<global::Quire.DelegateFunction0<bool>, string> RequestUndoableCommandExecution;

        event global::System.Action ValueChanged;

        event global::System.EventHandler<RequestRestoreInsertionPointEventArgs> RequestRestoreInsertionPoint;

        void NotifyValueChanged();

        void DelegateRequestRestoreInsertionPoint(RequestRestoreInsertionPointEventArgs source);
    }

    public partial interface IComboBoxControlNodeInfo
    {
        ComboBoxInputControlData Value { get; set; }
    }

    public partial interface IInputControlNodeInfo
    {
        IInputControlDataCalculatedValue Value { get; set; }

        string ValueSExpression { get; }

        global::Quire.Controls.Core.InputControlTypes InputControlType { get; }
    }
}
