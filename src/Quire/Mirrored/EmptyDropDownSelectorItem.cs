#nullable disable

namespace Quire;

public partial class EmptyDropDownSelectorItem
{
    public object Value { get; set; }

    public object Header { get; set; } = string.Empty;

    public bool IsChecked
    {
        get
        {
            return false;
        }
        set
        {
            DebugTools.FailAssertion(null, System.Array.Empty<object>());
        }
    }
}
