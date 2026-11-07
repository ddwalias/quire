#nullable disable
#pragma warning disable
using Quire.Undo;

namespace Quire.Controls.Include;

internal partial class IncludeWorksheetFilePathChange : IUndoableCommand
{
    private readonly IncludeWorksheetControl _control;
    private readonly string _newText;
    private readonly string _oldText;
    private readonly int _index;
    private readonly bool _cacheValid;
    private readonly bool _regionsLoaded;

    public string Name => "Change IncludeFilePath";

    public IncludeWorksheetFilePathChange(IncludeWorksheetControl control, string newText)
    {
        _control = control;
        _newText = newText;
        _index = _control.CaretIndex;
        _oldText = _control.Text;
        _cacheValid = _control.CacheValid;
        _regionsLoaded = _control.RegionsLoaded;
    }

    public bool Do()
    {
        _control.Text = _newText;
        return true;
    }

    public void Undo()
    {
        bool flag = !_control.IsLastLoadedPath(_oldText) && _control.RegionsLoaded;
        _control.CaretIndex = _index;
        if (flag)
        {
            _control.Reset();
        }
        else
        {
            _control.CacheValid = _cacheValid;
            _control.RegionsLoaded = _regionsLoaded;
        }
        _control.Text = _oldText;
    }

    public void Redo()
    {
        Do();
    }
}
