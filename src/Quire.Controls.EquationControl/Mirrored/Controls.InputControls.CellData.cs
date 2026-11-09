using System.Runtime.CompilerServices;
using System.ComponentModel;

namespace Quire.Controls.InputControls;

public partial class CellData
{
    private int _row;
    private int _column;
    private string _text;

    internal CellData(string text, int row, int column)
    {
        _row = row;
        _column = column;
        _text = text;
    }

    public string Text
    {
        get => _text;
        set
        {
            if (_text != value)
            {
                _text = DebugTools.AssertNotNull(value, null).Passed ? value : string.Empty;
                OnPropertyChanged("Text");
            }
        }
    }

    public int Row
    {
        get => _row;
        set
        {
            if (DebugTools.DevAssert(value >= 0, null).Passed && _row != value)
            {
                _row = value;
                OnPropertyChanged("Row");
            }
        }
    }

    public int Column
    {
        get => _column;
        set
        {
            if (DebugTools.DevAssert(value >= 0, null).Passed && _column != value)
            {
                _column = value;
                OnPropertyChanged("Column");
            }
        }
    }

    public event PropertyChangedEventHandler PropertyChanged;

    public bool IsEmpty() => Text.IsNullOrEmpty();

    public bool IsEmptyOrWhitespace() => string.IsNullOrWhiteSpace(Text);

    public void Clear() => Text = string.Empty;

    protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = default)
    {
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }
}
