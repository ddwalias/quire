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

    protected override global::Quire.Calculation.Core.ICalculatable GetWrappedCalculatable(
        global::Quire.Calculation.Core.ICalculatable calculatable)
    {
        if (!(calculatable is global::Quire.Controls.ISolveBlockCalculatable))
            return calculatable;
        return (calculatable as global::Quire.Controls.ISolveBlockCalculatable).WrappedCalculatable;
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
