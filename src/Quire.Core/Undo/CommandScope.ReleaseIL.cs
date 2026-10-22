#nullable disable
using Quire.Mirror;

namespace Quire.Undo;

public partial class CommandScope
{
    [MirrorVerified("Release IL")]
    public static bool CanUndoTreeOperations
    {
        get
        {
            lock (_scopeStack)
            {
                if (CommandBuffer.CurrentInstance == null)
                {
                    return false;
                }
                return _scopeStack.Count > 0 && _scopeStack.Peek()._canUndoTreeOperations;
            }
        }
    }
}
