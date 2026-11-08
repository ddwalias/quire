namespace Quire.EquationEditor.Tree
{
    public partial interface IRootMathNode
    {
        event global::System.Action SubTreeNodeValueChanged;

        event global::System.EventHandler<global::Quire.EquationEditor.Tree.RequestRestoreInsertionPointEventArgs> RequestRestoreInsertionPoint;

        void RequestNotifySubTreeNodeValueChanged(MathNode mathNode);

        void DispatchRequestUndoableCommandExecution(global::Quire.DelegateFunction0<bool> action, string commandName, MathNode nodeToRestore);

        void DelegateRequestRestoreInsertionPoint(RequestRestoreInsertionPointEventArgs requestRestoreInsertionPointEventArgs);
    }
}
