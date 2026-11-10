#nullable disable

namespace Quire;

internal partial class ChangeTextStyleCommand
{
    public string Name => "undo text style change";

    public ChangeTextStyleCommand(TextStyleData oldTextStyleData, TextStyleData newTextStyleData, global::Quire.Controls.Core.ITextStylesManager textStyleManager)
    {
        _textStylesManager = textStyleManager;
        _oldTextStyleData = oldTextStyleData;
        _newTextStyleData = newTextStyleData;
    }

    public bool Do()
    {
        _textStylesManager.UpdateTextStyleData(_newTextStyleData.StyleName, _newTextStyleData.StyleKey, _newTextStyleData.Style, _newTextStyleData.BaseOnKey);
        return true;
    }

    public void Undo()
    {
        _textStylesManager.UpdateTextStyleData(_newTextStyleData.StyleName, _oldTextStyleData.StyleKey, _oldTextStyleData.Style, _oldTextStyleData.BaseOnKey);
    }

    public void Redo()
    {
        _textStylesManager.UpdateTextStyleData(_newTextStyleData.StyleName, _newTextStyleData.StyleKey, _newTextStyleData.Style, _newTextStyleData.BaseOnKey);
    }

    public bool Execute()
    {
        return global::Quire.Undo.CommandBuffer.Execute(this);
    }
}
