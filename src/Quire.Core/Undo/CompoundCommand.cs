#nullable disable
using Quire.Mirror;
using Quire.Mirror;

namespace Quire.Undo;

public partial class CompoundCommand
{
    [MirrorVerified("PtcUndoCompoundCommandMechanicalDifferentialTests")]
    private string _name;

    [MirrorVerified("PtcUndoCompoundCommandMechanicalDifferentialTests")]
    public CompoundCommand(string name, IEnumerable<IUndoableCommand> subCommands) : base(subCommands)
    {
        _name = name;
    }

    [MirrorVerified("PtcUndoCompoundCommandMechanicalDifferentialTests")]
    public CompoundCommand()
    {
        _name = string.Empty;
    }

    [MirrorVerified("Release IL")]
    public virtual bool Do()
    {
        for (int index = 0; index < Count; index++)
        {
            if (!this[index].Do())
            {
                for (int undoIndex = index - 1; undoIndex >= 0; undoIndex--)
                {
                    this[undoIndex].Undo();
                }
                return false;
            }
        }
        return true;
    }

    [MirrorVerified("Release IL")]
    public virtual void Undo()
    {
        int index = Count - 1;
        while (index >= 0)
        {
            this[index].Undo();
            index--;
        }
    }

    [MirrorVerified("Release IL")]
    public virtual void UndoSideEffects()
    {
        int index = Count - 1;
        while (index >= 0)
        {
            IUndoableCommand command = this[index];
            if (command is IUndoableCommandWithSideEffects)
            {
                command.Undo();
            }
            else if (command is CompoundCommand)
            {
                (command as CompoundCommand).UndoSideEffects();
            }
            index--;
        }
    }

    [MirrorVerified("Release IL")]
    public virtual void RedoSideEffects()
    {
        foreach (IUndoableCommand command in this)
        {
            if (command is IUndoableCommandWithSideEffects)
            {
                command.Redo();
            }
            else if (command is CompoundCommand)
            {
                (command as CompoundCommand).RedoSideEffects();
            }
        }
    }

    [MirrorVerified("Release IL")]
    public virtual void Redo()
    {
        foreach (IUndoableCommand command in this)
        {
            command.Redo();
        }
    }

    [MirrorVerified("PtcUndoCompoundCommandMechanicalDifferentialTests")]
    public string Name { get => _name; set => _name = value; }

    [MirrorVerified("Release IL")]
    public override string ToString() => string.Format("CompoundCommand Name: {0}", Name);
}
