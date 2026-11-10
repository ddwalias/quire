#nullable disable

namespace Quire;

internal partial class DeleteTextStyleCommand
{
    public string Name => "delete text style";

    public DeleteTextStyleCommand(TextStyleData deleteTextStyleData, global::Quire.Controls.Core.ITextStylesManager textStyleManager)
    {
        _textStylesManager = textStyleManager;
        _deleteTextStyleData = deleteTextStyleData;
        _deleteTextStyleDataIndex = _textStylesManager.StylesCollection.IndexOf(_deleteTextStyleData);
    }

    public bool Do()
    {
        _textStylesManager.RemoveStyle(_deleteTextStyleData);
        return true;
    }

    public void Undo()
    {
        _textStylesManager.AddStyle(_deleteTextStyleData, _deleteTextStyleDataIndex);
    }

    public void Redo()
    {
        _textStylesManager.RemoveStyle(_deleteTextStyleData);
    }

    public bool Execute()
    {
        return global::Quire.Undo.CommandBuffer.Execute(this);
    }
}
