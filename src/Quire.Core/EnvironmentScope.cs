#nullable disable
using Quire.Mirror;
using Quire.Undo;

namespace Quire;

public partial class EnvironmentScope
{
    [MirrorVerified("Release IL")]
    private Stack<ActionItem> _actionItemsStack;

    [MirrorVerified("Release IL")]
    private EnvironmentScope()
    {
        _actionItemsStack = new Stack<ActionItem>();
    }

    [MirrorVerified("Release IL")]
    private void HandleBeforeCommand(object sender, CommandEventArgs e) => _actionItemsStack.Push(new ActionItem(e));

    [MirrorVerified("Release IL")]
    private void HandleAfterCommand(object sender, CommandEventArgs e)
    {
        DebugTools.DevAssert(_actionItemsStack.Peek().Command == e.Command, null, Array.Empty<object>());
        _actionItemsStack.Pop();
    }
}
