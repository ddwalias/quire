namespace Quire;

public partial class AutomationSpiritInputsOutputsStates
{
    private readonly global::Quire.Controls.Core.IORegions.IInputOutputItemsStates _itemsWithStates;
    private global::System.Collections.Generic.IDictionary<string, global::Quire.Controls.Core.IORegions.IScalarItemWithOriginalState> _inputAliasToItemStatesMap;
    private global::System.Collections.Generic.IDictionary<string, global::Quire.Controls.Core.IORegions.IScalarItem> _outputAliasToItemStatesMap;

    public AutomationSpiritInputsOutputsStates(global::Quire.Controls.Core.IORegions.IInputOutputItemsStates itemsWithValues) { _itemsWithStates = itemsWithValues; }

    private global::Quire.Controls.Core.IORegions.IScalarItemWithOriginalState GetInputItem(int idx, out string alias)
    {
        global::Quire.Controls.Core.IORegions.IScalarItemWithOriginalState item;
        if (idx >= 0 && idx < ScalarInputsCount)
        {
            item = _itemsWithStates.InputItems[idx];
            alias = item.Alias;
        }
        else
        {
            item = null;
            alias = null;
        }

        return item;
    }

    private global::Quire.Controls.Core.IORegions.IScalarItem GetOutputItem(int idx, out string alias)
    {
        global::Quire.Controls.Core.IORegions.IScalarItem item;
        if (idx >= 0 && idx < ScalarOutputsCount)
        {
            item = _itemsWithStates.OutputItems[idx];
            alias = item.Alias;
        }
        else
        {
            item = null;
            alias = null;
        }

        return item;
    }
}
