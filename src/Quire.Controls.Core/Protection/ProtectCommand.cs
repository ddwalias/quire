#nullable disable
using System;
using Quire.Undo;

namespace Quire.Controls.Core.Protection;

public partial class ProtectCommand : ProtectionCommonBase, IUndoableCommand
{
    private readonly Action _toggleDisplayState;

    private readonly CompoundCommand _undoCommand = new CompoundCommand();

    public string Name => "Set Control Protection";

    public ProtectCommand(IProtectableModel model, ProtectionDialogState state, Action toggleDisplayState, Action<string> ShowWarningDialogRequest)
        : base(model, ShowWarningDialogRequest)
    {
        ProtectionLevel protectionLevel = ProtectionLevel.Activation | ProtectionLevel.Selection;
        protectionLevel = (state.IsDisplayStateLocked ? (protectionLevel | ProtectionLevel.DisplayState) : protectionLevel);
        _protectionData = new ProtectionData(state.Password, DateTime.Now, state.ShowTimeStamp, protectionLevel);
        _toggleDisplayState = toggleDisplayState;
    }

    public bool Do()
    {
        using (new CommandScope(_undoCommand))
        {
            if (_toggleDisplayState != null)
            {
                _toggleDisplayState();
            }
            Protect();
        }
        return true;
    }

    public void Undo()
    {
        Unprotect();
        _undoCommand.Undo();
    }

    public void Redo()
    {
        Do();
    }
}
