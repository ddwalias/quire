using System.Collections.Generic;

namespace Quire.Controls;

internal partial class SolveBlockModel
{
    public override object Clone()
    {
        var clone = new SolveBlockModel();
        CopyStateTo(clone);
        return clone;
    }

    protected override void AddItems(IEnumerable<global::Quire.Controls.Core.IWhiteboardItem> items)
    {
        base.AddItems(items);
        UpdateWhichItemIsSolver();
    }

    protected override void RemoveItems(IEnumerable<global::Quire.Controls.Core.IWhiteboardItem> items)
    {
        base.RemoveItems(items);
        UpdateWhichItemIsSolver();
    }
}
