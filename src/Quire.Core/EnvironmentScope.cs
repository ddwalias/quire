#nullable disable
using Quire.Mirror;
using Quire.Undo;

namespace Quire;

public partial class EnvironmentScope
{
    [MirrorVerified("Release IL")]
    private Stack<ActionItem> _actionItemsStack;

    [MirrorVerified("Release IL")]
    public bool Contains(DelegateFunction1<bool, ActionItem> predicate)
    {
        foreach (ActionItem item in _actionItemsStack)
        {
            if (predicate(item))
            {
                return true;
            }
        }
        return false;
    }
}
