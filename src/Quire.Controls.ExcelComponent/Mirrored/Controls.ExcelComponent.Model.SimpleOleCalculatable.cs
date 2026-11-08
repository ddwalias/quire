#nullable disable
#pragma warning disable

using Quire.Calculation.Core;

namespace Quire.Controls.ExcelComponent.Model;

public partial class SimpleOleCalculatable
{
    private readonly bool _isFragment;

    public SimpleOleCalculatable(bool isFragment)
    {
        _isFragment = isFragment;
    }

    public Pair<bool, bool> IsFragment => new Pair<bool, bool>
    {
        First = _isFragment,
        Second = true
    };

    public bool IsConnectedToTheEngine { get; set; }

    public override string Rl
    {
        get => base.Rl;
        set
        {
            base.Rl = value;
            DebugTools.DevAssert(!IsConnectedToTheEngine || Rl.StartsWith("(fragment") == IsFragment.First, null);
        }
    }
}
