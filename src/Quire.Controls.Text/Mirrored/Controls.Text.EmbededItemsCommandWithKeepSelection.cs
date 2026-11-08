#nullable disable
#pragma warning disable

using System;
using Quire.Controls.Core;
using Quire.Undo;

namespace Quire.Controls.Text;

internal partial class EmbededItemsCommandWithKeepSelection
{
    private readonly IRichTextBase _texRegion;
    private readonly Func<bool> _commandAction;
    private readonly CompoundCommand _wrappingCommand;
    private readonly int _selectionStart;
    private readonly int _selectionEnd;

    public EmbededItemsCommandWithKeepSelection(IRichTextBase textRegion, string name, Func<bool> commandAction = default)
    {
        Name = name;
        if (DebugTools.AssertNotNull(textRegion).Passed)
        {
            _texRegion = textRegion;
            _selectionStart = textRegion.SelectionStart;
            _selectionEnd = textRegion.SelectionEnd;
        }
        _commandAction = commandAction;
        if (HasCommandAction)
        {
            _wrappingCommand = new CompoundCommand();
            return;
        }
        DebugTools.DevAssert(HasSelection);
    }

    private bool HasCommandAction => _commandAction != null;

    private bool HasSelection => _selectionStart >= 0 && _selectionStart < _selectionEnd;

    public string Name { get; private set; }

    public bool Do()
    {
        bool result;
        if (HasCommandAction)
        {
            using (CommandScope scope = new CommandScope(_wrappingCommand))
            {
                result = _commandAction();
            }
        }
        else
        {
            result = HasSelection;
        }
        return result;
    }

    public void Undo()
    {
        if (HasCommandAction)
        {
            _wrappingCommand.Undo();
        }
        if (HasSelection)
        {
            MakeSelection();
        }
    }

    public void Redo()
    {
        if (HasCommandAction)
        {
            _wrappingCommand.Redo();
        }
        if (HasSelection)
        {
            MakeSelection();
        }
    }

    private void MakeSelection()
    {
        _texRegion.SetSelection(_selectionStart, _selectionEnd);
    }

    public bool Execute()
    {
        return CommandBuffer.Execute(this, null);
    }
}
