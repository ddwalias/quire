#nullable disable
#pragma warning disable

namespace Quire;

partial class EnvironmentScope
{
    private static global::Quire.EnvironmentScope _currentInstance = new global::Quire.EnvironmentScope();
    public static bool IsCurrentlyInsideUndo => _currentInstance.Contains((global::Quire.ActionItem item) => global::Quire.Undo.CommandEventArgs.ActionTypeEnum.Undo == item.ActionType);
    public static bool IsCurrentlyInsideRedo => _currentInstance.Contains((global::Quire.ActionItem item) => global::Quire.Undo.CommandEventArgs.ActionTypeEnum.Redo == item.ActionType);
    public static bool IsCurrentlyInsideUndoOrRedo => _currentInstance.Contains((global::Quire.ActionItem item) => global::Quire.Undo.CommandEventArgs.ActionTypeEnum.Redo == item.ActionType || global::Quire.Undo.CommandEventArgs.ActionTypeEnum.Undo == item.ActionType);
    [global::Quire.Mirror.MirrorVerified("Release IL")]
    private EnvironmentScope()
    {
        _actionItemsStack = new global::System.Collections.Generic.Stack<global::Quire.ActionItem>();
    }

    public static void RegisterWithCommandBuffer(global::Quire.Undo.CommandBuffer commandBuffer)
    {
        commandBuffer.BeforeCommand += _currentInstance.HandleBeforeCommand;
        commandBuffer.AfterCommand += _currentInstance.HandleAfterCommand;
    }

    public static void UnregisterWithCommandBuffer(global::Quire.Undo.CommandBuffer commandBuffer)
    {
        commandBuffer.BeforeCommand -= _currentInstance.HandleBeforeCommand;
        commandBuffer.AfterCommand -= _currentInstance.HandleAfterCommand;
        _currentInstance.ClearStaleCommands();
    }
    private void ClearStaleCommands()
    {
        global::System.Collections.Generic.List<global::Quire.ActionItem> list = global::System.Linq.Enumerable.ToList(_actionItemsStack);
        if (list.RemoveAll((global::Quire.ActionItem item) => !item.IsCommandAlive) > 0)
        {
            list.Reverse();
            _actionItemsStack = new global::System.Collections.Generic.Stack<global::Quire.ActionItem>(list);
            global::Quire.DebugTools.DevAssert(false, null, global::System.Array.Empty<object>());
        }
    }

    [global::Quire.Mirror.MirrorVerified("Release IL")]
    private void HandleBeforeCommand(object sender, global::Quire.Undo.CommandEventArgs e) => _actionItemsStack.Push(new global::Quire.ActionItem(e));

    [global::Quire.Mirror.MirrorVerified("Release IL")]
    private void HandleAfterCommand(object sender, global::Quire.Undo.CommandEventArgs e)
    {
        global::Quire.DebugTools.DevAssert(_actionItemsStack.Peek().Command == e.Command, null, global::System.Array.Empty<object>());
        _actionItemsStack.Pop();
    }
}
