#nullable disable
using System.Collections.Generic;
using Quire;
using Quire.Undo;

namespace Quire.Controls.Core.IORegions;

public partial class ToggleRegionsDesignationCommand : IUndoableCommand
{
    private List<UndoableCommand> _commandsToUndo;

    private readonly List<UndoableCommand> _commands;

    private readonly string _name;

    public string Name => _name;

    public ToggleRegionsDesignationCommand(List<UndoableCommand> commands, string name)
    {
        _commands = commands;
        _name = name;
    }

    public bool Execute()
    {
        return CommandBuffer.Execute(this);
    }

    public bool Do()
    {
        using (new CommandScope(new CompoundCommand()))
        {
            if (_commands.IsNotEmpty())
            {
                _commandsToUndo = _commands.FindAll((UndoableCommand cmd) => cmd.Do());
            }
        }
        DebugTools.DevAssert(_commandsToUndo.IsNotEmpty(), null);
        return _commandsToUndo.IsNotEmpty();
    }

    public void Undo()
    {
        using (new CommandScope(new CompoundCommand()))
        {
            if (_commandsToUndo != null)
            {
                _commandsToUndo.Each((UndoableCommand cmd) =>
                {
                    cmd.Undo();
                });
            }
        }
    }

    public void Redo()
    {
        using (new CommandScope(new CompoundCommand()))
        {
            if (_commandsToUndo != null)
            {
                _commandsToUndo.Each((UndoableCommand cmd) =>
                {
                    cmd.Redo();
                });
            }
        }
    }
}
