#nullable disable
#pragma warning disable

using System;
using System.Windows.Documents;
using DocumentTextRange = System.Windows.Documents.TextRange;
using Quire.Controls.Core;
using Quire.Wpf;
using Quire.Undo;

namespace Quire.Controls.Text;

public partial class ToggleScriptUndoCommand
{
    private readonly ScriptState _switchToScript;
    private readonly CompoundCommand _nestedCommand;
    private readonly RichTextBase _textRegion;
    private readonly int _rangeStartOffset;
    private readonly int _rangeEndOffset;
    private readonly int _insertionPoint;
    private readonly bool _shouldRecreateSelection;

    public string Name => "Toggle Range Text";

    public bool Do()
    {
        bool result = false;
        using (CommandScope scope = new CommandScope(_nestedCommand))
        {
            if (CustomScriptRunBase.ToggleRangeText(RecreatedRange(), _switchToScript))
            {
                RecreateSelection();
                result = true;
            }
        }
        return result;
    }

    private DocumentTextRange RecreatedRange() =>
        new DocumentTextRange(_textRegion.GetPointerToInsertionPoint(_rangeStartOffset), _textRegion.GetPointerToInsertionPoint(_rangeEndOffset));

    private void RecreateSelection()
    {
        if (!_shouldRecreateSelection)
        {
            return;
        }
        if (_insertionPoint == -1)
        {
            DocumentTextRange range = RecreatedRange();
            _textRegion.Selection.Select(range.Start, range.End);
        }
        else
        {
            _textRegion.SetSelection(_insertionPoint, _insertionPoint);
        }
    }

    public void Undo()
    {
        _nestedCommand.Undo();
        RecreateSelection();
    }

    public void Redo()
    {
        _nestedCommand.Redo();
        RecreateSelection();
    }
}
