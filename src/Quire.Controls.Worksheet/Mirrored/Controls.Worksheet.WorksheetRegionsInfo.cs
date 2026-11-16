namespace Quire.Controls.Worksheet;

public partial struct WorksheetRegionsInfo
{
    private bool _isScriptableControlExists;

    public bool IsScriptableControlExists
    {
        get => _isScriptableControlExists;
        set => _isScriptableControlExists = value;
    }
}
